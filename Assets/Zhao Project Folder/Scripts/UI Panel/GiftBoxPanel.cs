using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GiftBoxPanel : UIBase
{
    public Text showCoinCountText;
    public Button noThanksButton;
    public Button getADButton;
    public GameObject AngelImage;
    public GameObject DevilImage;
    public Transform pointerImage;
    private GameDataModel gameData;
    public Image baseImage;
    public List<Color> colors = new List<Color>();
    public override void Init()
    {
        base.Init();
        noThanksButton.onClick.AddListener(NoThanksButton);
        getADButton.onClick.AddListener(GetADButton);
        MsgManager.Add(MsgManagerType.ShowGiftBoxPanelType, ShowGiftBoxPanelType);
        gameData = ModelManager.GetModel<GameDataModel>();
    }
    private int coinCount;
    private bool isAngel;
    private void ShowGiftBoxPanelType(object obj)
    {
        isAngel = (bool)obj;
        AngelImage.SetActive(isAngel);
        DevilImage.SetActive(!isAngel);
        coinCount = gameData.GetAllCoinCount(isAngel);
        baseImage.color = isAngel ? colors[0] : colors[1];

        if (isAngel)
        {
            getADButton.transform.Find("Get Text").gameObject.SetActive(PlayerPrefs.GetInt("A_GiftBoxPanel", 0) == 0);
            getADButton.transform.Find("AD Base").gameObject.SetActive(PlayerPrefs.GetInt("A_GiftBoxPanel", 0) == 1);
        }
        else
        {

            getADButton.transform.Find("Get Text").gameObject.SetActive(PlayerPrefs.GetInt("D_GiftBoxPanel", 0) == 0);
            getADButton.transform.Find("AD Base").gameObject.SetActive(PlayerPrefs.GetInt("D_GiftBoxPanel", 0) == 1);
        }


    }

    public override void Show()
    {
        base.Show();
        pointerImage.eulerAngles = Vector3.zero;
        noThanksButton.transform.localScale = Vector3.zero;
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(1.5f);
        seq.Append(noThanksButton.transform.DOScale(Vector3.one, 0.3f));
        isRotate = true;
        isUpRotate = true;
        rotateNumber = 0;
    }
    public override void Hide()
    {
        base.Hide();
    }

    private void NoThanksButton()
    {
        UIManager.Instance.CloseUI(PanelName.GiftBoxPanel);
        MsgManager.Invoke(MsgManagerType.DestroyGiftBox);

        VibrateClassScript.GetInstancee.Haptic(MoreMountains.NiceVibrations.HapticTypes.Selection);
    }

    private void GetADButton()
    {
        if (isAngel && PlayerPrefs.GetInt("A_GiftBoxPanel", 0) == 0)
        {
            ADButtonDown();
            PlayerPrefs.SetInt("A_GiftBoxPanel", 1);
        }
        else if (!isAngel && PlayerPrefs.GetInt("D_GiftBoxPanel", 0) == 0)
        {
            ADButtonDown();
            PlayerPrefs.SetInt("D_GiftBoxPanel", 1);
        }
        else
        {
            MAX_SDK_Manager.GetInstantiate().ShowRewardedAd(() =>
            {
                ADButtonDown();
            }, "GiftBox");
        }
        VibrateClassScript.GetInstancee.Haptic(MoreMountains.NiceVibrations.HapticTypes.Selection);
    }
    private void ADButtonDown()
    {
        UIManager.Instance.CloseUI(PanelName.GiftBoxPanel);
        MsgManager.Invoke(MsgManagerType.DestroyGiftBox);
        isRotate = false;
        if (rotateNumber >= -50)
        {
            gameData.X_2Coin((int)(coinCount * 2f) - coinCount, isAngel);
        }
        else if (rotateNumber >= -100)
        {
            gameData.X_2Coin((int)(coinCount * 1.8f) - coinCount, isAngel);
        }
        else if (rotateNumber >= -155)
        {
            gameData.X_2Coin((int)(coinCount * 1.5f) - coinCount, isAngel);
        }
        else
        {
            gameData.X_2Coin((int)(coinCount * 1.2f) - coinCount, isAngel);
        }
    }
    private bool isRotate = true;
    private bool isUpRotate = true;
    private float rotateNumber = 0;
    private void Update()
    {
        if (isRotate)
        {
            if (isUpRotate)
            {
                rotateNumber = Mathf.MoveTowards(rotateNumber, -180, Time.deltaTime * 200);
                if (rotateNumber <= -180)
                {
                    isUpRotate = false;
                }
            }
            else
            {
                rotateNumber = Mathf.MoveTowards(rotateNumber, 0, Time.deltaTime * 200);
                if (rotateNumber >= 0)
                {
                    isUpRotate = true;
                }
            }
            pointerImage.eulerAngles = new Vector3(0, 0, rotateNumber);
            if (rotateNumber >= -50)
            {
                showCoinCountText.text = ((int)(coinCount * 2f)).ToString();
            }
            else if (rotateNumber >= -100)
            {
                showCoinCountText.text = ((int)(coinCount * 1.8f)).ToString();
            }
            else if (rotateNumber >= -155)
            {
                showCoinCountText.text = ((int)(coinCount * 1.5f)).ToString();
            }
            else
            {
                showCoinCountText.text = ((int)(coinCount * 1.2f)).ToString();
            }
        }
    }

}
