using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BuildCoinNotPanel : UIBase
{
    //public Button closeButton;
    public Button goGameSceneButton;
    public Text title;
    public Text wenan;
    public GameObject A_Image;
    public GameObject D_Image;
    public override void Init()
    {
        base.Init();
        //closeButton.onClick.AddListener(CloseButton);
        goGameSceneButton.onClick.AddListener(GoGameSceneButton);
    }

    public override void Show()
    {
        base.Show();
        if (SceneManager.GetActiveScene().name == "Heaven Game Scene")
        {
            title.text = "ANGEL COIN NOT ENOUGH?";
            wenan.text = "Help more good people get angel coins";
            A_Image.SetActive(true);
            D_Image.SetActive(false);
        }
        else
        {
            title.text = "DEVIL COIN NOT ENOUGH?";
            wenan.text = "Punish more bad guys to get devil coins";
            A_Image.SetActive(false);
            D_Image.SetActive(true);
        }
    }
    public override void Hide()
    {
        base.Hide();
    }
    private void CloseButton()
    {
        UIManager.Instance.CloseUI(PanelName.BuildCoinNotPanel);
        VibrateClassScript.GetInstancee.Haptic(MoreMountains.NiceVibrations.HapticTypes.Selection);
    }

    private void GoGameSceneButton()
    {
        UIManager.Instance.CloseUI(PanelName.BuildCoinNotPanel);
        SceneLevelManager.Loading(ModelManager.GetModel<GameDataModel>().LoadSceneName);
        VibrateClassScript.GetInstancee.Haptic(MoreMountains.NiceVibrations.HapticTypes.Selection);
    }

}
