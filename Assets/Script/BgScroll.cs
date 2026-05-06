using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgScroll : MonoBehaviour
{
    public float scrollSpeed;
    private Vector2 _startPos; 
    private SpriteRenderer _sr;
    private float _bgHeight;
    
    // Start is called before the first frame update
    void Start()
    {
        _startPos = transform.position;
        _sr = GetComponent<SpriteRenderer>();
        _bgHeight = _sr.bounds.size.y;
    }

    // Update is called once per frame
    void Update()
    {
        float dis = Mathf.Repeat(scrollSpeed * Time.time, _bgHeight);
        transform.position = _startPos + dis * new Vector2(0, -1);
    }
}
