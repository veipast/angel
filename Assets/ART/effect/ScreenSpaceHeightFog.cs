﻿
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
[ExecuteInEditMode]
public class ScreenSpaceHeightFog : MonoBehaviour {
 
    [Range(0.0f, 10.0f)]
    public float fogHeight = 0.1f;
    public Color fogColor = Color.white;
    public float horizontalPlane = 0.0f;
 
    private Material postEffectMat = null;
    private Camera currentCamera = null;
 
    void Awake()
    {
        currentCamera = GetComponent<Camera>();
    }
 
    void OnEnable()
    {
        if (postEffectMat == null)
            postEffectMat = new Material(Shader.Find("QKX/volume fog"));
        Camera.main.depthTextureMode = DepthTextureMode.Depth;
    }
 
    void OnDisable()
    {
        Camera.main.depthTextureMode = DepthTextureMode.Depth;
    }
 
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (postEffectMat == null)
        {
            Graphics.Blit(source, destination);
        }
        else
        {
            var aspect = currentCamera.aspect;
            var far = currentCamera.farClipPlane;
            var right = transform.right;
            var up = transform.up;
            var forward = transform.forward;
            var halfFovTan = Mathf.Tan(currentCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
 
            //计算相机在远裁剪面处的xyz三方向向量
            var rightVec = right * far * halfFovTan * aspect;
            var upVec = up * far * halfFovTan;
            var forwardVec = forward * far;
 
            //构建四个角的方向向量
            var topLeft = (forwardVec - rightVec + upVec);
            var topRight = (forwardVec + rightVec + upVec);
            var bottomLeft = (forwardVec - rightVec - upVec);
            var bottomRight = (forwardVec + rightVec - upVec);
 
            var viewPortRay = Matrix4x4.identity;
            viewPortRay.SetRow(0, topLeft);
            viewPortRay.SetRow(1, topRight);
            viewPortRay.SetRow(2, bottomLeft);
            viewPortRay.SetRow(3, bottomRight);
 
            postEffectMat.SetMatrix("_ViewPortRay", viewPortRay);
            postEffectMat.SetFloat("_WorldFogHeight", horizontalPlane + fogHeight);
            postEffectMat.SetFloat("_FogHeight", fogHeight);
            postEffectMat.SetColor("_FogColor", fogColor);
            Graphics.Blit(source, destination, postEffectMat);
        }
    }
}
