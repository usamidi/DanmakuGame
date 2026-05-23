using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Spell Card6", menuName = "STG/Bullet Spawn/Extra Stage/Spell Card6")]
[System.Serializable]
public class SpellCard6 : EnemyBulletSpawner
{
    [Header("散弹的条数 (奇数必有一发正对玩家)")]
    public int spreadLines = 48;
    [Header("扇形总角度")]
    public float spreadAngle = 360f;
    [Header("发射波数")]
    public int wave = 12;
    [Header("减速参数")]
    public float param = 1.8f;
    [Header("颜色")]
    public Vector3 color;

    public IEnumerator BulletSpawn(ESContext context)
    {
        yield return new WaitUntil(() => moveSteps != 0);
        while (true)
        {
            int spreadLines = 36;
            float spreadAngle = 330f;
            float speed = 3.5f;
            Vector3 playerPos = context.player.position;
            Vector3 firePos = context.self.position;
            float baseAngle = 90f;
            float startAngle = baseAngle - (spreadAngle / 2f);
            float angleStep = spreadLines > 1 ? spreadAngle / (spreadLines - 1) : 0f;
            for (int i = 0; i < spreadLines; i++)
            {
                float currentAngle = startAngle + (angleStep * i);
                GetLaser()
                  .SetInstant()
                  .SetPosition(firePos)
                  .SetSpeed(speed, currentAngle)
                  .SetArea(4f, 0.3f)
                  .SetAppearance("Rice", new Vector3(255f, 128f, 0f))
                  .Active();
            }
            yield return new WaitForSeconds(3f);
        }
    }

    public IEnumerator Water(ESContext context)
    {
        yield return new WaitUntil(() => moveSteps != 0);
        GetLaser()
          .SetWarning()
          .SetPosition(new Vector3(-3.8f, -3.6f, 0f))
          .SetSpeed(0, 0f)
          .SetArea(12f, 1.2f)
          .SetDuration(2.2f)
          .SetAppearance("Laser", new Vector3(204f, 204f, 0f))
          .Active();
        yield return new WaitForSeconds(2f);
        float speed = 10f;
        while (true)
        {
            Vector3 firePos = new Vector3(-4f, 0f, 0f);
            EBulletBatch batch1 = new EBulletBatch();
            for (int i = 0; i < 12; i++)
            {
                firePos.y = UnityEngine.Random.Range(-3.3f, -4.0f);
                float angle = 0f + UnityEngine.Random.Range(-3f, 3f);
                batch1.AddBullet(
                    GetBullet()
                    .SetPosition(firePos)
                    .SetSpeed(speed + UnityEngine.Random.Range(-0.8f, 0.8f), angle)
                    .SetSpeedRate(2.0f)
                );
            }

            EBulletBatch batch2 = new EBulletBatch();
            for (int i = 0; i < 12; i++)
            {
                firePos.y = UnityEngine.Random.Range(-3.3f, -4.0f);
                float angle = 0f + UnityEngine.Random.Range(-3f, 3f);
                batch2.AddBullet(
                    GetBullet()
                    .SetPosition(firePos)
                    .SetSpeed(speed + UnityEngine.Random.Range(-0.8f, 0.8f), angle)
                    .SetSpeedRate(2.0f)
                );
            }
            EBulletBatch batch3 = new EBulletBatch();
            for (int i = 0; i < 8; i++)
            {
                firePos.y = UnityEngine.Random.Range(-3.3f, -4.0f);
                float angle = 0f + UnityEngine.Random.Range(-3f, 3f);
                batch3.AddBullet(
                    GetBullet()
                    .SetPosition(firePos)
                    .SetSpeed(speed + UnityEngine.Random.Range(-0.8f, 0.8f), angle)
                    .SetSpeedRate(2.0f)
                );
            }

            batch1.Packed("Small-1", new Vector3(153f, 76f, 0f)).Active();
            batch2.Packed("Small-2", new Vector3(153f, 153f, 0f)).Active();
            batch3.Packed("Big", new Vector3(102f, 51f, 0f)).Active();
            yield return new WaitForSeconds(0.05f);

        }
    }

    public IEnumerator Float(ESContext context)
    {
        yield return new WaitUntil(() => moveSteps != 0);
        yield return new WaitForSeconds(2.5f);
        Vector3 firePos = new Vector3();
        Vector2 acc = new Vector2(0, -1.6f);
        float speed = 4f;
        float angle = 0f;
        while (true)
        {
            EBulletBatch batch1 = new EBulletBatch();
            for (int i = 0; i < 2; i++)
            {
                angle = 45f + UnityEngine.Random.Range(-15f, 15f);
                firePos.x = UnityEngine.Random.Range(-4.0f, 4.0f);
                firePos.y = UnityEngine.Random.Range(-4.0f, -3.3f);

                batch1.AddBullet(
                    GetBullet()
                    .SetPosition(firePos)
                    .SetSpeed(speed + UnityEngine.Random.Range(-0.8f, 0.8f), angle)
                    .SetAccelarate(acc)
                );
            }

            EBulletBatch batch2 = new EBulletBatch();
            for (int i = 0; i < 2; i++)
            {
                angle = 45f + UnityEngine.Random.Range(-15f, 15f);
                firePos.x = UnityEngine.Random.Range(-4.0f, 4.0f);
                firePos.y = UnityEngine.Random.Range(-4.0f, -3.3f);

                batch2.AddBullet(
                    GetBullet()
                    .SetPosition(firePos)
                    .SetSpeed(speed + UnityEngine.Random.Range(-0.8f, 0.8f), angle)
                    .SetAccelarate(acc)
                );
            }

            EBulletBatch batch3 = new EBulletBatch();
            angle = 45f + UnityEngine.Random.Range(-15f, 15f);
            firePos.x = UnityEngine.Random.Range(-4.0f, 4.0f);
            firePos.y = UnityEngine.Random.Range(-4.0f, -3.3f);
            batch3.AddBullet(
                GetBullet()
                .SetPosition(firePos)
                .SetSpeed(speed + UnityEngine.Random.Range(-0.8f, 0.8f), angle)
                .SetAccelarate(acc)
            );

            batch1.Packed("Small-1", new Vector3(153f, 76f, 0f)).Active();
            batch2.Packed("Small-2", new Vector3(153f, 153f, 0f)).Active();
            batch3.Packed("Big", new Vector3(102f, 51f, 0f)).Active();
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.2f, 0.4f));
        }
    }

    public override List<Func<ESContext, IEnumerator>> SpawnerList()
    {
        return new List<Func<ESContext, IEnumerator>>
        {
          (context) => BulletSpawn(context),
          (context) => Water(context),
          (context) => Float(context)
        };
    }
}
