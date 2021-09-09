using Excel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using UnityEngine;


public class ExcelTool
{

    /// <summary>
    /// 读取表数据，生成对应的数组
    /// </summary>
    /// <param name="filePath">excel文件全路径</param>
    /// <returns>Item数组</returns>
    public static List<PlayerRoleData> CreateRoleDataExcel(string filePath)
    {
        //获得表数据
        int columnNum = 0, rowNum = 0;
        DataRowCollection collect = ReadExcel(filePath, 0, ref columnNum, ref rowNum);

        //根据excel的定义，第二行开始才是数据
        List<PlayerRoleData> array = new List<PlayerRoleData>();
        for (int i = 1; i < rowNum; i++)
        {
            PlayerRoleData item = new PlayerRoleData();
            item.roleModelID = int.Parse(collect[i][0].ToString());
            item.roleName = collect[i][1].ToString();
            item.roleAge = int.Parse(collect[i][2].ToString());
            item.roleSexIsM = collect[i][3].ToString() == "M" ? true : false;
            item.roleRef = collect[i][4].ToString();
            item.startNumber = int.Parse(collect[i][5].ToString());
            item.levelTargetType = (PlayerRoleData.LevelTargetType)Enum.Parse(typeof(PlayerRoleData.LevelTargetType), collect[i][6].ToString());
            item.roleRef_Chinese = collect[i][7].ToString();
            item._ID = int.Parse(collect[i][8].ToString());
            array.Add(item);
        }
        return array;
    }

    public static BuildSceneData CreateBuildDataExcel(string filePath)
    {
        BuildSceneData data = new BuildSceneData();

        //获得表数据
        int columnNum = 0, rowNum = 0;

        //地狱
        DataRowCollection collect = ReadExcel(filePath, 0, ref columnNum, ref rowNum);
        for (int i = 1; i < rowNum; i++)
        {
            int key = int.Parse(collect[i][1].ToString());
            if (!data.hellUpgrades.ContainsKey(key))
            {
                data.hellUpgrades.Add(key, new List<BuildSceneUpgradeData>());
            }
            BuildSceneUpgradeData temp = new BuildSceneUpgradeData();
            temp.upgradeID = int.Parse(collect[i][2].ToString());
            temp.upgradeName = collect[i][3].ToString().Split('+');
            temp.upgradeLevel = int.Parse(collect[i][4].ToString());
            temp.upgradeCost = int.Parse(collect[i][5].ToString());
            data.hellUpgrades[key].Add(temp);
        }

        //天堂
        collect = ReadExcel(filePath, 1, ref columnNum, ref rowNum);
        for (int i = 1; i < rowNum; i++)
        {
            int key = int.Parse(collect[i][1].ToString());
            if (!data.heavenUpgrades.ContainsKey(key))
            {
                data.heavenUpgrades.Add(key, new List<BuildSceneUpgradeData>());
            }
            BuildSceneUpgradeData temp = new BuildSceneUpgradeData();
            temp.upgradeID = int.Parse(collect[i][2].ToString());
            temp.upgradeName = collect[i][3].ToString().Split('+');
            temp.upgradeLevel = int.Parse(collect[i][4].ToString());
            temp.upgradeCost = int.Parse(collect[i][5].ToString());
            data.heavenUpgrades[key].Add(temp);
        }
        return data;
    }

    /// <summary>
    /// 读取excel文件内容
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <param name="tablesIndex">多表格表格的下标</param>
    /// <param name="columnNum">行数</param>
    /// <param name="rowNum">列数</param>
    /// <returns></returns>
    static DataRowCollection ReadExcel(string filePath, int tablesIndex, ref int columnNum, ref int rowNum)
    {
        FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
        DataSet result = excelReader.AsDataSet();
        //Tables[0] 下标0表示excel文件中第一张表的数据
        columnNum = result.Tables[tablesIndex].Columns.Count;
        rowNum = result.Tables[tablesIndex].Rows.Count;
        return result.Tables[tablesIndex].Rows;
    }
}

/// <summary>
/// 
/// </summary>
/// <param name="filePath"></param>
/// <param name="columnNum"></param>
/// <param name="rowNum"></param>
/// <returns></returns>