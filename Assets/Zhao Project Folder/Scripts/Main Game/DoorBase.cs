using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class DoorBase : MonoBehaviour
{
    public List<DoorScript> doorScripts = new List<DoorScript>();
    public TMP_Text titleText;
    public Transform entityDoor;
    public List<Transform> getRewards = new List<Transform>();

    public void SetDoorBaseData(DoorBaseData data)
    {
        //根据数据创建 getRewards
        titleText.text = data.titleText;
        //
        //entityDoor
        doorScripts[0].SetDoorData(0, data.doorDatas[0], this);
        doorScripts[1].SetDoorData(1, data.doorDatas[1], this);
        getRewards.Clear();
        data.transformData.GetTransfor(transform);
        data.doorTransformData.GetTransfor(entityDoor);
        for (int i = 0; i < data.rewardsDatas.Count; i++)
        {
            DoorRewardsData item = data.rewardsDatas[i];
            getRewards.Add(Instantiate(ResourcesManager.Load<GameObject>(ResourcesType.ElseFolder, item.name)).transform);
            item.transformData.GetTransfor(getRewards[i]);
        }
    }
    public DoorBaseData GetDoorBaseData()
    {
        DoorBaseData data = new DoorBaseData();

        //根据数据创建 getRewards
        data.titleText = titleText.text;
        data.transformData.SetTransfor(transform);
        data.doorTransformData.SetTransfor(entityDoor);
        //entityDoor

        for (int i = 0; i < doorScripts.Count; i++)
        {
            data.doorDatas.Add(doorScripts[i].GetDoorData());
        }

        for (int i = 0; i < getRewards.Count; i++)
        {
            data.rewardsDatas.Add(new DoorRewardsData());
            if (data.doorDatas[0].situationalState == SituationalState.色欲)
            {
                data.rewardsDatas[i].name = "nv";
            }
            else
            {
                data.rewardsDatas[i].name = getRewards[i].GetComponent<MeshFilter>().sharedMesh.name.Split(' ')[0];
            }
            data.rewardsDatas[i].transformData.SetTransfor(getRewards[i]);
        }
        return data;
    }
    private int doorID = -1;

    internal void DoorOnTriggerEnter(int _ID)
    {
        if (doorID == -1)
        {
            doorID = _ID;
            MsgManager.Invoke(MsgManagerType.DoorTriggerEnter, doorScripts[doorID].transform);//限制
        }
    }

    internal void DoorOnTriggerExit(int _ID)
    {
        if (doorID == _ID)
        {
            MsgManager.Invoke(MsgManagerType.DoorTriggerExit, this);//结束限制 结算分数

            Destroy(gameObject);
        }
    }

    internal DoorData GetDoorData()
    {
        return doorScripts[doorID].doorData;
    }
}
