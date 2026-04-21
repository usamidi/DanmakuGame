using UnityEngine;
using UnityEditor; // 引入编辑器命名空间

public class STGMeshGenerator
{
    // ==========================================
    // 1. 生成正三角形 Mesh
    // ==========================================
    [MenuItem("Tools/STG Mesh/Create Triangle")]
    public static void CreateTriangleMesh()
    {
        Mesh mesh = new Mesh();
        mesh.name = "Triangle";

        // 顶点：底边在下，尖端在上
        mesh.vertices = new Vector3[] {
            new Vector3(-0.4f, -0.4f, 0),
            new Vector3( 0.4f, -0.4f, 0),
            new Vector3( 0.0f,  0.8f, 0)
        };

        // UV：贴图映射
        mesh.uv = new Vector2[] {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0.5f, 1)
        };

        // 三角形索引（顺时针）
        mesh.triangles = new int[] { 0, 2, 1 };

        SaveMeshAsset(mesh, "TriangleMesh");
    }

    // ==========================================
    // 2. 生成菱形 (长菱形，适合激光头或飞镖)
    // ==========================================
    [MenuItem("Tools/STG Mesh/Create Diamond")]
    public static void CreateDiamondMesh()
    {
        Mesh mesh = new Mesh();
        mesh.name = "Diamond";

        mesh.vertices = new Vector3[] {
            new Vector3( 0.0f,  1.0f, 0), // 上
            new Vector3( 0.5f,  0.0f, 0), // 右
            new Vector3( 0.0f, -1.0f, 0), // 下
            new Vector3(-0.5f,  0.0f, 0)  // 左
        };

        mesh.uv = new Vector2[] {
            new Vector2(0.5f, 1),
            new Vector2(1, 0.5f),
            new Vector2(0.5f, 0),
            new Vector2(0, 0.5f)
        };

        // 两个三角形拼成菱形 (顺时针)
        mesh.triangles = new int[] {
            0, 1, 3, // 上半部
            1, 2, 3  // 下半部
        };

        SaveMeshAsset(mesh, "DiamondMesh");
    }

    // ==========================================
    // 3. 生成正六边形 (非常适合做特效或特殊弹幕)
    // ==========================================
    [MenuItem("Tools/STG Mesh/Create Hexagon")]
    public static void CreateHexagonMesh()
    {
        Mesh mesh = new Mesh();
        mesh.name = "Hexagon";

        float c = Mathf.Cos(Mathf.Deg2Rad * 30) * 0.5f;
        float s = Mathf.Sin(Mathf.Deg2Rad * 30) * 0.5f;

        mesh.vertices = new Vector3[] {
            new Vector3(0, 0, 0),            // 中心点 0
            new Vector3(0, 0.5f, 0),         // 顶部 1
            new Vector3(c, s, 0),            // 右上 2
            new Vector3(c, -s, 0),           // 右下 3
            new Vector3(0, -0.5f, 0),        // 底部 4
            new Vector3(-c, -s, 0),          // 左下 5
            new Vector3(-c, s, 0)            // 左上 6
        };

        mesh.uv = new Vector2[] {
            new Vector2(0.5f, 0.5f), // 中心
            new Vector2(0.5f, 1f),
            new Vector2(0.5f + c, 0.5f + s),
            new Vector2(0.5f + c, 0.5f - s),
            new Vector2(0.5f, 0f),
            new Vector2(0.5f - c, 0.5f - s),
            new Vector2(0.5f - c, 0.5f + s)
        };

        mesh.triangles = new int[] {
            0, 1, 2,  0, 2, 3,  0, 3, 4,
            0, 4, 5,  0, 5, 6,  0, 6, 1
        };

        SaveMeshAsset(mesh, "HexagonMesh");
    }

    // ==========================================
    // 4. 生成圆形 Mesh
    // ==========================================
    [MenuItem("Tools/STG Mesh/Create Circle")]
    public static void CreateCircleMesh()
    {
        int segments = 32; // 细分度
        Mesh mesh = new Mesh();
        mesh.name = "Circle";

        Vector3[] vertices = new Vector3[segments + 1];
        Vector2[] uv = new Vector2[segments + 1];
        int[] triangles = new int[segments * 3];

        // 中心点
        vertices[0] = Vector3.zero;
        uv[0] = new Vector2(0.5f, 0.5f);

        // 生成圆周点
        for (int i = 0; i < segments; i++)
        {
            float angle = (float)i / segments * Mathf.PI * 2f;
            float x = Mathf.Cos(angle) * 0.5f; // 半径 0.5
            float y = Mathf.Sin(angle) * 0.5f;

            vertices[i + 1] = new Vector3(x, y, 0);
            uv[i + 1] = new Vector2(x + 0.5f, y + 0.5f); // 映射到 0~1 空间
        }

        // 构建三角形
        for (int i = 0; i < segments; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = (i + 1 == segments) ? 1 : i + 2;
            triangles[i * 3 + 2] = i + 1;
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        SaveMeshAsset(mesh, "CircleMesh");
    }

    // ==========================================
    // 5. 生成方形 Mesh
    // ==========================================
    [MenuItem("Tools/STG Mesh/Create Square")]
    public static void CreateSquareMesh()
    {
        Mesh mesh = new Mesh();
        mesh.name = "Square";

        // 顶点：以中心为原点，0.5为半宽/半高
        mesh.vertices = new Vector3[] {
            new Vector3(-0.5f, -0.5f, 0), // 左下
            new Vector3( 0.5f, -0.5f, 0), // 右下
            new Vector3( 0.5f,  0.5f, 0), // 右上
            new Vector3(-0.5f,  0.5f, 0)  // 左上
        };

        // UV：完整映射 0 到 1
        mesh.uv = new Vector2[] {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(1, 1),
            new Vector2(0, 1)
        };

        // 两个三角形拼成正方形
        mesh.triangles = new int[] {
            0, 2, 1, // 第一个三角形
            0, 3, 2  // 第二个三角形
        };

        SaveMeshAsset(mesh, "SquareMesh");
    }

    // ==========================================
    // 保存逻辑：将 Mesh 写入 Assets 文件夹
    // ==========================================
    private static void SaveMeshAsset(Mesh mesh, string defaultName)
    {
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        // 弹出保存文件对话框
        string path = EditorUtility.SaveFilePanelInProject("Save Mesh", defaultName, "asset", "Save Mesh Asset");

        if (string.IsNullOrEmpty(path)) return; // 用户取消了保存

        // 创建并保存 Asset
        AssetDatabase.CreateAsset(mesh, path);
        AssetDatabase.SaveAssets();

        Debug.Log($"成功生成 Mesh 文件: {path}");
    }
}
