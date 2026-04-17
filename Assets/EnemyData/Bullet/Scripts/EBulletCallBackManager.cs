using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public struct EBContext
{
    public EBulletData bullet;
    public Vector2 playerPos;
    public float dt;
}


public delegate bool EBConditionFunc(in EBContext context);

public struct EBCallBackInfo
{
    public EBConditionFunc condition;
    public Func<EBContext, IEnumerator> func;

    public EBCallBackInfo(EBConditionFunc condition, Func<EBContext, IEnumerator> func)
    {
        this.condition = condition;
        this.func = func;
    }
}

public class EBulletCallBackManager : MonoBehaviour
{
    public static EBulletCallBackManager Instance;

    private Dictionary<EBulletRenderBatch, List<Coroutine>> coroutines = new();
    private Dictionary<Func<EBContext, IEnumerator>, HashSet<EBulletData>> visitTable = new();


    void Awake()
    {
        Instance = this;
    }

    public void AddCoroutine(IEnumerator c, in EBulletRenderBatch batch)
    {
        Coroutine coroutine = StartCoroutine(c);

        if (!coroutines.ContainsKey(batch))
        {
            coroutines.Add(batch, new List<Coroutine>());
        }
        coroutines[batch].Add(coroutine);
    }

    public bool CheckOnce(Func<EBContext, IEnumerator> c, in EBulletData data)
    {
        if (!visitTable.ContainsKey(c))
        {
            visitTable.Add(c, new HashSet<EBulletData>());
            visitTable[c].Add(data);
            return true;
        }

        if (!visitTable[c].Contains(data))
        {
            visitTable[c].Add(data);
            return true;
        }
        return false;
    }

    public void ClearTable(in EBulletRenderBatch batch)
    {
        if (coroutines.ContainsKey(batch))
        {

            foreach (var c in coroutines[batch])
            {
                if (c != null)
                {
                    StopCoroutine(c);
                }
            }
            coroutines.Remove(batch);
        }

        foreach (var (infos, bullets) in batch.bulletsTable)
        {
            foreach (var info in infos)
            {
                foreach (var b in bullets)
                {
                    visitTable[info.func].Remove(b);
                }
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            Debug.Log($"Coroutine: {coroutines.Count}");
            Debug.Log($"Table: {visitTable.Count}");
        }
    }
}
