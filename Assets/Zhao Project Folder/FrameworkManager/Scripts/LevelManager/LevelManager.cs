using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
public static class SceneLevelManager// : Singleton<SceneLevelManager>
{
    //记录下一个加载场景信息
    private static string _sceneName;
    public static string SceneName { get { return _sceneName; } }

    public static void LoadingStartScene()
    {
        CleanSceneData();
        SceneManager.LoadScene("StartScene");
        CleanGC();
    }

    private static LevelBase _levelBase;
    public static void AddSceneController(LevelBase level)
    {
        _levelBase = level;
    }
    /// <summary>
    /// 加载到异步场景
    /// </summary>
    /// <param name="sceneName"></param>
    public static void Loading(string sceneName)
    {
        _sceneName = sceneName;
        UIManager.Instance._sceneLoadingPanel.OpenPanel();
    }
    public static void Loading(string sceneName, bool v)
    {
        _sceneName = sceneName;
        CleanSceneData();
        SceneManager.LoadScene("LoadingScene");
        CleanGC();
    }

    //卸载/关闭上个场景的UI/消息中心
    private static void CleanSceneData()
    {
        if (_levelBase != null)
        {
            _levelBase.OnLeave();
            CleanGC();
            _levelBase = null;
        }
    }
    public static void ToLoadingScene()
    {
        CleanSceneData();
        SceneManager.LoadScene("LoadingScene");
        CleanGC();
    }
    //清理GC
    private static void CleanGC()
    {
        //Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }

    internal static void Loading(object loadSceneName)
    {
        throw new NotImplementedException();
    }
}


