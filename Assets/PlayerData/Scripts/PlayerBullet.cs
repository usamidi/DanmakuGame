
using UnityEngine;
using UnityEngine.Pool; // 必须引用

public class PlayerBullet : MonoBehaviour
{
    [Header("属性设置")]
    public float speed = 15f;
    public int damage = 1;            // 伤害值
    public Vector2 direction = Vector2.up;

    // 核心：引用所属的对象池，用于回收自己
    private IObjectPool<PlayerBullet> _pool;

    // 设置对象池引用
    public void SetPool(IObjectPool<PlayerBullet> pool)
    {
        _pool = pool;
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);

        // 超过边界时自动回收（假设 y > 10 为出屏）
        if (transform.position.y > 10f || transform.position.y < -10f ||
            transform.position.x > 6f || transform.position.x < -6f)
        {
            Deactivate();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy != null && gameObject.activeSelf != false)
        {
            enemy.TakeDamage(damage);
            Deactivate(); // 回收子弹
        }
    }

    // 回收子弹的统一入口
    void Deactivate()
    {
        // 不再是 Destroy，而是告诉池子：我不用了
        _pool.Release(this);
    }
}
