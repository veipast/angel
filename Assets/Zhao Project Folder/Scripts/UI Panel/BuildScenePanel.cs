using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//using Google.Play.Review;

public class BuildScenePanel : UIBase
{
    public Button homeButton;
    public Button To_HELL_Button;
    public Button To_HEAVEN_Button;
    public Text CoinNotEnough;
    public Animator animator;
    public GameObject XSYD;//新手引导
    private int XSYD_Index
    {
        get { return PlayerPrefs.GetInt("XSYD_Index", 1); }
        set
        {
            int i = value;
            //-1 0 1 2 3
            if (i >= 5) i = 5;
            PlayerPrefs.SetInt("XSYD_Index", i);
        }
    }

    public GameObject coinBuildButton;
    public GameObject adBuildButton;
    public GameObject lockBuildButton;

    private GameDataModel gameData;
    private BuildDataModel buildData;
    //private Transform buileBase;
    public override void Init()
    {
        base.Init();
        gameData = ModelManager.GetModel<GameDataModel>();
        buildData = ModelManager.GetModel<BuildDataModel>();
        homeButton.onClick.AddListener(HomeButton);
        To_HELL_Button.onClick.AddListener(ToHELL_Button);
        To_HEAVEN_Button.onClick.AddListener(ToHEAVEN_Button);
        MsgManager.Add(MsgManagerType.BuildLoadButton, BuildLoadButton);

    }
    private bool isHeavenScene;
    public override void Show()
    {
        base.Show();
        isHeavenScene = SceneManager.GetActiveScene().name == "Heaven Game Scene";
        To_HELL_Button.gameObject.SetActive(isHeavenScene);
        To_HEAVEN_Button.gameObject.SetActive(!isHeavenScene);
        CoinNotEnough.text = isHeavenScene ? "Angel Coin Not Enough" : "Devil Coin Not Enough";

        //XSYD_FUN();
    }
    public override void Hide()
    {
        base.Hide();
        foreach (var item in buildButtonItems)
        {
            Destroy(item.gameObject);
        }
        buildButtonItems.Clear();
        //Resources.UnloadUnusedAssets();//卸载未占用的asset资源
        System.GC.Collect();
    }

    private void HomeButton()
    {
        if (XSYD_Index == 4 && !isHeavenScene)
        {
            Destroy(homeButton.gameObject.GetComponent<GraphicRaycaster>());
            Destroy(homeButton.gameObject.GetComponent<Canvas>());
            XSYD.SetActive(false);
            XSYD_Index++;
            homeButton.transform.Find("XSYD Image").gameObject.SetActive(false);
        }
        SceneLevelManager.Loading(gameData.LoadSceneName);
        VibrateClassScript.GetInstancee.Haptic(MoreMountains.NiceVibrations.HapticTypes.Selection);
    }
    private void ToHELL_Button()
    {
        if (XSYD_Index == 2 && isHeavenScene)
        {
            Destroy(To_HELL_Button.gameObject.GetComponent<GraphicRaycaster>());
            Destroy(To_HELL_Button.gameObject.GetComponent<Canvas>());
            XSYD.SetActive(false);
            XSYD_Index++;
            To_HELL_Button.transform.Find("XSYD Image").gameObject.SetActive(false);
        }
        SceneLevelManager.Loading("Hell Game Scene");
        VibrateClassScript.GetInstancee.Haptic(MoreMountains.NiceVibrations.HapticTypes.Selection);
    }

