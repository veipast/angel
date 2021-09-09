using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class CloudController : MonoBehaviour
{
    public Transform cloudObject;
    public Transform cloudUpPostiion;
    private bool isTrigger = false;
    private PlayerModel playerModel;
    private void Awake()
    {
        playerModel = ModelManager.GetModel<PlayerModel>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (isTrigger) return;
        if (other.CompareTag("Player"))
        {
            isTrigger = true;
            if (playerModel.GetFraction <= 50) return;
            PlayerController player = other.GetComponent<PlayerController>();
            player.PlayerStopMove(); player.shangshengYun(true);
            float time = (cloudUpPostiion.position.y - cloudObject.position.y) / 9.9853f;
            UnityEngine.Vector3 pos1 = player.transform.position + player.transform.forward * 1.3f;
            UnityEngine.Vector3 pos2 = new UnityEngine.Vector3(pos1.x, cloudUpPostiion.position.y, pos1.z);
            UnityEngine.Vector3 pos3 = pos2 - cloudUpPostiion.forward * 2.5f;
            Sequence s = DOTween.Sequence();
            s.Append(player.transform.DOMove(pos1, 0.5f));
            s.AppendCallback(() =>
            {
                player.ani.CrossFadeInFixedTime("A等待1", 0.2f);
                cloudObject.DOMove(cloudUpPostiion.position, time);
                player.transform.DOMove(pos2, time);
            });
            s.AppendInterval(time + 0.1f);
            s.AppendCallback(() =>
            {
                player.GetPao();
            });
            //UnityEngine.Vector3 pos = new UnityEngine.Vector3(player.transform.position.x, cloudUpPostiion.position.y, cloudUpPostiion.position.z);
            s.Append(player.transform.DOMove(pos3, 0.2f));
            s.AppendCallback(() =>
            {
                player.PlayerPlayMove(); player.shangshengYun(false);
            });
            s.AppendInterval(1.5f);
            s.AppendCallback(() =>
            {
                player.shangshengYun(false);
            });
        }
    }

    public UpCloudData GetData()
    {
        UpCloudData upCloudData = new UpCloudData();
        upCloudData.transformData.SetTransfor(transform);
        upCloudData.cloudUpPostiionData.SetTransfor(cloudUpPostiion);
        //upCloudData.fXShapeData.SetFXShape(cloudObject.GetComponent<ParticleSystem>());
        upCloudData.colliderSizeData.SetBoxCollider(transform.GetComponent<BoxCollider>());
        return upCloudData;
    }

    internal void SetData(UpCloudData upCloudData)
    {
        upCloudData.transformData.GetTransfor(transform);
        upCloudData.cloudUpPostiionData.GetTransfor(cloudUpPostiion);
        //upCloudData.fXShapeData.GetFXShape(cloudObject.GetComponent<ParticleSystem>());
        upCloudData.colliderSizeData.GetBoxCollider(transform.GetComponent<BoxCollider>());
    }
}
