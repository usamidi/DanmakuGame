using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StageState
{
    Idle,
    Running,
    Cleared
}

public class StageManager : MonoBehaviour
{
    public static StageManager Instance;

    [Header("关卡列表")]
    [SerializeField] private Stage[] stages;
    [SerializeField] private bool autoStartOnPlay = true;
    [SerializeField] private int startStageIndex = 0;

    private Coroutine stageCoroutine;
    private int currentStageIndex = -1;
    private StageState state = StageState.Idle;

    public StageState State => state;
    public int CurrentStageIndex => currentStageIndex;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (autoStartOnPlay)
            StartStage(startStageIndex);
    }

    public bool StartStage(int index)
    {
        if (state == StageState.Running) return false;
        if (index < 0 || index >= stages.Length) return false;
        if (stages[index] == null) return false;

        currentStageIndex = index;
        stageCoroutine = StartCoroutine(RunStageRoutine(stages[index]));
        return true;
    }

    public void StopStage()
    {
        if (stageCoroutine != null)
        {
            StopCoroutine(stageCoroutine);
            stageCoroutine = null;
        }

        state = StageState.Idle;

        if (EnemyManager.Instance != null)
            EnemyManager.Instance.ClearAllEnemies();
    }

    public bool StartNextStage()
    {
        int next = currentStageIndex + 1;
        if (next >= stages.Length) return false;
        return StartStage(next);
    }

    private IEnumerator RunStageRoutine(Stage stage)
    {
        state = StageState.Running;

        yield return StartCoroutine(stage.Run());

        state = StageState.Cleared;
        stageCoroutine = null;

        // 这里可以接结算/过场UI
        // UIManager.Instance?.ShowStageClear(stage.StageId);
    }
}
