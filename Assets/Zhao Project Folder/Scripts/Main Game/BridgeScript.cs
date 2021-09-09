using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeScript : MonoBehaviour
{
    public BoxCollider box;
    private void Awake()
    {
        MsgManager.Add(MsgManagerType.TriggerDownBridge, TriggerDownBridge);
    }
    private void OnDestroy()
    {
        MsgManager.Remove(MsgManagerType.TriggerDownBridge, TriggerDownBridge);
    }

    private void TriggerDownBridge(object obj)
    {
        if (isTrigger)
            Destroy(box);
    }
    private bool isTrigger = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isTrigger = true;
            MsgManager.Invoke(MsgManagerType.TriggerEnterBridge);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MsgManager.Invoke(MsgManagerType.TriggerDownBridge);
        }
    }
}
