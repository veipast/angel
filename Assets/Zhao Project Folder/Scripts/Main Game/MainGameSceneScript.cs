using System;
using System.Collections;
using System.Collections.Generic;
using GameAnalyticsSDK;
using UnityEngine;

public class MainGameSceneScript : MonoBehaviour
{
    private GameDataModel gameData;
    public GameSceneDataManager gsdManager;
    private void Awake()
    {
        gameData = ModelManager.GetModel<GameDataModel>();

        gsdManager = new GameSceneDataManager();
        gsdManager.Read(gameData.GetSceneName, gameData.angelAndDevilData.selectItemIndex);
        //GA 断点消息 开始游戏
        gameData.Load();
        UIManager.Instance.OpenUI(PanelName.HeadStatePanel);
        UIManager.Instance.OpenUI(PanelName.RoleDescriptionPanel);
        UIManager.Instance.OpenUI(PanelName.PlayerCoinPanel);
        UIManager.Instance.OpenUI(PanelName.MainPanel);
        UIManager.Instance.OpenUI(PanelName.MainScenePanel);
        MsgManager.Add(MsgManagerType.RefreshWingsItem, RefreshWingItem);



    }

    private void RefreshWingItem(object obj)
    {
        gsdManager.CreateWings(gameData.angelAndDevilData.selectItemIndex);
    }

    private void OnEnable()
    {
        gameData = ModelManager.GetModel<GameDataModel>();
        gameData.Load();
    }
    public void Update()
    {

    }
    private void OnDisable()
    {
        try
        {
            MsgManager.Remove(MsgManagerType.RefreshWingsItem, RefreshWingItem);
            UIManager.Instance.CloseUI(PanelName.MainPanel);
            UIManager.Instance.CloseUI(PanelName.HeadStatePanel);
            UIManager.Instance.CloseUI(PanelName.PlayerCoinPanel);
            UIManager.Instance.CloseUI(PanelName.MainScenePanel);
            UIManager.Instance.CloseUI(PanelName.RoleDescriptionPanel);
            //Resources.UnloadUnusedAssets();//卸载未占用的asset资源
            System.GC.Collect();//回收内存
        }
        catch { }
    }
}




