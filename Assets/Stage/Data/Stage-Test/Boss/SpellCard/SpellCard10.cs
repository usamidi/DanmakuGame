using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Spell Card10", menuName = "STG/Bullet Spawn/Extra Stage/Spell Card10")]
[System.Serializable]
public class SpellCard10 : EnemyBulletSpawner
{
    [Header("扇形总角度")]
    public float spreadAngle = 360f;
    [Header("发射波数")]
    public int wave = 48;

    private IEnumerator func(EBContext context)
    {
        EBulletBatch batch = new EBulletBatch();
        Vector2 acc = new();
        if (context.bullet.boundType == EBBoundType.Top)
        {
            acc.x = 0f;
            acc.y = 8f;
        }
        else if (context.bullet.boundType == EBBoundType.Left)
        {
            acc.x = -8f;
            acc.y = 0f;
        }
        else if (context.bullet.boundType == EBBoundType.Right)
        {
            acc.x = 8f;
            acc.y = 0f;
        }
        batch.AddBullet(
          GetBullet()
            .SetPosition(context.bullet.position)
            .SetSpeed(6f, context.bullet.GetReflectAngle())
            .SetAccelarate(acc)
          );

        batch.Packed("Small-1", new Vector3(204f, 204f, 204f)).Active();
        yield break;
    }

    bool check(in EBContext context)
    {
        return context.bullet.state == EBulletState.ReachBound && context.bullet.boundType != EBBoundType.Bottom;
    }

    public IEnumerator BulletSpawn1(ESContext context)
    {
        yield return new WaitUntil(() => moveSteps != 0);
        float baseAngle = 0f;
        int param = 0;
        while (true)
        {
            float speed = 5.0f;
            Vector3 playerPos = context.player.position;
            Vector3 firePos = context.self.position;
            float spreadAngle = 60f;
            int spreadLines = 8;
            int sign = UnityEngine.Random.value > 0.5f ? -1 : 1;

            EBulletBatch batch = new EBulletBatch();
            float startAngle = baseAngle - (spreadAngle / 2f);
            for (int i = 0; i < spreadLines; i++)
            {
                float currentAngle = startAngle + (spreadAngle / spreadLines * i);
                float radians = currentAngle * Mathf.Deg2Rad;
                Vector3 offsetPos = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0f).normalized * 0.1f;
                batch.AddBullet(
                  GetBullet()
                    .SetPosition(firePos + offsetPos)
                    .SetSpeed(2.2f, currentAngle + 180f)
                  );
            }

            batch.Packed("Chain", new Vector3(0f, 255f, 0f)).Active();
            baseAngle += 15f;
            yield return new WaitForSeconds(0.2f);
            param++;
        }
    }

    public IEnumerator BulletSpawn2(ESContext context)
    {
        yield return new WaitUntil(() => moveSteps != 0);
        yield return new WaitForSeconds(6f);
        float baseAngle = 180f;
        int param = 0;
        while (true)
        {
            float speed = 5.0f;
            Vector3 playerPos = context.player.position;
            Vector3 firePos = context.self.position;
            float spreadAngle = 60f;
            int spreadLines = 12;
            int sign = UnityEngine.Random.value > 0.5f ? -1 : 1;

            EBulletBatch batch = new EBulletBatch();
            float startAngle = baseAngle - (spreadAngle / 2f);
            for (int i = 0; i < spreadLines; i++)
            {
                float currentAngle = startAngle + (spreadAngle / spreadLines * i);
                float radians = currentAngle * Mathf.Deg2Rad;
                Vector3 offsetPos = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0f).normalized * 0.1f;
                batch.AddBullet(
                  GetBullet()
                    .SetPosition(firePos + offsetPos)
                    .SetSpeed(2.2f, currentAngle + 180f)
                  );
            }

            batch.Packed("Scale", new Vector3(255f, 102f, 0f)).Active();
            baseAngle -= 15f;
            yield return new WaitForSeconds(0.2f);
            param++;
        }
    }

    public IEnumerator BulletSpawn3(ESContext context)
    {
        yield return new WaitUntil(() => moveSteps != 0);
        yield return new WaitForSeconds(12f);
        float baseAngle = 270f;
        int param = 0;
        while (true)
        {
            float speed = 5.0f;
            Vector3 playerPos = context.player.position;
            Vector3 firePos = context.self.position;
            float spreadAngle = 60f;
            int spreadLines = 10;
            int sign = UnityEngine.Random.value > 0.5f ? -1 : 1;

            EBulletBatch batch = new EBulletBatch();
            float startAngle = baseAngle - (spreadAngle / 2f);
            for (int i = 0; i < spreadLines; i++)
            {
                float currentAngle = startAngle + (spreadAngle / spreadLines * i);
                float radians = currentAngle * Mathf.Deg2Rad;
                Vector3 offsetPos = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0f).normalized * 0.1f;
                batch.AddBullet(
                  GetBullet()
                    .SetPosition(firePos + offsetPos)
                    .SetSpeed(2.2f, currentAngle + 180f)
                  );
            }

            batch.Packed("Bullet", new Vector3(102f, 0f, 255f)).Active();
            baseAngle += 15f;
            yield return new WaitForSeconds(0.2f);
            param++;
        }
    }

    public IEnumerator BulletSpawn4(ESContext context)
    {
        yield return new WaitUntil(() => moveSteps != 0);
        yield return new WaitForSeconds(16f);
        float baseAngle = 90f;
        int param = 0;
        while (true)
        {
            float speed = 5.0f;
            Vector3 playerPos = context.player.position;
            Vector3 firePos = context.self.position;
            float spreadAngle = 60f;
            int spreadLines = 10;
            int sign = UnityEngine.Random.value > 0.5f ? -1 : 1;

            EBulletBatch batch = new EBulletBatch();
            float startAngle = baseAngle - (spreadAngle / 2f);
            for (int i = 0; i < spreadLines; i++)
            {
                float currentAngle = startAngle + (spreadAngle / spreadLines * i);
                float radians = currentAngle * Mathf.Deg2Rad;
                Vector3 offsetPos = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0f).normalized * 0.1f;
                batch.AddBullet(
                  GetBullet()
                    .SetPosition(firePos + offsetPos)
                    .SetSpeed(2.2f, currentAngle + 180f)
                  );
            }

            batch.AttachCallBack(check, (context) => func(context));
            batch.Packed("Rice", new Vector3(102f, 102f, 102f)).Active();
            baseAngle -= 15f;
            yield return new WaitForSeconds(0.2f);
            param++;
        }
    }

    public override List<Func<ESContext, IEnumerator>> SpawnerList()
    {
        return new List<Func<ESContext, IEnumerator>>
        {
          (context) => BulletSpawn1(context),
          (context) => BulletSpawn2(context),
          (context) => BulletSpawn3(context),
          (context) => BulletSpawn4(context),
        };
    }
}
