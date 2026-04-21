using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Wave5", menuName = "STG/Bullet Spawn/Wave/5-A")]
[System.Serializable]
public class Wave5 : EnemyBulletSpawner
{
    private IEnumerator generate(EBContext context)
    {
        EBulletManager.Instance.SpawnWarningLaser(
            "Rice", context.bullet.position, 2f, context.bullet.GetReflectAngle(),
            10f, 0.3f, new Vector3(0f, 0f, 204f));        // 被切一次后不再切
        yield break;
    }

    bool check(in EBContext context)
    {
        return context.bullet.state == EBulletState.ReachBound;
    }

    public override IEnumerator BulletSpawn(ESContext context)
    {
        yield return new WaitForSeconds(2f);
        int bulletCount = 32; // 一圈 36 发 (每 10 度一发)

        float angleStep = 360f / bulletCount;

        float offsetAng = UnityEngine.Random.Range(-angleStep, angleStep);
        while (true)
        {
            EBulletBatch batch = new EBulletBatch();
            for (int i = 0; i < bulletCount; i++)
            {
                float finalAngle = angleStep * i + offsetAng;
                float radians = finalAngle * Mathf.Deg2Rad;
                Vector3 offsetPos = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0f).normalized * 0.2f;

                batch.AddBullet(context.self.position + offsetPos, 3.5f, finalAngle);
            }
            batch.AttachCallBack(check, (context) => generate(context));

            EBulletManager.Instance.SpawnBullet(
                batch.Packed("Crystal", new Vector3(0f, 0f, 204f)));

            yield return new WaitForSeconds(4f);
        }

    }

}

