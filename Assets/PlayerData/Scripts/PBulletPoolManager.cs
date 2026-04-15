using UnityEngine;
using UnityEngine.Pool;

public class PBulletManager : MonoBehaviour
{
    // 单例，方便 Player 随时调用
    public static PBulletManager Instance;

    [SerializeField] private PlayerBullet bulletPrefab;
    [SerializeField] private int defaultCapacity = 100; // 初始容量
    [SerializeField] private int maxPoolSize = 1000;    // 最大上限

    private IObjectPool<PlayerBullet> _pool;

    void Awake()
    {
        Instance = this;

        // 初始化对象池
        _pool = new ObjectPool<PlayerBullet>(
            CreateBullet,       // 1. 创建函数：池子空了怎么造新子弹
            OnGetBullet,        // 2. 取出函数：从池子拿出时要做什么
            OnReleaseBullet,    // 3. 回收函数：放回池子时要做什么
            OnDestroyBullet,    // 4. 销毁函数：超过上限彻底删掉
            collectionCheck: true,
            defaultCapacity,
            maxPoolSize
        );
    }

    // 1. 创建逻辑
    private PlayerBullet CreateBullet()
    {
        PlayerBullet b = Instantiate(bulletPrefab);
        b.SetPool(_pool); // 告诉子弹它属于哪个池子
        return b;
    }

    // 2. 取出逻辑
    private void OnGetBullet(PlayerBullet bullet)
    {
        bullet.gameObject.SetActive(true);
    }

    // 3. 回收逻辑
    private void OnReleaseBullet(PlayerBullet bullet)
    {
        bullet.gameObject.SetActive(false);
    }

    // 4. 彻底销毁
    private void OnDestroyBullet(PlayerBullet bullet)
    {
        Destroy(bullet.gameObject);
    }

    // 外部调用的接口
    public PlayerBullet GetBullet()
    {
        return _pool.Get();
    }
}
