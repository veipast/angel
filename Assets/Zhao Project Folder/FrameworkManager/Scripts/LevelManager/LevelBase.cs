using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBase : MonoBehaviour
{
    public virtual void Awake()
    {
        SceneLevelManager.AddSceneController(this);
    }
    /// <summary>
    /// 离开场景时
    /// </summary>
    public virtual void OnLeave() { }
}
