using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TriggerGodScript : MonoBehaviour
{
    public Animator ani;
    private bool isTrigger = false;
    private PlayerController player;
    private PlayerModel playerModel;
    private void Awake()
    {
        playerModel = ModelManager.GetModel<PlayerModel>();
        MsgManager.Add(MsgManagerType.TriggerGod, TriggerGod);
        transformBase = transform.parent;
        upMovePos = roleTrans.localPosition + new Vector3(0, .4f, 0);
        downMovePos = roleTrans.localPosition;
        //UI关闭时
    }
    private void OnDestroy()
    {
        MsgManager.Remove(MsgManagerType.TriggerGod, TriggerGod);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isTrigger)
        {
            isTrigger = true;
            FindObjectOfType<TerminalPointBase>().isTriggerGod = true;
            player = other.GetComponent<PlayerController>();
            player.PlayerStopMove();
            player.ani.CrossFadeInFixedTime("A等待1", 0.2f);

            UIManager.Instance.OpenUI(PanelName.TriggerGodPanel);
            //弹出UI
            //StartCoroutine(Wait());
        }
    }
    public Transform roleTrans;
    private Vector3 upMovePos;
    private Vector3 downMovePos;
    private bool isUpMove = true;
    private void Update()
    {
        if (roleTrans)
        {
            if (isUpMove)
            {
                roleTrans.localPosition = Vector3.MoveTowards(roleTrans.localPosition, upMovePos, Time.deltaTime * .25f);
                if (Vector3.Distance(roleTrans.localPosition, upMovePos) <= 0.05f)
                {
                    isUpMove = !isUpMove;
                }
            }
            else
            {
                roleTrans.localPosition = Vector3.MoveTowards(roleTrans.localPosition, downMovePos, Time.deltaTime * .25f);
                if (Vector3.Distance(roleTrans.localPosition, downMovePos) <= 0.05f)
                {
                    isUpMove = !isUpMove;
                }
            }
        }
    }
    private Transform transformBase;
    private void TriggerGod(object obj)
    {
        bool flag = (bool)obj;
        roleTrans = null;
        if (!ani) ani = GetComponent<Animator>();
        Sequence seq = DOTween.Sequence();
        if (flag)
        {
            if (playerModel.GetFraction >= 50)
            {
                ani.SetTrigger("Yes Down");
                seq.AppendInterval(1f);
                seq.AppendCallback(() =>
                {
                    ModelManager.GetModel<PlayerModel>().SetFraction(PlayerModel.FractionType.TriggerGod, -100);
                    GameObject abc = Instantiate(ResourcesManager.Load<GameObject>(ResourcesType.ElseFolder, "Strike"), player.transform.position, Quaternion.Euler(-90, 0, 0));
                    Destroy(abc, 1);
                });
                seq.AppendInterval(0.5f);
            }
            else
            {
                ani.SetTrigger("Yes Up");
                seq.AppendInterval(0.5f);
                seq.AppendCallback(() =>
                {
                    ModelManager.GetModel<PlayerModel>().SetFraction(PlayerModel.FractionType.TriggerGod, 100);
                });
                seq.AppendInterval(0.5f);
            }
        }
        else
        {
            ani.SetTrigger("No");
            seq.AppendInterval(1f);
        }
        seq.AppendCallback(() =>
        {
            transformBase.DOMove(transformBase.position + new Vector3(0, 20, 0), 3f);
        });
        seq.AppendInterval(1f);
        seq.AppendCallback(() =>
        {
            player.PlayerPlayMove();
            player.GetPao();
            transformBase.gameObject.SetActive(false);
        });
    }

}
