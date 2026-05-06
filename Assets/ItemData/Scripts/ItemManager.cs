using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

[System.Serializable]
public struct ItemMaterialConfig
{
    public ItemType item;
    public Material material;
}

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance;

    [Header("Settings")]
    public Mesh itemMesh;
    public List<ItemMaterialConfig> itemMaterials;

    public Dictionary<ItemType, Material> materialDict = new();
    public Vector3 gravity = new Vector3(0f, -2f, 0f);
    public float collectionRadius = 0.5f;

    private const int MAX_ITEMS = 512;

    // 分类存储，以便渲染不同贴图/颜色的道具
    private Dictionary<ItemType, List<Item>> items = new();

    private Matrix4x4[] matrixBuffer = new Matrix4x4[1023]; // DrawMeshInstanced 最多支持 1023 个
    private Vector3 defaultVelocity = new Vector3(0f, 1f, 0f);
    [SerializeField] private Transform player;
    [SerializeField] private Vector3 scale;


    private void InitMaterials()
    {
        foreach (var pair in itemMaterials)
        {
            materialDict[pair.item] = pair.material;
        }

        foreach (var type in materialDict.Keys)
        {
            items[type] = new List<Item>();
            for (int i = 0; i < MAX_ITEMS; i++)
            {
                items[type].Add(new Item(Vector3.zero));
            }
        }
    }

    void Awake()
    {
        Instance = this;
        InitMaterials();
    }

    public void SpawnItem(Vector3 pos, ItemType type)
    {
        int index = items[type].FindIndex((i) => !i.isActive);
        Item item = items[type][index];
        items[type][index] = item.SetPosition(pos).Active();
    }

    void Update()
    {
        int drawCount = 0;

        // 1. 更新逻辑与碰撞检测
        foreach (var (type, itemList) in items)
        {
            Array.Clear(matrixBuffer, 0, matrixBuffer.Length);
            drawCount = 0;
            for (int i = 0; i < itemList.Count; i++)
            {
                Item item = itemList[i];
                if (!item.isActive) continue;

                item.Move(Time.deltaTime, gravity, player.position);

                // 简易碰撞检测
                if (Vector3.Distance(item.position, player.position) < collectionRadius)
                {
                    ApplyEffect(type, item.position);
                    item.Deactive();
                }

                // 屏幕外回收
                if (item.position.y < -6f) item.Deactive();

                // 更新回列表
                itemList[i] = item;

                // 如果还没失效，加入渲染队列
                if (item.isActive)
                {
                    matrixBuffer[drawCount] = Matrix4x4.TRS(item.position, Quaternion.identity, scale);
                    drawCount++;
                }
            }

            // 2. 一次性绘制
            if (drawCount > 0)
            {
                Graphics.DrawMeshInstanced(itemMesh, 0, materialDict[type], matrixBuffer, drawCount);
            }
        }
        AttractScoreItem();
    }

    void AttractScoreItem()
    {
        float max = EBulletManager.Instance.boundsMax.y;
        float min = EBulletManager.Instance.boundsMin.y;
        if (player.position.y - min > (max - min) * 0.8f)
        {
            for (int i = 0; i < items[ItemType.Score].Count; i++)
            {
                Item item = items[ItemType.Score][i];
                if (!item.isActive) continue;
                item.SetAttract();
                items[ItemType.Score][i] = item;
            }
        }
    }

    void ApplyEffect(ItemType type, Vector3 pos)
    {
        switch (type)
        {
            case ItemType.Score:
                ulong itemScore = UIManager.Instance.ItemScore;

                float max = EBulletManager.Instance.boundsMax.y;
                float min = EBulletManager.Instance.boundsMin.y;

                float borderHeight = max - min;
                float height = pos.y - min;

                float t = height > 0.8f * borderHeight ? 1.0f : height / borderHeight;

                ulong score = (ulong)Mathf.Lerp(itemScore * 0.4f, (float)itemScore, t);

                UIManager.Instance.AddScore(score);

                break;
            default:
                //Debug.Log($"Collected: {type}");
                break;

        }
    }
}
