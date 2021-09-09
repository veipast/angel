using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TerminalPointScript : TerminalPointBase
{
    public Transform Angel, Demon;
    public override void Start()
    {
        base.Start();
        //
    }
    private bool isTrigger = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isTrigger)
        {
            UIManager.Instance.CloseUI(PanelName.HeadStatePanel);
            isTrigger = true;
            PlayerController plr = other.GetComponent<PlayerController>();
            plr.SetTurnACorner();
            if (playerModel.GetFraction >= 50)
            {
                Sequence seq = DOTween.Sequence();
                Vector3 pos = Angel.position;
                pos.y = 0;
                plr.transform.DORotate(Angel.eulerAngles - new Vector3(0, 180, 0), 0.5f).SetEase(Ease.Linear);
                plr.CM_Vcam1_Follow.DORotate(Angel.eulerAngles - new Vector3(0, 180, 0), 0.3f);
                seq.AppendInterval(0.5f);
                seq.AppendCallback(() =>
                {
                    plr.PlayerPlayMove();
                    plr.SetRoadOrigin(Angel, Angel.eulerAngles - new Vector3(0, 180, 0));
                });
                MsgManager.Invoke(MsgManagerType.ChangeSky, true);
            }
            else
            {
                Sequence seq = DOTween.Sequence();
                Vector3 pos = Demon.position;
                pos.y = 0;
                plr.transform.DORotate(Demon.eulerAngles - new Vector3(0, 180, 0), 0.5f).SetEase(Ease.Linear);
                plr.CM_Vcam1_Follow.DORotate(Demon.eulerAngles - new Vector3(0, 180, 0), 0.3f);
                seq.AppendInterval(0.5f);
                seq.AppendCallback(() =>
                {
                    plr.PlayerPlayMove();
                    plr.SetRoadOrigin(Demon, Demon.eulerAngles - new Vector3(0, 180, 0));
                });
                MsgManager.Invoke(MsgManagerType.ChangeSky, false);
            }
        }
    }
}
