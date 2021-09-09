using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataModel : ModelBase
{
    public int LevelNnmber { get { return PlayerPrefs.GetInt("LEVEL", 1); } set { PlayerPrefs.SetInt("LEVEL", value); } }
    public int sceneIndex { get { return PlayerPrefs.GetInt("SceneIndex"); } set { PlayerPrefs.SetInt("SceneIndex", value); } }
    public string[] sceneNames = new string[]
    {
        //"Scene Data video1", "Scene Data video2", "Scene Data video3","Scene Data video4",  "Scene Data CeShi",
        //0.4
       // "Scene Data 1" ,        "Scene Data 2" ,       "Scene Data 3" ,        "Scene Data 4"  ,        "Scene Data video4",
       // "Scene Data Move1",     "Scene Data Move2",    "Scene Data 5" ,        "Scene Data 7"  ,        "Scene Data video5",
       // "Scene Data video2",    "Scene Data 9" ,       "Scene Data Move3",     "Scene Data 6" ,         "Scene Data video4",
       // "Scene Data Move2",     "Scene Data Move4",    "Scene Data 12" ,       "Scene Data 13",         "Scene Data video5",
       // "Scene Data 14",        "Scene Data 15",       "Scene Data video2" ,   "Scene Data 7" ,         "Scene Data video4",
       //"Scene Data Hell1",      "Scene Data Hell2",    "Scene Data Hell3",     "Scene Data Heaven1",    "Scene Data Heaven2",    "Scene Data Heaven3",

        //v2.6
        //"Scene Data Move1",     "Scene Data Move3Hell", "Scene Data Move2",     "Scene Data Move4",   "Scene Data video4",
        //"Scene Data Move5Hell", "Scene Data Heaven1",   "Scene Data Heaven2",   "Scene Data Hell3",   "Scene Data video5",
        //"Scene Data Heaven3",   "Scene Data Hell1",     "Scene Data Move3",     "Scene Data Move5",   "Scene Data video4",
        //"Scene Data Move9",     "Scene Data Move10",    "Scene Data Hell2",     "Scene Data Move11",  "Scene Data video5",

        "Scene Data Move1",  "Scene Data Move1Hell",  "Scene Data Move0",  "Scene Data Move2", "Scene Data video4",
        "Scene Data Move5Hell", "Scene Data Heaven1",   "Scene Data Heaven2",   "Scene Data Hell3",   "Scene Data video5",

    };
    public string GetSceneName
    {
        get
        {
            if (sceneIndex >= sceneNames.Length) sceneIndex = 0;
            try
            {
                return sceneNames[sceneIndex];
            }
            catch (Exception ex)
            {
                sceneIndex = 0;
                return sceneNames[sceneIndex];
            }

        }
    }

    private int _angelCoin;
    private int _tempAngelCoin;
    public int TempAngelCoin { get { return _angelCoin - _tempAngelCoin; } }
    public int AngelCoin { get { return _angelCoin; } set { _angelCoin = value; } }
    private int _devilCoin;
    private int _tempDevilCoin;
    public int TempDevilCoin { get { return _devilCoin - _tempDevilCoin; } }
    public int DevilCoin { get { return _devilCoin; } set { _devilCoin = value; } }

    public int CoinCountMultiple;
    internal int GetCoinCount()
    {
        PlayerModel playerModel = ModelManager.GetModel<PlayerModel>();
        GameDataModel gameData = ModelManager.GetModel<GameDataModel>();
        //int X_Number = (int)obj;

        if (playerModel.GetFraction >= 50)
            return gameData.TempAngelCoin * CoinCountMultiple;
        else
            return gameData.TempDevilCoin * CoinCountMultiple;
    }

    public string LoadSceneName
    {
        get
        {
            return "Player Game Scene";
        }
    }
    public void LoadNextSceneData()
    {
        LevelNnmber++;
        sceneIndex++;
        playerRoleModel.roleDatasIndex += 1;
        if (sceneIndex >= sceneNames.Length) sceneIndex = 0;
    }


    internal int GetAllCoinCount(bool isAngel)
    {
        if (isAngel)
        {
            return PlayerPrefs.GetInt("AngelCoin");
        }
        else
        {
            return PlayerPrefs.GetInt("DevilCoin");
        }
    }
    public void X_2Coin(int count, bool isAngel)
    {
        if (isAngel)
        {
            PlayerPrefs.SetInt("AngelCoin", PlayerPrefs.GetInt("AngelCoin") + count);
            _angelCoin += count;
            _tempAngelCoin += count;
        }
        else
        {
            PlayerPrefs.SetInt("DevilCoin", PlayerPrefs.GetInt("DevilCoin") + count);
            _devilCoin += count;
            _tempDevilCoin += count;
        }
        MsgManager.Invoke(MsgManagerType.ShowDevilCoin, isAngel.ToString() + "|+ " + count.ToString());
    }


    public AngelAndDevilData angelAndDevilData = new AngelAndDevilData();

    public PlayerRoleModel playerRoleModel = new PlayerRoleModel();

    public override void Load()
    {
        _angelCoin = PlayerPrefs.GetInt("AngelCoin", 100);
        _devilCoin = PlayerPrefs.GetInt("DevilCoin", 100);
        _tempAngelCoin = _angelCoin;
        _tempDevilCoin = _devilCoin;
        angelAndDevilData.Load();
        playerRoleModel.Load();
    }
    public void SaveData()
    {
        PlayerPrefs.SetInt("AngelCoin", _angelCoin);
        PlayerPrefs.SetInt("DevilCoin", _devilCoin);
        angelAndDevilData.SaveData();
        playerRoleModel.SaveData();
    }
}





