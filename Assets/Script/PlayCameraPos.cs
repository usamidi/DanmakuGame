using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class PlayCameraPos : MonoBehaviour
{
    public RectTransform playPanelTransform;
    public Camera uiCamera;
    private Camera _playCamera;
    
    void Start()
    {
        _playCamera = GetComponent<Camera>();
        AlignPlayCameraPos();
    }
    
    void AlignPlayCameraPos()
    {
        Vector3[] playPanelCorners = new Vector3[4];
        // screen space-camera下得到的 worldCorner 是物体相对于 UICamera的相对坐标
        playPanelTransform.GetWorldCorners(playPanelCorners);
        
        //左下和右上两点确定屏幕范围，转换为屏幕坐标
        Vector3 screenPosMin = uiCamera.WorldToScreenPoint(playPanelCorners[0]);
        Vector3 screenPosMax = uiCamera.WorldToScreenPoint(playPanelCorners[2]);
        
        //传给PlayCamera的是四个比例: x, y, w, h
        float x = screenPosMin.x / Screen.width;
        float y = screenPosMin.y / Screen.height;
        float w = (screenPosMax.x - screenPosMin.x) / Screen.width;
        float h = (screenPosMax.y - screenPosMin.y) / Screen.height;

        Rect newCameraViewportRect = new Rect(x, y, w, h);
        if ( _playCamera.rect != newCameraViewportRect )
        {
            _playCamera.rect = newCameraViewportRect;
        }
        
        //设置高度, 不然缩放有问题, 这里直接用相对坐标
        float panelWorldHeight = Vector3.Distance(playPanelCorners[1], playPanelCorners[0]);
        // Height = 2 * orthographicSize
        _playCamera.orthographicSize = panelWorldHeight / 2f;
        // 相机对准 UI 面板的中心
        Vector3 panelCenter = (playPanelCorners[0] + playPanelCorners[2]) / 2f;
        _playCamera.transform.position = new Vector3(panelCenter.x, panelCenter.y, _playCamera.transform.position.z);
    }
}
