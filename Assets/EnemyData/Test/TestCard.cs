using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Test", menuName = "STG/Bullet Spawn/Spell Card/Test")]
[System.Serializable]
public class TestCard : EnemyBulletSpawner
{
    public string type;
    public string typeBig;

    private IEnumerator generate(EBContext context)
    {
        yield return new WaitForSeconds(6f);
        context.bullet.SetTurnRate(0f);
        yield break;
    }

    bool check(in EBContext context)
    {
        return false;
    }

    private IEnumerator BigSpawn(ESContext context)
    {
        while (true)
        {
            EBulletBatch batchBig = new EBulletBatch();
            float angleStep = 360f / 8f;
            float offsetAng = UnityEngine.Random.Range(-angleStep, angleStep);
            for (int i = 0; i < 8; i++)
            {
                float finalAngle = angleStep * i + offsetAng;
                float radians = finalAngle * Mathf.Deg2Rad;
                Vector3 offsetPos = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0f).normalized * 0.2f;

                batchBig.AddBullet(
                    GetBullet()
                    .SetPosition(context.self.position + offsetPos)
                    .SetSpeed(1.8f, finalAngle)
                );
            }
            batchBig.Packed(typeBig, new Vector3(204f, 0f, 0f)).Active();
            yield return new WaitForSeconds(1.2f);
        }
    }

    public IEnumerator BulletSpawn(ESContext context)
    {
        yield return new WaitForSeconds(2f);
        //StartCoroutine(BigSpawn(context));
        int bulletCount = 12; // 一圈 36 发 (每 10 度一发)

        float angleStep = 360f / bulletCount;

        /*
        EBulletManager.Instance.SpawnWarningLaser(
            "Laser", context.self.position, 200f, 300f,
            10f, 0.6f, new Vector3(0f, 0f, 204f)
            ).SpawnWarningLaser(
            "Laser", context.self.position, 200f, 240f,
            10f, 0.6f, new Vector3(0f, 0f, 204f));
        */

        while (true)
        {
            EBulletBatch batch = new EBulletBatch();
            EBulletBatch batchCenter = new EBulletBatch();
            float offsetAng = UnityEngine.Random.Range(-angleStep, angleStep);
            angleStep = 360f / bulletCount;
            for (int i = 0; i < bulletCount; i++)
            {
                float finalAngle = angleStep * i + offsetAng;
                float radians = finalAngle * Mathf.Deg2Rad;
                Vector3 offsetPos = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0f).normalized * 0.2f;

                for (int j = 0; j < 5; j++)
                {
                    batch.AddBullet(
                        GetBullet()
                        .SetPosition(context.self.position + offsetPos)
                        .SetSpeed(1.5f, finalAngle)
                        .SetPolar(360f / 5f * j, 0.2f)
                        .SetSpinRate(120f)
                    //.SetAccelarate(new Vector2(0f, -4f), 4f)
                    );
                }

                batchCenter.AddBullet(
                    GetBullet()
                    .SetPosition(context.self.position + offsetPos, 0.1f)
                    .SetSpeed(1.5f, finalAngle)
                );
            }
            //batch.AttachCallBack(check, (context) => generate(context));

            batch.Packed(type, new Vector3(204f, 0f, 102f)).Active();

            /*
            EBulletManager.Instance.SpawnBullet(
                batchCenter.Packed("Small-1", new Vector3(102f, 0f, 102f)));
                */

            yield return new WaitForSeconds(0.4f);
        }
    }

    public override List<Func<ESContext, IEnumerator>> SpawnerList()
    {
        return new List<Func<ESContext, IEnumerator>>
        {
          (context) => BigSpawn(context),
          (context) => BulletSpawn(context)
        };
    }
}
