using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Boss : Enemy
{

    private float totalTime;
    private float currentTime;
    private bool isTimerRunning = false;

    private void StartCountdown(float seconds)
    {
        totalTime = seconds;
        currentTime = totalTime;
        isTimerRunning = true;
        UIManager.Instance.SetCountdown(currentTime);
        if (currentPhase.type == BossSubPhase.Special)
        {
            UIManager.Instance.SetBonus(currentPhase.baseBonus + (ulong)(currentTime * currentPhase.secondBonus));
        }
    }

    private void UpdateTimer()
    {
        if (!isTimerRunning) return;

        if (currentTime > 0)
        {
            // Time.deltaTime 是上一帧到这一帧所消耗的时间（大约 0.016 秒）
            currentTime -= Time.deltaTime;
            UIManager.Instance.SetCountdown(currentTime);
            if (currentPhase.type == BossSubPhase.Special)
            {
                UIManager.Instance.SetBonus(currentPhase.baseBonus + (ulong)(currentTime * currentPhase.secondBonus));
            }
        }
        else
        {
            currentTime = 0;
            UIManager.Instance.SetCountdown(currentTime);
            if (currentPhase.type == BossSubPhase.Special)
            {
                UIManager.Instance.SetBonus(currentPhase.baseBonus);
            }
            isTimerRunning = false;
        }
    }

    private void ClearTimer()
    {
        isTimerRunning = false;
        UIManager.Instance.SetCountdown(-1f);
    }

    void Update()
    {
        UpdateTimer();
    }
}

