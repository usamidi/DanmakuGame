using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Spell Card5", menuName = "STG/Bullet Spawn/Extra Stage/Spell Card5")]
[System.Serializable]
public class SpellCard5 : EnemyBulletSpawner
{
    [Header("散弹的条数 (奇数必有一发正对玩家)")]
    public int spreadLines = 48;
    [Header("扇形总角度")]
    public float spreadAngle = 360f;
    [Header("发射波数")]
    public int wave = 48;

    [Header("偏移")]
    public float offset1 = 0.2f;
    public float offset2 = 0.32f;
    public float offset3 = 0.48f;

    private IEnumerator func1(EBContext context)
    {
        EBulletBatch batch = new EBulletBatch();
        batch.AddBullet(
          GetBullet()
            .SetPosition(context.bullet.position)
            .SetSpeed(0.2f, UnityEngine.Random.Range(260f, 280f))
            .SetAccelarate(new Vector2(0f, -1.2f))
          );

        batch.Packed("Bird", new Vector3(255f, 0f, 0f)).Active();
        yield break;
    }

    bool check1(in EBContext context)
    {
        return context.bullet.timer >= 1.2f;
        //return context.bullet.speed <= 0.1f;
    }

    private IEnumerator func2(EBContext context)
    {
        EBulletBatch batch = new EBulletBatch();
        float angle = context.playerPos.x > context.bullet.position.x ? 0f : 180f;
        batch.AddBullet(
          GetBullet()
            .SetPosition(context.bullet.position)
            .SetSpeed(0.2f, angle + UnityEngine.Random.Range(-3f, 3f))
            .SetSpeedRate(1.2f)
          );

        batch.Packed("Dog", new Vector3(0f, 255f, 0f)).Active();
        yield break;
    }

    bool check2(in EBContext context)
    {
        return Mathf.Abs(context.bullet.position.y - context.playerPos.y) <= 0.2f;
        //return context.bullet.speed <= 0.1f;
    }

    public IEnumerator BulletSpawn(ESContext context)
    {
        yield return new WaitUntil(() => moveSteps != 0);
        while (true)
        {
            float speed = 1.8f;
            Vector3 playerPos = context.player.position;
            float spreadAngle = 345f;
            int spreadLines = 36;
            int sign = UnityEngine.Random.value > 0.5f ? -1 : 1;
            for (int n = 0; n < wave; n++)
            {
                Vector3 firePos = context.self.position;
                EBulletBatch batch1 = new EBulletBatch();
                EBulletBatch batch2 = new EBulletBatch();
                float finalSpeed = speed;
                float currentAngle = UnityEngine.Random.Range(160f, 30f);
                float spinRate = 60f;
                Vector2 acc = new Vector2(0, -1.2f);


                batch1.AddBullet(
                    GetBullet()
                    .SetPosition(firePos)
                    .SetPolar(270 - 60f, offset1)
                    .SetSpinRate(spinRate)
                    .SetSpeed(finalSpeed, currentAngle)
                    .SetAccelarate(acc)
                ).AddBullet(
                    GetBullet()
                    .SetPosition(firePos)
                    .SetPolar(270 + 60f, offset1)
                    .SetSpinRate(spinRate)
                    .SetSpeed(finalSpeed, currentAngle)
                    .SetAccelarate(acc)
                ).AddBullet(
                    GetBullet()
                    .SetPosition(firePos)
                    .SetPolar(270 - 30f, offset2)
                    .SetSpinRate(spinRate)
                    .SetSpeed(finalSpeed, currentAngle)
                    .SetAccelarate(acc)
                ).AddBullet(
                    GetBullet()
                    .SetPosition(firePos)
                    .SetPolar(270 + 30f, offset2)
                    .SetSpinRate(spinRate)
                    .SetSpeed(finalSpeed, currentAngle)
                    .SetAccelarate(acc)
                );
                batch2.AddBullet(
                    GetBullet()
                    .SetPosition(firePos)
                    .SetPolar(270f, offset3)
                    .SetSpinRate(spinRate)
                    .SetSpeed(finalSpeed, currentAngle)
                    .SetAccelarate(acc)
                );
                batch2.AttachCallBack(check1, (context) => func1(context));
                batch2.AttachCallBack(check2, (context) => func2(context));
                batch1.Packed("Rice", new Vector3(0f, 153f, 0f)).Active();
                batch2.Packed("Rice", new Vector3(0f, 153f, 0f)).Active();

                yield return new WaitForSeconds(0.05f);
            }
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
