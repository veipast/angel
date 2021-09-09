using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
/// <summary>
/// 用于记录所有配置表的类型
/// </summary>
[System.Serializable]
public class ConfigTypeData : ConfigDataBase
{
    [SerializeField]
    private Dictionary<string, Type> _configTypes = new Dictionary<string, Type>();
    public Dictionary<string, Type> ConfigTypes { get { return _configTypes; } set { _configTypes = value; } }
}
