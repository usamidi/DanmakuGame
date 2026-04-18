using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Wave4", menuName = "STG/Bullet Spawn/Wave/4-A")]
[System.Serializable]
public class Wave4 : EnemyBulletSpawner
{
    public override IEnumerator BulletSpawn(ESContext context)
    {
        yield return new WaitForSeconds(2f);
        int bulletCount = 10; // 一圈 36 发 (每 10 度一发)

        float angleStep = 360f / bulletCount;

        float offsetAng = 0f;

        float param = 0f;
        while (true)
        {
            offsetAng = Mathf.Cos(param * Mathf.Deg2Rad) * 1080f + 90f;
            EBulletBatch batch = new EBulletBatch();
            for (int i = 0; i < bulletCount; i++)
            {
                float finalAngle = angleStep * i + offsetAng;
                float radians = finalAngle * Mathf.Deg2Rad;
                Vector3 offsetPos = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0f).normalized * 0.2f;

                batch.AddBullet(context.self.position + offsetPos, 3.5f, finalAngle);
            }

            EBulletManager.Instance.SpawnBullet(
                batch.Packed("Chain", new Vector3(102f, 0f, 204f)));

            yield return new WaitForSeconds(0.03f);
            param += 1.2f;
        }

    }

}

