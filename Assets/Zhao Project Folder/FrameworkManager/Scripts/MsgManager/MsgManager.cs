using System;
using System.Collections.Generic;

public static class MsgManager
{
    private static Dictionary<MsgManagerType, Action<object>> messages = new Dictionary<MsgManagerType, Action<object>>();

    public static void Add(MsgManagerType key, Action<object> value)
    {
        if (!messages.ContainsKey(key))
        {
            messages.Add(key, value);
        }
        else
        {
            messages[key] += value;
        }
    }
    public static void Invoke(MsgManagerType key, object obj = null)
    {
        if (messages.ContainsKey(key))
        {
            messages[key]?.Invoke(obj);
        }
    }
    public static void Remove()
    {
        messages.Clear();
        System.GC.Collect();
    }
    public static void Remove(MsgManagerType key)
    {
        if (messages.ContainsKey(key))
        {
            messages.Remove(key);
        }
    }
    public static void Remove(MsgManagerType key, Action<object> value)
    {
        if (messages.ContainsKey(key))
        {
            messages[key] -= value;
            if (messages[key] == null)
            {
                messages.Remove(key);
            }
        }
    }
}

public enum MsgManagerType
{
    RefreshFraction,
    RefreshHearSlider,
    ChangeSky,
    StepMove,
    DevilEndDistance,
    AngelEndDistance,
    GameEndReward_Infernal,
    GameEndReward_Lineage,
    CoinCountPlus,
    TriggerTurnACorner,
    TriggerAngelSkyState,
    TriggerJumpState,
    TriggerFallState,
    DoorTriggerEnter,
    DoorTriggerExit,
    TriggerEnterBridge,
    TriggerDownBridge,
    CloudsDissipated,
    DestroyCollider,
    ShowAngelCoin,
    ShowDevilCoin,
    ShowDamageNumber,
    TriggerDevilRole,
    RefreshScore,
    LockRefreshScore,
    ShowGiftBoxPanelType,
    DestroyGiftBox,
    SetPlayerMove,
    RefreshWingsItem,
    BuildLoadButton,
    RefreshBuildScene,
    TriggerAngelRole,
    TriggerQuestionRole,
    FORGIVENESS_PUNISHMENT,
    AllEndObject,
    TriggerGod,
    TextScale,
}