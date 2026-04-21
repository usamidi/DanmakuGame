using UnityEngine;
using System.Collections.Generic;

public class ItemManager : MonoBehaviour
{
    public static ItemManager instance;

    [Header("Settings")]
    public Mesh itemMesh;
    public Material itemMaterial;
    public float fallSpeed = 3f;
    public float collectionRadius = 0.5f;

    // 分类存储，以便渲染不同贴图/颜色的道具
    private List<Item> items = new List<Item>();
    private Matrix4x4[] matrixBuffer = new Matrix4x4[1023]; // DrawMeshInstanced 最多支持 1023 个
    private Vector3 defaultVelocity = new Vector3(0f, 1f, 0f);
    private Transform player;


    void Awake()
    {
        instance = this;
        player = Player.instance.transform;
    }

    public void SpawnItem(Vector3 pos, ItemType type)
    {
        items.Add(new Item(pos, defaultVelocity, type));
    }

    void Update()
    {
        int drawCount = 0;

        // 1. 更新逻辑与碰撞检测
        for (int i = items.Count - 1; i >= 0; i--)
        {
            Item item = items[i];
            if (!item.isActive) continue;

            // 移动
            item.position += Vector3.down * fallSpeed * Time.deltaTime;

            // 简易碰撞检测
            if (Vector3.Distance(item.position, player.position) < collectionRadius)
            {
                ApplyEffect(item.type);
                item.isActive = false;
            }

            // 屏幕外回收
            if (item.position.y < -6f) item.isActive = false;

            // 更新回列表
            items[i] = item;

            // 如果还没失效，加入渲染队列
            if (item.isActive)
            {
                matrixBuffer[drawCount] = Matrix4x4.TRS(item.position, Quaternion.identity, Vector3.one);
                drawCount++;
            }
        }

        // 清理失效数据
        items.RemoveAll(x => !x.isActive);

        // 2. 一次性绘制
        if (drawCount > 0)
        {
            Graphics.DrawMeshInstanced(itemMesh, 0, itemMaterial, matrixBuffer, drawCount);
        }
    }

    void ApplyEffect(ItemType type)
    {
        // 这里根据类型给予奖励
        Debug.Log($"Collected: {type}");
    }
}
