using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveAnim : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;

    [Header("Sprite：[0]极左 -> [中间]正面 -> [末尾]极右")]
    public Sprite[] blendSprites;

    public float transitionSpeed = 15f;

    private float _currentFrameIndex;
    private int _centerIndex;
    private int _len;
    
    // Start is called before the first frame update
    void Awake()
    {
        _len = blendSprites.Length - 1;
        _centerIndex = _len / 2;
        _currentFrameIndex = _centerIndex;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    ///传参input = Input.GetAxisRaw("Horizontal")
    public void MoveAnim(float input)
    {
        if (input < 0)
        {
            _currentFrameIndex -= transitionSpeed * Time.deltaTime;
        }
        else if (input > 0)
        {
            _currentFrameIndex += transitionSpeed * Time.deltaTime;
        }
        else
        {
            _currentFrameIndex = Mathf.MoveTowards(_currentFrameIndex, _centerIndex, transitionSpeed * Time.deltaTime);
        }
        _currentFrameIndex = Mathf.Clamp(_currentFrameIndex, 0, _len);
        int finalIndex = Mathf.RoundToInt(_currentFrameIndex);
        spriteRenderer.sprite = blendSprites[finalIndex];
    }
}
