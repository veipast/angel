using System.Collections;
using System.Collections.Generic;
using GameAnalyticsSDK;
using UnityEngine;

public class TerminalPointDoor : MonoBehaviour
{
    public enum DoorStart
    {
        Angel, Demon
    }
    public DoorStart doorState;
    [Tooltip("打开门的动画")]
    private Animator doorAni;
    [Tooltip("通过所需分数")]
    public int scores;
    [Tooltip("最终结算倍数")]
    public int multiple;
    private PlayerModel playerModel;
    private GameDataModel gameData;
    // Start is called before the first frame update
    void Start()
    {
        playerModel = ModelManager.GetModel<PlayerModel>();
        gameData = ModelManager.GetModel<GameDataModel>();
        doorAni = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //分数够打开门
            //不够停止移动 计算最终金币价格
            if (doorState == DoorStart.Angel)
            {
                if (playerModel.GetFraction >= scores)
                {
                    if (doorAni)
                        doorAni.Play("Open");
                    Destroy(gameObject, 0.5f);
                }
                else
                {
                    PlayerController p = other.GetComponent<PlayerController>();
                    p.TerminalPointStop(true);
                    PlayerRoleData data = gameData.playerRoleModel.GetRoleDtat;
                    if (multiple == 5 && data.levelTargetType == PlayerRoleData.LevelTargetType.Heaven)
                    {
                        GameAnalytics.NewDesignEvent("CompletePerfect", gameData.LevelNnmber);
                    }
                }
            }
            else
            {
                if (playerModel.GetFraction <= scores)
                {
                    if (doorAni)
                        doorAni.Play("Open");
                    Destroy(gameObject, 0.5f);
                }
                else
                {
                    PlayerController p = other.GetComponent<PlayerController>();
                    p.TerminalPointStop(false);

                    PlayerRoleData data = gameData.playerRoleModel.GetRoleDtat;
                    if (multiple == 5 && data.levelTargetType == PlayerRoleData.LevelTargetType.Hell)
                    {
                        GameAnalytics.NewDesignEvent("CompletePerfect", gameData.LevelNnmber);
                    }
                }
            }
            gameData.CoinCountMultiple = multiple;
        }
    }
}
