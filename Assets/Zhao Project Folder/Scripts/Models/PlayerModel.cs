using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家数据模型
/// </summary>
public class PlayerModel : ModelBase
{
    //玩家分数
    private bool isSky = true;
    private int _fraction;
    public int InitFraction { set { _fraction = value; isSky = true; } }


    public enum FractionType
    {
        LittleAngelWings, BigAngelWings, LittleDevilWings, BigDevilWings,
        DevilRoleNumber, AngelRoleNumber, QuestionRoleNumber, PoolNumber, DoorNumber,
        TriggerGod
    }
    /// <summary>
    /// 小天使翅膀
    /// </summary>
    public int LittleAngelWings = 1;
    /// <summary>
    /// 大天使翅膀
    /// </summary>
    public int BigAngelWings = 3;
    /// <summary>
    /// 小恶魔翅膀
    /// </summary>
    public int LittleDevilWings = -1;
    /// <summary>
    /// 大恶魔翅膀
    /// </summary>
    public int BigDevilWings = -3;
    ///// <summary>
    ///// 恶魔角色
    ///// </summary>
    public int DevilRoleNumber = -5;
    public int AngelRoleNumber = 5;
    public int QuestionRoleNumber = 10;
    /// <summary>
    /// 水池
    /// </summary>
    public int PoolNumber = 1;

    public int GetFraction
    {
        get { return _fraction; }
    }
    public bool isPulsNumber = false;
    public void SetFraction(FractionType fractionType, int number, int coinCount = 0)
    {
        switch (fractionType)
        {
            case FractionType.LittleAngelWings://小天使
                MsgManager.Invoke(MsgManagerType.ShowDamageNumber, 1);
                break;
            case FractionType.BigAngelWings://大天使
                MsgManager.Invoke(MsgManagerType.ShowDamageNumber, 3);
                break;
            case FractionType.LittleDevilWings://小恶魔
                MsgManager.Invoke(MsgManagerType.ShowDamageNumber, -1);
                break;
            case FractionType.BigDevilWings://
                MsgManager.Invoke(MsgManagerType.ShowDamageNumber, -3);
                break;
            case FractionType.DevilRoleNumber:
                MsgManager.Invoke(MsgManagerType.ShowDamageNumber, -5);
                break;
            case FractionType.AngelRoleNumber:
                MsgManager.Invoke(MsgManagerType.ShowDamageNumber, 5);
                break;
            case FractionType.QuestionRoleNumber:
                MsgManager.Invoke(MsgManagerType.ShowDamageNumber, coinCount);
                break;
            case FractionType.PoolNumber:
                if (number >= 0)
                    MsgManager.Invoke(MsgManagerType.ShowDamageNumber, 1);
                else
                    MsgManager.Invoke(MsgManagerType.ShowDamageNumber, -1);
                break;
            case FractionType.DoorNumber:
                if (number >= 0)
                    MsgManager.Invoke(MsgManagerType.ShowDamageNumber, 10);
                else
                    MsgManager.Invoke(MsgManagerType.ShowDamageNumber, -10);
                break;
            case FractionType.TriggerGod:
                MsgManager.Invoke(MsgManagerType.ShowDamageNumber, 0);
                break;
            default:
                if (number >= 0)
                    MsgManager.Invoke(MsgManagerType.ShowDamageNumber, 5);
                else
                    MsgManager.Invoke(MsgManagerType.ShowDamageNumber, -5);
                break;
        }
        isPulsNumber = number >= 0;
        _fraction += number;
        if (_fraction < 0) _fraction = 0; else if (_fraction > 100) _fraction = 100;
        MsgManager.Invoke(MsgManagerType.RefreshFraction);

        if (isSky)
        {
            //小于了30 变
            if (_fraction < 30)
            {
                isSky = false;
                MsgManager.Invoke(MsgManagerType.ChangeSky, false);
            }
        }
        else
        {
            //da于了70 变
            if (_fraction > 70)
            {
                isSky = true;
                MsgManager.Invoke(MsgManagerType.ChangeSky, true);
            }
        }
        MsgManager.Invoke(MsgManagerType.RefreshScore, _fraction);
    }


    //public int SetFraction
    //{
    //    get { return _fraction; }
    //    set
    //    {
    //        if (value >= 0)
    //        {
    //            MsgManager.Invoke(MsgManagerType.ShowDamageNumber, 1);
    //        }
    //        else
    //        {
    //            MsgManager.Invoke(MsgManagerType.ShowDamageNumber, -1);
    //        }

    //        _fraction += value;

    //        if (_fraction < 0) _fraction = 0; else if (_fraction > 100) _fraction = 100;

    //        MsgManager.Invoke(MsgManagerType.RefreshFraction);


    //        if (isSky)
    //        {
    //            //小于了30 变
    //            if (_fraction < 30)
    //            {
    //                isSky = false;
    //                MsgManager.Invoke(MsgManagerType.ChangeSky, false);
    //            }
    //        }
    //        else
    //        {
    //            //da于了70 变
    //            if (_fraction > 70)
    //            {
    //                isSky = true;
    //                MsgManager.Invoke(MsgManagerType.ChangeSky, true);
    //            }
    //        }
    //        MsgManager.Invoke(MsgManagerType.RefreshScore, _fraction);
    //    }
    //}


    //#if UNITY_IOS

    //    public float speed = 12f;
    //    [HideInInspector]
    //    public float tuSpeed = 0.6f;
    //    public float tuSpeed_2 = 0.35f;

    //    [HideInInspector]
    //    public float tuXZ = 30;
    //    public float tuXZ_If = 29;
    //    [HideInInspector]
    //    public float tuYC = 1000;
    //#elif UNITY_ANDROID || !UNITY_EDITOR
    //public float speed = 2;
    //[HideInInspector]
    //public float tuSpeed = 0.6f;
    //public float tuSpeed_2 = 0.6f;

    //[HideInInspector]
    //public float tuXZ = 60;
    //public float tuXZ_If = 59;
    //[HideInInspector]
    //public float tuYC = 750;
    //#endif






}










