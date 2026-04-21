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

    public Dictionary<HashSet<EBCallBackInfo>, List<EBulletData>> bulletsTable = new();

    // 渲染相关
    public Matrix4x4[] matricesBatch;
    public Vector4[] colorBatch;
    public int batchCounts;
    public MaterialPropertyBlock propertyBlock;

    public bool isActive = false;


    public EBulletRenderBatch()
    {
        id = idIncrementer++;

        matricesBatch = new Matrix4x4[1023];
        colorBatch = new Vector4[1023];
        batchCounts = 0;

        propertyBlock = new MaterialPropertyBlock();
    }

    public void AddBullets(List<EBulletData> bulletDatas, HashSet<EBCallBackInfo> callBacks)
    {
        if (!bulletsTable.ContainsKey(callBacks))
        {
            bulletsTable[callBacks] = bulletDatas;
        }
        else
        {
            bulletsTable[callBacks].AddRange(bulletDatas);
        }
    }

    public bool IsEmpty()
    {
        return bulletsTable.Values.All(bullets => !(bullets.Any(b => b.state != EBulletState.Dead)));
    }

    public int TotalCount()
    {
        int totalCount = 0;
        foreach (var list in bulletsTable.Values)
        {
            if (list != null)
            {
                totalCount += list.Count;
            }
        }

        return totalCount;
    }


    public bool EqualsByRender(in EBulletBatch batch)
    {
        return (appearance.Equals(batch.appearance));
    }

    public void Clear()
    {
        EBulletCallBackManager.Instance.ClearTable(this);

        foreach (var (_, bullets) in bulletsTable)
        {
            bullets.ForEach((b) =>
            {
                EBulletManager.Instance.ReleaseBullet(b);
            });
        }
        bulletsTable.Clear();

        batchCounts = 0;
        isActive = false;
    }


}

public partial class EBulletManager : MonoBehaviour
{
    private HashSet<EBulletRenderBatch> renderBatches = new();

    public EBulletRenderBatch FindSameBatch(EBulletBatch batch)
    {
        foreach (var btch in renderBatches)
        {
            if (btch.EqualsByRender(batch) && btch.TotalCount() + batch.bulletBatch.Count < 1023)
            {
                return btch;
            }
        }
        return null;
    }

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
        callbacks.Clear();

        foreach (var batch in renderBatches)
        {
            if (batch.isActive == false) continue;

            EBulletStyle style = GetBulletStyle(batch.appearance.style);
            float sqrHitSum = (playerHitboxRadius + style.hitboxRadius) * (playerHitboxRadius + style.hitboxRadius);
            float sqrGrazeSum = (playerGrazeRadius + style.hitboxRadius) * (playerGrazeRadius + style.hitboxRadius);

            float currentScale = style.visualScale;
            float currentAlpha = 1f;
            float t = 0f;

            context.playerPos = playerPos;
            context.dt = dt;

            int renderIndex = 0;
            foreach (var (infos, bullets) in batch.bulletsTable)
            {
                for (int j = 0; j < bullets.Count; j++)
                {
                    EBulletData b = bullets[j];
                    b.timer += dt;

                    currentAlpha = 1f;
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
                                currentScale = Mathf.Lerp(style.visualScale * 3f, style.visualScale, t);
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

                    context.bullet = b;
                    // CallBack
                    foreach (var info in infos)
                    {
                        var condition = info.condition;
                        var f = info.func;
                        if (condition(in context) && EBulletCallBackManager.Instance.CheckOnce(f, b))
                        {
                            callbacks.Add((f(context), batch));
                        }
                    }


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

            }

            batch.propertyBlock.SetVectorArray("_Color", batch.colorBatch);
            Graphics.DrawMeshInstanced(style.mesh, 0, style.material, batch.matricesBatch, renderIndex, batch.propertyBlock);
        }

        bool IsEmpty(EBulletRenderBatch batch)
        {
            if (batch.IsEmpty() & batch.isActive)
            {
                pool.Release(batch);
            }
            ;
            return false;
        }
        renderBatches.RemoveWhere(IsEmpty);

        foreach (var (c, batch) in callbacks)
        {
            EBulletCallBackManager.Instance.AddCoroutine(c, batch);
        }
    }
}
