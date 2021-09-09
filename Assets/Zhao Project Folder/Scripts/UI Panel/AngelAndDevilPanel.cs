using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class AngelAndDevilPanel : UIBase
{
    public CanvasGroup  _canvasGroup;
    public RectTransform baseImage;
    public Button closeButton;
    public RectTransform itemParent;
    public GameObject itemObject;



    private GameDataModel gameData;
    private List<AngelAndDevilItem> angelAndDevilItems = new List<AngelAndDevilItem>();

    
    public override void Init()
    {
        base.Init();
        gameData = ModelManager.GetModel<GameDataModel>();
        closeButton.onClick.AddListener(CloseButton);


        angelAndDevilItems.Add(itemObject.GetComponent<AngelAndDevilItem>());
        angelAndDevilItems[0].Init(0, this);
        for (int i = 1; i < gameData.angelAndDevilData.unlockItems.Length; i++)
        {
            angelAndDevilItems.Add(Instantiate(itemObject, itemParent, false).GetComponent<AngelAndDevilItem>());
            angelAndDevilItems[i].Init(i, this);
        }
    }


    private float upMoveNumber = -583.5f;
    private float toMoveNumber = 583.5f;
    private bool isUpMove = false;
    public override void Show()
    {
        base.Show();

        for (int i = 0; i < gameData.angelAndDevilData.unlockItems.Length; i++)
        {
            angelAndDevilItems[i].RefreshItem(gameData.angelAndDevilData.unlockItems[i], gameData.angelAndDevilData.selectItemIndex);
        }

        upMoveNumber = -583.5f;
        toMoveNumber = 583.5f;
        itemParent.anchoredPosition = Vector2.zero;
        baseImage.anchoredPosition = new Vector2(0, upMoveNumber);
        isUpMove = true;
        isClosePanel = false;
        _canvasGroup.interactable = true;
    }
    public override void Hide()
    {
        base.Hide();
    }
    private void Update()
    {
        if (isUpMove)
        {
            upMoveNumber = Mathf.MoveTowards(upMoveNumber, toMoveNumber, Time.deltaTime * 5000);
            baseImage.anchoredPosition = new Vector2(0, upMoveNumber);
            if (upMoveNumber == toMoveNumber)
            {
                isUpMove = false;
                if (isClosePanel)
                {
                    isClosePanel = false;
                    UIManager.Instance.OpenUI(PanelName.MainScenePanel);
                    UIManager.Instance.CloseUI(PanelName.AngelAndDevilPanel);
                    //UIManager.Instance.OpenUI(PanelName.);
                }
            }
        }
    }
    private bool isClosePanel = false;
    private void CloseButton()
    {
        _canvasGroup.interactable = false;
        toMoveNumber = -583.5f;
        isClosePanel = true;
        isUpMove = true;
        VibrateClassScript.GetInstancee.Haptic(MoreMountains.NiceVibrations.HapticTypes.Selection);
    }


    internal void SelectItem(int _ID)
    {
        if (gameData.angelAndDevilData.selectItemIndex != _ID && gameData.angelAndDevilData.unlockItems[_ID])
        {
            angelAndDevilItems[gameData.angelAndDevilData.selectItemIndex].SetSelectItemActive(false);
            gameData.angelAndDevilData.selectItemIndex = _ID;
            angelAndDevilItems[gameData.angelAndDevilData.selectItemIndex].SetSelectItemActive(true);
            MsgManager.Invoke(MsgManagerType.RefreshWingsItem);
            gameData.angelAndDevilData.SaveData();
        }
    }
}
