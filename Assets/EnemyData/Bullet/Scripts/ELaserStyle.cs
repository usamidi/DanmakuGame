using UnityEngine;

[CreateAssetMenu(fileName = "NewELaserStyle", menuName = "STG/Enemy Laser Style")]
public class ELaserStyle : ScriptableObject
{
    [Header("=== 渲染 ===")]
    [Tooltip("建议用中心锚点的 1x1 Quad，方便 TRS 直接按 (length, width, 1) 缩放")]
    public Mesh quadMesh;
    public Material laserMaterial;

    [Header("=== 图层 ===")]
    [Tooltip("Z轴深度：值越小（如 -1, -2）越显示在前面")]
    public float zDepth = -1f;
}
