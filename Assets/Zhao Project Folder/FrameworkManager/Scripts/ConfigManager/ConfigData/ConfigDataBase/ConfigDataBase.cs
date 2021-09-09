using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[System.Serializable]
public class ConfigDataBase
{
    [SerializeField]
    private int _id;
    public int ID { get { return _id; } set { _id = value; } }
}
