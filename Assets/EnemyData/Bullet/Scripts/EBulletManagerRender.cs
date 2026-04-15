using System.Collections;
using System.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;

public partial class EBulletRenderBatch
{
    private static int idIncrementer = 0;
    private int id;
    public EBulletAppearance appearance;
    public List<EBulletData> bullets;

    // 回调函数
    // public EBulletCallBack onUpdate;
    //public Dictionary<EBulletCallBackType, EBulletCallBack> callBackDict;

    // 渲染相关
    public Matrix4x4[] matricesBatch;
    public Vector4[] colorBatch;
    public int batchCounts;
    public MaterialPropertyBlock propertyBlock;

    public bool isActive = false;

    public List<(EBConditionFunc condition, Func<EBContext, IEnumerator> func)> callBacks = new();

    public EBulletRenderBatch()
    {
        id = idIncrementer++;
        bullets = new List<EBulletData>();

        matricesBatch = new Matrix4x4[1023];
        colorBatch = new Vector4[1023];
        batchCounts = 0;

        propertyBlock = new MaterialPropertyBlock();

        // callBackDict = new();
    }

    public EBulletRenderBatch AttachCallBack(EBConditionFunc condition, Func<EBContext, IEnumerator> func)
    {
        callBacks.Add((condition, func));
        return this;
    }

    public void Clear()
    {
        foreach (var (_, c) in callBacks)
        {
            EBulletCallBackManager.Instance.ClearTable(c, this);
        }

        bullets.ForEach((b) =>
        {
            EBulletManager.Instance.ReleaseBullet(b);
        });
        bullets.Clear();
        callBacks.Clear();

        batchCounts = 0;
        isActive = false;
    }


}

public partial class EBulletManager : MonoBehaviour
{
    private HashSet<EBulletRenderBatch> renderBatches = new();

    private bool isOutOfBound(ref EBulletData b)
    {
        if (b.position.x < boundsMin.x)
        {
            b.boundType = EBBoundType.Left;
        }
        else if (b.position.x > boundsMax.x)
        {
            b.boundType = EBBoundType.Right;
        }
        else if (b.position.y < boundsMin.y)
        {
            b.boundType = EBBoundType.Bottom;
        }
        else if (b.position.y > boundsMax.y)
        {
            b.boundType = EBBoundType.Top;
        }

        return b.boundType != EBBoundType.None;
    }


    private List<(IEnumerator, EBulletRenderBatch)> callbacks = new();
    private EBContext context = new();

