using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Wave1-A", menuName = "STG/Bullet Spawn/Extra Stage/1-A")]
[System.Serializable]
public class Wave1A : EnemyBulletSpawner
{
    [Header("散弹的条数 (奇数必有一发正对玩家)")]
    public int spreadLines = 5;
    [Header("扇形总角度")]
    public float spreadAngle = 60f;
    public float speedRate = 0.5f;

    private IEnumerator func(EBContext context)
    {
        float angle = UnityEngine.Random.Range(-45f, 45f);
        context.bullet.Rotate(angle);
        yield break;
    }

    bool check(in EBContext context)
    {
        return context.bullet.speed <= 0.8f;
    }

    public IEnumerator BulletSpawn(ESContext context)
    {
        //yield return new WaitUntil(() => moveSteps == 0);
        while (true)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 0.3f));
            float speed = 5f;
            Vector3 playerPos = context.player.position;
            Vector3 firePos = context.self.position;
            float baseAngle = GetAngleToPosition(firePos, playerPos);
            for (int n = 0; n < 4; n++)
            {

                float finalSpeed = speed;
                float startAngle = baseAngle - (spreadAngle / 2f);
                float angleStep = spreadLines > 1 ? spreadAngle / (spreadLines - 1) : 0f;
                EBulletBatch batch = new EBulletBatch();
                for (int i = 0; i < spreadLines; i++)
                {
                    float currentAngle = startAngle + (angleStep * i);
                    batch.AddBullet(
                        GetBullet()
                        .SetPosition(firePos)
                        .SetSpeedRate(speedRate)
                        .SetLimit(new Vector2(0.5f, -1f))
                        .SetSpeed(finalSpeed, currentAngle)
                    );
                }
                batch.AttachCallBack(check, (context) => func(context));

                batch.Packed("Scale", new Vector3(0f, 0f, 255f)).Active();

                yield return new WaitForSeconds(0.08f);
            }
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
