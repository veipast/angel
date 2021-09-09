//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Text;
//using Newtonsoft.Json;
//using UnityEngine;

///// <summary>
///// 
///// </summary>
//public static class ConfigManager
//{
//    private static bool isEncryption = false;

//    /// <summary>
//    /// 文件存储路径 asset目录 P目录 S目录
//    /// </summary>
//    public enum FilePath { dataPath, persistentDataPath, streamingAssetsPath }

//    //  路径 
//    private static string _configDataPath = "ConfigFolder";
//    private static string _configPersistentDataPath = "/ConfigFolder";
//    private static string _configStreamingAssetsPath = "/ConfigFolder";

//    /// <summary>
//    /// json文件数据   是否加密 校验 存储路径 类型
//    /// </summary>
//    private class JsonFileData
//    {
//        public bool isEncryption;
//        public ushort crc;
//        public FilePath filePath;
//        public Type configType;
//    }
//    /// <summary>
//    /// key -> 文件名（数据类型）    value -> 文件数据
//    /// </summary>
//    private static Dictionary<string, JsonFileData> filesData = new Dictionary<string, JsonFileData>();

//    /// <summary>
//    /// 存储所有配置表的数据 
//    /// </summary>
//    private static Dictionary<string, ConfigDataBase> _configDatas = new Dictionary<string, ConfigDataBase>();

//    /// <summary>
//    /// 初始化配置表  游戏开始时检测是否有数据 没有则拷贝初始数据到P目录
//    /// </summary>
//    /// <param name="isReadAllConfig"></param>
//    public static void Inst(bool isReadAllConfig = false)
//    {
//        //查找p目录是否有ConfigFolder文件
//        if (!Directory.Exists(Application.persistentDataPath + _configPersistentDataPath))
//        {
//            //拷贝到P目录
//            Directory.CreateDirectory(Application.persistentDataPath + _configPersistentDataPath);
//            //ManagementTool.CopyDirectory(Application.dataPath + _configDataPath, Application.persistentDataPath + _configPersistentDataPath);
//        }
//        //if (!Directory.Exists(Application.persistentDataPath + _configStreamingAssetsPath))
//        //{
//        //    Directory.CreateDirectory("ConfigFolder");
//        //}
//        string typeTxt = Application.persistentDataPath + _configPersistentDataPath + "/" + "ConfigTypeData.txt";
//        //读取所有游戏数据 filesData
//        if (File.Exists(typeTxt))
//        {
//            string str = GetTextAsset(typeTxt, true);
//            filesData = JsonConvert.DeserializeObject<Dictionary<string, JsonFileData>>(str);
//            if (isReadAllConfig)
//            {
//                foreach (var item in filesData)
//                {
//                    ReadConfig(item.Key, false);
//                }
//            }
//        }
//        else
//        {
//            typeTxt = _configDataPath + "/ConfigTypeData";
//            string str = GetTextAsset(typeTxt, false);
//            filesData = JsonConvert.DeserializeObject<Dictionary<string, JsonFileData>>(str);
//            if (isReadAllConfig)
//            {
//                foreach (var item in filesData)
//                {
//                    ReadConfig(item.Key, false);
//                }
//            }
//            SaveGameData();
//        }
//    }
//    private static string GetTextAsset(string path, bool isReadP)
//    {
//        if (isReadP)
//            return File.ReadAllText(path);
//        else
//            return Resources.Load<TextAsset>(path).text;
//    }
//    //获取数据
//    public static T GetConfigData<T>(string fileName, bool isReadP = true) where T : ConfigDataBase
//    {
//        if (!_configDatas.ContainsKey(fileName))
//        {
//            ReadConfig(fileName, isReadP);
//        }
//        return _configDatas[fileName] as T;
//        //return new T();
//    }
//    //读取
//    private static ConfigDataBase ReadConfig(string fileName, bool isReadP)
//    {
//        //lujing duqu
//        string path = Application.persistentDataPath + _configPersistentDataPath + "/" + fileName + ".txt";
//        if (!isReadP)
//        {
//            switch (filesData[fileName].filePath)
//            {
//                case FilePath.dataPath:
//                    path = _configDataPath + "/" + fileName;
//                    break;
//                case FilePath.persistentDataPath:
//                    path = Application.persistentDataPath + _configPersistentDataPath + "/" + fileName + ".txt";
//                    isReadP = true;
//                    break;
//                case FilePath.streamingAssetsPath:
//                    path = Application.streamingAssetsPath + _configStreamingAssetsPath + "/" + fileName + ".txt";
//                    isReadP = true;
//                    break;
//            }
//        }
//        string configData = GetTextAsset(path, isReadP);
//        //解密
//        ConfigDataBase temp = JsonDeciphering(fileName, configData);
//        _configDatas.Add(fileName, temp);
//        return temp;
//    }
//    /// <summary>
//    /// 写入数据
//    /// </summary>
//    /// <typeparam name="T"></typeparam>
//    /// <param name="fileName">文件名称</param>
//    /// <param name="configData">配置表数据</param>
//    /// <param name="filePath">保存路径</param>
//    public static void SetConfigData<T>(string fileName, ConfigDataBase configData, FilePath filePath = FilePath.dataPath) where T : ConfigDataBase
//    {
//        JsonFileData fileData = new JsonFileData();
//        fileData.isEncryption = isEncryption;
//        fileData.configType = typeof(T);
//        fileData.filePath = filePath;

