using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerCoinPanel : UIBase
{
    public Text txText;
    public Text emText;
    public Text upText;
    public GameObject angelCoinFX;
    public GameObject devilCoinFX;
    private GameDataModel gameData;
    private PlayerModel playerModel;

    public override void Init()
    {
        base.Init();
        MsgManager.Add(MsgManagerType.ShowAngelCoin, ShowAngelCoin);
        MsgManager.Add(MsgManagerType.ShowDevilCoin, ShowDevilCoin);
        MsgManager.Add(MsgManagerType.ShowDamageNumber, ShowDamageNumber);
        MsgManager.Add(MsgManagerType.CoinCountPlus, CoinCountPlus);


        gameData = ModelManager.GetModel<GameDataModel>();
        playerModel = ModelManager.GetModel<PlayerModel>();
    }
    public override void Show()
    {
        base.Show();
        upText.gameObject.SetActive(false);
        angelCoinFX.SetActive(false);
        devilCoinFX.SetActive(false);
        upText.GetComponent<RectTransform>().position = txText.GetComponent<RectTransform>().position;
        txText.text = gameData.AngelCoin.ToString();
        emText.text = gameData.DevilCoin.ToString();
    }
    public override void Hide()
    {
        base.Hide();
    }



    private void ShowDamageNumber(object obj)
    {
        int num = (int)obj;
        if (num >= 0)
        {
            gameData.AngelCoin += num;
            txText.text = gameData.AngelCoin.ToString();
        }
        else
        {
            gameData.DevilCoin += -num;
            emText.text = gameData.DevilCoin.ToString();
        }
    }

    private void ShowAngelCoin(object obj)
    {
        int X_Number = (int)obj;
        if (playerModel.GetFraction >= 50)
        {
            int num = gameData.GetCoinCount() * X_Number;
            gameData.AngelCoin += num;

            upText.text = "<color=#FFFFFF>+ " + (num).ToString() + "</color>";
            upText.GetComponent<RectTransform>().position = txText.GetComponent<RectTransform>().position;
            upText.gameObject.SetActive(true);
            Sequence seq = DOTween.Sequence();
            seq.Append(upText.transform.DOMove(txText.transform.position + new Vector3(0, .5f, 0), 0.5f));
            seq.AppendCallback(() => { upText.gameObject.SetActive(false); });
        }
        else
        {
            int num = gameData.GetCoinCount() * X_Number;
            gameData.DevilCoin += num;


            upText.text = "<color=#FFFFFF>+ " + (num).ToString() + "</color>";
            upText.GetComponent<RectTransform>().position = emText.GetComponent<RectTransform>().position;
            upText.gameObject.SetActive(true);
            Sequence seq = DOTween.Sequence();
            seq.Append(upText.transform.DOMove(emText.transform.position + new Vector3(0, .5f, 0), 0.5f));
            seq.AppendCallback(() => { upText.gameObject.SetActive(false); });
        }
        txText.text = gameData.AngelCoin.ToString();
        emText.text = gameData.DevilCoin.ToString();
    }

    private void ShowDevilCoin(object obj)
    {
        //int X_Number = (int)obj;
        string[] str = obj.ToString().Split('|');

        if (bool.Parse(str[0]))
        {
            upText.text = "<color=#FFFFFF>" + str[1] + "</color>";
            upText.GetComponent<RectTransform>().position = txText.GetComponent<RectTransform>().position;
            upText.gameObject.SetActive(true);
            Sequence seq = DOTween.Sequence();
            seq.Append(upText.transform.DOMove(txText.transform.position + new Vector3(0, .5f, 0), 0.5f));
            seq.AppendCallback(() => { upText.gameObject.SetActive(false); });
        }
        else
        {
            upText.text = "<color=#FFFFFF>" + str[1] + "</color>";
            upText.GetComponent<RectTransform>().position = emText.GetComponent<RectTransform>().position;
            upText.gameObject.SetActive(true);
            Sequence seq = DOTween.Sequence();
            seq.Append(upText.transform.DOMove(emText.transform.position + new Vector3(0, .5f, 0), 0.5f));
            seq.AppendCallback(() => { upText.gameObject.SetActive(false); });
        }
        txText.text = gameData.AngelCoin.ToString();
        emText.text = gameData.DevilCoin.ToString();
    }

    private void CoinCountPlus(object obj)
    {
        //播放硬币特效
        if (ModelManager.GetModel<PlayerModel>().GetFraction >= 50)
            angelCoinFX.SetActive(true);
        else
            devilCoinFX.SetActive(true);
    }
}
