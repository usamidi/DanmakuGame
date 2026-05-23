using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Spell Card8", menuName = "STG/Bullet Spawn/Extra Stage/Spell Card8")]
[System.Serializable]
public class SpellCard8 : EnemyBulletSpawner
{
    [Header("扇形总角度")]
    public float spreadAngle = 360f;
    [Header("发射波数")]
    public int wave = 48;

    private IEnumerator func(EBContext context)
    {
        EBulletBatch batch = new EBulletBatch();
        batch.AddBullet(
          GetBullet()
            .SetPosition(context.bullet.position)
            .SetSpeed(0.2f, UnityEngine.Random.Range(265f, 275f))
            .SetAccelarate(new Vector2(0f, -0.8f))
          );

        batch.Packed("Small-1", new Vector3(255f, 255f, 0f)).Active();
        yield break;
    }

    public IEnumerator BulletSpawn(ESContext context)
    {
        yield return new WaitUntil(() => moveSteps != 0);
        float baseAngle = 0f;
        int param = 0;
        while (true)
        {
            float speed = 5.0f;
            Vector3 playerPos = context.player.position;
            Vector3 firePos = context.self.position;
            float spreadAngle = 360f;
            int spreadLines = 8;

            EBulletBatch batch = new EBulletBatch();
            float startAngle = baseAngle - (spreadAngle / 2f);
            int accIdx = UnityEngine.Random.Range(0, 3);
            Vector2 acc = new Vector2();
            for (int i = 0; i < 8; i++)
            {
                for (int t = 0; t < 16; t++)
                {
                    firePos.x = -3.8f + (3.8f * 2 / 15) * t;
                    firePos.y = 1.2f + (2.6f / 7) * i;
                    if (accIdx == 0)
                    {
                        acc = new Vector2(-0.3f, -0.8f) * (t + i) / 36f;
                    }
                    else if (accIdx == 1)
                    {
                        acc = new Vector2(0.3f, -0.8f) * (22 - t - i) / 36f;
                    }
                    else if (accIdx == 2)
                    {
                        acc = new Vector2(UnityEngine.Random.Range(-0.4f, 0.4f), UnityEngine.Random.Range(-0.4f, 0.4f));
                    }
                    batch.AddBullet(
                      GetBullet()
                        .SetPosition(firePos)
                        .SetSpeed(0.4f, UnityEngine.Random.Range(260f, 280f))
                        .SetLimit(new Vector2(0f, 3f))
                        .SetAccelarate(acc)
                      );
                }
            }


            EBulletBatch batch1 = new EBulletBatch();
            for (int i = 0; i < 5; i++)
            {
                firePos.x = UnityEngine.Random.Range(-3.8f, 3.8f);
                firePos.y = UnityEngine.Random.Range(1.2f, 3.8f);
                batch1.AddBullet(
                  GetBullet()
                    .SetPosition(firePos)
                    .SetSpeed(0.4f, GetAngleToPosition(firePos, playerPos))
                    .SetLimit(new Vector2(0f, 3f))
                    .SetSpeedRate(0.6f)
                  );
            }
            batch.Packed("Small-1", new Vector3(0f, 0f, 204f)).Active();
            batch1.Packed("Medium", new Vector3(255f, 255f, 0f)).Active();
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
