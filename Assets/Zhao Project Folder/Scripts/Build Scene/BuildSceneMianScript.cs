using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class BuildSceneMianScript : MonoBehaviour
{
    public RuntimeAnimatorController animatorController;
    public BuildDataModel buildData;
    public float aniTimeNum = 4.3f;
    private GameDataModel gameData;
    public virtual void Awake()
    {
       
        UIManager.Instance.OpenUI(PanelName.MainPanel);
        UIManager.Instance.OpenUI(PanelName.PlayerCoinPanel);
   
        buildData = ModelManager.GetModel<BuildDataModel>();
        MsgManager.Add(MsgManagerType.RefreshBuildScene, RefreshBuildScene);
    }
    public virtual void OnDisable()
    {
        try
        {
            UIManager.Instance.CloseUI(PanelName.MainPanel);
            UIManager.Instance.CloseUI(PanelName.PlayerCoinPanel);
            UIManager.Instance.CloseUI(PanelName.BuildScenePanel);
            MsgManager.Remove(MsgManagerType.RefreshBuildScene, RefreshBuildScene);
            //Resources.UnloadUnusedAssets();//卸载未占用的asset资源
            System.GC.Collect();//回收内存
        }
        catch { }
    }


    //是否第一次加载场景
    public void FirstLoad(bool value)
    {
        if (value)
        {
            Sequence seq = DOTween.Sequence();
            seq.AppendInterval(1.5f);
            seq.AppendCallback(() =>
            {
                gameObject.AddComponent<Animator>().runtimeAnimatorController = animatorController;
            });
            seq.AppendInterval(aniTimeNum);
            seq.AppendCallback(() =>
            {
                Destroy(GetComponent<Animator>());
                UIManager.Instance.OpenUI(PanelName.BuildScenePanel);
                RefreshBuildScene();
            });
        }
        else
        {
            UIManager.Instance.OpenUI(PanelName.BuildScenePanel); 
            //根据要求显示相应的内容
            RefreshBuildScene();
        }
    }
    private void RefreshBuildScene(object obj)
    {
        BuildButtonItem.BuildIndex buildIndex = (BuildButtonItem.BuildIndex)obj;
        Transform keyT = transform.GetChild(buildIndex.key);
        for (int ind = 0; ind < buildIndex.buildSceneUpgradeData.upgradeName.Length; ind++)
        {
            Transform t = keyT.Find(buildIndex.buildSceneUpgradeData.upgradeName[ind]);
            Vector3 tempPos = t.position;
            t.position = tempPos + new Vector3(0, 1f, 0);
            Sequence seq = DOTween.Sequence();
            seq.AppendInterval(0.5f);
            seq.Append(t.DOMove(tempPos, 0.3f));
            seq.AppendCallback(() => { });
        }
        RefreshBuildScene();
    }

    private void RefreshBuildScene()
    {
        Dictionary<int, List<BuildSceneUpgradeData>> upgrades = buildData.LoadBuildSceneData;
        foreach (var item in upgrades)
        {
            int key = item.Key;
            List<BuildSceneUpgradeData> value = item.Value;
            Transform keyT = transform.GetChild(key);
            for (int i = 0; i < value.Count; i++)
            {
                for (int ind = 0; ind < value[i].upgradeName.Length; ind++)
                {
                    //Debug.Log(key + " : " + value[i].upgradeName[ind]);
                    keyT.Find(value[i].upgradeName[ind]).gameObject.SetActive(value[i].isUnlock);
                }
            }
        }
        MsgManager.Invoke(MsgManagerType.BuildLoadButton, transform);
    }
}
