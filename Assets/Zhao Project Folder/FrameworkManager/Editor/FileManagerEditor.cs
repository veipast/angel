using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class FileManagerEditor : Editor
{
    // dataPath, persistentDataPath, streamingAssetsPath
    [MenuItem("自定义工具/打开Asset文件夹")]
    public static void OpenDataPath()
    {
        ManagementTool.OpenDirectory(ManagementTool.FilePath.dataPath);
    }
    [MenuItem("自定义工具/打开P目录文件夹")]
    public static void OpenPersistentDataPath()
    {
        ManagementTool.OpenDirectory(ManagementTool.FilePath.persistentDataPath);
    }
    [MenuItem("自定义工具/删除游戏数据")]
    public static void RemoveConfig()
    {
        PlayerPrefs.DeleteAll();
        ManagementTool.DeleteDirectory(Application.persistentDataPath + "/ConfigFolder");
    }
    //    [MenuItem("自定义工具/CSV->JSON/A.创建道路片段数据")]
    //    public static void LevelDesignCSV()
    //    {
    //        string str = Resources.Load<TextAsset>("ConfigFolder/RoadDataList").text;
    //        InitDataModel.InitData initData = JsonUtility.FromJson<InitDataModel.InitData>(str);
    //        initData.roadDatas = new List<RoadData>();//其他数据保持不变

    //        string[] csvString = File.ReadAllLines(Application.persistentDataPath + "/LevelDesign.csv");

    //        int index = 0;
    //        while (index < csvString.Length)
    //        {
    //            string[] idAndCount = csvString[index].Split(',');

    //            RoadData tempRoad = new RoadData();
    //            tempRoad._ID = int.Parse(idAndCount[0]);

    //            int endIndex = int.Parse(idAndCount[1]) + index;
    //            int plusIndex = index + 1;

    //            while (plusIndex <= endIndex)
    //            {
    //                string[] roadString = csvString[plusIndex].Split(',');
    //                tempRoad.roadArray.Add(new RoadData.RoadArrat());
    //                for (int i = 0; i < roadString.Length; i++)
    //                {
    //                    try
    //                    {
    //                        tempRoad.roadArray[plusIndex - index - 1].roadArray.Add((RoadType)System.Enum.Parse(typeof(RoadType), roadString[i]));
    //                    }
    //                    catch (System.Exception ex)
    //                    {
    //                        Debug.LogError("A.创建道路片段数据 CSV --> JSON 创建出错！\n" + ex + " \n" + roadString[i] + "  " + plusIndex + "  " + i);
    //                    }
    //                }
    //                plusIndex++;
    //            }
    //            initData.roadDatas.Add(tempRoad);
    //            index = plusIndex;
    //        }
    //        System.IO.File.WriteAllText(Application.dataPath + "/Zhao_Script_Project/Resources/ConfigFolder/RoadDataList.json", JsonUtility.ToJson(initData, true));
    //        AssetDatabase.Refresh();
    //        Debug.Log("A.创建道路片段数据 CSV --> JSON 创建成功！");
    //    }
    //    [MenuItem("自定义工具/CSV->JSON/B.创建各个关卡路径数据")]
    //    public static void CreateLevelData()
    //    {
    //        string str = Resources.Load<TextAsset>("ConfigFolder/RoadDataList").text;
    //        InitDataModel.InitData initData = JsonUtility.FromJson<InitDataModel.InitData>(str);
    //        initData.levelRoadData = new List<LevelRoadData>();//其他数据保持不变 Project Runway关卡设计 - 关卡

    //        string[] csvString = File.ReadAllLines(Application.persistentDataPath + "/Project Runway关卡设计 - 关卡.csv");
    //        for (int i = 1; i < csvString.Length; i++)
    //        {
    //            LevelRoadData temp = new LevelRoadData();
    //            string[] strArr = csvString[i].Split(',');
    //            temp.levelID = int.Parse(strArr[0]);
    //            temp.describe = strArr[1];
    //            for (int k = 2; k < strArr.Length; k++)
    //            {
    //                if (strArr[k] == "" || strArr[k] == null) break;
    //                temp.roadTypes.Add(strArr[k]);
    //            }
    //            initData.levelRoadData.Add(temp);
    //        }
    //        System.IO.File.WriteAllText(Application.dataPath + "/Zhao_Script_Project/Resources/ConfigFolder/RoadDataList.json", JsonUtility.ToJson(initData, true));
    //        AssetDatabase.Refresh();
    //        Debug.Log("B.创建各个关卡路径数据 CSV --> JSON 创建成功！");
    //    }
    //    [MenuItem("自定义工具/CSV->JSON/C.创建服饰描述数据")]
    //    public static void CreateClothingDescribeData()
    //    {
    //        string str = Resources.Load<TextAsset>("ConfigFolder/RoadDataList").text;
    //        InitDataModel.InitData initData = JsonUtility.FromJson<InitDataModel.InitData>(str);
    //        initData.clothingDescribes = new List<ClothingDescribeData>();//其他数据保持不变 Project Runway关卡设计 - 关卡

    //        string[] csvString = File.ReadAllLines(Application.persistentDataPath + "/ClothMatch.csv");
    //        for (int i = 0; i < csvString.Length; i++)
    //        {
    //            ClothingDescribeData temp = new ClothingDescribeData();
    //            string[] strArr = csvString[i].Split(',');
    //            temp.describe = strArr[0];

    //            temp.upper_Body = strArr[1].Split('|');
    //            temp.lower_Body = strArr[2].Split('|');
    //            temp.shoe_Body = strArr[3].Split('|');

    //            initData.clothingDescribes.Add(temp);
    //        }

    //        System.IO.File.WriteAllText(Application.dataPath + "/Zhao_Script_Project/Resources/ConfigFolder/RoadDataList.json", JsonUtility.ToJson(initData, true));
    //        AssetDatabase.Refresh();
    //        Debug.Log("C.创建服饰描述数据 CSV --> JSON 创建成功！");
    //    }

    //    [MenuItem("自定义工具/CSV->JSON/D.创建所有服饰数据")]
    //    public static void CreateAllClothingData()
    //    {
    //        string str = Resources.Load<TextAsset>("ConfigFolder/RoadDataList").text;
    //        InitDataModel.InitData initData = JsonUtility.FromJson<InitDataModel.InitData>(str);
    //        initData.allClothingData = new List<ClothingData>();//其他数据保持不变

    //        string[] csvString = File.ReadAllLines(Application.persistentDataPath + "/AllClothingData.csv");
    //        for (int i = 5; i < csvString.Length; i++)
    //        {
    //            ClothingData temp = new ClothingData();
    //            string[] strArr = csvString[i].Split(',');

    //            temp.name = strArr[0];
    //            temp.clothingType = (ClothingType)System.Enum.Parse(typeof(ClothingType), strArr[1]);
    //            temp.bodyType = (BodyType)System.Enum.Parse(typeof(BodyType), strArr[2]);

    //            initData.allClothingData.Add(temp);
    //        }

    //        System.IO.File.WriteAllText(Application.dataPath + "/Zhao_Script_Project/Resources/ConfigFolder/RoadDataList.json", JsonUtility.ToJson(initData, true));
    //        AssetDatabase.Refresh();
    //        Debug.Log("D.创建所有服饰数据 CSV --> JSON 创建成功！");
    //    }
    //    [MenuItem("自定义工具/CSV->JSON/E.创建饰品数据")]
    //    public static void CreateGoodsData()
    //    {
    //        string str = Resources.Load<TextAsset>("ConfigFolder/RoadDataList").text;
    //        InitDataModel.InitData initData = JsonUtility.FromJson<InitDataModel.InitData>(str);
    //        initData.shopGoodsDatas = new ShopGoodsData();//其他数据保持不变

    //        string[] csvString = File.ReadAllLines(Application.persistentDataPath + "/GoodsData.csv");
    //        int endIndex = int.Parse(csvString[0].Split(',')[0]);
    //        int i = 1;
    //        for (; i <= endIndex; i++)
    //        {
    //            GoodsData data = new GoodsData();
    //            string[] strArr = csvString[i].Split(',');
    //            data._ID = int.Parse(strArr[0]);
    //            data.name = strArr[1];
    //            data.goodsType = (GoodsType)System.Enum.Parse(typeof(GoodsType), strArr[2]);
    //            data.doubleReward = float.Parse(strArr[3]);
    //            data.isUnlock = bool.Parse(strArr[4]);
    //            initData.shopGoodsDatas.hairstyleData.Add(data);
    //        }
    //        endIndex = i + int.Parse(csvString[i].Split(',')[0]);
    //        i++;
    //        for (; i <= endIndex; i++)
    //        {
    //            GoodsData data = new GoodsData();
    //            string[] strArr = csvString[i].Split(',');
    //            data._ID = int.Parse(strArr[0]);
    //            data.name = strArr[1];
    //            data.goodsType = (GoodsType)System.Enum.Parse(typeof(GoodsType), strArr[2]);
    //            data.doubleReward = float.Parse(strArr[3]);
    //            data.isUnlock = bool.Parse(strArr[4]);
    //            initData.shopGoodsDatas.accessoryData.Add(data);
    //        }

    //        System.IO.File.WriteAllText(Application.dataPath + "/Zhao_Script_Project/Resources/ConfigFolder/RoadDataList.json", JsonUtility.ToJson(initData, true));
    //        AssetDatabase.Refresh();
    //        Debug.Log("E.创建饰品数据 CSV --> JSON 创建成功！");
    //    }














    //    [MenuItem("自定义工具/Data->JSON/A.创建路径难度数据")]
    //    public static void CreateRoadDifficultyData()
    //    {
    //        string str = Resources.Load<TextAsset>("ConfigFolder/RoadDataList").text;
    //        InitDataModel.InitData initData = JsonUtility.FromJson<InitDataModel.InitData>(str);
    //        initData.roadDifficultys = new List<RoadDifficultyData>();//其他数据保持不变 Project Runway关卡设计 - 关卡

    //        initData.roadDifficultys.Add(new RoadDifficultyData());
    //        initData.roadDifficultys[0].roadDifficultyType = RoadDifficultyData.RoadDifficultyType.ordinary;
    //        initData.roadDifficultys[0].roadIDs = new List<int>()
    //        {
    //            1,
    //            2,3,4,5,
    //            23,24,25,
    //            17,19,
    //            14
    //        };

    //        initData.roadDifficultys.Add(new RoadDifficultyData());
    //        initData.roadDifficultys[1].roadDifficultyType = RoadDifficultyData.RoadDifficultyType.simple;
    //        initData.roadDifficultys[1].roadIDs = new List<int>()
    //        {
    //            6,7,8,
    //            12,13,
    //            28,29,30
    //        };

    //        initData.roadDifficultys.Add(new RoadDifficultyData());
    //        initData.roadDifficultys[2].roadDifficultyType = RoadDifficultyData.RoadDifficultyType.Difficulty;
    //        initData.roadDifficultys[2].roadIDs = new List<int>()
    //        {
    //            9,10,11,
    //            20,21,
    //            22,23
    //        };

    //        initData.roadDifficultys.Add(new RoadDifficultyData());
    //        initData.roadDifficultys[3].roadDifficultyType = RoadDifficultyData.RoadDifficultyType.Difficulty;
    //        initData.roadDifficultys[3].roadIDs = new List<int>()
    //        {
    //           26,27
    //        };

    //        System.IO.File.WriteAllText(Application.dataPath + "/Zhao_Script_Project/Resources/ConfigFolder/RoadDataList.json", JsonUtility.ToJson(initData, true));
    //        AssetDatabase.Refresh();
    //        Debug.Log("A.创建路径难度数据 Data --> JSON 创建成功！");
    //    }



    //    [MenuItem("自定义工具/模型修改/A.拆解角色MeshRenderer")]
    //    public static void DismantlingRolesMeshRenderer()
    //    {
    //        GameObject[] obj = Selection.gameObjects;
    //        foreach (var item in obj)
    //        {
    //            SkinnedMeshRenderer smr = item.GetComponent<SkinnedMeshRenderer>();
    //            if (smr == null) continue;
    //            Mesh me = smr.sharedMesh;
    //            Material[] ma = smr.sharedMaterials;
    //            item.AddComponent<MeshFilter>().sharedMesh = me;
    //            item.AddComponent<MeshRenderer>().sharedMaterials = ma;
    //            DestroyImmediate(smr);
    //        }
    //    }
}
