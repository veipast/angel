using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class TriggerGodPanel : UIBase
{
    public Text showText;
    public Animator ani;
    public Button noButton;
    public Button adButton;
    private GameDataModel gameData;
    private PlayerModel playerModel;
    public override void Init()
    {
        base.Init();
        gameData = ModelManager.GetModel<GameDataModel>();
        playerModel = ModelManager.GetModel<PlayerModel>();
        noButton.onClick.AddListener(NO_Button);
        adButton.onClick.AddListener(AD_Button);
        Sequence seq = DOTween.Sequence();
        seq.Append(adButton.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.7f));
        seq.Append(adButton.transform.DOScale(Vector3.one, 0.7f));
        seq.SetLoops(-1);
    }

    public override void Show()
    {
        base.Show();
        ani.Play("Show");
        adButton.transform.localScale = Vector3.one;

        bool isShowAdImagee = PlayerPrefs.GetInt("isShowGodAdImagee", 0) > FirebaseConfigData_AD._this.RVFreeTimes;
        adButton.transform.GetChild(0).gameObject.SetActive(!isShowAdImagee);
        adButton.transform.GetChild(1).gameObject.SetActive(isShowAdImagee);

        PlayerRoleData data = gameData.playerRoleModel.GetRoleDtat;
        string gender = data.roleSexIsM ? "him" : "her";

        if (playerModel.GetFraction >= 50)//本该送去地狱的人, 送去了天堂Do you want the Goddess to punish him/her?
        {
            showText.text = "Do you want the Goddess to \n<color=#FF0000>punish</color> " + gender + "?";
        }
        else//本该送去天堂的人, 送去了地狱//文案: Do you hope Goddess to pardon him / her ?        
        {
            showText.text = "Do you hope Goddess to \n<color=#55FF00>pardon</color> " + gender + "?";
        }
    }
    public override void Hide()
    {
        base.Hide();
        ani.Play("Hide");
    }


    private void NO_Button()
    {
        UIManager.Instance.CloseUI(PanelName.TriggerGodPanel);
        MsgManager.Invoke(MsgManagerType.TriggerGod, false);
    }

    private void AD_Button()
    {
        if (PlayerPrefs.GetInt("isShowGodAdImagee", 0) <= FirebaseConfigData_AD._this.RVFreeTimes)
        {
            PlayerPrefs.SetInt("isShowGodAdImagee", PlayerPrefs.GetInt("isShowGodAdImagee", 1) + 1);
            AdButtonDown();
        }
        else
        {
            MAX_SDK_Manager.GetInstantiate().ShowRewardedAd(AdButtonDown, "TriggerGod");
        }
    }
    private void AdButtonDown()
    {
        UIManager.Instance.CloseUI(PanelName.TriggerGodPanel);
        MsgManager.Invoke(MsgManagerType.TriggerGod, true);
    }
}
