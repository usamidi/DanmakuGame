using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ProceduralCurveMesh : MonoBehaviour
{
    public int segments = 50;
    public float width = 0.5f;
    public Transform p0, p1, p2; // 贝塞尔控制点

    private Mesh mesh;

    void Start() { mesh = new Mesh(); GetComponent<MeshFilter>().mesh = mesh; }

    void Update()
    {
        Vector3[] vertices = new Vector3[segments * 2];
        Vector2[] uvs = new Vector2[segments * 2];
        int[] triangles = new int[(segments - 1) * 6];

        for (int i = 0; i < segments; i++)
        {
            float t = i / (float)(segments - 1);
            Vector3 pos = GetBezierPoint(t, p0.position, p1.position, p2.position);

            // 计算切线方向 (用于确定激光宽度的方向)
            Vector3 tangent = GetBezierTangent(t, p0.position, p1.position, p2.position).normalized;
            Vector3 normal = new Vector3(-tangent.y, tangent.x, 0); // 垂直于切线

            // 生成左右两个点
            vertices[i * 2] = pos + normal * width;
            vertices[i * 2 + 1] = pos - normal * width;

            // UV 映射：V随长度拉伸，U在0-1之间
            uvs[i * 2] = new Vector2(0, t);
            uvs[i * 2 + 1] = new Vector2(1, t);

            // 构建三角形
            if (i < segments - 1)
            {
                int baseIdx = i * 2;
                triangles[i * 6] = baseIdx;
                triangles[i * 6 + 1] = baseIdx + 2;
                triangles[i * 6 + 2] = baseIdx + 1;
                triangles[i * 6 + 3] = baseIdx + 1;
                triangles[i * 6 + 4] = baseIdx + 2;
                triangles[i * 6 + 5] = baseIdx + 3;
            }
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    Vector3 GetBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        return u * u * p0 + 2 * u * t * p1 + t * t * p2;
    }

    Vector3 GetBezierTangent(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        // 导数公式: 2 * (1-t) * (p1-p0) + 2 * t * (p2-p1)
        return 2 * (1 - t) * (p1 - p0) + 2 * t * (p2 - p1);
    }
}
