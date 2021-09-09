using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System;

public enum LoadBuildSceneType
{
    Hell, Heaven
}
public class BuildDataModel : ModelBase
{
    public BuildSceneData buildSceneData = new BuildSceneData();
    private GameDataModel gameData;
    private LoadBuildSceneType _loadBuildSceneType;
    public LoadBuildSceneType LoadBuildSceneType { set { _loadBuildSceneType = value; } get { return _loadBuildSceneType; } }
    public Dictionary<int, List<BuildSceneUpgradeData>> LoadBuildSceneData
    {
        get
        {
            switch (_loadBuildSceneType)
            {
                case LoadBuildSceneType.Hell:
                    return buildSceneData.hellUpgrades;
                case LoadBuildSceneType.Heaven:
                    return buildSceneData.heavenUpgrades;
                default:
                    return null;
            }
        }
    }


    /// <summary>
    /// 是否第一次看天堂
    /// </summary>
    public bool isA_Look
    {
        get { return PlayerPrefs.GetInt("isA_Look", 0) == 1; }
        set { PlayerPrefs.SetInt("isA_Look", value ? 1 : 0); }
    }
    /// <summary>
    /// 是否第一次看地狱
    /// </summary>
    public bool isD_Look
    {
        get { return PlayerPrefs.GetInt("isD_Look", 0) == 1; }
        set { PlayerPrefs.SetInt("isD_Look", value ? 1 : 0); }
    }

