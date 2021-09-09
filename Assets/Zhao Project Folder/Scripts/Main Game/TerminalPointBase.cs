using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TerminalPointBase : MonoBehaviour
{
    public PlayerModel playerModel;
    public enum TerminalPointStart
    {
        UpAndFront, UpAndDown,
    }
    private GameDataModel gameData;
    public TerminalPointStart terminalPointStart;//台阶样式
    public virtual void Awake()
    {
        gameData = ModelManager.GetModel<GameDataModel>();
        MsgManager.Add(MsgManagerType.ChangeSky, ChangeSky);
        MsgManager.Add(MsgManagerType.RefreshHearSlider, ShowEndRoads);
        MsgManager.Add(MsgManagerType.AllEndObject, AllEndObject);
    }

    public virtual void OnDestroy()
    {
        MsgManager.Remove(MsgManagerType.ChangeSky, ChangeSky);
        MsgManager.Remove(MsgManagerType.RefreshHearSlider, ShowEndRoads);
        MsgManager.Remove(MsgManagerType.AllEndObject, AllEndObject);
    }
    public virtual void Start()
    {
        playerModel = ModelManager.GetModel<PlayerModel>();
        StartCoroutine(Wait());
        try
        {
            skyBox = GameObject.Find("Main/SkyBox").GetComponent<MeshRenderer>();
        }
        catch { }
        //翅膀也要跟着隐藏
        if (SceneManager.GetActiveScene().name != "temp Player Game Scene")
        {
            //if (FindObjectOfType<TurnACorner>())
            {
                AllEndObject(!FindObjectOfType<TurnACorner>());
                foreach (var item in allOBJ_A)
                {
                    item.SetActive(false);
                }
                foreach (var item in allOBJ_D)
                {
                    item.SetActive(false);
                }
            }
        }

    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(1);
        ChangeSky(true);
    }




    #region 控制天空盒和环境
    public Color lineageColor;
    public Color infernalColor;
    [Tooltip("天空盒")]
    public MeshRenderer skyBox;
    [Tooltip("天堂")]
    public List<GameObject> lineageOBJ = new List<GameObject>();
    [Tooltip("地狱")]
    public List<GameObject> infernalOBJ = new List<GameObject>();
    [Tooltip("地狱")]
    public List<GameObject> allOBJ_A = new List<GameObject>();
    public List<GameObject> allOBJ_D = new List<GameObject>();
    public GameObject yun;
    [HideInInspector]
    public GameObject wingsObject;
    public GameObject godObj_1;
    public GameObject godObj_2;
    private bool isShowEnd = false;
    private void AllEndObject(object obj)
    {
        isShowEnd = (bool)obj;
        yun.SetActive(isShowEnd);
        wingsObject.SetActive(isShowEnd);
        ShowEndRoads();

    }
    [HideInInspector]
    public bool isTriggerGod = false;
    private float jlcj = -1;
    private void ShowEndRoads(object obj = null)
    {
        if (!isShowEnd) return;


        bool flag = playerModel.GetFraction >= 50;
        if (!isTriggerGod)
        {
            bool isGodType_1 = false;
            bool isGodType_2 = false;
            if (flag && gameData.playerRoleModel.GetRoleDtat.levelTargetType == PlayerRoleData.LevelTargetType.Hell)
                isGodType_2 = true;
            else if (!flag && gameData.playerRoleModel.GetRoleDtat.levelTargetType == PlayerRoleData.LevelTargetType.Heaven)
                isGodType_1 = true;
            godObj_1.SetActive(isGodType_1);
            godObj_2.SetActive(isGodType_2);
        }
        foreach (var item in allOBJ_A)
        {
            item.SetActive(flag);
        }
        foreach (var item in allOBJ_D)
        {
            item.SetActive(!flag);
        }
        if (jlcj != playerModel.GetFraction)
        {
            jlcj = playerModel.GetFraction;
            Transform w = wingsObject.transform;
            for (int i = 0; i < w.childCount; i++)
            {
                if (flag)
                    w.GetChild(i).gameObject.SetActive(!GetTransformCross(w.GetChild(i)));
                else
                    w.GetChild(i).gameObject.SetActive(GetTransformCross(w.GetChild(i)));
            }
        }
    }
    private bool GetTransformCross(Transform t)
    {
        Vector3 f = transform.forward;
        Vector3 r = t.position - transform.position;
        //Debug.Log(Vector3.Cross(f, r));
        return Vector3.Cross(f, r).y >= 0;
    }
    private float skyNum = 0;
    private bool isSkyMove = false;
    /// <summary>
    /// 刷新环境
    /// </summary>
    /// <param name="obj"></param>
    private void ChangeSky(object obj)
    {
        bool flag = (bool)obj;
        if (flag)//天堂
        {
            skyNum = 0;
            isSkyMove = true;
            RenderSettings.fogColor = lineageColor;
        }
        else//地狱
        {
            skyNum = 1;
            isSkyMove = true;
            RenderSettings.fogColor = infernalColor;
        }
        foreach (var item in lineageOBJ)
        {
            item.SetActive(flag);
        }
        foreach (var item in infernalOBJ)
        {
            item.SetActive(!flag);
        }
    }
    private float skyNumMove = 0;
    public virtual void Update()
    {
        if (isSkyMove)
        {
            skyNumMove = Mathf.MoveTowards(skyNumMove, skyNum, Time.deltaTime * 3f);
            if (skyBox)
                skyBox.material.SetFloat("_Invert", skyNumMove);
            if (skyNumMove == skyNum) isSkyMove = false;
        }
    }
    #endregion

}
