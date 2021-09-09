using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelBase
{
    public virtual void Load() { }
    public virtual void UnLoad() { }
    /// <summary>
    /// 保存数据模型
    /// </summary>
    public virtual void SaveModelData() { }
    /// <summary>
    /// 加载数据模型
    /// </summary>
    public virtual void LoadModelData() { }
}
