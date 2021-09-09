using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerFlagScript : MonoBehaviour
{
    public Animator ani;
    private bool isTrigger = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isTrigger)
        {
            isTrigger = true;
            ani.SetBool("UpFlag", true);
            StartCoroutine(Wait());
        }
    }
    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(0.5f);
        if (MAX_SDK_Manager.GetInstantiate().isShowFlagAD)//判断是否展示广告
        {
            MAX_SDK_Manager.GetInstantiate().ShowInterstitial(() => { }, "TriggerFlag");
        }
    }
}
