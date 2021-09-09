using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System;
public class GameSceneDataManager
{
    private string configPath = "/Zhao Project Folder/Resources/ConfigFolder/";

    public void Save(string fileName, object gameSceneData)
    {
        string str = JsonConvert.SerializeObject(gameSceneData, Formatting.Indented);
        File.Delete(Application.dataPath + configPath + fileName);
        File.WriteAllText(Application.dataPath + configPath + fileName, str);
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
    }
    private Transform sdBase;
    private GameSceneData gsData;
    public List<ParticleSystem> MoveClouds = new List<ParticleSystem>();
    public void Read(string fileName, int wingID)
    {
        //TextAsset text = Resources.Load<TextAsset>("ConfigFolder/" + fileName);

        //gsData = JsonConvert.DeserializeObject<GameSceneData>(text.text);
        gsData = AllGameSceneData.GetData(fileName);

        sdBase = LoadObject<GameObject>(ResourcesType.ElseFolder, "SceneDataBase", true).transform;
        sdBase.name = "SceneDataBase";


        TextAsset text = Resources.Load<TextAsset>("ConfigFolder/EndData");
        if (text)
        {
            endGameData = JsonConvert.DeserializeObject<EndGameData>(text.text);
        }

        //实例路面
        Transform road_base = sdBase.Find("Roads");
        foreach (RoadData item in gsData.roadDatas)
        {
            Transform road = LoadObject<GameObject>(ResourcesType.Roads, item.name, true).transform;
            Material[] materials = new Material[item.roatMaterial.Count];
            for (int i = 0; i < item.roatMaterial.Count; i++)
            {
                string mat = item.roatMaterial[i];
                materials[i] = LoadObject<Material>(ResourcesType.Materials, mat, false);
            }
            road.GetComponent<MeshRenderer>().materials = materials;
            item.transformData.GetTransfor(road);
            if (item.isAddBoxCollider)
            {
                item.colliderSize.GetBoxCollider(road.gameObject.AddComponent<BoxCollider>());
            }
            road.SetParent(road_base);
        }

        if (end_wingBase) GameObject.Destroy(end_wingBase.gameObject);
        end_wingBase = new GameObject("终点翅膀放这里").transform;
        end_wingBase.SetParent(sdBase);

        //终点
        end_GameObject = LoadObject<GameObject>(ResourcesType.ElseFolder, "End Game Object", true).transform;
        gsData.endGameObject.GetTransfor(end_GameObject);
        end_GameObject.name = "End Game Object";
        end_GameObject.SetParent(sdBase);
        end_GameObject.Find("end").GetComponent<TerminalPointBase>().wingsObject = end_wingBase.gameObject;

        //实例翅膀
        CreateWings(wingID);

        //实例高楼
        Transform floor_base = sdBase.Find("Floor");
        foreach (FloorData item in gsData.floorDatas)
        {
            Transform floor = LoadObject<GameObject>(ResourcesType.Floors, item.name, true).transform;
            item.transformData.GetTransfor(floor);
            floor.SetParent(floor_base);
        }
        //实例跳跃机关数据
        Transform jump_base = sdBase.Find("JumpTrigger");
        foreach (var item in gsData.jumpTriggerDatas)
        {
            Transform jump = LoadObject<GameObject>(ResourcesType.ElseFolder, "JumpTrigger", true).transform;
            item.transformData.GetTransfor(jump);
            item.szieTransform.GetTransfor(jump.GetChild(0));
            jump.GetComponent<TakeOff>().moveState = item.moveState;
            jump.SetParent(jump_base);
        }
        //实例转弯机关数据
        Transform turn_base = sdBase.Find("TurnTrigger");
        foreach (var item in gsData.turnTriggerDatas)
        {
            Transform turn = LoadObject<GameObject>(ResourcesType.ElseFolder, "TurnTrigger", true).transform;
            TurnACorner turnACorner = turn.GetComponent<TurnACorner>();
            turnACorner.turningDirection = item.turningDirection;
            turnACorner.nextRoadOrigin = new GameObject("NextRoadOrigin").transform;
            turnACorner.isEndTurnACorner = item.isEndTurnACorner;
            item.transformData.GetTransfor(turn);
            item.nextRoadTransformData.GetTransfor(turnACorner.nextRoadOrigin);
            turn.SetParent(turn_base);
            turnACorner.nextRoadOrigin.SetParent(turn_base);
        }

        //创建跳跃机关数据
        Transform flag_Base = sdBase.transform.Find("Flag");
        foreach (var item in gsData.flagGames)
        {
            Transform flag = LoadObject<GameObject>(ResourcesType.ElseFolder, "Flag", true).transform;
            item.GetTransfor(flag);
            flag.SetParent(flag_Base);
        }


        //实例可以踩踏的云朵数据
        Transform moveCloud_base = sdBase.Find("MoveCloud");
        for (int i = 0; i < gsData.moveCloudDatas.Count; i++)
        {
            MoveCloudData item = gsData.moveCloudDatas[i];
            Transform moveCloud;
            if (i > 2 && ModelManager.GetModel<GameDataModel>().GetSceneName == "Scene Data video3")
            {
                moveCloud = LoadObject<GameObject>(ResourcesType.FX, "MoveCloudFX 1", true).transform;
            }
            else
            {
                moveCloud = LoadObject<GameObject>(ResourcesType.FX, "MoveCloudFX", true).transform;
            }

            item.transformData.GetTransfor(moveCloud);
            MoveClouds.Add(moveCloud.GetComponent<ParticleSystem>());
            item.fXShapeData.GetFXShape(moveCloud.GetComponent<ParticleSystem>());
            moveCloud.SetParent(moveCloud_base);
        }
        //实例可以踩踏的云朵数据
        Transform upCloud_base = sdBase.Find("UpCloud");
        foreach (var item in gsData.upCloudDatas)
        {
            Transform upCloud = LoadObject<GameObject>(ResourcesType.ElseFolder, "UpCloud", true).transform;
            upCloud.GetComponent<CloudController>().SetData(item);
            upCloud.SetParent(upCloud_base);
        }
        //实例水池数据
        Transform pool_base = sdBase.Find("Pool");
        foreach (var item in gsData.poolDatas)
        {
            Transform pool = LoadObject<GameObject>(ResourcesType.ElseFolder, item.poolType.ToString(), true).transform;
            item.transformData.GetTransfor(pool);
            for (int i = 0; i < item.waterTransformData.Count; i++)
            {
                item.waterTransformData[i].GetTransfor(pool.GetChild(i));
            }
            //upCloud.GetComponent<CloudController>().SetData(item);
            pool.SetParent(pool_base);
        }
        Transform poolTrigger_base = sdBase.Find("PoolTrigger");
        foreach (var item in gsData.poolTriggerDatas)
        {
            Transform pool = LoadObject<GameObject>(ResourcesType.ElseFolder, item.poolTriggerType.ToString(), true).transform;
            item.transformData.GetTransfor(pool);
            //upCloud.GetComponent<CloudController>().SetData(item);
            pool.SetParent(poolTrigger_base);
        }
        Transform door_base = sdBase.Find("Door");
        foreach (var item in gsData.doorBaseDatas)
        {
            Transform door = LoadObject<GameObject>(ResourcesType.ElseFolder, "Door", true).transform;
            door.GetComponent<DoorBase>().SetDoorBaseData(item);
            door.SetParent(door_base);
        }

        Transform bridge_base = sdBase.Find("Bridge");
        foreach (var item in gsData.bridgeDatas)
        {
            Transform bridge = LoadObject<GameObject>(ResourcesType.ElseFolder, "Bridge", true).transform;
            item.GetTransfor(bridge);
            bridge.SetParent(bridge_base);
        }

        Transform devilRole_base = sdBase.Find("DevilRole");
        foreach (var item in gsData.devilRoleDatas)
        {
            Transform devilRole = LoadObject<GameObject>(ResourcesType.ElseFolder, item.roleType.ToString(), true).transform;
            item.transformData.GetTransfor(devilRole);
            DevilRoleController drc = devilRole.GetComponent<DevilRoleController>();
            drc.devilRoleState = item.devilRoleState;
            drc.roleType = item.roleType;
            drc.speed = item.speed;
            devilRole.SetParent(devilRole_base);
        }

        //Transform Column_base = sdBase.Find("Column");
        //foreach (var item in gsData.columns)
        //{
        //    Transform Column = LoadObject<GameObject>(ResourcesType.ElseFolder, "Column", true).transform;
        //    item.GetTransfor(Column);
        //    Column.localScale = Vector3.one;
        //    Column.SetParent(Column_base);
        //}

        Transform ArcadeGame_base = sdBase.Find("ArcadeGame");
        foreach (var item in gsData.arcadeGames)
        {
            Transform ArcadeGame = LoadObject<GameObject>(ResourcesType.ElseFolder, "ArcadeGame", true).transform;
            item.GetTransfor(ArcadeGame);
            ArcadeGame.SetParent(ArcadeGame_base);
        }

        Transform GiftBox_base = sdBase.Find("GiftBox");
        foreach (var item in gsData.giftBoxs)
        {
            Transform GiftBox = LoadObject<GameObject>(ResourcesType.ElseFolder, item.giftBoxState.ToString(), true).transform;
            item.transformData.GetTransfor(GiftBox);
            GiftBox.SetParent(GiftBox_base);
        }

        gsData.floor.GetTransfor(sdBase.Find("GameOverTrigger"));


        //
        //ReadEnd();
    }
    public T LoadObject<T>(ResourcesType resType, string name, bool isInstantiate) where T : UnityEngine.Object
    {
        if (isInstantiate)//需要实例
        {
            return GameObject.Instantiate(ResourcesManager.Load<T>(resType, name));
        }
        else//不需要实例
        {
            return ResourcesManager.Load<T>(resType, name);
        }
    }
    public List<GameObject> wings = new List<GameObject>();
    public void CreateWings(int wingID)
    {
        foreach (var item in wings)
        {
            GameObject.Destroy(item);
        }
        wings.Clear();
        Transform wing_base = sdBase.Find("Wings");
        foreach (WingData item in gsData.wingDatas)
        {
            Transform wing;//= null;
            switch (item.wingType)
            {
                case WingData.WingType.LittleAngelWings:
                    wing = LoadObject<GameObject>(ResourcesType.Wings, "Angel_" + wingID, true).transform;
                    break;
                case WingData.WingType.BigAngelWings:
                    wing = LoadObject<GameObject>(ResourcesType.Wings, item.wingType.ToString(), true).transform;
                    break;
                case WingData.WingType.LittleDevilWings:
                    wing = LoadObject<GameObject>(ResourcesType.Wings, "Devil_" + wingID, true).transform;
                    break;
                case WingData.WingType.BigDevilWings:
                    wing = LoadObject<GameObject>(ResourcesType.Wings, item.wingType.ToString(), true).transform;
                    break;
                default:
                    wing = LoadObject<GameObject>(ResourcesType.Wings, item.wingType.ToString(), true).transform;
                    break;
            }
            item.transformData.GetTransfor(wing);
            wing.SetParent(wing_base);
            wings.Add(wing.gameObject);
        }
        //创建终点的翅膀
        {
            end_wingBase.position = Vector3.zero;
            end_wingBase.eulerAngles = Vector3.zero;
            string key;
            if (endGameData.GetRandomKey(out key))
            {
                foreach (WingData item in endGameData.endDatas[key].wingDatas)
                {
                    Transform wing;
                    switch (item.wingType)
                    {
                        case WingData.WingType.LittleAngelWings:
                            wing = LoadObject<GameObject>(ResourcesType.Wings, "Angel_" + wingID, true).transform;
                            break;
                        case WingData.WingType.BigAngelWings:
                            wing = LoadObject<GameObject>(ResourcesType.Wings, item.wingType.ToString(), true).transform;
                            break;
                        case WingData.WingType.LittleDevilWings:
                            wing = LoadObject<GameObject>(ResourcesType.Wings, "Devil_" + wingID, true).transform;
                            break;
                        case WingData.WingType.BigDevilWings:
                            wing = LoadObject<GameObject>(ResourcesType.Wings, item.wingType.ToString(), true).transform;
                            break;
                        default:
                            wing = LoadObject<GameObject>(ResourcesType.Wings, item.wingType.ToString(), true).transform;
                            break;
                    }
                    item.transformData.GetTransfor(wing);
                    wing.SetParent(end_wingBase);
                    wings.Add(wing.gameObject);
                }
            }
            end_wingBase.position = end_GameObject.position;
            end_wingBase.rotation = end_GameObject.rotation;
        }
    }
    private Transform end_wingBase;
    private Transform end_GameObject;
    private EndGameData endGameData = new EndGameData();

}





public static class AllGameSceneData
{
    private static Dictionary<string, GameSceneData> allData = new Dictionary<string, GameSceneData>();

    internal static void SetData(string name, GameSceneData data)
    {
        if (!allData.ContainsKey(name))
            allData.Add(name, data);
        else
            allData[name] = data;
    }
    internal static GameSceneData GetData(string name)
    {
#if UNITY_EDITOR
        TextAsset text = Resources.Load<TextAsset>("ConfigFolder/" + name);
        return JsonConvert.DeserializeObject<GameSceneData>(text.text);
#else
        GameSceneData data;
        if (allData.TryGetValue(name, out data))
            return data;
        TextAsset text = Resources.Load<TextAsset>("ConfigFolder/" + name);
        data = JsonConvert.DeserializeObject<GameSceneData>(text.text);
        allData.Add(name, data);
        return data;
#endif

    }
}
