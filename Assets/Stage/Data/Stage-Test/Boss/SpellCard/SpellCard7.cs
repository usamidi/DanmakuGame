using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Spell Card7", menuName = "STG/Bullet Spawn/Extra Stage/Spell Card7")]
[System.Serializable]
public class SpellCard7 : EnemyBulletSpawner
{
    [Header("扇形总角度")]
    public float spreadAngle = 360f;
    [Header("发射波数")]
    public int wave = 48;

    private IEnumerator func(EBContext context)
    {
        EBulletBatch batch = new EBulletBatch();
        batch.AddBullet(
          GetBullet()
            .SetPosition(context.bullet.position)
            .SetSpeed(0.2f, UnityEngine.Random.Range(265f, 275f))
            .SetAccelarate(new Vector2(0f, -0.8f))
          );

        batch.Packed("Small-2", new Vector3(255f, 255f, 0f)).Active();
        yield break;
    }

    bool check1(in EBContext context)
    {
        return context.bullet.timer >= 1f;
    }

    bool check2(in EBContext context)
    {
        return context.bullet.timer >= 2f;
    }

    bool check3(in EBContext context)
    {
        return context.bullet.timer >= 3f;
    }

    public IEnumerator BulletSpawn(ESContext context)
    {
        yield return new WaitUntil(() => moveSteps != 0);
        float baseAngle = 0f;
        int param = 0;
        while (true)
        {
            float speed = 5.0f;
            Vector3 playerPos = context.player.position;
            Vector3 firePos = context.self.position;
            float spreadAngle = 360f;
            int spreadLines = 8;
            int sign = UnityEngine.Random.value > 0.5f ? -1 : 1;

            EBulletBatch batch2 = new EBulletBatch();
            float startAngle = baseAngle - (spreadAngle / 2f);
            for (int i = 0; i < spreadLines; i++)
            {
                float currentAngle = startAngle + (360 / spreadLines * i);
                float radians = currentAngle * Mathf.Deg2Rad;
                Vector3 offsetPos = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0f).normalized * 0.4f;
                batch2.AddBullet(
                  GetBullet()
                    .SetPosition(firePos + offsetPos)
                    .SetSpeed(2.2f, currentAngle + 180f)
                  );
            }
            if (param % 8 == 0)
            {
                batch2.AttachCallBack(check1, (context) => func(context));
                batch2.AttachCallBack(check2, (context) => func(context));
                batch2.AttachCallBack(check3, (context) => func(context));
            }

            batch2.Packed("Big", new Vector3(233f, 233f, 0f)).Active();
            baseAngle += 15f;
            yield return new WaitForSeconds(0.3f);
            param++;
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
