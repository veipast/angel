using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class BuildButtonItem : MonoBehaviour
{
    public struct BuildIndex
    {
        public int key, index;
        public BuildSceneUpgradeData buildSceneUpgradeData;
    }
    private BuildIndex buildIndex;
    private Vector3 position;
    private LoadBuildSceneType loadBuildSceneType;
    private Camera _camera;
    private CanvasScaler _scaler;
    private RectTransform rectTransform;
    private int buttonType;
    private GameDataModel gameData;
    private BuildDataModel buildData;
    private Animator animator;
    internal void Init(int key, int index, BuildSceneUpgradeData buildSceneUpgradeData, Vector3 position, LoadBuildSceneType loadBuildSceneType, int buttonType, Animator animator)
    {
        gameData = ModelManager.GetModel<GameDataModel>();
        buildData = ModelManager.GetModel<BuildDataModel>();
        buildIndex.key = key;
        buildIndex.index = index;
        buildIndex.buildSceneUpgradeData = buildSceneUpgradeData;
        this.position = position;
        this.loadBuildSceneType = loadBuildSceneType;
        this.buttonType = buttonType;
        this.animator = animator;
        switch (buttonType)
        {
            case 0:
                {
                    Text coinCountText = transform.Find("Coin Count Text").GetComponent<Text>();
                    coinCountText.text = buildSceneUpgradeData.upgradeCost.ToString();
                    if (loadBuildSceneType == LoadBuildSceneType.Heaven)
                        transform.Find("Coin_A Image").gameObject.SetActive(true);
                    else
                        transform.Find("Coin_D Image").gameObject.SetActive(true);
                    float coin = SceneManager.GetActiveScene().name == "Heaven Game Scene" ? gameData.AngelCoin : gameData.DevilCoin;
                    if (buildSceneUpgradeData.upgradeCost <= coin)
                    {
                        Sequence seq = DOTween.Sequence();
                        seq.Append(transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 1));
                        seq.Append(transform.DOScale(new Vector3(1f, 1f, 1f), 1));
                        seq.SetLoops(-1);
                    }
                }
                break;
            case 1:
                {
                    Sequence seq = DOTween.Sequence();
                    seq.Append(transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 1));
                    seq.Append(transform.DOScale(new Vector3(1f, 1f, 1f), 1));
                    seq.SetLoops(-1);
                }
                break;
            case 2:
                {
                    Text coinCountText = transform.Find("Unlock Level Text").GetComponent<Text>();
                    coinCountText.text = "LEVEL " + buildSceneUpgradeData.upgradeLevel.ToString();
                }
                break;
            default:
                break;
        }
        //if (SceneManager.GetActiveScene().name == "Heaven Game Scene")
        _camera = Camera.main;
        _scaler = UIManager.Instance.GetCanvasScaler;
        rectTransform = GetComponent<RectTransform>();
        GetComponent<Button>().onClick.AddListener(SelectButton);
        gameObject.SetActive(true);
    }
    private void LateUpdate()
    {
        Vector3 playerHead = position + new Vector3(0, 2, 0);
        playerHead = WorldToUI(_scaler, _camera, playerHead);
        rectTransform.anchoredPosition3D = playerHead;
    }
    public static Vector3 WorldToUI(CanvasScaler scaler, Camera camera, Vector3 pos)
    {
        float resolutionX = scaler.referenceResolution.x;
        float resolutionY = scaler.referenceResolution.y;

        Vector3 viewportPos = camera.WorldToViewportPoint(pos);

        Vector3 uiPos = new Vector3(viewportPos.x * resolutionX - resolutionX * 0.5f,
             viewportPos.y * resolutionY - resolutionY * 0.5f, 0);

        return uiPos;
    }
    private void SelectButton()
    {
        if (buttonType == 0)
        {
            //花钱建造
            StartBuild(buildIndex.buildSceneUpgradeData.upgradeCost);
        }
        else if (buttonType == 1)
        {
            //看广告建造
            MAX_SDK_Manager.GetInstantiate().ShowRewardedAd(() =>
            {
                StartBuild(0);
            }, "BuileScene");
        }
        VibrateClassScript.GetInstancee.Haptic(MoreMountains.NiceVibrations.HapticTypes.Selection);
    }

    private void StartBuild(int coinCount)
    {
        //gameData.LevelNnmber = 50;
        //gameData.AngelCoin = 1000000;
        //gameData.DevilCoin = 1000000;
        //coinCount = 1;
        PlayerPrefs.SetInt("XSYD_Index", PlayerPrefs.GetInt("XSYD_Index", 1) + 1);
        if (coinCount != 0)
        {
            switch (loadBuildSceneType)
            {
                case LoadBuildSceneType.Hell:
                    if (gameData.DevilCoin - coinCount >= 0)
                    {
                        gameData.DevilCoin -= coinCount;
                        MsgManager.Invoke(MsgManagerType.ShowDevilCoin, "false|- " + coinCount.ToString());
                        buildData.BuildUnlock(buildIndex.key, buildIndex.index);
                        MsgManager.Invoke(MsgManagerType.RefreshBuildScene, buildIndex);
                    }
                    else
                    {
                        //animator.Play("Show");
                        UIManager.Instance.OpenUI(PanelName.BuildCoinNotPanel);
                    }
                    break;
                case LoadBuildSceneType.Heaven:
                    if (gameData.AngelCoin - coinCount >= 0)
                    {
                        gameData.AngelCoin -= coinCount;
                        MsgManager.Invoke(MsgManagerType.ShowDevilCoin, "true|- " + coinCount.ToString());
                        buildData.BuildUnlock(buildIndex.key, buildIndex.index);
                        MsgManager.Invoke(MsgManagerType.RefreshBuildScene, buildIndex);
                    }
                    else
                    {
                        //animator.Play("Show");
                        UIManager.Instance.OpenUI(PanelName.BuildCoinNotPanel);
                    }
                    break;
                default:
                    break;
            }
        }
        else
        {
            buildData.BuildUnlock(buildIndex.key, buildIndex.index);
            MsgManager.Invoke(MsgManagerType.RefreshBuildScene, buildIndex);
        }
        gameData.SaveData();
        buildData.SaveData();
    }
}
