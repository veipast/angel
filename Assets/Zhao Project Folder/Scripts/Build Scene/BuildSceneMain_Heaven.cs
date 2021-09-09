using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildSceneMain_Heaven : BuildSceneMianScript
{
    void Start()
    {
        buildData.LoadBuildSceneType = LoadBuildSceneType.Heaven;
        base.FirstLoad(!buildData.isA_Look);
        buildData.isA_Look = true;
    }
    //public void Init()
    //{
    //    
    //    MsgManager.Invoke(MsgManagerType.BuildLoadButton, transform);
    //}
}

