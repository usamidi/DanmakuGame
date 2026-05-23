using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Spell Card3", menuName = "STG/Bullet Spawn/Extra Stage/Spell Card3")]
[System.Serializable]
public class SpellCard3 : EnemyBulletSpawner
{
    public float speed = 2.5f;
    public float speedRate = 0.2f;

    private IEnumerator bomb(EBContext context)
    {
        context.bullet
          .SetPosition(context.bullet.position)
          .SetSpeed(0.8f, context.bullet.rotation)
          .SetSpinRate(0f)
          .SetPolar(0f, 0f)
          .SetLimit(new Vector2(0.2f, -1f))
          .SetSpeedRate(-0.3f);
        yield break;
    }

    bool check(in EBContext context)
    {
        return context.bullet.isGrazed == true ||
          (context.bullet.state == EBulletState.ReachBound && context.bullet.boundType != EBBoundType.Bottom); ;
    }

    public IEnumerator BulletSpawn(ESContext context)
    {
        yield return new WaitUntil(() => moveSteps != 0);
        int bulletCount = 5;

        float angleStep = 360f / bulletCount;
        float offsetAng = GetAngleToPosition(context.self.position, context.player.position);

        while (true)
        {
            offsetAng += 15f;
            EBulletBatch batch1 = new EBulletBatch();
            EBulletBatch batch2 = new EBulletBatch();
            EBulletBatch batch3 = new EBulletBatch();
            for (int i = 0; i < bulletCount; i++)
            {
                float finalAngle = angleStep * i + offsetAng;
                float radians = finalAngle * Mathf.Deg2Rad;
                Vector3 offsetPos = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0f).normalized * 0.2f;
                for (int j = 0; j < 5; j++)
                {
                    batch2.AddBullet(
                        GetBullet()
                        .SetPosition(context.self.position + offsetPos)
                        .SetSpeed(speed, finalAngle)
                        .SetPolar(UnityEngine.Random.Range(0f, 360f), UnityEngine.Random.Range(0f, 0.4f))
                        .SetSpinRate(90f)
                        .SetSpeedRate(speedRate)
                    );
                }

                for (int j = 0; j < 10; j++)
                {
                    batch3.AddBullet(
                        GetBullet()
                        .SetPosition(context.self.position + offsetPos)
                        .SetSpeed(speed, finalAngle)
                        .SetPolar(UnityEngine.Random.Range(0f, 360f), UnityEngine.Random.Range(0f, 0.4f))
                        .SetSpinRate(90f)
                        .SetSpeedRate(speedRate)
                    );
                }

                batch1.AddBullet(
                    GetBullet()
                    .SetPosition(context.self.position + offsetPos)
                    .SetSpeed(speed, finalAngle)
                    .SetPolar(UnityEngine.Random.Range(0f, 360f), UnityEngine.Random.Range(0f, 0.5f))
                    .SetSpinRate(90f)
                    .SetSpeedRate(speedRate)
                );
            }
            batch1.AttachCallBack(check, (context) => bomb(context));
            batch2.AttachCallBack(check, (context) => bomb(context));
            batch3.AttachCallBack(check, (context) => bomb(context));

            batch1.Packed("Medium", new Vector3(204f, 204f, 204f)).Active();
            batch2.Packed("Small-2", new Vector3(224f, 224f, 224f)).Active();
            batch3.Packed("Small-1", new Vector3(189f, 189f, 189f)).Active();

            yield return new WaitForSeconds(0.5f);
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

