using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Spell Card2", menuName = "STG/Bullet Spawn/Extra Stage/Spell Card2")]
[System.Serializable]
public class SpellCard2 : EnemyBulletSpawner
{
    [Header("散弹的条数 (奇数必有一发正对玩家)")]
    public int spreadLines = 48;
    [Header("扇形总角度")]
    public float spreadAngle = 360f;
    [Header("发射波数")]
    public int wave = 12;

    private IEnumerator func(EBContext context)
    {
        context.bullet.state = EBulletState.Dying;
        EBulletBatch batch = new EBulletBatch();
        for (int i = 0; i < 2; i++)
        {
            batch.AddBullet(
              GetBullet()
                .SetPosition(context.bullet.position)
                .SetSpeed(1.0f, UnityEngine.Random.Range(0f, 360f))
                .SetAccelarate(new Vector2(0f, -0.6f))
              );
        }

        batch.Packed("Rice", new Vector3(0f, 0f, 204f)).Active();
        yield break;
    }

    bool check(in EBContext context)
    {
        return context.bullet.timer >= 2.5f;
        //return context.bullet.speed <= 0.1f;
    }

    public IEnumerator BulletSpawn(ESContext context)
    {
        yield return new WaitUntil(() => moveSteps != 0);
        while (true)
        {
            float speed = 1.2f;
            Vector3 playerPos = context.player.position;
            Vector3 firePos = context.self.position;
            float spreadAngle = 360f;
            for (int n = 0; n < wave; n++)
            {
                float windx = UnityEngine.Random.Range(-1f, 1f);
                float windy = UnityEngine.Random.Range(-1.2f, 0.4f);
                int spreadLines = (n + 1) * 2;
                float baseAngle = GetAngleToPosition(firePos, playerPos) + UnityEngine.Random.Range(-30f, 30f);
                float startAngle = baseAngle - (spreadAngle / 2f);
                float angleStep = spreadLines > 1 ? spreadAngle / (spreadLines - 1) : 0f;
                EBulletBatch batch = new EBulletBatch();
                for (int i = 0; i < spreadLines; i++)
                {
                    float currentAngle = startAngle + (angleStep * i);
                    float radians = currentAngle * Mathf.Deg2Rad;
                    Vector3 offsetPos = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0f).normalized * 0.2f;

                    float finalSpeed = speed;
                    batch.AddBullet(
                        GetBullet()
                        .SetPosition(firePos + offsetPos)
                        .SetAccelarate(new Vector2(windx, windy))
                        .SetSpeed(finalSpeed, currentAngle)
                    );
                }
                //batch.AttachCallBack(check, (context) => func(context));
                batch.Packed("Amulet", new Vector3(204f, 0f, 51f)).Active();

                yield return new WaitForSeconds(0.10f);
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
