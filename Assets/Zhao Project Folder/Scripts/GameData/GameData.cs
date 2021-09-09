using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System;



/// <summary>
/// 游戏场景所有的数据
/// </summary>
public class GameSceneData
{
    /// <summary>
    /// 路径数据
    /// </summary>
    public List<RoadData> roadDatas = new List<RoadData>();
    /// <summary>
    /// 翅膀数据
    /// </summary>
    public List<WingData> wingDatas = new List<WingData>();
    /// <summary>
    /// 高楼数据
    /// </summary>
    public List<FloorData> floorDatas = new List<FloorData>();
    /// <summary>
    /// 跳跃机关数据
    /// </summary>
    public List<JumpTriggerData> jumpTriggerDatas = new List<JumpTriggerData>();
    /// <summary>
    /// 转弯触发器机关数据
    /// </summary>
    public List<TurnTriggerData> turnTriggerDatas = new List<TurnTriggerData>();
    /// <summary>
    /// 可以踩踏的云朵数据
    /// </summary>
    public List<MoveCloudData> moveCloudDatas = new List<MoveCloudData>();
    /// <summary>
    /// 可以Up的云朵数据
    /// </summary>
    public List<UpCloudData> upCloudDatas = new List<UpCloudData>();

    public List<PoolData> poolDatas = new List<PoolData>();

    public List<PoolTriggerData> poolTriggerDatas = new List<PoolTriggerData>();

    public List<DoorBaseData> doorBaseDatas = new List<DoorBaseData>();

    public List<TransformData> bridgeDatas = new List<TransformData>();

    public TransformData floor = new TransformData();

    public TransformData endGameObject = new TransformData();

    public List<DevilRoleData> devilRoleDatas = new List<DevilRoleData>();

    //礼盒 柱子 老虎机
    public List<TransformData> columns = new List<TransformData>();
    public List<TransformData> arcadeGames = new List<TransformData>();

    public List<GiftBoxData> giftBoxs = new List<GiftBoxData>();
    //旗帜
    public List<TransformData> flagGames = new List<TransformData>();
}
/// <summary>
/// 终点模板
/// </summary>
public class EndGameData
{
    public Dictionary<string, EndData> endDatas = new Dictionary<string, EndData>();
    public class EndData
    {
        /// <summary>
        /// 翅膀数据
        /// </summary>
        public List<WingData> wingDatas = new List<WingData>();
    }

    internal bool GetRandomKey(out string key)
    {
        if (endDatas.Count == 0)
        {
            key = "";
            return false;
        }
        int index = UnityEngine.Random.Range(0, endDatas.Count);
        int count = 0;
        foreach (var item in endDatas)
        {
            if (count == index)
            {
                key = item.Key;
                return true;
            }
            count++;
        }
        key = "";
        return false;
    }
}
#region

public struct G_Vector3
{
    public float x;
    public float y;
    public float z;
    public G_Vector3(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
    public void SetVector3(Vector3 v3)
    {
        this.x = v3.x;
        this.y = v3.y;
        this.z = v3.z;
    }
    public Vector3 GetVector3()
    {
        return new Vector3(x, y, z);
    }
}

public struct G_Quaternion
{
    public float x;
    public float y;
    public float z;
    public float w;
    public G_Quaternion(float x, float y, float z, float w)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
    }
    public void SetQuaternion(Quaternion q)
    {
        this.x = q.x;
        this.y = q.y;
        this.z = q.z;
        this.w = q.w;
    }
    public Quaternion GetQuaternion()
    {
        return new Quaternion(x, y, z, w);
    }

}

/// <summary>
/// 物体位置数据
/// </summary>
public class TransformData
{
    /// <summary>
    /// 位置
    /// </summary>
    public G_Vector3 position;
    /// <summary>
    /// 旋转
    /// </summary>
    public G_Quaternion rotation;
    /// <summary>
    /// 缩放
    /// </summary>
    public G_Vector3 localScale = new G_Vector3(1, 1, 1);

    public void SetTransfor(Transform tra)
    {
        position.SetVector3(tra.position);
        rotation.SetQuaternion(tra.rotation);
        localScale.SetVector3(tra.localScale);
    }
    public void GetTransfor(Transform tra)
    {
        tra.position = position.GetVector3();
        tra.rotation = rotation.GetQuaternion();
        tra.localScale = localScale.GetVector3();
    }
}

public class FXShapeData
{
    /// <summary>
    /// 位置
    /// </summary>
    public G_Vector3 position;
    /// <summary>
    /// 旋转
    /// </summary>
    public G_Vector3 rotation;
    /// <summary>
    /// 缩放
    /// </summary>
    public G_Vector3 localScale;

    public void SetFXShape(ParticleSystem ps)
    {
        position.SetVector3(ps.shape.position);
        rotation.SetVector3(ps.shape.rotation);
        localScale.SetVector3(ps.shape.scale);
    }
    public void GetFXShape(ParticleSystem ps)
    {
        ParticleSystem.ShapeModule shape = ps.shape;
        shape.position = position.GetVector3();
        shape.rotation = rotation.GetVector3();
        shape.scale = localScale.GetVector3();
    }
}


