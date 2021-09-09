using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// 终点站
/// </summary>
public class TerminalPoint : TerminalPointBase
{
    public override void Start()
    {
        base.Start();
        StepUp();
    }


    #region 控制台阶位移
    public List<StepsDoMove> stepsArray = new List<StepsDoMove>();
    //public void

    private bool isTrigger = true;
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && isTrigger)
        {
            isTrigger = false;
            //GameObject a = new GameObject("AA");

            PlayerController player = other.GetComponent<PlayerController>();
            player.PlayerStopMove();

            UnityEngine.Vector3 upPosition = UnityEngine.Vector3.zero;
            UnityEngine.Vector3 downPosition = UnityEngine.Vector3.zero;
            //PlayerModel.Fraction = 0;
            int count = (int)(playerModel.GetFraction * 0.2f);

            //计算到移动到那个台阶  上
            if (playerModel.GetFraction >= 50)//天使状态  玩家爬楼梯
            {
                //
                upPosition = transform.position + new UnityEngine.Vector3(0, count * 0.99853f, count * -2);
            }
            else
            {
                upPosition = transform.position + new UnityEngine.Vector3(0, 20 * 0.99853f, -40);
                //计算向下的楼梯
                switch (terminalPointStart)
                {
                    case TerminalPointStart.UpAndFront:
                        downPosition = transform.position + new UnityEngine.Vector3(0, 0, (20 - count + 1) * -2);
                        break;
                    case TerminalPointStart.UpAndDown:
                        downPosition = transform.position + new UnityEngine.Vector3(0, (20 - count) * -0.99853f, (20 - count + 1) * -2);
                        break;
                }
                count = 20 - count;
            }

            player.SetClimbStairs(upPosition, downPosition, playerModel.GetFraction >= 50);
        }
    }

    private void StepMove(object obj)
    {
        switch (terminalPointStart)
        {
            case TerminalPointStart.UpAndFront:
                StepFront();
                break;
            case TerminalPointStart.UpAndDown:
                StepDown();
                break;
            default:
                break;
        }
    }

    //楼梯重置
    [ContextMenu("楼梯重置")]
    public void StepFront()
    {
        //-0.99853
        StartCoroutine(StepFrontMoveWait());
    }
    private IEnumerator StepFrontMoveWait()//0.99853
    {
        for (int i = 0; i < stepsArray.Count; i++)
        {
            stepsArray[i].Init(new UnityEngine.Vector3(stepsArray[i].transform.position.x, transform.position.y - 0.99853f, stepsArray[i].transform.position.z), false);
            yield return new WaitForSeconds(0.01f);
        }
    }
    //台阶下落
    [ContextMenu("台阶下落")]
    public void StepDown()
    {
        StartCoroutine(StepDownMoveWait());
        //-0.99853
    }
    private IEnumerator StepDownMoveWait()//0.99853
    {
        for (int i = 0; i < stepsArray.Count; i++)
        {
            UnityEngine.Vector3 pos = new UnityEngine.Vector3(stepsArray[i].transform.position.x, transform.position.y + (i * -0.99853f - 0.99853f) - 0.99853f, stepsArray[i].transform.position.z);
            stepsArray[i].Init(pos, false);
            yield return new WaitForSeconds(0.01f);
        }
    }
    //台阶上升
    [ContextMenu("台阶上升")]
    public void StepUp()
    {
        StartCoroutine(StepUpMoveWait());
        //0
    }
    private IEnumerator StepUpMoveWait()//0.99853
    {
        for (int i = 0; i < stepsArray.Count; i++)
        {
            UnityEngine.Vector3 pos = new UnityEngine.Vector3(stepsArray[i].transform.position.x, transform.position.y + (i * 0.99853f), stepsArray[i].transform.position.z);
            stepsArray[i].Init(pos, true);
            yield return new WaitForSeconds(0.01f);
        }
    }

    #endregion




    #region 消息中心添加移除消息
    public override void Awake()
    {
        MsgManager.Add(MsgManagerType.StepMove, StepMove);
        MsgManager.Add(MsgManagerType.GameEndReward_Lineage, GameEndReward_Lineage);//游戏宝箱奖励  天堂
        MsgManager.Add(MsgManagerType.GameEndReward_Infernal, GameEndReward_Infernal);//地狱
    }

    public override void OnDestroy()
    {
        MsgManager.Remove(MsgManagerType.StepMove, StepMove);
        MsgManager.Remove(MsgManagerType.GameEndReward_Lineage, GameEndReward_Lineage);
        MsgManager.Remove(MsgManagerType.GameEndReward_Infernal, GameEndReward_Infernal);
    }

    #endregion

    public Transform Lineage_Chest;
    public Transform Lineage_Door_1, Lineage_Door_2;
    /// <summary>
    /// 最终奖励宝箱 天堂
    /// </summary>
    /// <param name="obj"></param>
    private void GameEndReward_Lineage(object obj)
    {
        Transform player = obj as Transform;
        Lineage_Door_1.DOLocalRotate(new UnityEngine.Vector3(0, 0, 0), 0.3f);
        Lineage_Door_2.DOLocalRotate(new UnityEngine.Vector3(0, 180, 0), 0.3f);

        Sequence s = DOTween.Sequence();
        //移动到宝箱前
        s.Append(player.DOMove(new UnityEngine.Vector3(player.position.x, player.position.y, Lineage_Chest.position.z + 2.5f), 2f));
        s.AppendCallback(() =>
        {
            //打开宝箱播放特效
            Lineage_Chest.DOLocalRotate(new UnityEngine.Vector3(65, 0, 0), 0.3f);
            //MsgManager.Invoke(MsgManagerType.CoinCountPlus, true);
            player.DORotate(UnityEngine.Vector3.zero, 0.3f);
            player.GetComponent<PlayerController>().ani.CrossFadeInFixedTime("A开心", 0.25f);
            //MsgManager.Invoke(MsgManagerType.CoinCountPlus, true);
            //MsgManager.Invoke(MsgManagerType.ShowAngelCoin);
        });
        s.AppendInterval(2f);
        s.AppendCallback(() =>
        {
            //MsgManager.Invoke(MsgManagerType., true);
            //弹出UIGameWins
            //FindObjectOfType<GameOverPanel>().gameWins.SetActive(true);
            UIManager.Instance.OpenUI(PanelName.GameWinsPanel);
        });
        //打开门

    }
    public Transform Infernal_Chest;
    public Transform Infernal_Door;
    private void GameEndReward_Infernal(object obj)
    {
        Transform player = obj as Transform;
        Infernal_Door.DOLocalMoveY(-7.5f, 0.3f);
        Sequence s = DOTween.Sequence();
        //移动到宝箱前
        s.Append(player.DOMove(new UnityEngine.Vector3(player.position.x, player.position.y, Infernal_Chest.position.z + 2.5f), 2f));
        s.AppendCallback(() =>
        {
            //打开宝箱播放特效
            Infernal_Chest.DOLocalRotate(new UnityEngine.Vector3(65, 0, 0), 0.3f);
            //MsgManager.Invoke(MsgManagerType.CoinCountPlus, true);
            player.DORotate(UnityEngine.Vector3.zero, 0.3f);
            player.GetComponent<PlayerController>().ani.CrossFadeInFixedTime("A害怕", 0.25f);
            //MsgManager.Invoke(MsgManagerType.CoinCountPlus, true);
            //MsgManager.Invoke(MsgManagerType.ShowAngelCoin);
        });
        s.AppendInterval(2f);
        s.AppendCallback(() =>
        {
            //MsgManager.Invoke(MsgManagerType., true);
            //弹出UIGameWins

            //FindObjectOfType<GameOverPanel>().gameWins.SetActive(true);
            UIManager.Instance.OpenUI(PanelName.GameWinsPanel);
        });
        //打开门
    }

}