//        //加入文件数据类
//        if (!filesData.ContainsKey(fileName))
//            filesData.Add(fileName, fileData);
//        else
//            filesData[fileName] = fileData;

//        //将数据存入本地
//        if (!_configDatas.ContainsKey(fileName))
//            _configDatas.Add(fileName, configData);
//        else
//            _configDatas[fileName] = configData;
//    }
//    //存档
//    public static void SaveGameData(bool isReadP = true)
//    {
//        foreach (var item in filesData)
//        {
//            if (_configDatas.ContainsKey(item.Key))
//            {
//                string str = JsonConvert.SerializeObject(_configDatas[item.Key], Formatting.Indented);
//                str = JsonEncryption(item.Key, str);
//                if (isReadP)
//                    File.WriteAllText(Application.persistentDataPath + _configPersistentDataPath + "/" + item.Key + ".txt", str);
//                else
//                    switch (item.Value.filePath)
//                    {
//                        case FilePath.dataPath:
//                            File.WriteAllText(Application.dataPath + _configDataPath + "/" + item.Key + ".txt", str);
//                            break;
//                        case FilePath.persistentDataPath:
//                            File.WriteAllText(Application.persistentDataPath + _configPersistentDataPath + "/" + item.Key + ".txt", str);
//                            break;
//                        case FilePath.streamingAssetsPath:
//                            File.WriteAllText(Application.streamingAssetsPath + _configStreamingAssetsPath + "/" + item.Key + ".txt", str);
//                            break;
//                        default:
//                            break;
//                    }
//            }
//        }
//        string configTypeData = JsonConvert.SerializeObject(filesData, Formatting.Indented);
//        if (isReadP)
//            File.WriteAllText(Application.persistentDataPath + _configPersistentDataPath + "/" + "ConfigTypeData.txt", configTypeData);
//        else
//            File.WriteAllText(Application.dataPath + _configDataPath + "/" + "ConfigTypeData.txt", configTypeData);

//        ////*****************************///
//        //UnityEditor.AssetDatabase.Refresh();
//    }
//    //加密
//    private static string JsonEncryption(string fileName, string content)
//    {
//        string str;
//        //是否要加密
//        if (isEncryption)
//        {
//            byte[] data = Encoding.UTF8.GetBytes(content);
//            //压缩
//            data = ZlibHelper.CompressBytes(data);
//            //加密
//            data = SecurityUtil.Xor(data);
//            //校验
//            ushort crc = Crc16.CalculateCrc16(data);
//            filesData[fileName].crc = crc;
//            str = JsonConvert.SerializeObject(data);
//        }
//        else
//        {
//            //校验
//            byte[] data = Encoding.UTF8.GetBytes(content);
//            ushort crc = Crc16.CalculateCrc16(data);
//            filesData[fileName].crc = crc;
//            str = content;
//        }
//        return str;
//    }
//    private static ConfigDataBase JsonDeciphering(string fileName, string content)
//    {
//        if (filesData[fileName].isEncryption)
//        {
//            byte[] jsondata = JsonConvert.DeserializeObject<byte[]>(content);
//            if (filesData[fileName].crc == Crc16.CalculateCrc16(jsondata))
//            {
//                byte[] data = SecurityUtil.Xor(jsondata);
//                data = ZlibHelper.DeCompressBytes(jsondata);
//                content = Encoding.UTF8.GetString(jsondata);
//            }
//            return null;
//        }
//        //jsonData = JsonConvert.DeserializeObject<ConfigDataBase>(content);
//        Type t = filesData[fileName].configType;
//        ConfigDataBase jsonData = JsonConvert.DeserializeObject(content, t) as ConfigDataBase;
//        return jsonData;
//    }
//}