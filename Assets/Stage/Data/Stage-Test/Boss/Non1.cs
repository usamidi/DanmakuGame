using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Non-Spell1", menuName = "STG/Bullet Spawn/Extra Stage/Non Spell1")]
[System.Serializable]
public class Non1 : EnemyBulletSpawner
{
    [Header("散弹的条数 (奇数必有一发正对玩家)")]
    public int spreadLines = 48;
    [Header("扇形总角度")]
    public float spreadAngle = 360f;
    [Header("发射波数")]
    public int wave = 12;
    [Header("初速度")]
    public float speed = 2f;
    [Header("减速参数")]
    public float param = 1.8f;
    [Header("发弹源是否跟随")]
    public bool isFollowed;
    [Header("颜色")]
    public Vector3 color;


    private IEnumerator func(EBContext context)
    {
        context.bullet.SetSpeed(0.1f, context.bullet.direction)
          .SetSpeedRate(1f)
          .SetTurnRate(UnityEngine.Random.value > 0.5 ? 15f : -15f);
        yield break;
    }

    bool check(in EBContext context)
    {
        return context.bullet.timer >= 2.0f;
        //return context.bullet.speed <= 0.1f;
    }

    public IEnumerator BulletSpawn(ESContext context)
    {
        yield return new WaitUntil(() => moveSteps != 0);
        yield return new WaitForSeconds(1.5f);
        while (true)
        {
            Vector3 playerPos = context.player.position;
            Vector3 firePos = context.self.position;
            for (int n = 0; n < wave; n++)
            {
                if (isFollowed) firePos = context.self.position;
                float baseAngle = GetAngleToPosition(firePos, playerPos) + UnityEngine.Random.Range(-30f, 30f);
                float startAngle = baseAngle - (spreadAngle / 2f);
                float angleStep = spreadLines > 1 ? spreadAngle / (spreadLines - 1) : 0f;
                EBulletBatch batch = new EBulletBatch();
                for (int i = 0; i < spreadLines; i++)
                {
                    float currentAngle = startAngle + (angleStep * i);
                    float finalSpeed = speed;
                    float speedRate = -finalSpeed / param + UnityEngine.Random.Range(-0.1f, 0.1f);
                    batch.AddBullet(
                        GetBullet()
                        .SetPosition(firePos)
                        .SetSpeedRate(speedRate)
                        .SetLimit(new Vector2(0f, 2.2f))
                        .SetSpeed(finalSpeed, currentAngle)
                    );
                }
                batch.AttachCallBack(check, (context) => func(context));

                batch.Packed("Scale", color).Active();

                yield return new WaitForSeconds(0.15f);
            }
            yield return new WaitForSeconds(3f);
        }
    }

    public override List<Func<ESContext, IEnumerator>> SpawnerList()
    {
        return new List<Func<ESContext, IEnumerator>>
        {
          (context) => BulletSpawn(context)
        };
    }
}
