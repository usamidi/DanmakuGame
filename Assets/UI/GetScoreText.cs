using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GetScoreText : MonoBehaviour
{
    public float moveSpeed = 1.5f;    // 向上飘移的速度
    public float destroyTime = 0.8f;  // 多久后销毁
    private TextMeshProUGUI textMesh;

    // 提供一个公开方法，让外部可以设置文字内容（比如具体的分数）
    public void SetText(string text, Color color)
    {
        textMesh = GetComponentInChildren<TextMeshProUGUI>();
        textMesh.text = text;
        textMesh.color = color;
    }

    void Start()
    {
        // 倒计时销毁自己
        Destroy(gameObject, destroyTime);
    }

    void Update()
    {
        // 每帧往上飘移
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);
    }
}
