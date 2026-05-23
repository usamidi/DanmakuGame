using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Spell Card4", menuName = "STG/Bullet Spawn/Extra Stage/Spell Card4")]
[System.Serializable]
public class SpellCard4 : EnemyBulletSpawner
{
    [Header("散弹的条数 (奇数必有一发正对玩家)")]
    public int spreadLines = 48;
    [Header("扇形总角度")]
    public float spreadAngle = 360f;
    [Header("发射波数")]
    public int wave = 48;

    public IEnumerator BulletSpawn(ESContext context)
    {
        yield return new WaitUntil(() => moveSteps != 0);
        while (true)
        {
            float speed = 5.0f;
            Vector3 playerPos = context.player.position;
            Vector3 firePos = context.self.position;
            float spreadAngle = 345f;
            int spreadLines = 36;
            int sign = UnityEngine.Random.value > 0.5f ? -1 : 1;
            float baseAngle = GetAngleToPosition(firePos, playerPos) + 180f;
            for (int n = 0; n < wave; n++)
            {
                if (n > 10 && n <= 25)
                {
                    baseAngle += 1.5f * sign;
                }
                else if (n > 25)
                {
                    baseAngle -= 1.5f * sign;
                }
                float startAngle1 = baseAngle - (spreadAngle / 2f) + 1.5f * (UnityEngine.Random.value > 0.5f ? -1 : 1);
                float angleStep = spreadLines > 1 ? spreadAngle / (spreadLines - 1) : 0f;
                EBulletBatch batch1 = new EBulletBatch();
                for (int i = 0; i < spreadLines; i++)
                {
                    float currentAngle = startAngle1 + (angleStep * i);
                    float radians = currentAngle * Mathf.Deg2Rad;
                    Vector3 offsetPos = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0f).normalized * 0.2f;

                    float finalSpeed = speed;
                    batch1.AddBullet(
                        GetBullet()
                        .SetPosition(firePos + offsetPos)
                        .SetSpeed(finalSpeed, currentAngle)
                    );
                }
                batch1.Packed("Knife", new Vector3(102f, 204f, 0f)).Active();

                yield return new WaitForSeconds(0.05f);
            }

            EBulletBatch batch2 = new EBulletBatch();
            float startAngle2 = GetAngleToPosition(firePos, playerPos) - (spreadAngle / 2f);
            for (int i = 0; i < 6; i++)
            {
                float currentAngle = startAngle2 + (360 / 5 * i);
                float radians = currentAngle * Mathf.Deg2Rad;
                for (int t = 0; t < 12; t++)
                {
                    Vector3 offsetPos = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0f).normalized * 0.8f * (t + 1);
                    batch2.AddBullet(
                      GetBullet()
                        .SetPosition(firePos + offsetPos)
                        .SetSpeed(0.2f, UnityEngine.Random.Range(0f, 360f))
                        .SetAccelarate(new Vector2(0f, -0.6f))
                      );
                }
            }

            batch2.Packed("Small-1", new Vector3(204f, 204f, 204f)).Active();


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
