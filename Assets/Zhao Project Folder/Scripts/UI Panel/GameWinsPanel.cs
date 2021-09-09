using System.Collections;
using System.Collections.Generic;
using AppsFlyerSDK;
using DG.Tweening;
using GameAnalyticsSDK;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameWinsPanel : UIBase
{
    public GameObject getCoinBase;
    public GameObject getFillBase;
    public TMP_Text youWinText;
    [Space]
    public Button noShanksButton;
    public Button getFillADButton;
    public Button getFillNoADButton;
    public Text fillNumberText;
    public Image fillImage;
    public Image angelImage;
    public Image devilImage;

    [Space]
    public Button nextSceneButton;
    public Text coinCountText;
    public Button nextADSceneButton;
    public Text rewardCoinCountText;
    public Text get_X_NumberText;
    public Transform pointerImage;
    public List<GameObject> angeCoinImage = new List<GameObject>();
    public List<GameObject> devilCoinImage = new List<GameObject>();

    private PlayerModel playerModel;
    private GameDataModel gameData;
    private BuildDataModel buildData;
    public override void Init()
    {
        base.Init();
        gameData = ModelManager.GetModel<GameDataModel>();
        playerModel = ModelManager.GetModel<PlayerModel>();
        buildData = ModelManager.GetModel<BuildDataModel>();
        noShanksButton.onClick.AddListener(NoShanksButton);
        getFillADButton.onClick.AddListener(GetFillADButton);
        getFillNoADButton.onClick.AddListener(GetFillADButton);
        //下一个场景
        nextSceneButton.onClick.AddListener(Level_Next);
        nextADSceneButton.onClick.AddListener(Level_AD_Next);
    }

    public override void Hide()
    {
        base.Hide();
    }
    private int coinCounr;
    public override void Show()
    {
        base.Show();

        UIManager.Instance.CloseUI(PanelName.MainPanel);
        UIManager.Instance.CloseUI(PanelName.RoleDescriptionPanel);
        if (gameData.LevelNnmber == 1 || gameData.LevelNnmber == 35)
        {
            AppStoreRatingTool.Instance.PullUpRatingWindow();
        }
        if (!gameData.angelAndDevilData.isAllUnlock)//没有全部解锁
        {
            ShowGetFillItemPanel();
        }
        else
        {
            getFillBase.SetActive(false);
            ShowNextScenePanel();
        }
        //下一个场景
        //进行尝尽判断
        PlayerRoleData data = gameData.playerRoleModel.GetRoleDtat;

        bool isHeaven = playerModel.GetFraction >= 50;
        if (data.levelTargetType == PlayerRoleData.LevelTargetType.Heaven)
        {
            if (isHeaven)//应去Heaven, 实际去了Heaven: HE/SHE WORTH IT
            {
                youWinText.text = (data.roleSexIsM ? "HE" : "SHE") + " WORTH IT";
                youWinText.color = Color.green;
            }
            else//应去Heaven, 但去了Hell, 且没有被反转: DEPRAVED
            {
                youWinText.text = "DEPRAVED";
                youWinText.color = Color.red;
            }
        }
        else
        {
            if (isHeaven)//应去Hell, 但去了Heaven: YOU SAVED HIM/HER
            {
                youWinText.text = "YOU SAVED " + (data.roleSexIsM ? "HIM" : "HER");
                youWinText.color = Color.green;
            }
            else//应去Hell, 实际去了Hell: HE/SHE DESERVED
            {
                youWinText.text = (data.roleSexIsM ? "HE" : "SHE") + " DESERVED";
                youWinText.color = Color.red;
            }
        }

        youWinText.transform.localScale = Vector3.zero;
        youWinText.transform.DOScale(Vector3.one, 0.3f);

        if ((data.levelTargetType == PlayerRoleData.LevelTargetType.Heaven && playerModel.GetFraction >= 50) ||
            (data.levelTargetType == PlayerRoleData.LevelTargetType.Hell && playerModel.GetFraction < 50))
        {
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, gameData.LevelNnmber.ToString("00000"));
        }
        else
        {
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, gameData.LevelNnmber.ToString("00000"));
        }
    }
    #region 解锁物品
    private int itemIndex = 0;
    private float toMoveNumber = 0;
    private bool isMoveNumber = false;
    private float speedPluse = 0;
    private void ShowGetFillItemPanel()
    {
        getCoinBase.SetActive(false);
        getFillBase.SetActive(true);
        noShanksButton.gameObject.SetActive(true);
        //getFillADButton.gameObject.SetActive(true);
        getFillADButton.gameObject.SetActive(PlayerPrefs.GetInt("Get different wings", 0) != 0);
        getFillNoADButton.gameObject.SetActive(PlayerPrefs.GetInt("Get different wings", 0) == 0);
        noShanksButton.transform.localScale = Vector3.zero;
        getFillADButton.transform.localScale = Vector3.zero;
        getFillNoADButton.transform.localScale = Vector3.zero;
        speedPluse = gameData.angelAndDevilData.GetRandomMoveNumber();
        toMoveNumber = gameData.angelAndDevilData.progressNumber + speedPluse;
        if (toMoveNumber > 100)
        {
            toMoveNumber = 100;
        }
        fillImage.fillAmount = (100 - gameData.angelAndDevilData.progressNumber) / 100;
        fillNumberText.text = (gameData.angelAndDevilData.progressNumber).ToString("0.0") + "%";
        if (gameData.angelAndDevilData.unlockItemIndex != -1)
        {
            angelImage.sprite = ResourcesManager.Load<Sprite>(ResourcesType.Sprite, "Angel_" + gameData.angelAndDevilData.unlockItemIndex);
            devilImage.sprite = ResourcesManager.Load<Sprite>(ResourcesType.Sprite, "Devil_" + gameData.angelAndDevilData.unlockItemIndex);
        }
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.5f);
        seq.AppendCallback(() => { isMoveNumber = true; });
    }

    private void NoShanksButton()
    {
        ShowNextScenePanel();
        gameData.angelAndDevilData.SaveData();

        VibrateClassScript.GetInstancee.Haptic(MoreMountains.NiceVibrations.HapticTypes.Selection);
    }

    private void GetFillADButton()
    {
        if (PlayerPrefs.GetInt("Get different wings", 0) == 0)
        {
            ShowNextScenePanel();
            gameData.angelAndDevilData.UnlockItem(itemIndex);
            gameData.angelAndDevilData.SaveData();
            PlayerPrefs.SetInt("Get different wings", 1);
        }
        else
            MAX_SDK_Manager.GetInstantiate().ShowRewardedAd((System.Action)(() =>
            {
                ShowNextScenePanel();
                gameData.angelAndDevilData.UnlockItem(itemIndex);
                gameData.angelAndDevilData.SaveData();
            }), "Get different wings");

        VibrateClassScript.GetInstancee.Haptic(MoreMountains.NiceVibrations.HapticTypes.Selection);
    }
    private void FillImageUpdate()
    {
        if (isMoveNumber)
        {
            gameData.angelAndDevilData.progressNumber = Mathf.MoveTowards(gameData.angelAndDevilData.progressNumber, toMoveNumber, Time.deltaTime * speedPluse);

            fillImage.fillAmount = (100 - gameData.angelAndDevilData.progressNumber) / 100;
            fillNumberText.text = (gameData.angelAndDevilData.progressNumber).ToString("0.0") + "%";

            if (gameData.angelAndDevilData.progressNumber == toMoveNumber)
            {
                isMoveNumber = false;
                if (gameData.angelAndDevilData.progressNumber >= 100)
                {
                    gameData.angelAndDevilData.progressNumber = 0;
                    itemIndex = gameData.angelAndDevilData.unlockItemIndex;
                    gameData.angelAndDevilData.FindLockItem(true);
                    getFillADButton.transform.DOScale(Vector3.one, 0.3f);
                    getFillNoADButton.transform.DOScale(Vector3.one, 0.3f);
                    Sequence seq = DOTween.Sequence();
                    seq.AppendInterval(1.5f);
                    seq.Append(noShanksButton.transform.DOScale(Vector3.one, 0.3f));
                }
                else
                {
                    ShowNextScenePanel();
                }
            }
        }
    }


    #endregion
    #region 下一个场景
    private void ShowNextScenePanel()
    {
        noShanksButton.gameObject.SetActive(false);
        getFillADButton.gameObject.SetActive(false);
        getFillNoADButton.gameObject.SetActive(false);
        getCoinBase.SetActive(true);
        rotateNumber = 0;
        isRotate = true;
        isUpRotate = true;
        pointerImage.eulerAngles = Vector3.zero;
        nextADSceneButton.transform.parent.localScale = Vector3.zero;
        nextADSceneButton.transform.parent.DOScale(Vector3.one, 0.3f);
        nextSceneButton.transform.localScale = Vector3.zero;
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(1.5f);
        seq.Append(nextSceneButton.transform.DOScale(Vector3.one, 0.3f));
        ShowCoinObject(playerModel.GetFraction >= 50);
        coinCounr = gameData.GetCoinCount();
        coinCountText.text = coinCounr.ToString();
        rewardCoinCountText.text = coinCounr.ToString();
        nextADSceneButton.interactable = true;
        nextSceneButton.interactable = true;
    }
    public void Level_AD_Next()
    {
        MAX_SDK_Manager.GetInstantiate().ShowRewardedAd(() =>
        {
            isRotate = false;
            //判断分数
            //0 -50  -100 -155 -180
            if (rotateNumber >= -50)
            {
                SetMoney(5);
            }
            else if (rotateNumber >= -100)
            {
                SetMoney(4);
            }
            else if (rotateNumber >= -155)
            {
                SetMoney(3);
            }
            else
            {
                SetMoney(2);
            }
            StartCoroutine(LoadNextScene());
        }, "GameOver");

        VibrateClassScript.GetInstancee.Haptic(MoreMountains.NiceVibrations.HapticTypes.Selection);
    }
    public void Level_Next()
    {
        PlayerRoleData data = gameData.playerRoleModel.GetRoleDtat;
        if (!((data.levelTargetType == PlayerRoleData.LevelTargetType.Heaven && playerModel.GetFraction >= 50) ||
           (data.levelTargetType == PlayerRoleData.LevelTargetType.Hell && playerModel.GetFraction < 50)))
        {
            GameAnalytics.NewDesignEvent("FailNext", gameData.LevelNnmber);
        }


        MAX_SDK_Manager.GetInstantiate().ShowInterstitial(() =>
        {
            SetMoney(1);
            isRotate = false;
            StartCoroutine(LoadNextScene());
        }, "GameOver", true);

        VibrateClassScript.GetInstancee.Haptic(MoreMountains.NiceVibrations.HapticTypes.Selection);
    }
    public void SetMoney(int X_Number)
    {
        //计算钱财
        if (Inter_First_Show._this.InterFirstShow <= gameData.LevelNnmber) Inter_First_Show._this.isShowAD = true;
        MsgManager.Invoke(MsgManagerType.ShowAngelCoin, X_Number);
        MsgManager.Invoke(MsgManagerType.CoinCountPlus);
    }
    private IEnumerator LoadNextScene()
    {
        nextADSceneButton.interactable = false;
        nextSceneButton.interactable = false;
        yield return new WaitForSeconds(1f);
        nextADSceneButton.transform.parent.localScale = Vector3.zero;
        nextSceneButton.transform.localScale = Vector3.zero;
        yield return new WaitForSeconds(2f);
        if (gameData.LevelNnmber % 5 == 0)
        {
            AppsFlyer.sendEvent("LEVEL_" + gameData.LevelNnmber.ToString("00000"), new Dictionary<string, string>());
        }
        //GA 断点消息 结束游戏

        UIManager.Instance.CloseUI(PanelName.GameWinsPanel);
        UIManager.Instance.CloseUI(PanelName.SlotMachinePanel);
        gameData.SaveData();
        int ShowBuildSceneCount = PlayerPrefs.GetInt("ShowBuildSceneCount", 0);
        if (gameData.LevelNnmber == 3)
            SceneLevelManager.Loading("Heaven Game Scene");
        else if (gameData.LevelNnmber > 3 && ShowBuildSceneCount >= 5)
        {
            ShowBuildSceneCount = 0;
            if (buildData.IsShowAngelRedDot)
            {
                SceneLevelManager.Loading("Heaven Game Scene");
            }
            else if (buildData.IsShowDevilRedDot)
            {
                SceneLevelManager.Loading("Hell Game Scene");
            }
            else
                SceneLevelManager.Loading(gameData.LoadSceneName);
        }
        else
        {
            SceneLevelManager.Loading(gameData.LoadSceneName);
            ShowBuildSceneCount++;
        }
        PlayerPrefs.SetInt("ShowBuildSceneCount", ShowBuildSceneCount);
        gameData.LoadNextSceneData();
    }
    private void ShowCoinObject(bool isShowAnge)
    {
        foreach (var item in angeCoinImage)
        {
            item.SetActive(isShowAnge);
        }
        foreach (var item in devilCoinImage)
        {
            item.SetActive(!isShowAnge);
        }
    }
    private bool isRotate = false;
    private bool isUpRotate = true;
    private float rotateNumber = 0;
    private void Update()
    {
        FillImageUpdate();
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
                get_X_NumberText.text = "GET X" + "5";
                rewardCoinCountText.text = (coinCounr * 5).ToString();
            }
            else if (rotateNumber >= -100)
            {
                get_X_NumberText.text = "GET X" + "4";
                rewardCoinCountText.text = (coinCounr * 4).ToString();
            }
            else if (rotateNumber >= -155)
            {
                get_X_NumberText.text = "GET X" + "3";
                rewardCoinCountText.text = (coinCounr * 3).ToString();
            }
            else
            {
                get_X_NumberText.text = "GET X" + "2";
                rewardCoinCountText.text = (coinCounr * 2).ToString();
            }
        }
    }
    #endregion
}