    public bool IsShowAngelRedDot
    {
        get
        {
            try
            {
                gameData = ModelManager.GetModel<GameDataModel>();
                foreach (var item in buildSceneData.heavenUpgrades)
                {
                    foreach (BuildSceneUpgradeData temp in item.Value)
                    {
                        if (temp.upgradeCost <= gameData.AngelCoin && temp.upgradeLevel <= gameData.LevelNnmber && !temp.isUnlock)
                        {
                            return true;
                        }
                    }
                }
            }
            catch { }
            return false;
        }
    }
    public bool IsShowDevilRedDot
    {
        get
        {
            gameData = ModelManager.GetModel<GameDataModel>();
            foreach (var item in buildSceneData.hellUpgrades)
            {
                foreach (BuildSceneUpgradeData temp in item.Value)
                {
                    if (temp.upgradeCost <= gameData.DevilCoin && temp.upgradeLevel <= gameData.LevelNnmber && !temp.isUnlock)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }

    private int buildKey_1;//标记第一个建造下标
    //private int upgradesIndex;//标记第一个升级下堡
    internal void GetOneKey(out int key, out int index, int levelNnmber)
    {
        //找到第一个key未解锁但LEVEL达到的 //非广告
        foreach (var item in LoadBuildSceneData)
        {
            key = item.Key;
            buildKey_1 = item.Key;
            List<BuildSceneUpgradeData> value = item.Value;
            for (int i = 0; i < value.Count; i++)
            {
                if (!value[i].isUnlock && value[i].upgradeLevel <= levelNnmber)
                {
                    if (value[i].upgradeCost == -1) break;
                    index = i;
                    //upgradesIndex = i;
                    return;
                }
            }
        }
        key = -1;
        index = -1;
        buildKey_1 = -1;
        //upgradesIndex = -1;
    }
    private int buildKey_2;//标记第2个建造下标
    internal bool GetTowKey(out int key, out int index, int levelNnmber)
    {
        foreach (var item in LoadBuildSceneData)
        {
            key = item.Key;
            buildKey_2 = item.Key;
            if (key != buildKey_1)
            {
                List<BuildSceneUpgradeData> value = item.Value;
                for (int i = 0; i < value.Count; i++)
                {
                    if (!value[i].isUnlock && value[i].upgradeLevel <= levelNnmber)
                    {
                        index = i;
                        return value[i].upgradeCost != -1;//返回是否为广告Button
                    }
                }
            }
        }
        key = -1;//没有了
        index = -1;
        buildKey_2 = -1;
        return false;
    }

    internal void BuildUnlock(int key, int index)
    {
        LoadBuildSceneData[key][index].isUnlock = true;
    }

    internal void GetLockKey(out int key, out int index, int levelNnmber)
    {
        foreach (var item in LoadBuildSceneData)
        {
            key = item.Key;
            if (key != buildKey_1 && key != buildKey_2)
            {
                List<BuildSceneUpgradeData> value = item.Value;
                for (int i = 0; i < value.Count; i++)
                {
                    if (value[i].upgradeLevel > levelNnmber)
                    {
                        index = i;
                        return;
                    }
                }
            }

        }
        key = -1;//没有了
        index = -1;
        return;
    }

    public override void Load()
    {
        string str;
        string path = Application.persistentDataPath + "/ConfigFolder";
        base.Load();
        if (PlayerPrefs.GetInt("CreateBuildSceneData", 0) == 0)
        {
            str = Resources.Load<TextAsset>("ConfigFolder/Build Scene Data").text;

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            File.WriteAllText(path + "/Build Scene Data.json", str);
            PlayerPrefs.SetInt("CreateBuildSceneData", 1);
        }
        else
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            str = File.ReadAllText(path + "/Build Scene Data.json");

        }
        buildSceneData = JsonConvert.DeserializeObject<BuildSceneData>(str);
        if (PlayerPrefs.GetInt("CreateBuildSceneData", 0) <= 2)
        {
            str = Resources.Load<TextAsset>("ConfigFolder/Build Scene Data").text;

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            File.WriteAllText(path + "/Build Scene Data.json", str);
            PlayerPrefs.SetInt("CreateBuildSceneData", 3);
            BuildSceneData data = JsonConvert.DeserializeObject<BuildSceneData>(str);
            foreach (var item in data.heavenUpgrades)
            {
                for (int i = 0; i < data.heavenUpgrades[item.Key].Count; i++)
                {
                    buildSceneData.heavenUpgrades[item.Key][i].upgradeCost = data.heavenUpgrades[item.Key][i].upgradeCost;
                    buildSceneData.heavenUpgrades[item.Key][i].upgradeLevel = data.heavenUpgrades[item.Key][i].upgradeLevel;
                }
            }
            foreach (var item in data.hellUpgrades)
            {
                for (int i = 0; i < data.hellUpgrades[item.Key].Count; i++)
                {
                    buildSceneData.hellUpgrades[item.Key][i].upgradeCost = data.hellUpgrades[item.Key][i].upgradeCost;
                    buildSceneData.hellUpgrades[item.Key][i].upgradeLevel = data.hellUpgrades[item.Key][i].upgradeLevel;
                }
            }
        }
    }

    public void SaveData()
    {
        string path = Application.persistentDataPath + "/ConfigFolder";
        string str = JsonConvert.SerializeObject(buildSceneData);
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        File.WriteAllText(path + "/Build Scene Data.json", str);
    }
}



public class BuildSceneData
{
    /// <summary>
    /// 地狱升级集合 升级阶段 升级内容集合
    /// key = BuildIndex
    /// listIndex = UpgradesIndex
    /// </summary>
    public Dictionary<int, List<BuildSceneUpgradeData>> hellUpgrades = new Dictionary<int, List<BuildSceneUpgradeData>>();

    /// <summary>
    /// 天堂升级集合 升级阶段 升级内容集合
    /// </summary>
    public Dictionary<int, List<BuildSceneUpgradeData>> heavenUpgrades = new Dictionary<int, List<BuildSceneUpgradeData>>();

    //public void Set
}

public class BuildSceneUpgradeData
{
    /// <summary>
    /// 升级顺序ID
    /// </summary>
    public int upgradeID;
    /// <summary>
    /// 升级模型的名字
    /// </summary>
    public string[] upgradeName;
    /// <summary>
    /// 升级所需的关卡数
    /// </summary>
    public int upgradeLevel;
    /// <summary>
    /// 升级成本
    /// </summary>
    public int upgradeCost;
    /// <summary>
    /// 
    /// </summary>
    public bool isUnlock = false;
}
