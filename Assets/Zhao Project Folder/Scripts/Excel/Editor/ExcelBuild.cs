using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

public class ExcelBuild : Editor
{

    [MenuItem("自定义工具/Excel/角色数据")]
    public static void CreateRoleDataAsset()
    {
        List<PlayerRoleData> data = ExcelTool.CreateRoleDataExcel(Application.dataPath + "/Zhao Project Folder/Scripts/Excel/Editor/Character Data.xlsx");
        //Debug.LogError(data.Count);
        string configPath = "/Zhao Project Folder/Resources/ConfigFolder/";

        string str = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.Delete(Application.dataPath + configPath + "Character Data.json");
        File.WriteAllText(Application.dataPath + configPath + "Character Data.json", str);
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif

    }
    [MenuItem("自定义工具/Excel/建造场景数据")]
    public static void CreateBuildDataAsset()
    {
        BuildSceneData data =  ExcelTool.CreateBuildDataExcel(Application.dataPath + "/Zhao Project Folder/Scripts/Excel/Editor/HOH_Build Data.xlsx");

        //Debug.LogError(data.Count);
        string configPath = "/Zhao Project Folder/Resources/ConfigFolder/";

        string str = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.Delete(Application.dataPath + configPath + "Build Scene Data.json");
        File.WriteAllText(Application.dataPath + configPath + "Build Scene Data.json", str);
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
    }
}
