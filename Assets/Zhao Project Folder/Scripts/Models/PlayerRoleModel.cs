using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;

public class PlayerRoleModel
{
    public List<PlayerRoleData> playerRoleDatas = new List<PlayerRoleData>();
    public int roleDatasIndex { get { return PlayerPrefs.GetInt("RoleDatasIndex", 0); } set { PlayerPrefs.SetInt("RoleDatasIndex", value); } }
    public PlayerRoleData GetRoleDtat
    {
        get
        {
            //if (roleDatasIndex >= playerRoleDatas.Count) roleDatasIndex = 0;
            int index = 0;
            for (int i = 0; i < playerRoleDatas.Count; i++)
            {
                PlayerRoleData item = playerRoleDatas[i];
                if (roleDatasIndex == item.roleModelID)
                {
                    index = i;
                    break;
                }
            }
            if (index == 0) roleDatasIndex = 0;
            return playerRoleDatas[index];
        }
    }

    public void Load()
    {
        //playerRoleDatas
        TextAsset text = Resources.Load<TextAsset>("ConfigFolder/Character Data");
        playerRoleDatas = JsonConvert.DeserializeObject<List<PlayerRoleData>>(text.text);
    }

    internal void SaveData()
    {

    }
}
