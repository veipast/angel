using System;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;

public static class ManagementTool
{
    #region 
    public enum FilePath { customPath, dataPath, persistentDataPath, streamingAssetsPath }
    /// <summary>
    /// 打开文件夹
    /// </summary>
    /// <param name="filePath">快捷打开文件夹路径</param>
    /// <param name="path">自定义路径输入</param>
    public static void OpenDirectory(FilePath filePath, string path = null)
    {
        switch (filePath)
        {
            case FilePath.customPath:
                OpenDirectory(path);
                break;
            case FilePath.dataPath:
                OpenDirectory(Application.dataPath);
                break;
            case FilePath.persistentDataPath:
                OpenDirectory(Application.persistentDataPath);
                break;
            case FilePath.streamingAssetsPath:
                OpenDirectory(Application.streamingAssetsPath);
                break;
            default:
                break;
        }
    }
    private static void OpenDirectory(string path)
    {
        if (path == null || path == "") return;
        if (!Directory.Exists(path)) return;
        System.Diagnostics.Process.Start(path);
    }
    /// <summary>
    /// 拷贝文件到指定路径
    /// </summary>
    /// <param name="sourceDirPath">拷贝文件夹路径</param>
    /// <param name="SaveDirPath">保存路径</param>
    public static void CopyDirectory(string sourceDirPath, string SaveDirPath)
    {
        try
        {
            //如果指定的存储路径不存在，则创建该存储路径
            if (!Directory.Exists(SaveDirPath))
            {
                //创建
                Directory.CreateDirectory(SaveDirPath);
            }
            //获取源路径文件的名称
            string[] files = Directory.GetFiles(sourceDirPath);
            //遍历子文件夹的所有文件
            foreach (string file in files)
            {
                string pFilePath = SaveDirPath + "/" + Path.GetFileName(file);
                if (File.Exists(pFilePath))
                    continue;
                if (Path.GetExtension(pFilePath) == ".meta")
                    continue;
                File.Copy(file, pFilePath, true);
            }
            string[] dirs = Directory.GetDirectories(sourceDirPath);
            //递归，遍历文件夹
            foreach (string dir in dirs)
            {
                CopyDirectory(dir, SaveDirPath + "/" + Path.GetFileName(dir));
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }
    /// <summary>
    /// 删除指定路径
    /// </summary>
    /// <param name="path"></param>
    public static void DeleteDirectory(string path)
    {
        DirectoryInfo dir = new DirectoryInfo(path);
        if (dir.Exists)
        {
            DirectoryInfo[] childs = dir.GetDirectories();
            foreach (DirectoryInfo child in childs)
            {
                child.Delete(true);
            }
            dir.Delete(true);
        }
    }
    #endregion

    public static bool IsPointerOverGameObject()
    {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
        try
        {
            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                return true;
        }
        catch (Exception ex)
        {
            //if (EventSystem.current.IsPointerOverGameObject())
                return true;
        }
#elif UNITY_EDITOR
        if (EventSystem.current.IsPointerOverGameObject())
            return true;
#endif
        return false;
    }
}
