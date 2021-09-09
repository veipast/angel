using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

public class GetSceneDataEditor : EditorWindow
{
    private static GetSceneDataEditor getSceneWindow;
    [MenuItem("自定义工具/场景数据工具")]
    private static void AddWindow()
    {
        getSceneWindow = EditorWindow.GetWindow<GetSceneDataEditor>(true, "场景数据工具", false);
        getSceneWindow.Show();
    }
    private GameSceneDataManager gameSceneDataManager = new GameSceneDataManager();
    private GameSceneData gameSceneData = new GameSceneData();

    private string configPath = "/Zhao Project Folder/Resources/ConfigFolder/SceneData";
    private string fileName;
    private string endKey;
    private GameObject sceneDataBase;
    private bool isEndObj = false;

    void OnGUI()
    {
        //move = EditorWindow.mouseOverWindow ? EditorWindow.mouseOverWindow.ToString() : "Nothing";
        //EditorGUILayout.LabelField(move);
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "temp Player Game Scene")
        {
            return;
        }
        //填写配置表保持或读取路径
        GUI.enabled = false;
        if (!sceneDataBase)
        {
            sceneDataBase = GameObject.Find("SceneDataBase");
            if (!sceneDataBase)
            {
                sceneDataBase = Instantiate(Resources.Load<GameObject>("ElseFolder/SceneDataBase"));
                sceneDataBase.name = "SceneDataBase";
            }
        }
        sceneDataBase = EditorGUILayout.ObjectField(sceneDataBase, typeof(GameObject)) as GameObject;
        GUI.enabled = true;
        EditorGUILayout.BeginHorizontal();
        fileName = EditorGUILayout.TextField("Scene Data Name:", fileName);
        EditorGUILayout.LabelField(".josn");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUI.enabled = fileName != null && fileName != "" && Application.isPlaying;
        if (GUILayout.Button("创建场景"))
        {
            Read();
        }
        GUI.enabled = true;

        GUI.enabled = fileName != null && fileName != "";
        if (GUILayout.Button("保存场景"))
        {
            Save();
        }
        GUI.enabled = true;
        if (GUILayout.Button("清空场景"))
        {
            DestroyImmediate(sceneDataBase);
            //getSceneWindow.Close();
        }
        if (GUILayout.Button("场景查重"))
        {
            Transform baseT = sceneDataBase.transform.Find("Wings");
            int index = 0;
            FindWing(baseT, index);
            Transform baseDoor = sceneDataBase.transform.Find("Door");
            for (int i = 0; i < baseDoor.childCount; i++)
            {
                SetDoor(baseDoor.GetChild(i), 0);
                SetDoor(baseDoor.GetChild(i), 1);
            }
           
        }
        EditorGUILayout.EndHorizontal();

