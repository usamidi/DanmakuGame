using UnityEngine;

[CreateAssetMenu(fileName = "NewEBulletStyle", menuName = "STG/Enemy Bullet Style")]
public class EBulletStyle : ScriptableObject
{
    [Header("=== 渲染外观 ===")]
    public Material material;
    public Mesh mesh;
    public float visualScale = 0.5f;

    [Header("=== 物理判定 ===")]
    [Tooltip("该子弹的碰撞判定半径 (不同子弹判定可以不同)")]
    public float hitboxRadius = 0.1f;

    [Tooltip("视觉朝向偏移(度)。\n如果你的子弹图片默认朝上，请填 -90；\n如果默认朝左，请填 180；\n如果默认朝右，填 0。")]
    public float visualAngleOffset = 0f;

    [Header("=== 图层与排序 ===")]
    [Tooltip("Z轴深度：值越小（如 -1, -2），越显示在前面。用于控制子弹遮挡关系。")]
    public float zDepth = -1f; // 【新增这一行】默认设为 -1，确保挡住 Z=0 的物体

    [Header("=== 内存分配 ===")]
    [Tooltip("该种类子弹同屏最大允许数量")]
    public int maxCount = 2000;
}