/// <summary>
/// 碰撞器尺寸
/// </summary>
public class ColliderSizeData
{
    /// <summary>
    /// 居中位置
    /// </summary>
    public G_Vector3 center;
    /// <summary>
    /// 大小
    /// </summary>
    public G_Vector3 size;
    public void SetBoxCollider(BoxCollider box)
    {
        center.SetVector3(box.center);
        size.SetVector3(box.size);
    }
    public void GetBoxCollider(BoxCollider box)
    {
        box.center = center.GetVector3(); ;
        box.size = size.GetVector3(); ;
    }
}

#endregion



/// <summary>
/// 路面数据 预制体名字 路面材质颜色 碰撞起类型（nil，box）方形碰撞起尺寸 位置 旋转 缩放
/// </summary>
public class RoadData
{
    public string name;
    public List<string> roatMaterial = new List<string>();
    public TransformData transformData = new TransformData();
    public bool isAddBoxCollider;
    public ColliderSizeData colliderSize = new ColliderSizeData();
}


/// <summary>
/// 翅膀数据 翅膀类型（小天使翅膀，大天使翅膀，小恶魔翅膀，大恶魔翅膀）方形碰撞起尺寸 位置 旋转 缩放
/// </summary>
public class WingData
{
    /// <summary>
    /// 天使翅膀类型
    /// </summary>
    public enum WingType
    {
        /// <summary>
        /// 小天使翅膀
        /// </summary>
        LittleAngelWings,
        /// <summary>
        /// 大天使翅膀
        /// </summary>
        BigAngelWings,
        /// <summary>
        /// 小恶魔翅膀
        /// </summary>
        LittleDevilWings,
        /// <summary>
        /// 大恶魔翅膀
        /// </summary>
        BigDevilWings
    }
    public WingType wingType;
    public TransformData transformData = new TransformData();
}
/// <summary>
/// 楼梯数据 位置 旋转 缩放
/// </summary>
public class StairsData
{
    public TransformData transformData;
}

/// <summary>
/// 高楼数据 预制体名字 位置 旋转 缩放
/// </summary>
public class FloorData
{
    public string name;
    public TransformData transformData = new TransformData();
}


/// <summary>
/// 机关数据 跳跃机关
/// </summary>
public class JumpTriggerData
{
    public MoveState moveState;
    public TransformData transformData = new TransformData();
    public TransformData szieTransform = new TransformData();
}


/// <summary>
/// 机关数据 转弯触发器
/// </summary>
public class TurnTriggerData
{
    public TurningDirection turningDirection;
    public TransformData transformData = new TransformData();
    public TransformData nextRoadTransformData = new TransformData();
    public bool isEndTurnACorner = false;
}



/// <summary>
/// 可以踩踏的云朵
/// </summary>
public class MoveCloudData
{
    public TransformData transformData = new TransformData();
    public FXShapeData fXShapeData = new FXShapeData();
}


/// <summary>
/// 可以Up的云朵
/// </summary>
public class UpCloudData
{
    public TransformData transformData = new TransformData();
    public TransformData cloudUpPostiionData = new TransformData();
    public FXShapeData fXShapeData = new FXShapeData();
    public ColliderSizeData colliderSizeData = new ColliderSizeData();
}

/// <summary>
/// 机关数据 机关类型（转弯触发器，跳跃机关，云朵陷阱，云朵升降机，圣水烈焰池，独木桥，问题选择门）位置 旋转 缩放
/// </summary>
public class PoolData
{
    public enum PoolType
    {
        AllPool, AngelPool_1, AngelPool_2, DevilPool_1, DevilPool_2
    }
    public PoolType poolType;
    public TransformData transformData = new TransformData();
    public List<TransformData> waterTransformData = new List<TransformData>();
}

public class PoolTriggerData
{
    public enum PoolTriggerType
    {
        AngelPoolTrigger, DevilPoolTrigger
    }
    public PoolTriggerType poolTriggerType;
    public TransformData transformData = new TransformData();
}


/// <summary>
/// 机关数据 机关类型（转弯触发器，跳跃机关，云朵陷阱，云朵升降机，圣水烈焰池，独木桥，问题选择门）位置 旋转 缩放
/// </summary>
public class MechanismTrapData
{
    public string name;
    public TransformData transformData = new TransformData();
}







public class DoorBaseData
{
    //头顶标题
    public string titleText;
    //位置
    public TransformData transformData = new TransformData();

    public TransformData doorTransformData = new TransformData();
    //两扇门数据
    public List<DoorData> doorDatas = new List<DoorData>();
    //交互物体集合
    public List<DoorRewardsData> rewardsDatas = new List<DoorRewardsData>();
}

[System.Serializable]
public class DoorData
{
    public string titleText;
    public string spriteName;
    /// <summary>
    /// 分数
    /// </summary>
    public int score;
    /// <summary>
    /// 情境状态
    /// </summary>
    public SituationalState situationalState;
}

/// <summary>
/// 奖励实体数据 汉堡  前 
/// </summary>
public class DoorRewardsData
{
    public string name;
    public TransformData transformData = new TransformData();
}


//
public enum DevilRoleState
{
    Idle, Move
}
public class DevilRoleData
{
    public DevilRoleState devilRoleState;
    public TransformData transformData = new TransformData();
    public DevilRoleController.RoleType roleType;
    public float speed = 3.5f;
}

public enum GiftBoxState
{
    AngleBox,
    DevilBox
}
public class GiftBoxData
{
    public TransformData transformData = new TransformData();
    public GiftBoxState giftBoxState;
}

