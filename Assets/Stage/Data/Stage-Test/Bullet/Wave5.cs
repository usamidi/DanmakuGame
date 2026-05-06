using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Wave5", menuName = "STG/Bullet Spawn/Extra Stage/5-A")]
[System.Serializable]
public class Wave5 : EnemyBulletSpawner
{
    public float speed = 1.5f;
    public float speedRate = -0.8f;

    private IEnumerator laser(EBContext context)
    {
        GetLaser()
          .SetWarning()
          .SetPosition(context.bullet.position)
          .SetSpeed(0f, context.bullet.GetReflectAngle())
          .SetArea(10f, 0.3f)
          .SetDuration(2f)
          .SetAppearance("Rice", new Vector3(0f, 0f, 204f))
          .Active();
        yield break;
    }


    bool check(in EBContext context)
    {
        return context.bullet.state == EBulletState.ReachBound;
    }

    private IEnumerator bomb(EBContext context)
    {
        //float offsetAng = UnityEngine.Random.Range(-angleStep, angleStep);
        context.bullet
          .SetPosition(context.bullet.position)
          .SetSpeed(3f, context.bullet.rotation)
          .SetSpinRate(0f)
          .SetPolar(0f, 0f)
          .SetSpeedRate(1f);
        yield break;
    }

    private IEnumerator bombCenter(EBContext context)
    {
        int bulletCount = 12;
        float angleStep = 360f / bulletCount;
        EBulletBatch batch = new EBulletBatch();
        for (int i = 0; i < bulletCount; i++)
        {
            float finalAngle = angleStep * i;
            float radians = finalAngle * Mathf.Deg2Rad;
            Vector2 offsetPos = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)).normalized * 0.2f;
            batch.AddBullet(
                GetBullet()
                .SetPosition(context.bullet.position + offsetPos)
                .SetSpeed(0.2f, finalAngle)
                .SetSpeedRate(1.5f)
            );
        }
        batch.Packed("Butterfly", new Vector3(204f, 0f, 102f)).Active();
        yield return new WaitForSeconds(0.1f);
        context.bullet.state = EBulletState.Dying;
        yield break;
    }

    bool checkBomb(in EBContext context)
    {
        return context.bullet.speed < 0.5f;
    }

    public IEnumerator BulletSpawn(ESContext context)
    {
        yield return new WaitUntil(() => moveSteps != 0);
        int bulletCount = 3;

        float angleStep = 360f / bulletCount;

        while (true)
        {
            EBulletBatch batch1 = new EBulletBatch();
            EBulletBatch batch2 = new EBulletBatch();
            EBulletBatch batch3 = new EBulletBatch();
            float offsetAng = GetAngleToPosition(context.self.position, context.player.position);
            for (int i = 0; i < bulletCount; i++)
            {
                float finalAngle = angleStep * i + offsetAng;
                float radians = finalAngle * Mathf.Deg2Rad;
                Vector3 offsetPos = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0f).normalized * 0.2f;
                for (int j = 0; j < 12; j++)
                {
                    batch2.AddBullet(
                        GetBullet()
                        .SetPosition(context.self.position + offsetPos)
                        .SetSpeed(speed, finalAngle)
                        .SetPolar(360f / 12f * j, 0.4f)
                        .SetSpinRate(120f)
                        .SetSpeedRate(speedRate)
                    );
                }

                for (int j = 0; j < 16; j++)
                {
                    batch3.AddBullet(
                        GetBullet()
                        .SetPosition(context.self.position + offsetPos)
                        .SetSpeed(speed, finalAngle)
                        .SetPolar(360f / 16f * j, 0.5f)
                        .SetSpinRate(120f)
                        .SetSpeedRate(speedRate)
                    );
                }

                batch1.AddBullet(
                    GetBullet()
                    .SetPosition(context.self.position + offsetPos)
                    .SetSpeed(speed, finalAngle)
                    .SetSpeedRate(speedRate)
                );
            }

            batch1.AttachCallBack(checkBomb, (context) => bombCenter(context));
            batch2.AttachCallBack(checkBomb, (context) => bomb(context));
            batch3.AttachCallBack(checkBomb, (context) => bomb(context));
            batch3.AttachCallBack(check, (context) => laser(context));

            batch1.Packed("Big", new Vector3(204f, 0f, 0f)).Active();
            batch2.Packed("Small-2", new Vector3(204f, 102f, 0f)).Active();
            batch3.Packed("Chain", new Vector3(102f, 204f, 0f)).Active();

            yield return new WaitForSeconds(4f);
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

