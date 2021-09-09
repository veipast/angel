using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using System;

public enum SituationalState
{
    暴食, 色欲, 贪念, 懒惰
}
public class DoorScript : MonoBehaviour
{
    public int _ID;
    public DoorData doorData;
    public DoorBase doorBase;

    public TMP_Text titleText;
    public SpriteRenderer RewardsSprite;

    public void SetDoorData(int _ID, DoorData data, DoorBase doorBase)
    {
        this._ID = _ID;
        doorData = data;
        this.doorBase = doorBase;
        //根据数据创建 getRewards
        titleText.text = data.titleText;
        //
        RewardsSprite.sprite = ResourcesManager.Load<Sprite>(ResourcesType.Sprite, data.spriteName);
        //RewardsSprite.sprite.name
    }


    internal DoorData GetDoorData()
    {
        doorData.titleText = titleText.text;
        doorData.spriteName = RewardsSprite.sprite.name;
        return doorData;
    }




    private bool isTriggerEnter = false;
    private void OnTriggerEnter(Collider other)
    {
        if (isTriggerEnter) return;
        if (other.CompareTag("Player"))
        {
            isTriggerEnter = true;
            doorBase.DoorOnTriggerEnter(_ID);
            VibrateClassScript.GetInstancee.Haptic(MoreMountains.NiceVibrations.HapticTypes.LightImpact);
        }
    }

    [HideInInspector]
    public bool isTriggerExit = false;
    private void OnTriggerExit(Collider other)
    {
        if (isTriggerExit) return;
        if (other.CompareTag("Player"))
        {
            isTriggerExit = true;
            doorBase.DoorOnTriggerExit(_ID);
        }
    }

}
