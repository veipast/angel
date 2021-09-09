using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainScenePanel : UIBase
{
    public CanvasGroup _canvasGroup;
    public Button selectA_DButton;//-98.15 x
    public Button goAngelSceneButton;//98.15 x
    public Button goDevilSceneButton;//98.15 x
    private RectTransform selectA_DButtonTrans;//-98.15 x
    private RectTransform goAngelSceneButtonTrans;//98.15 x
    private RectTransform goDevilSceneButtonTrans;//98.15 x
    public GameObject occlusionRayImage;
    private GameDataModel gameData;

    public GameObject AngelRedDot;
    public GameObject DevilRedDot;
    private BuildDataModel buildData;
    public override void Init()
    {
        base.Init();
        gameData = ModelManager.GetModel<GameDataModel>();
        buildData = ModelManager.GetModel<BuildDataModel>();
        selectA_DButton.onClick.AddListener(AelectA_DButton);
        goAngelSceneButton.onClick.AddListener(GoAngelSceneButton);
        goDevilSceneButton.onClick.AddListener(GoDevilSceneButton);
        selectA_DButtonTrans = selectA_DButton.GetComponent<RectTransform>();
        goAngelSceneButtonTrans = goAngelSceneButton.GetComponent<RectTransform>();
        goDevilSceneButtonTrans = goDevilSceneButton.GetComponent<RectTransform>();
    }

    public override void Show()
    {
        base.Show();
        _canvasGroup.interactable = true;
        occlusionRayImage.SetActive(false);
        moveNumber = -98.15f;
        toMoveNumber = 98.15f;
        isImageMove = true;
        isCloseUI = false;
        isKeyDown = false;
        implementAction = null;
        selectA_DButtonTrans.anchoredPosition = new Vector2(-moveNumber, selectA_DButtonTrans.anchoredPosition.y);
        goAngelSceneButtonTrans.anchoredPosition = new Vector2(moveNumber, goAngelSceneButtonTrans.anchoredPosition.y);
        goDevilSceneButtonTrans.anchoredPosition = new Vector2(moveNumber, goDevilSceneButtonTrans.anchoredPosition.y);

        if (gameData.LevelNnmber <= 3)
        {
            goAngelSceneButton.interactable = false;
            goDevilSceneButton.interactable = false;
            goAngelSceneButton.transform.Find("Base Image").GetComponent<CanvasGroup>().alpha = 0.8f;
            goDevilSceneButton.transform.Find("Base Image").GetComponent<CanvasGroup>().alpha = 0.8f;
            goAngelSceneButton.transform.Find("Lock Image").gameObject.SetActive(true);
            goDevilSceneButton.transform.Find("Lock Image").gameObject.SetActive(true);
            goAngelSceneButton.transform.Find("Unlock Image").gameObject.SetActive(false);
            goDevilSceneButton.transform.Find("Unlock Image").gameObject.SetActive(false);
            //AngelRedDot.SetActive(false);
            //DevilRedDot.SetActive(false);
        }
        else
        {
            goAngelSceneButton.interactable = true;
            goDevilSceneButton.interactable = true;
            goAngelSceneButton.transform.Find("Base Image").GetComponent<CanvasGroup>().alpha = 1;
            goDevilSceneButton.transform.Find("Base Image").GetComponent<CanvasGroup>().alpha = 1;
            goAngelSceneButton.transform.Find("Lock Image").gameObject.SetActive(false);
            goDevilSceneButton.transform.Find("Lock Image").gameObject.SetActive(false);
            goAngelSceneButton.transform.Find("Unlock Image").gameObject.SetActive(true);
            goDevilSceneButton.transform.Find("Unlock Image").gameObject.SetActive(true);
        }
            AngelRedDot.SetActive(buildData.IsShowAngelRedDot);
            DevilRedDot.SetActive(buildData.IsShowDevilRedDot);
    }
    public override void Hide()
    {
        base.Hide();
    }

    private float moveNumber = -98.15f;
    private float toMoveNumber = 98.15f;
    private bool isImageMove = false;
    private bool isCloseUI = false;
    private System.Action implementAction;

    private bool isKeyDown = false;
    private void Update()
    {
        if (isImageMove)
        {
            moveNumber = Mathf.MoveTowards(moveNumber, toMoveNumber, Time.deltaTime * 3000);
            if (moveNumber == toMoveNumber)
            {
                isImageMove = false;
                if (isCloseUI)
                {
                    isCloseUI = false;
                    implementAction?.Invoke();
                    UIManager.Instance.CloseUI(PanelName.MainScenePanel);
                }
            }
            selectA_DButtonTrans.anchoredPosition = new Vector2(-moveNumber, selectA_DButtonTrans.anchoredPosition.y);
            goAngelSceneButtonTrans.anchoredPosition = new Vector2(moveNumber, goAngelSceneButtonTrans.anchoredPosition.y);
            goDevilSceneButtonTrans.anchoredPosition = new Vector2(moveNumber, goDevilSceneButtonTrans.anchoredPosition.y);
        }
        if (!isKeyDown && Input.anyKey && !ManagementTool.IsPointerOverGameObject())
        {
            isKeyDown = true;
            ClosePanel();
        }
    }

    private void AelectA_DButton()
    {
        ClosePanel();
        implementAction = () =>
        {
            UIManager.Instance.OpenUI(PanelName.AngelAndDevilPanel);
        };

        VibrateClassScript.GetInstancee.Haptic(MoreMountains.NiceVibrations.HapticTypes.Selection);
    }

    private void GoAngelSceneButton()
    {
        ClosePanel();
        implementAction = () =>
        {
            SceneLevelManager.Loading("Heaven Game Scene");
        };

        VibrateClassScript.GetInstancee.Haptic(MoreMountains.NiceVibrations.HapticTypes.Selection);
    }

    private void GoDevilSceneButton()
    {
        ClosePanel();
        implementAction = () =>
        {
            SceneLevelManager.Loading("Hell Game Scene");
        };

        VibrateClassScript.GetInstancee.Haptic(MoreMountains.NiceVibrations.HapticTypes.Selection);
    }


    private void ClosePanel()
    {
        occlusionRayImage.SetActive(true);
        _canvasGroup.interactable = false;
        isCloseUI = true;
        isImageMove = true;
        toMoveNumber = -98.15f;
    }
}
