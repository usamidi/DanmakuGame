using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct DroppedItems
{
    public ItemType main;
    public int power;
    public int score;

    public void Clear()
    {
        main = ItemType.Score;
        power = 0;
        score = 0;
    }
}

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] protected int maxHp = 50;
    [SerializeField] protected float bodyHitRadius = 0.35f;
    protected int type;

    protected EnemyBulletSpawner spawner;
    protected Player player;

    protected EnemyMotion motion;

    protected Coroutine moveCoroutine;
    protected List<Coroutine> bulletCoroutine = new();

    protected int hp;
    protected bool isAlive;

    protected DroppedItems items;


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
        this.spawner = Instantiate(spawner);
        this.spawner.StartCoroutine = StartCoroutine;
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

    public Enemy SetItems(DroppedItems items)
    {
        this.items = items;
        return this;

    }

    public virtual void OnSpawned()
    {
        StartShoot();
        StartMove();
    }

    public virtual void OnDespawned()
    {
        StopAllCoroutines();
        items.Clear();
        moveCoroutine = null;
        bulletCoroutine = null;
    }


    private void StartShoot()
    {
        ESContext context = new();
        context.player = player.transform;
        context.self = transform;
        if (spawner != null)
        {
            spawner.ClearMoveStep();
            foreach (var func in spawner.SpawnerList())
            {
                bulletCoroutine.Add(StartCoroutine(func(context)));
            }
        }
    }


    private void StartMove()
    {
        EMContext context = new();
        context.self = transform;
        context.spawner = spawner;

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

    public virtual void TakeDamage(int damage)
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

    protected virtual void Die()
    {
        isAlive = false;
        //if (bulletCoroutine != null) StopCoroutine(bulletCoroutine);
        gameObject.SetActive(false); // 或播放死亡动画后回收
        ItemManager.Instance.SpawnItem(transform.position, items.main);
        for (int i = 0; i < items.power; i++)
        {
            Vector3 pos = UnityEngine.Random.insideUnitCircle;
            ItemManager.Instance.SpawnItem(transform.position + pos, ItemType.SmallPower);
        }

        for (int i = 0; i < items.score; i++)
        {
            Vector3 pos = UnityEngine.Random.insideUnitCircle;
            ItemManager.Instance.SpawnItem(transform.position + pos, ItemType.Score);
        }
        OnDespawned();
    }
}
