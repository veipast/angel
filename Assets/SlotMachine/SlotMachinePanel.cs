using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SlotMachinePanel : UIBase
{

    [Tooltip("是否随机相同奖励")]
    public bool isRandomSameReward = true;

    public List<RewardItemData> rewardItemDatas = new List<RewardItemData>();

    /// <summary>
    /// 奖励图标
    /// </summary>
    public List<Sprite> rewardSprites = new List<Sprite>();


    public List<RollerItem> rollerItems = new List<RollerItem>();

    [Tooltip("两个金币的FX 根据RewardCoinType决定")]
    public GameObject[] coinFX = new GameObject[2];

    public Sprite pullDownSprite;
    private Sprite pullUpSprite;
    public Button pullRodButton;
    public Button spinButton;
    private Animator ani;

    public GameObject ShowRewardItem;
    public Transform ShowRewardItemBase;

    [Space]
    public GameObject showRewardBase;
    [Tooltip("两个金币的Image 根据RewardCoinType决定")]
    public GameObject[] coinImages = new GameObject[2];
    public Text showCoinCountText;
    public Button adGetButton;
    public Button noThanksButton;


    public override void Init()
    {
        base.Init();
        pullUpSprite = pullRodButton.GetComponent<Image>().sprite;
        pullRodButton.onClick.AddListener(PullRodDown);
        spinButton.onClick.AddListener(PullRodDown);
        ani = transform.Find("SlotMachineBase/Base/lighting Base").GetComponent<Animator>();

        adGetButton.onClick.AddListener(AdGetButton);
        noThanksButton.onClick.AddListener(NoThanksButton);
        ShowRewardItemInit(ShowRewardItem.transform, 0);
        for (int i = 1; i < rewardItemDatas.Count; i++)
        {
            GameObject obj = Instantiate(ShowRewardItem, ShowRewardItemBase, false);
            ShowRewardItemInit(obj.transform, i);
        }
    }

    private void ShowRewardItemInit(Transform obj, int index)
    {
        for (int i = 0; i < 3; i++)
        {
            obj.GetChild(i).GetComponent<Image>().sprite = rewardItemDatas[index].rewardSprite;
        }
        obj.GetChild(3).GetComponent<Text>().text = "= " + rewardItemDatas[index].rewardVaiue;
        obj.GetChild(4).GetComponent<Image>().sprite = coinImages[(int)rewardItemDatas[index].rewardCoinType].GetComponent<Image>().sprite;
    }


    public override void Show()
    {
        base.Show();
        MsgManager.Invoke(MsgManagerType.SetPlayerMove, false);
        noThanksButton.interactable = true;
        adGetButton.interactable = true;
        coinFX[0].SetActive(false);
        coinFX[1].SetActive(false);
        coinImages[0].SetActive(false);
        coinImages[1].SetActive(false);
        showRewardBase.SetActive(false);
        for (int i = 0; i < rollerItems.Count; i++)
        {
            rollerItems[i].Init(rewardItemDatas, i);
        }
        spinButton.transform.GetChild(0).gameObject.SetActive(true);
        spinButton.transform.GetChild(1).gameObject.SetActive(false);


        adGetButton.transform.Find("Get Text").gameObject.SetActive(PlayerPrefs.GetInt("SlotMachineAD", 0) == 0);
        adGetButton.transform.Find("AD Base").gameObject.SetActive(PlayerPrefs.GetInt("SlotMachineAD", 0) == 1);
    }
    public override void Hide()
    {
        base.Hide();
    }

    private bool isButtonDown = false;
    private int countID;
    private bool SlotMachine_FlashOfLight = true;
    /// <summary>
    /// 拉下拉杆或者点下按钮
    /// </summary>
    private void PullRodDown()
    {
        if (isButtonDown) return;
        isButtonDown = true;
        ani.SetBool("SlotMachine_FlashOfLight", SlotMachine_FlashOfLight);//播放动画
        SlotMachine_FlashOfLight = !SlotMachine_FlashOfLight;//锁定
        pullRodButton.GetComponent<Image>().sprite = pullDownSprite;//刷新拉杆
        spinButton.transform.GetChild(0).gameObject.SetActive(false);//刷新按钮
        spinButton.transform.GetChild(1).gameObject.SetActive(true);//刷新按钮
        StartCoroutine(PullRodDownWait());//按钮弹起
        rewardItemIndex = new int[rollerItems.Count];
        countID = 0;
        int moveIndex = RandomRewardItemID();
        for (int i = 0; i < rollerItems.Count; i++)
        {
            if (!isRandomSameReward)
                moveIndex = RandomRewardItemID();
            rollerItems[i].StartTurning(EndRotation, moveIndex);
        }

        VibrateClassScript.GetInstancee.Haptic(MoreMountains.NiceVibrations.HapticTypes.Selection);
    }
    private IEnumerator PullRodDownWait()
    {
        yield return new WaitForSeconds(0.2f);
        pullRodButton.GetComponent<Image>().sprite = pullUpSprite;
        spinButton.transform.GetChild(0).gameObject.SetActive(true);
        spinButton.transform.GetChild(1).gameObject.SetActive(false);
    }
    private int[] rewardItemIndex;
    private RewardItemData getRewardData;
    private void EndRotation(int index)
    {
        rewardItemIndex[countID] = index;
        countID++;
        if (countID >= rewardItemIndex.Length)
        {
            for (int i = 1; i < rewardItemIndex.Length; i++)
            {
                if (rewardItemIndex[i] != rewardItemIndex[i - 1])
                    return;
                getRewardData = rewardItemDatas[rewardItemIndex[i]];
                StartCoroutine(ShowRewardBaseWait());
            }
        }
    }

    private IEnumerator ShowRewardBaseWait()
    {
        coinFX[(int)getRewardData.rewardCoinType].SetActive(true);
        coinImages[(int)getRewardData.rewardCoinType].SetActive(true);
        switch (getRewardData.rewardCoinType)
        {
            case RewardCoinType.AngelCoin:
                showCoinCountText.text = "<color=green>" + getRewardData.rewardVaiue + "</color>";
                break;
            case RewardCoinType.DevilCoin:
                showCoinCountText.text = "<color=red>" + getRewardData.rewardVaiue + "</color>";
                break;
            default:
                showCoinCountText.text = "<color=black>" + getRewardData.rewardVaiue + "</color>";
                break;
        }
        yield return new WaitForSeconds(2f);
        showRewardBase.SetActive(true);
        showRewardBase.transform.localScale = Vector3.zero;
        showRewardBase.transform.DOScale(Vector3.one, 0.25f);
        noThanksButton.transform.localScale = Vector3.zero;
        isButtonDown = false;
        yield return new WaitForSeconds(2f);
        noThanksButton.transform.DOScale(Vector3.one, 0.25f);
    }

    private int randomUpperLimit = 1000;
    private int RandomRewardItemID()
    {
        //创建 万分之一的集合
        int[] probability = new int[rewardItemDatas.Count];
        int num = 0;
        for (int i = 0; i < rewardItemDatas.Count; i++)
        {
            probability[i] = rewardItemDatas[i].rewardChance * randomUpperLimit;
            num += probability[i];
        }
        int[] randomPool = new int[num];
        int index = 0;
        int count = 0;
        for (int i = 0; i < randomPool.Length; i++)
        {
            randomPool[i] = index;
            count++;
            if (index < probability.Length)
                if (count >= probability[index])
                {
                    count = 0;
                    index++;
                }
        }
        return randomPool[Random.Range(0, randomPool.Length)];
    }


    private void NoThanksButton()
    {
        CloasUI();

        VibrateClassScript.GetInstancee.Haptic(MoreMountains.NiceVibrations.HapticTypes.Selection);
    }

    private void AdGetButton()
    {
        if (PlayerPrefs.GetInt("SlotMachineAD", 0) == 1)
            MAX_SDK_Manager.GetInstantiate().ShowRewardedAd(() =>
            {

            }, "SlotMachine");
        else
        {
            PlayerPrefs.SetInt("SlotMachineAD", 1);
            GameDataModel gameData = ModelManager.GetModel<GameDataModel>();
            gameData.X_2Coin(getRewardData.rewardVaiue, getRewardData.rewardCoinType == RewardCoinType.AngelCoin);
            CloasUI();
        }
        VibrateClassScript.GetInstancee.Haptic(MoreMountains.NiceVibrations.HapticTypes.Selection);
    }

    private void CloasUI()
    {
        noThanksButton.interactable = false;
        adGetButton.interactable = false;
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.5f);
        seq.AppendCallback(() =>
        {
            UIManager.Instance.CloseUI(PanelName.SlotMachinePanel);
        });
        seq.AppendInterval(0.5f);
        seq.AppendCallback(() =>
        {
            MsgManager.Invoke(MsgManagerType.SetPlayerMove, true);
        });
    }

}


public enum RewardCoinType
{
    //金币 钻石
    AngelCoin, DevilCoin
}
[System.Serializable]
public class RewardItemData
{
    /// <summary>
    /// 奖励类型
    /// </summary>
    public RewardCoinType rewardCoinType;
    /// <summary>
    /// 随机几率 按照按照百分比 总数和的百分比
    /// </summary>
    public int rewardChance;
    /// <summary>
    /// 奖励
    /// </summary>
    public int rewardVaiue;
    /// <summary>
    /// 奖励图标
    /// </summary>
    public Sprite rewardSprite;
}