        isEndObj = EditorGUILayout.Toggle("终点数据", isEndObj);
        //创建  保存 青龙
        if (isEndObj)
        {
            endKey = EditorGUILayout.TextField("End Wing Key:", endKey);
            if (end_GameObject == null)
            {
                GameObject o = GameObject.Find("终点");
                if (o)
                    end_GameObject = o.transform;
                else
                {
                    end_GameObject = LoadObject<GameObject>(ResourcesType.ElseFolder, "End Game Object", true).transform;
                    end_GameObject.name = "终点";
                }
            }
            if (end_wingBase == null)
            {
                GameObject o = GameObject.Find("终点翅膀放这里");
                if (o)
                    end_wingBase = o.transform;
                else
                    end_wingBase = new GameObject("终点翅膀放这里").transform;
            }
            EditorGUILayout.BeginHorizontal();
            GUI.enabled = endKey != null && endKey != "" && Application.isPlaying;
            if (GUILayout.Button("创建终点"))
            {
                ReadEnd();
            }
            GUI.enabled = true;

            GUI.enabled = endKey != null && endKey != "";
            if (GUILayout.Button("保存终点"))
            {
                SaveEnd();
            }
            GUI.enabled = true;
            if (GUILayout.Button("清空终点"))
            {
                if (end_wingBase) GameObject.Destroy(end_wingBase.gameObject);
                if (end_GameObject) GameObject.Destroy(end_GameObject.gameObject);
            }
            EditorGUILayout.EndHorizontal();
        }
    }
    void SetDoor(Transform door, int index)
    {
        DoorScript doordata = door.GetChild(index).GetComponent<DoorScript>();
        doordata.doorData.score = doordata.doorData.score >= 0 ? 15 : -15;
    }

    private void FindWing(Transform t, int index)
    {
        for (int i = t.childCount - 1; i > index; i--)
        {
            float a = Vector3.Distance(t.GetChild(index).position, t.GetChild(i).position);
            //Debug.Log(a);
            if (a <= 0.01f)
            {
                Destroy(t.GetChild(i).gameObject);
            }
        }
        index++;
        if (index >= t.childCount) return;
        FindWing(t, index);
    }

    private void Read()
    {
        Destroy(sceneDataBase);
        gameSceneDataManager.Read("Scene Data " + fileName, 0);
    }

    private void Save()
    {
        //创建路面数据
        Transform road = sceneDataBase.transform.Find("Roads");
        for (int i = 0; i < road.childCount; i++)
        {
            Transform temp = road.GetChild(i);
            RoadData roadData = new RoadData();
            roadData.name = temp.GetComponent<MeshFilter>().sharedMesh.name.Split(' ')[0];
            Material[] mat = temp.GetComponent<MeshRenderer>().sharedMaterials;
            foreach (var item in mat)
            {
                roadData.roatMaterial.Add(item.name.Split(' ')[0]);
            }
            BoxCollider box = temp.GetComponent<BoxCollider>();
            if (box)
            {
                roadData.isAddBoxCollider = true;
                roadData.colliderSize.SetBoxCollider(box);
            }
            roadData.transformData.SetTransfor(temp);
            gameSceneData.roadDatas.Add(roadData);
        }
        //gameSceneData

        //创建翅膀数据
        Transform wing = sceneDataBase.transform.Find("Wings");
        for (int i = 0; i < wing.childCount; i++)
        {
            Transform temp = wing.GetChild(i);
            WingData wingData = new WingData();
            switch (temp.tag)
            {
                case "angel":
                    wingData.wingType = WingData.WingType.LittleAngelWings;
                    break;
                case "d_angel":
                    wingData.wingType = WingData.WingType.BigAngelWings;
                    break;
                case "devil":
                    wingData.wingType = WingData.WingType.LittleDevilWings;
                    break;
                case "d_devil":
                    wingData.wingType = WingData.WingType.BigDevilWings;
                    break;
            }
            wingData.transformData.SetTransfor(temp);
            gameSceneData.wingDatas.Add(wingData);
        }

        //创建高楼数据
        Transform floor = sceneDataBase.transform.Find("Floor");
        for (int i = 0; i < floor.childCount; i++)
        {
            Transform temp = floor.GetChild(i);
            FloorData floorData = new FloorData();
            floorData.name = temp.GetComponent<MeshFilter>().sharedMesh.name.Split(' ')[0];
            floorData.transformData.SetTransfor(temp);
            gameSceneData.floorDatas.Add(floorData);
        }

        //创建跳跃机关数据
        Transform jump = sceneDataBase.transform.Find("JumpTrigger");
        for (int i = 0; i < jump.childCount; i++)
        {
            Transform temp = jump.GetChild(i);
            if (temp.GetComponent<TakeOff>())
            {
                JumpTriggerData jumpTriggerData = new JumpTriggerData();
                jumpTriggerData.moveState = temp.GetComponent<TakeOff>().moveState;
                jumpTriggerData.szieTransform.SetTransfor(temp.GetChild(0));
                jumpTriggerData.transformData.SetTransfor(temp);
                gameSceneData.jumpTriggerDatas.Add(jumpTriggerData);
            }
        }
        //创建跳跃机关数据
        Transform flagT = sceneDataBase.transform.Find("Flag");
        for (int i = 0; i < flagT.childCount; i++)
        {
            Transform temp = flagT.GetChild(i);
            TransformData data = new TransformData();
            data.SetTransfor(temp);
            gameSceneData.flagGames.Add(data);
        }
        //创建跳跃机关数据
        Transform turn = sceneDataBase.transform.Find("TurnTrigger");
        for (int i = 0; i < turn.childCount; i++)
        {
            Transform temp = turn.GetChild(i);
            if (!temp.GetComponent<TurnACorner>()) continue;
            TurnTriggerData turnTriggerData = new TurnTriggerData();
            turnTriggerData.turningDirection = temp.GetComponent<TurnACorner>().turningDirection;
            turnTriggerData.transformData.SetTransfor(temp);
            turnTriggerData.isEndTurnACorner = temp.GetComponent<TurnACorner>().isEndTurnACorner;
            turnTriggerData.nextRoadTransformData.SetTransfor(temp.GetComponent<TurnACorner>().nextRoadOrigin);
            gameSceneData.turnTriggerDatas.Add(turnTriggerData);
        }

        //创建可以踩踏的云朵数据
        Transform moveCloud = sceneDataBase.transform.Find("MoveCloud");
        for (int i = 0; i < moveCloud.childCount; i++)
        {
            Transform temp = moveCloud.GetChild(i);
            MoveCloudData moveCloudData = new MoveCloudData();
            moveCloudData.transformData.SetTransfor(temp);
            moveCloudData.fXShapeData.SetFXShape(temp.GetComponent<ParticleSystem>()); ;
            gameSceneData.moveCloudDatas.Add(moveCloudData);
        }

        //创建可以Up的云朵数据
        Transform upCloud = sceneDataBase.transform.Find("UpCloud");
        for (int i = 0; i < upCloud.childCount; i++)
        {
            Transform temp = upCloud.GetChild(i);
            UpCloudData upCloudData = temp.GetComponent<CloudController>().GetData();
            gameSceneData.upCloudDatas.Add(upCloudData);
        }

        //创建可以Up的云朵数据
        Transform pool = sceneDataBase.transform.Find("Pool");
        for (int i = 0; i < pool.childCount; i++)
        {
            Transform temp = pool.GetChild(i);
            PoolData poolData = new PoolData();
            poolData.transformData.SetTransfor(temp);
            if (temp.childCount == 2)
            {
                bool isAngelPool, isAngelWater;
                Debug.Log(temp.GetChild(0).GetComponent<MeshFilter>().sharedMesh.name);
                Debug.Log(temp.GetChild(1).GetComponent<MeshRenderer>().sharedMaterial.name);
                isAngelPool = temp.GetChild(0).GetComponent<MeshFilter>().sharedMesh.name.Split(' ')[0] == "polySurface5" ? true : false;
                isAngelWater = temp.GetChild(1).GetComponent<MeshRenderer>().sharedMaterial.name.Split(' ')[0] == "QKX_volume_fog_2" ? false : true;
                if (isAngelPool)
                    if (isAngelWater)
                        poolData.poolType = PoolData.PoolType.AngelPool_1;
                    else
                        poolData.poolType = PoolData.PoolType.DevilPool_1;
                else
                {
                    if (isAngelWater)
                        poolData.poolType = PoolData.PoolType.AngelPool_2;
                    else
                        poolData.poolType = PoolData.PoolType.DevilPool_2;
                }
            }
            else
                poolData.poolType = PoolData.PoolType.AllPool;
            for (int k = 0; k < temp.childCount; k++)
            {
                poolData.waterTransformData.Add(new TransformData());
                poolData.waterTransformData[k].SetTransfor(temp.GetChild(k));
            }
            gameSceneData.poolDatas.Add(poolData);
        }

        Transform poolTrigger = sceneDataBase.transform.Find("PoolTrigger");
        for (int i = 0; i < poolTrigger.childCount; i++)
        {
            Transform temp = poolTrigger.GetChild(i);
            PoolTriggerData poolTriggerData = new PoolTriggerData();
            poolTriggerData.poolTriggerType =
                temp.tag == "angel" ? PoolTriggerData.PoolTriggerType.AngelPoolTrigger : PoolTriggerData.PoolTriggerType.DevilPoolTrigger;
            poolTriggerData.transformData.SetTransfor(temp);

            gameSceneData.poolTriggerDatas.Add(poolTriggerData);
        }

        Transform door = sceneDataBase.transform.Find("Door");
        for (int i = 0; i < door.childCount; i++)
        {
            Transform temp = door.GetChild(i);
            DoorBaseData doorBaseData = temp.GetComponent<DoorBase>().GetDoorBaseData();
            gameSceneData.doorBaseDatas.Add(doorBaseData);
        }

        Transform bridge = sceneDataBase.transform.Find("Bridge");
        for (int i = 0; i < bridge.childCount; i++)
        {
            Transform temp = bridge.GetChild(i);
            TransformData transformData = new TransformData();
            transformData.SetTransfor(temp);
            gameSceneData.bridgeDatas.Add(transformData);
        }

        Transform devilRole = sceneDataBase.transform.Find("DevilRole");
        for (int i = 0; i < devilRole.childCount; i++)
        {
            Transform temp = devilRole.GetChild(i);
            DevilRoleData devilRoleData = new DevilRoleData();
            devilRoleData.devilRoleState = temp.GetComponent<DevilRoleController>().devilRoleState;
            devilRoleData.roleType = temp.GetComponent<DevilRoleController>().roleType;
            devilRoleData.speed = temp.GetComponent<DevilRoleController>().speed;
            devilRoleData.transformData.SetTransfor(temp);
            gameSceneData.devilRoleDatas.Add(devilRoleData);
        }

        Transform column = sceneDataBase.transform.Find("Column");
        for (int i = 0; i < column.childCount; i++)
        {
            Transform temp = column.GetChild(i);
            TransformData transformData = new TransformData();
            transformData.SetTransfor(temp);
            gameSceneData.columns.Add(transformData);
        }

        Transform ArcadeGame = sceneDataBase.transform.Find("ArcadeGame");
        for (int i = 0; i < ArcadeGame.childCount; i++)
        {
            Transform temp = ArcadeGame.GetChild(i);
            TransformData transformData = new TransformData();
            transformData.SetTransfor(temp);
            gameSceneData.arcadeGames.Add(transformData);
        }

        Transform GiftBox = sceneDataBase.transform.Find("GiftBox");
        for (int i = 0; i < GiftBox.childCount; i++)
        {
            Transform temp = GiftBox.GetChild(i);
            GiftBoxData giftBoxData = new GiftBoxData();
            giftBoxData.transformData.SetTransfor(temp);
            if (temp.CompareTag("AngleBox"))
                giftBoxData.giftBoxState = GiftBoxState.AngleBox;
            else
                giftBoxData.giftBoxState = GiftBoxState.DevilBox;
            gameSceneData.giftBoxs.Add(giftBoxData);
        }


        gameSceneData.floor.SetTransfor(sceneDataBase.transform.Find("GameOverTrigger"));

        gameSceneData.endGameObject.SetTransfor(sceneDataBase.transform.Find("End Game Object"));


        gameSceneDataManager.Save("Scene Data " + fileName + ".json", gameSceneData);
        gameSceneData = new GameSceneData();
        System.GC.Collect();
    }


    public Transform end_wingBase;
    public Transform end_GameObject;
    private EndGameData endGameData = new EndGameData();
    private void ReadEnd()
    {
        if (end_wingBase) GameObject.Destroy(end_wingBase.gameObject);
        if (end_GameObject) GameObject.Destroy(end_GameObject.gameObject);


        end_GameObject = LoadObject<GameObject>(ResourcesType.ElseFolder, "End Game Object", true).transform;
        end_GameObject.name = "终点";

        end_wingBase = new GameObject("终点翅膀放这里").transform;

        //读取数据

        TextAsset text = Resources.Load<TextAsset>("ConfigFolder/EndData");
        if (text)
        {
            endGameData = JsonConvert.DeserializeObject<EndGameData>(text.text);
            if (endGameData.endDatas.ContainsKey(endKey))
            {
                foreach (WingData item in endGameData.endDatas[endKey].wingDatas)
                {
                    Transform wing;//= null;
                    switch (item.wingType)
                    {
                        case WingData.WingType.LittleAngelWings:
                            wing = LoadObject<GameObject>(ResourcesType.Wings, "Angel_0", true).transform;
                            break;
                        case WingData.WingType.BigAngelWings:
                            wing = LoadObject<GameObject>(ResourcesType.Wings, item.wingType.ToString(), true).transform;
                            break;
                        case WingData.WingType.LittleDevilWings:
                            wing = LoadObject<GameObject>(ResourcesType.Wings, "Devil_0", true).transform;
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
                }
            }
        }
        end_wingBase.position = end_GameObject.position;
    }
    /// <summary>
    /// 保存
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="wingID"></param>
    private void SaveEnd()
    {
        List<WingData> temp = new List<WingData>();
        if (end_wingBase)
        {
            for (int i = 0; i < end_wingBase.childCount; i++)
            {
                Transform item = end_wingBase.GetChild(i);
                WingData wingData = new WingData();
                switch (item.tag)
                {
                    case "angel":
                        wingData.wingType = WingData.WingType.LittleAngelWings;
                        break;
                    case "d_angel":
                        wingData.wingType = WingData.WingType.BigAngelWings;
                        break;
                    case "devil":
                        wingData.wingType = WingData.WingType.LittleDevilWings;
                        break;
                    case "d_devil":
                        wingData.wingType = WingData.WingType.BigDevilWings;
                        break;
                }
                wingData.transformData.SetTransfor(item);
                temp.Add(wingData);
            }
        }
        if (!endGameData.endDatas.ContainsKey(endKey))
        {
            endGameData.endDatas.Add(endKey, new EndGameData.EndData());
        }
        endGameData.endDatas[endKey].wingDatas = temp;
        if (end_wingBase == null)
        {
            endGameData.endDatas.Remove(endKey);
        }
        else if (end_wingBase.childCount == 0)
        {
            endGameData.endDatas.Remove(endKey);
        }
        gameSceneDataManager.Save("EndData.json", endGameData);
        //endGameData = new EndGameData();
        System.GC.Collect();
    }

    private T LoadObject<T>(ResourcesType resType, string name, bool isInstantiate) where T : UnityEngine.Object
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

    //void OnInspectorUpdate()
    //{
    //    if (EditorWindow.mouseOverWindow)
    //        EditorWindow.mouseOverWindow.Focus();//就是当鼠标移到那个窗口，这个窗口就自动聚焦
    //    this.Repaint();//重画MyWindow窗口，更新Label
    //}p


}