[System.Serializable]
public class AngelAndDevilData
{
    private int itemCount = 4;
    [Tooltip("该解锁物品的集合")]
    public bool[] unlockItems;
    [Tooltip("该解锁物品的下标")]
    public int unlockItemIndex;
    [Tooltip("选择物品的下标")]
    public int selectItemIndex;
    public float progressNumber;
    public int sceneCount;

    public bool isAllUnlock = false;
    public void Load()
    {
        unlockItems = new bool[itemCount];
        unlockItems[0] = true;
        for (int i = 1; i < itemCount; i++)
        {
            unlockItems[i] = PlayerPrefs.GetInt("UnlockItem_" + i, 0) != 0;
        }
        unlockItemIndex = PlayerPrefs.GetInt("UnlockItems_Index", 1);
        selectItemIndex = PlayerPrefs.GetInt("SelectItems_Index", 0);
        progressNumber = PlayerPrefs.GetFloat("ProgressNumber", 0);
        sceneCount = PlayerPrefs.GetInt("UnlockItem_SceneCount", 0);
        //sceneCount = 9;
        FindLockItem(false);
    }

    public void SaveData()
    {
        for (int i = 1; i < unlockItems.Length; i++)
        {
            PlayerPrefs.SetInt("UnlockItem_" + i, unlockItems[i] ? 1 : 0);
        }
        PlayerPrefs.SetInt("UnlockItems_Index", unlockItemIndex);
        PlayerPrefs.SetInt("SelectItems_Index", selectItemIndex);
        PlayerPrefs.SetFloat("ProgressNumber", progressNumber);
        PlayerPrefs.SetInt("UnlockItem_SceneCount", sceneCount);
    }

    internal void UnlockItem(int index)
    {
        if (index == -1) return;
        selectItemIndex = index;
        unlockItems[index] = true;
        //FindLockItem();
    }

    internal void FindLockItem(bool value)
    {
        if (unlockItemIndex == -1) unlockItemIndex = 0;
        if (value)
        {
            unlockItemIndex++;
            sceneCount++;
            if (progressNumber >= 100) sceneCount = 0;
        }
        while (unlockItemIndex < unlockItems.Length)
        {
            if (!unlockItems[unlockItemIndex])
            {
                return;
            }
            unlockItemIndex++;
        }
        if (unlockItemIndex >= unlockItems.Length)
        {
            unlockItemIndex = 0;
            isAllUnlock = true;
            for (int i = 0; i < unlockItems.Length; i++)
            {
                //找到为解锁的  判断是否全解锁
                if (!unlockItems[i])
                {
                    unlockItemIndex = i;
                    isAllUnlock = false;
                    break;
                }
            }
            if (isAllUnlock) unlockItemIndex = -1;
        }
    }

    internal float GetRandomMoveNumber()
    {
        if (unlockItemIndex == 1)
            return UnityEngine.Random.Range(50.0f, 60.0f);
        else
            return UnityEngine.Random.Range(8.0f, 13.0f);
    }
}