    private void ToHEAVEN_Button()
    {
        SceneLevelManager.Loading("Heaven Game Scene");
        VibrateClassScript.GetInstancee.Haptic(MoreMountains.NiceVibrations.HapticTypes.Selection);
    }
    private List<BuildButtonItem> buildButtonItems = new List<BuildButtonItem>();
    private Transform trans;
    private void BuildLoadButton(object obj)
    {
        if (obj != null)
            trans = obj as Transform;
        foreach (var item in buildButtonItems)
        {
            Destroy(item.gameObject);
        }
        buildButtonItems.Clear();

        //金币按钮 广告按钮 解锁按钮

        //找到第一个key未解锁但LEVEL达到的 //非广告
        int key, index;
        buildData.GetOneKey(out key, out index, gameData.LevelNnmber);
        if (key != -1)
        {
            BuildButtonItem item = Instantiate(coinBuildButton, transform, false).AddComponent<BuildButtonItem>();
            buildButtonItems.Add(item);
            item.Init(key, index, buildData.LoadBuildSceneData[key][index], trans.GetChild(key).GetChild(0).position, buildData.LoadBuildSceneType, 0, animator);
            if (XSYD_Index == 1 && isHeavenScene)
            {
                Canvas c = item.gameObject.AddComponent<Canvas>();
                c.overrideSorting = true;
                c.sortingOrder = 2;
                item.gameObject.AddComponent<GraphicRaycaster>();
                XSYD.SetActive(true);
                item.transform.Find("XSYD Image").gameObject.SetActive(true);
            }
            if (XSYD_Index == 3 && !isHeavenScene)
            {
                Canvas c = item.gameObject.AddComponent<Canvas>();
                c.overrideSorting = true;
                c.sortingOrder = 2;
                item.gameObject.AddComponent<GraphicRaycaster>();
                XSYD.SetActive(true);
                item.transform.Find("XSYD Image").gameObject.SetActive(true);
            }

        }

        //找到第二个Key未解锁但LEVEL达到的 //广告金币都可以
        bool flag = buildData.GetTowKey(out key, out index, gameData.LevelNnmber);
        if (key != -1)
        {
            //if (flag)
            //{
                BuildButtonItem item = Instantiate(coinBuildButton, transform, false).AddComponent<BuildButtonItem>();
                buildButtonItems.Add(item);
                item.Init(key, index, buildData.LoadBuildSceneData[key][index], trans.GetChild(key).GetChild(0).position, buildData.LoadBuildSceneType, 0, animator);
            //}
            //else
            //{
            //    BuildButtonItem item = Instantiate(adBuildButton, transform, false).AddComponent<BuildButtonItem>();
            //    buildButtonItems.Add(item);
            //    item.Init(key, index, buildData.LoadBuildSceneData[key][index], trans.GetChild(key).GetChild(0).position, buildData.LoadBuildSceneType, 1, animator);
            //}
        }


        //找到不是 （第一个未解锁但LEVEL达到的）Key 的哪一个Key
        buildData.GetLockKey(out key, out index, gameData.LevelNnmber);
        if (key != -1)
        {
            BuildButtonItem item = Instantiate(lockBuildButton, transform, false).AddComponent<BuildButtonItem>();
            buildButtonItems.Add(item);
            item.Init(key, index, buildData.LoadBuildSceneData[key][index], trans.GetChild(key).GetChild(0).position, buildData.LoadBuildSceneType, 2, animator);
        }

        //Debug.Log(trans.GetChild(key).GetChild(0).name);
        //XSYD.SetActive(false);
        if (XSYD_Index == 2 && isHeavenScene)
        {
            Canvas c = To_HELL_Button.gameObject.AddComponent<Canvas>();
            c.overrideSorting = true;
            c.sortingOrder = 2;
            To_HELL_Button.gameObject.AddComponent<GraphicRaycaster>();
            Sequence seq = DOTween.Sequence();
            seq.Append(To_HELL_Button.transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 1));
            seq.Append(To_HELL_Button.transform.DOScale(new Vector3(1f, 1f, 1f), 1));
            seq.SetLoops(5);
            XSYD.SetActive(true);
            To_HELL_Button.transform.Find("XSYD Image").gameObject.SetActive(true);
        }
        if (XSYD_Index == 4 && !isHeavenScene)
        {
            Canvas c = homeButton.gameObject.AddComponent<Canvas>();
            c.overrideSorting = true;
            c.sortingOrder = 2;
            homeButton.gameObject.AddComponent<GraphicRaycaster>();
            XSYD.SetActive(true);
            Sequence seq = DOTween.Sequence();
            seq.Append(homeButton.transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 1));
            seq.Append(homeButton.transform.DOScale(new Vector3(1f, 1f, 1f), 1));
            seq.SetLoops(5);
            XSYD.SetActive(true);
            homeButton.transform.Find("XSYD Image").gameObject.SetActive(true);
        }
    }
}
