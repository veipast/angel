using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotMachineTrigger : MonoBehaviour
{
    private bool isTrigger = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isTrigger)
        {
            isTrigger = true;
            Destroy(gameObject, 2f);
            UIManager.Instance.OpenUI(PanelName.SlotMachinePanel);
            VibrateClassScript.GetInstancee.Haptic(MoreMountains.NiceVibrations.HapticTypes.LightImpact);
        }
    }
}
