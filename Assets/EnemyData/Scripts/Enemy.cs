using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private int maxHp = 50;
    [SerializeField] private float bodyHitRadius = 0.35f;
    private int type;

    private EnemyBulletSpawner spawner;
    private Player player;


    private EnemyMotion motion;

    private Coroutine moveCoroutine;
    private Coroutine bulletCoroutine;

    private int hp;
    private bool isAlive;


    public Enemy SetPlayer(Player player)
    {
        this.player = player;
        return this;
    }

    public Enemy SetMotion(EnemyMotion motion)
    {
        this.motion = motion;
        return this;
    }

    public Enemy SetSpawner(EnemyBulletSpawner spawner)
    {
        this.spawner = spawner;
        return this;
    }

    public Enemy SetType(int type)
    {
        this.type = type;
        return this;

    }

    public Enemy SetHP(int hp)
    {
        this.hp = hp;
        return this;

    }

    public void OnSpawned()
    {
        StartShoot();
        StartMove();
    }

    public void OnDespawned()
    {
        StopAllCoroutines();
        moveCoroutine = null;
        bulletCoroutine = null;
    }


    private void StartShoot()
    {
        ESContext context = new();
        context.player = player.transform;
        context.self = transform;
        if (spawner != null) bulletCoroutine = StartCoroutine(spawner.BulletSpawn(context));
    }


    private void StartMove()
    {
        EMContext context = new();
        context.self = transform;

        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(MoveCoroutine(context));
    }

    private IEnumerator MoveCoroutine(EMContext context)
    {
        yield return StartCoroutine(motion.Move(context));
        EnemyManager.Instance.DespawnEnemy(this, type);
    }


    public void StopMove()
    {
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = null;
    }

    void OnEnable()
    {
        hp = maxHp;
        isAlive = true;
    }

    void Update()
    {
        if (!isAlive) return;
        CheckBodyCollision();
    }

    public void TakeDamage(int damage)
    {
        if (!isAlive) return;
        hp -= damage;
        if (hp <= 0) Die();
    }

    private void CheckBodyCollision()
    {
        if (player == null) return;

        Transform playerTransform = player.transform;
        float dist = Vector2.Distance(transform.position, playerTransform.position);
        if (dist <= bodyHitRadius /* + player半径 */)
        {
            if (player.Missable()) player.PlayerMiss();
        }
    }

    private void Die()
    {
        isAlive = false;
        //if (bulletCoroutine != null) StopCoroutine(bulletCoroutine);
        gameObject.SetActive(false); // 或播放死亡动画后回收
        StopAllCoroutines();
        moveCoroutine = null;
        bulletCoroutine = null;
    }
}
