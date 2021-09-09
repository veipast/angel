using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildSceneMain_Hell : BuildSceneMianScript
{
    void Start()
    {
        buildData.LoadBuildSceneType = LoadBuildSceneType.Hell;
        base.FirstLoad(!buildData.isD_Look);
        buildData.isD_Look = true;
    }
    //private void Init()
    //{
    //    //加载UI
    //    MsgManager.Invoke(MsgManagerType.BuildLoadButton, transform);
    //}
}
