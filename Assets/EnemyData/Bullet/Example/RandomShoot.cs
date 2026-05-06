using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RandomShoot", menuName = "STG/Bullet Spawn/Random Shoot")]
[System.Serializable]
public class ERandomShoot : EnemyBulletSpawner
{
    [Header("弹幕样式名称")]
    public string style;

    [Header("弹幕深度")]
    public float depth = 1f;

    [Header("弹幕颜色")]
    public Vector3 color;

    [Header("一次性数量")]
    public int bulletCount;

    [Header("发射次数（小于等于0表示无限）")]
    public int spawnLimit;

    [Header("速度范围")]
    public Vector2 speedLimit;

    [Header("随机范围")]
    public float randomLimit;

    [Header("间隔时间")]
    public float interval;


    public IEnumerator BulletSpawn(ESContext context)
    {
        for (int t = 0; t < spawnLimit || spawnLimit <= 0; t++)
        {
            Vector2 position = context.self.position;
            EBulletBatch batch = new EBulletBatch();
            for (int i = 0; i < bulletCount; i++)
            {
                Vector3 pos = UnityEngine.Random.insideUnitCircle * randomLimit;

                float angle = UnityEngine.Random.Range(0f, 360f);

                float speed = Mathf.Lerp(speedLimit.x, speedLimit.y, UnityEngine.Random.Range(0f, 1f));

                batch.AddBullet(
                    GetBullet()
                    .SetPosition(position + new Vector2(pos.x, pos.y), depth)
                    .SetSpeed(speed, angle)
                );
            }
            batch.Packed(style, color).Active();
            yield return new WaitForSeconds(interval);
        }
        yield break;

    }

    public override List<Func<ESContext, IEnumerator>> SpawnerList()
    {
        return new List<Func<ESContext, IEnumerator>>
        {
          (context) => BulletSpawn(context)
        };
    }
}
