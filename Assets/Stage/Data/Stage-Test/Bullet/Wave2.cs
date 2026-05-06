using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Wave2-A", menuName = "STG/Bullet Spawn/Extra Stage/2-A")]
[System.Serializable]
public class Wave2A : EnemyBulletSpawner
{
    public float speed = 2.0f;
    public bool isAlways = false;
    public float spreadAngle = 45f;
    public int count = 6;
    public Vector2 fireOffset;

    private bool sync = false;

    public IEnumerator BulletSpawn(ESContext context)
    {
        yield return new WaitUntil(() => moveSteps != 0);
        //yield return new WaitForSeconds(2.5f);

        do
        {
            sync = true;

            int spreadLines = 8;
            Vector3 playerPos = context.player.position;
            Vector3 firePos = context.self.position;

            for (int n = 0; n < count; n++)
            {
                float baseAngle = GetAngleToPosition(firePos, playerPos);
                Quaternion rotation = Quaternion.Euler(0, 0, Vector3.Angle(Vector3.up, firePos - playerPos));

                float finalSpeed = speed + (float)n * 0.30f;
                float startAngle = baseAngle - (spreadAngle / 2f);
                float angleStep = spreadLines > 1 ? spreadAngle / (spreadLines - 1) : 0f;
                EBulletBatch batch = new EBulletBatch();
                for (int i = 0; i < spreadLines; i++)
                {
                    Vector2 originalDir = new Vector2(fireOffset.x * (i - 3.5f) * (n + 1) * 0.1f, fireOffset.y * n);
                    float currentAngle = startAngle + (angleStep * i);
                    float offset = Mathf.Abs(i - 3.5f);
                    batch.AddBullet(
                        GetBullet()
                        .SetPosition(firePos + rotation * new Vector3(originalDir.x, originalDir.y, 0f))
                        .SetSpeed(finalSpeed + offset * 0.5f, currentAngle + Mathf.Sign(i - 3.5f) * n * spreadAngle / count)
                    );
                }
                batch.Packed("Amulet", new Vector3(204f, 102f, 0f)).Active();

                yield return new WaitForSeconds(0.02f);
            }
            yield return new WaitForSeconds(0.8f);
        }
        while (isAlways);
        yield break;
    }

    public IEnumerator CenterSpawn(ESContext context)
    {
        do
        {
            yield return new WaitUntil(() => sync);
            sync = false;
            yield return new WaitForSeconds(0.2f);
            Vector3 playerPos = context.player.position;
            Vector3 firePos = context.self.position;

            for (int n = 0; n < 24; n++)
            {
                if (n % 2 == 1)
                {

                    GetLaser().SetInstant()
                      .SetPosition(context.self.position)
                      .SetSpeed(speed * 2f, GetAngleToPlayer(context.self.position) + 7.5f * n)
                      .SetArea(3.0f, 0.2f)
                      .SetAppearance("Rice", new Vector3(255f, 0f, 0f))
                      .Active();

                    GetLaser().SetInstant()
                      .SetPosition(context.self.position)
                      .SetSpeed(speed * 2f, GetAngleToPlayer(context.self.position) - 7.5f * n)
                      .SetArea(3.0f, 0.2f)
                      .SetAppearance("Rice", new Vector3(255f, 0f, 0f))
                      .Active();
                }

                EBulletBatch batchCenter = new EBulletBatch();
                float baseAngle = GetAngleToPosition(firePos, playerPos);
                batchCenter.AddBullet(
                    GetBullet()
                    .SetPosition(firePos)
                    .SetSpeed(speed + n * 0.2f, baseAngle)
                );
                batchCenter.Packed("Crystal", new Vector3(0f, 0f, 255f)).Active();
                yield return new WaitForSeconds(0.005f);
            }
        }
        while (isAlways);
    }


    public override List<Func<ESContext, IEnumerator>> SpawnerList()
    {
        return new List<Func<ESContext, IEnumerator>>
        {
          (context) => BulletSpawn(context),
          (context) => CenterSpawn(context)
        };
    }

}
