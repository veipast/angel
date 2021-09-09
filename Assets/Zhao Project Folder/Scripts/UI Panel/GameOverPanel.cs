using System.Collections;
using System.Collections.Generic;
using AppsFlyerSDK;
using GameAnalyticsSDK;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverPanel : UIBase
{
    public Button resetSceneButton;
    private GameDataModel gameData;
    public override void Init()
    {
        base.Init();
        gameData = ModelManager.GetModel<GameDataModel>();
        resetSceneButton.onClick.AddListener(Level_Reset);
    }
    public override void Hide()
    {
        base.Hide();
    }
    public override void Show()
    {
        base.Show();
    }

    public void Level_Reset()
    {
        MAX_SDK_Manager.GetInstantiate().ShowInterstitial(() =>
        {
            //GA 断点消息 失败游戏
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, gameData.LevelNnmber.ToString("00000"));
            UIManager.Instance.CloseUI(PanelName.GameOverPanel);
            UIManager.Instance.CloseUI(PanelName.SlotMachinePanel);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }, "ResetScene");
        VibrateClassScript.GetInstancee.Haptic(MoreMountains.NiceVibrations.HapticTypes.Selection);
    }
}
