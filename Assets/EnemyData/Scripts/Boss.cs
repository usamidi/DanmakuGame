using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BossSubPhase
{
    Normal,
    Special
}

[System.Serializable]
public class BossPhase
{
    [Header("阶段标识")]
    public string phaseName = "Phase";

    [Header("种类")]
    public BossSubPhase type;

    [Header("阶段血量")]
    public int hp = 1000;

    [Header("阶段移动方式")]
    public EnemyMotion motion;

    [Header("弹幕样式")]
    public EnemyBulletSpawner spawner;
    [Tooltip("持续时间，0 表示不限时（仅 HP 触发切换）")]
    public float duration = 0f;
}


public class Boss : Enemy
{
    [Header("阶段配置")]
    [SerializeField] private List<BossPhase> phases = new();

    [Header("阶段切换缓冲")]
    [SerializeField] private float phaseTransitionBuffer = 0.5f;
    [SerializeField] private float subPhaseTransitionBuffer = 0.5f;

    private int currentPhaseIndex = -1;
    private BossPhase currentPhase;
    private BossSubPhase currentSubPhase;

    private EnemyBulletSpawner runtimeSpawner;
    private EnemyMotion runtimeMotion;

    private Coroutine phaseFlowCoroutine;
    private Coroutine motionCoroutine;
    private readonly List<Coroutine> spawnerCoroutines = new();

    private bool isTransitioning = false;

    public BossPhase CurrentPhase => currentPhase;
    public BossSubPhase CurrentSubPhase => currentSubPhase;
    public int CurrentPhaseIndex => currentPhaseIndex;
    public int PhaseCount => phases != null ? phases.Count : 0;

    public Boss SetPhases(List<BossPhase> bossPhases)
    {
        phases = bossPhases ?? new List<BossPhase>();
        return this;
    }

    public override void OnSpawned()
    {
        currentPhaseIndex = -1;
        isTransitioning = false;
        AdvanceToNextPhase();
    }

    public override void OnDespawned()
    {
        StopPhaseFlow();
        StopMotionCoroutine();
        StopAllSpawnerActivity();
        base.OnDespawned();
    }

    protected override void Die()
    {
        if (isTransitioning) return;
        isTransitioning = true;
        AdvanceToNextPhase();
    }

    /*
    private void AdvanceSubPhaseOrPhase()
    {
        if (currentPhase != null
            && currentSubPhase == BossSubPhase.Normal
            && currentPhase.spawner != null)
        {
            StopPhaseFlow();
            phaseFlowCoroutine = StartCoroutine(EnterSpecialPhase());
        }
        else
        {
            AdvanceToNextPhase();
        }
    }
    */

    private void AdvanceToNextPhase()
    {
        StopPhaseFlow();
        phaseFlowCoroutine = StartCoroutine(EnterNextPhaseRoutine());
    }

    private IEnumerator EnterNextPhaseRoutine()
    {
        isTransitioning = true;

        StopMotionCoroutine();
        StopAllSpawnerActivity();

        currentPhaseIndex++;
        if (phases == null || currentPhaseIndex >= phases.Count)
        {
            OnAllPhasesCleared();
            yield break;
        }

        currentPhase = phases[currentPhaseIndex];
        ResetHPAndAlive(currentPhase.hp);

        Debug.Log($"[Boss] Enter Phase {currentPhaseIndex}: {currentPhase.phaseName}");

        InstantiateSpawner();
        StartPhaseMotion(currentPhase.motion);

        yield return new WaitForSeconds(phaseTransitionBuffer);

        isTransitioning = false;
        switch (currentPhase.type)
        {
            case BossSubPhase.Normal:
                yield return EnterNormalPhase();
                break;
            case BossSubPhase.Special:
                yield return EnterSpecialPhase();
                break;
        }
        AdvanceToNextPhase();
    }

    private IEnumerator EnterNormalPhase()
    {
        currentSubPhase = BossSubPhase.Normal;
        StartSpawnerCoroutines(runtimeSpawner);

        if (currentPhase.duration > 0f)
        {
            yield return new WaitForSeconds(currentPhase.duration);
        }
    }

    private IEnumerator EnterSpecialPhase()
    {
        isTransitioning = true;

        /*
        StopSpawnerCoroutines();
        DespawnSpawner(runtimeSpawner);
        runtimeSpawner = null;
        */

        if (currentPhase.spawner == null)
        {
            AdvanceToNextPhase();
            yield break;
        }

        currentSubPhase = BossSubPhase.Special;
        ResetHPAndAlive(currentPhase.hp);

        Debug.Log($"[Boss] Enter Special: {currentPhase.phaseName}");

        yield return new WaitForSeconds(subPhaseTransitionBuffer);

        StartSpawnerCoroutines(runtimeSpawner);
        isTransitioning = false;

        if (currentPhase.duration > 0f)
        {
            yield return new WaitForSeconds(currentPhase.duration);
        }
    }

    private void InstantiateSpawner()
    {
        if (currentPhase.spawner == null)
        {
            runtimeSpawner = null;
            return;
        }
        runtimeSpawner = Instantiate(currentPhase.spawner);
        runtimeSpawner.StartCoroutine = StartCoroutine;
        runtimeSpawner.ClearMoveStep();
    }

    private void StartPhaseMotion(EnemyMotion motionTemplate)
    {
        if (motionTemplate == null) return;

        runtimeMotion = Instantiate(motionTemplate);
        EMContext ctx = new EMContext
        {
            self = transform,
            spawner = runtimeSpawner,
        };
        motionCoroutine = StartCoroutine(runtimeMotion.Move(ctx));
    }

    private void StartSpawnerCoroutines(EnemyBulletSpawner spawnerInstance)
    {
        if (spawnerInstance == null) return;

        ESContext ctx = new ESContext
        {
            self = transform,
            player = player != null ? player.transform : null,
        };

        foreach (var func in spawnerInstance.SpawnerList())
        {
            spawnerCoroutines.Add(StartCoroutine(func(ctx)));
        }
    }

    private void DespawnSpawner(EnemyBulletSpawner spawnerInstance)
    {
        spawnerInstance?.OnDespawn();
    }

    private void StopSpawnerCoroutines()
    {
        foreach (var c in spawnerCoroutines)
            if (c != null) StopCoroutine(c);
        spawnerCoroutines.Clear();
    }

    private void StopMotionCoroutine()
    {
        if (motionCoroutine != null)
        {
            StopCoroutine(motionCoroutine);
            motionCoroutine = null;
        }
        runtimeMotion = null;
    }

    private void StopAllSpawnerActivity()
    {
        DespawnSpawner(runtimeSpawner);
        runtimeSpawner = null;
        StopSpawnerCoroutines();
    }

    private void StopPhaseFlow()
    {
        if (phaseFlowCoroutine != null)
        {
            StopCoroutine(phaseFlowCoroutine);
            phaseFlowCoroutine = null;
        }
    }

    private void ResetHPAndAlive(int newHp)
    {
        hp = newHp;
        isAlive = true;
    }

    protected virtual void OnAllPhasesCleared()
    {
        Debug.Log("[Boss] All phases cleared!");
        isAlive = false;
        StopMotionCoroutine();
        StopAllSpawnerActivity();
        StopPhaseFlow();

        if (EnemyManager.Instance != null)
            EnemyManager.Instance.DespawnEnemy(this, type);
    }
}
