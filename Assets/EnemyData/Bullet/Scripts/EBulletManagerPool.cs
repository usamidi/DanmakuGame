using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;



public partial class EBulletManager : MonoBehaviour
{
    private IObjectPool<EBulletRenderBatch> pool;
    private IObjectPool<EBulletData> bulletPool;

    public EBulletRenderBatch GetRenderBatch()
    {
        return pool.Get();
    }

    public EBulletData GetBullet()
    {
        return bulletPool.Get();
    }
    public void ReleaseBullet(EBulletData b)
    {
        bulletPool.Release(b);
    }

    // 初始化对象池
    private void initObjectPool()
    {
        pool = new ObjectPool<EBulletRenderBatch>(
            createFunc: () =>
            {
                return new EBulletRenderBatch();
            },       // 1. 创建函数：池子空了怎么造新子弹
            actionOnGet: (ebrb) =>
            {
                renderBatches.Add(ebrb);
            },        // 2. 取出函数：从池子拿出时要做什么
            actionOnRelease: (ebrb) =>
            {
                ebrb.Clear();
                renderBatches.Remove(ebrb);
            },    // 3. 回收函数：放回池子时要做什么
            actionOnDestroy: (ebrb) =>
            {
                ebrb = null;
            },    // 4. 销毁函数：超过上限彻底删掉
            collectionCheck: true,
            defaultCapacity: 800,
            maxSize: 2048
        );

        bulletPool = new ObjectPool<EBulletData>(
            createFunc: () =>
            {
                return new EBulletData();
            },       // 1. 创建函数：池子空了怎么造新子弹
            actionOnGet: (b) =>
            {
            },        // 2. 取出函数：从池子拿出时要做什么
            actionOnRelease: (b) =>
            {
                b.Clear();
            },    // 3. 回收函数：放回池子时要做什么
            actionOnDestroy: (b) =>
            {
                b = null;
            },    // 4. 销毁函数：超过上限彻底删掉
            collectionCheck: true,
            defaultCapacity: 2048,
            maxSize: 16384
            );
    }
}
