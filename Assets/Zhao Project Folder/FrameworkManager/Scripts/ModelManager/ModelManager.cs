using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class ModelManager
{
    private static Dictionary<Type, ModelBase> models = new Dictionary<Type, ModelBase>();
    private static bool isRegister = false;
    /// <summary>
    /// 初始化
    /// </summary>
    public static void Register()
    {
        RegisterAll(new PlayerModel());
        RegisterAll(new GameDataModel());
        RegisterAll(new BuildDataModel());
        isRegister = true;
    }
    private static void RegisterAll(ModelBase model)
    {
        if (!models.ContainsKey(model.GetType()))
        {
            models.Add(model.GetType(), model);
            model.Load();
        }
        else
        {
            Debug.LogError(model.GetType().ToString() + "数据模型重复注册！");
        }
    }

    public static T GetModel<T>() where T : ModelBase
    {
        if (!isRegister) Register();
        ModelBase model;
        if (!models.TryGetValue(typeof(T), out model))
        {
            Debug.LogError(typeof(T).ToString() + "数据模型未注册！");
        }
        return model as T;
    }
    /// <summary>
    /// 存储数据
    /// </summary>
    public static void SaveModelData()
    {
        foreach (var item in models)
        {
            item.Value.SaveModelData();
        }
    }
    /// <summary>
    /// 加载数据
    /// </summary>
    public static void LoadModelData()
    {
        foreach (var item in models)
        {
            item.Value.LoadModelData();
        }
    }
}
