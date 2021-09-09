using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ColumnScript : MonoBehaviour
{
    public List<Color> showColor = new List<Color>();
    private MeshRenderer mr;
    private float _number;//抵消分数
    private int _tempNumber;
    private Transform tsPag, emPag;
    private PlayerModel playerModel;
    public TMP_Text showNumberText;
    //public Transform  
    private void Awake()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "temp Player Game Scene")
        {
            enabled = false;
            return;
        }
        playerModel = ModelManager.GetModel<PlayerModel>();
        //时时获取分数
        MsgManager.Add(MsgManagerType.RefreshScore, RefreshScore);
        //锁定时时获取数据   锁定人物移动
        MsgManager.Add(MsgManagerType.LockRefreshScore, LockRefreshScore);

    }

    private void OnDestroy()
    {
        //时时获取分数
        MsgManager.Remove(MsgManagerType.RefreshScore, RefreshScore);
        //锁定时时获取数据   锁定人物移动i
        MsgManager.Remove(MsgManagerType.LockRefreshScore, LockRefreshScore);
    }
    private bool isLockScore = false;
    private void LockRefreshScore(object obj)
    {
        isLockScore = (bool)obj;
    }

    private bool isTS = true;
    private void RefreshScore(object obj)
    {
        if (isLockScore) return;
        int num = (int)obj;
        //时时切换两只🐷
        tsPag.gameObject.SetActive(false);//(num < 50);
        emPag.gameObject.SetActive(true);
        isTS = true;
        //根据分数切换颜色

    }
    Vector3 tempPos;
    float scaleNumber;
    // Start is called before the first frame update
    void Start()
    {
        tempPos = transform.position;
        mr = transform.GetChild(0).GetComponent<MeshRenderer>();
        //
        _number = Random.Range(20, 61);
        if (_number < 30)
        {
            mr.material.color = showColor[3];
        }
        else if (_number < 40)
        {
            mr.material.color = showColor[2];

        }
        else if (_number < 50)
        {
            mr.material.color = showColor[1];

        }
        else
        {
            mr.material.color = showColor[0];

        }
        scaleNumber = mr.transform.localScale.y / 60;

        _tempNumber = (int)_number;
        //加载两只猪
        tsPag = Instantiate(ResourcesManager.Load<GameObject>(ResourcesType.ElseFolder, "AnglePig"), transform.position + new Vector3(0, 1.8f, 0), transform.rotation).transform;
        emPag = Instantiate(ResourcesManager.Load<GameObject>(ResourcesType.ElseFolder, "DevilPig"), transform.position + new Vector3(0, 1.8f, 0), transform.rotation).transform;
        RefreshScore(50);
        GaoDu();
    }

    private bool isDown = false;
    // Update is called once per frame
    void Update()
    {
        if (isDown)
        {
            _number = Mathf.MoveTowards(_number, 0, Time.deltaTime * 50);
            int num = _tempNumber - (int)_number;
            if (isTS)
            {
                //playerModel.SetFraction = -num;
            }
            else
            {
                //playerModel.SetFraction = num;
            }
            //playerModel.SetFraction = num;
            _tempNumber = (int)_number;
            GaoDu();
            if (_number <= 0)
            {
                isDown = false;
                MsgManager.Invoke(MsgManagerType.LockRefreshScore, false);
                Destroy(gameObject);
                Destroy(tsPag.gameObject);
                Destroy(emPag.gameObject);
                //回复移动
                //解锁
            }
        }

        showNumberText.text = ((int)_number).ToString();

    }
    public void GaoDu()
    {
        Vector3 transPos = tempPos + new Vector3(0, -0.11f, 0) * (60 - _number);
        mr.transform.localScale = new Vector3(1, scaleNumber * _number, 1);
        Vector3 pos = transPos + new Vector3(0, 0.85f, 0) + new Vector3(0, 0.11f, 0) * 60;
        tsPag.position = pos;
        emPag.position = pos;
        showNumberText.transform.position = pos + transform.forward * 2.5f + new Vector3(0, -0.7f, 0);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (isLockScore) return;
        if (other.CompareTag("Player"))
        {
            //开启下降功能
            isDown = true;
            //停止移动
            MsgManager.Invoke(MsgManagerType.LockRefreshScore, true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (isLockScore && isDown) return;
        if (other.CompareTag("Player"))
        {
            //关闭下降功能
            isDown = false;
        }

    }
}
