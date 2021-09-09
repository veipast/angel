using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TakeOff : MonoBehaviour
{
    public MoveState moveState;
    //public ParticleSystem cloudFX;
    //public bool isGround = false;
    //public bool isShowInfernal = false;
    //public GameObject infernalObject;
    //public Material infernalMaterial;
    //public Material lineageMaterial;
    //public void ShowInfernal()
    //{
    //    if (isShowInfernal)
    //    {
    //        infernalObject.SetActive(true);
    //        Color nowColor;
    //        ColorUtility.TryParseHtmlString("#842A11", out nowColor);
    //        RenderSettings.fogColor = nowColor;
    //        RenderSettings.skybox = infernalMaterial;
    //    }
    //}
    //public void CloudsDispersed()
    //{
    //    if (moveState == PlayerController.MoveState.AngelSky)
    //    {
    //        cloudFX.Stop();
    //    }
    //}


    //#if UNITY_EDITOR
    //    [CustomEditor(typeof(TakeOff))]
    //    public class TakeOffEditor : Editor
    //    {
    //        public override void OnInspectorGUI()
    //        {
    //            //获取脚本对象
    //            TakeOff script = target as TakeOff;
    //            EditorGUILayout.BeginVertical();
    //            script.moveState = (PlayerController.MoveState)EditorGUILayout.EnumPopup("MoveState:", script.moveState);
    //            script.isGround = EditorGUILayout.Toggle("是否有地面:", script.isGround);
    //            if (script.moveState == PlayerController.MoveState.AngelSky)
    //            {
    //                script.cloudFX = EditorGUILayout.ObjectField("云:", script.cloudFX, typeof(ParticleSystem), true) as ParticleSystem;
    //            }
    //            if (script.isGround)
    //            {
    //                script.isShowInfernal = EditorGUILayout.Toggle("是否为地狱:", script.isShowInfernal);
    //                if (script.isShowInfernal)
    //                {
    //                    script.infernalObject = EditorGUILayout.ObjectField("地狱物品:", script.infernalObject, typeof(GameObject), true) as GameObject;
    //                    script.infernalMaterial = EditorGUILayout.ObjectField("地狱天空盒:", script.infernalMaterial, typeof(Material), true) as Material;
    //                    script.lineageMaterial = EditorGUILayout.ObjectField("天堂天空盒:", script.lineageMaterial, typeof(Material), true) as Material;
    //                }
    //            }
    //            EditorGUILayout.EndVertical();
    //        }
    //    }
    //#endif


    private bool isTrigger = true;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && isTrigger)
        {
            isTrigger = false;
            switch (moveState)
            {
                case MoveState.Ground://在地面上
                    break;
                case MoveState.Jump://跳过去
                    MsgManager.Invoke(MsgManagerType.TriggerJumpState, this);
                    break;
                case MoveState.AngelSky://只允许天使和人类走过去
                    MsgManager.Invoke(MsgManagerType.TriggerAngelSkyState, this);
                    break;
                case MoveState.Fall://起那方直接坠落
                    MsgManager.Invoke(MsgManagerType.TriggerFallState, this);
                    break;
            }
        }
    }

    private void Awake()
    {
        MsgManager.Add(MsgManagerType.DestroyCollider, DestroyCollider);
    }
    private void OnDestroy()
    {
        MsgManager.Remove(MsgManagerType.DestroyCollider, DestroyCollider);
    }

    private void DestroyCollider(object obj)
    {
        if (!isTrigger)
        {
            Destroy(gameObject);
        }
    }
}