    private void renderBullet()
    {
        float dt = Time.deltaTime;
        Vector2 playerPos = playerTransform != null ? (Vector2)playerTransform.position : Vector2.zero;

        //for (int i = 0; i < renderBatches.Count; i++)
        foreach (var batch in renderBatches)
        {
            if (batch.isActive == false) continue;

            EBulletStyle style = GetBulletStyle(batch.appearance.style);
            float sqrHitSum = (playerHitboxRadius + style.hitboxRadius) * (playerHitboxRadius + style.hitboxRadius);
            float sqrGrazeSum = (playerGrazeRadius + style.hitboxRadius) * (playerGrazeRadius + style.hitboxRadius);

            float currentScale = style.visualScale;
            float currentAlpha = 1f;
            float t = 0f;

            int renderIndex = 0;
            for (int j = 0; j < batch.bullets.Count; j++)
            {
                currentAlpha = 1f;
                EBulletData b = batch.bullets[j];
                b.timer += dt;
                currentScale = style.visualScale;

                switch (b.state)
                {
                    case EBulletState.Dead:
                        continue;
                    case EBulletState.Spawning:
                        b.position += b.velocity * dt * 0.2f;

                        t = b.timer / b.spawnDuration;
                        if (t >= 1f) b.state = EBulletState.Normal;
                        else
                        {
                            currentScale = Mathf.Lerp(style.visualScale * 2f, style.visualScale, t);
                            currentAlpha = t;
                        }
                        break;
                    case EBulletState.ReachBound:
                        if (b.reflectTimes > 0)
                        {
                            b.reflectTimes--;
                            // todo
                        }
                        else
                        {
                            b.state = EBulletState.Dead;
                            //b.isAlive = false;
                            //batch.bullets.RemoveAt(j);
                            break;
                        }
                        b.boundType = EBBoundType.None;
                        break;
                    case EBulletState.Normal:
                        // 检查边界
                        if (isOutOfBound(ref b))
                        {
                            b.state = EBulletState.ReachBound;
                            //Debug.Log("change");
                            break;
                        }

                        b.position += b.velocity * dt;

                        if (playerTransform != null)
                        {
                            float dx = b.position.x - playerPos.x;
                            float dy = b.position.y - playerPos.y;

                            float distance = dx * dx + dy * dy;

                            // 擦弹检测
                            if (distance <= sqrGrazeSum && !b.isGrazed)
                            {
                                grazeNum++;
                                grazeUI.SetGraze(grazeNum);
                                b.isGrazed = true;
                                if (grazeVFX != null)
                                {
                                    ParticleSystem particle = Instantiate(grazeVFX, b.position, Quaternion.identity);
                                }

                            }

                            // 碰撞检测
                            if (distance <= sqrHitSum)
                            {
                                if (player.Missable())
                                {
                                    player.PlayerMiss();
                                    missNum++;
                                    missUI.SetMiss(missNum);

                                }
                                b.state = EBulletState.Dying;
                                b.timer = 0f;
                                b.velocity *= 0.2f;
                                if (destroyVFX != null)
                                {
                                    ParticleSystem particle = Instantiate(destroyVFX, b.position, Quaternion.identity);
                                }
                            }

                        }
                        break;
                    case EBulletState.Dying:
                        t = b.timer / b.dieDuration;
                        if (t >= 1f)
                        {
                            b.state = EBulletState.Dead;
                            currentAlpha = 0f;
                            //b.isAlive = false;
                            //batch.bullets.RemoveAt(j);
                            break;
                        }
                        else
                        {
                            currentScale = Mathf.Lerp(style.visualScale, style.visualScale * 1.5f, t);
                            currentAlpha = 1f - t;
                        }
                        break;
                }
                //batch.bullets[j] = b;

                Vector4 poolColor = batch.appearance.color / 255.0f;
                poolColor.w = currentAlpha;
                batch.colorBatch[renderIndex] = poolColor;

                batch.matricesBatch[renderIndex] = Matrix4x4.TRS(
                    b.position,
                    Quaternion.Euler(0, 0, b.rotation + style.visualAngleOffset),
                    new Vector3(currentScale, currentScale, 1)
                );
                renderIndex++;
            }
            batch.propertyBlock.SetVectorArray("_Color", batch.colorBatch);
            Graphics.DrawMeshInstanced(style.mesh, 0, style.material, batch.matricesBatch, renderIndex, batch.propertyBlock);
        }

        bool IsEmpty(EBulletRenderBatch batch)
        {
            if (!batch.bullets.Any(b => b.state != EBulletState.Dead) & batch.isActive)
            {
                pool.Release(batch);
            }
            ;
            return false;
        }
        renderBatches.RemoveWhere(IsEmpty);

        // CallBack
        context.playerPos = playerPos;
        context.dt = dt;

        callbacks.Clear();
        foreach (var batch in renderBatches)
        {
            foreach (var b in batch.bullets)
            {
                if (batch.callBacks.Count > 0)
                {
                    context.bullet = b;
                    foreach (var (condition, c) in batch.callBacks)
                    {
                        if (condition(in context) && EBulletCallBackManager.Instance.CheckOnce(c, b))
                        {
                            callbacks.Add((c(context), batch));
                            //coroutines.Add(StartCoroutine(c(context)));
                        }
                    }
                    //batch.callBacks.RemoveAll((cb) => cb.condition(in context));
                }
            }
        }
        foreach (var (c, batch) in callbacks)
        {
            EBulletCallBackManager.Instance.AddCoroutine(c, batch);
        }


        /*
        foreach (var batch in renderBatches)
        {
            EBulletStyle style = GetBulletStyle(batch.appearance.style);
            batch.propertyBlock.SetVectorArray("_Color", batch.colorBatch);
            Graphics.DrawMeshInstanced(style.mesh, 0, style.material, batch.matricesBatch, batch.bullets.Count, batch.propertyBlock);
            //batch.bullets.RemoveAll(b => !b.isAlive);
        }
        {
            batch.propertyBlock.SetVectorArray("_Color", batch.colorBatch);
            Graphics.DrawMeshInstanced(batch.style.mesh, 0, batch.style.material, batch.matricesBatch, batch.bullets.Count, batch.propertyBlock);
        }
        */
    }

}
