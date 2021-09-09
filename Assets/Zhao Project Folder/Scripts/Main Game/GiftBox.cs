using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiftBox : MonoBehaviour
{
    private void Awake()
    {
        MsgManager.Add(MsgManagerType.DestroyGiftBox, DestroyGiftBox);
    }

    private void OnDestroy()
    {
        MsgManager.Remove(MsgManagerType.DestroyGiftBox, DestroyGiftBox);
    }
    private bool isTrigger = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //角色停止移动
            isTrigger = true;
            //刷新数据  打开UI
            UIManager.Instance.OpenUI(PanelName.GiftBoxPanel);
            MsgManager.Invoke(MsgManagerType.ShowGiftBoxPanelType, transform.CompareTag("AngleBox"));
            VibrateClassScript.GetInstancee.Haptic(MoreMountains.NiceVibrations.HapticTypes.LightImpact);
        }
    }
    private void DestroyGiftBox(object obj)
    {
        if (isTrigger)
            Destroy(gameObject);
    }

}
