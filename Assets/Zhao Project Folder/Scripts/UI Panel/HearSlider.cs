using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HearSlider : UIBase
{
    public Camera _camera;
    public Transform player;
    public CanvasScaler scaler;
    public RectTransform headImage;
    public TMP_Text showStateText;
    public Slider slider;
    public Image angelImage;
    private RectTransform angelImage_RT;
    public Image devilImage;
    private RectTransform devilImage_RT;
    public float _high;
    public float _speed;

    public Image spritesImage;
    public List<Sprite> sprites = new List<Sprite>();
    private PlayerModel playerModel;

    public Animator animator;
    public GameObject FORGIVENESS;//宽恕 FORGIVENESS
    public GameObject PUNISHMENT; //惩罚 PUNISHMENT
    public override void Init()
    {
        playerModel = ModelManager.GetModel<PlayerModel>();

        angelImage_RT = angelImage.GetComponent<RectTransform>();
        devilImage_RT = devilImage.GetComponent<RectTransform>();

        slider.onValueChanged.AddListener(RefreshImage);

        scaler = GameObject.Find("UIManagerCanvas").GetComponent<CanvasScaler>();

        MsgManager.Add(MsgManagerType.RefreshHearSlider, RefreshHearSlider);
        MsgManager.Add(MsgManagerType.AngelEndDistance, AngelEndDistance);
        MsgManager.Add(MsgManagerType.DevilEndDistance, DevilEndDistance);
        MsgManager.Add(MsgManagerType.FORGIVENESS_PUNISHMENT, FORGIVENESS_PUNISHMENT);
        MsgManager.Add(MsgManagerType.TextScale, TextScale);
    }

    private void TextScale(object obj)
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(showStateText.transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 0.5f));
        seq.Append(showStateText.transform.DOScale(Vector3.one, 0.5f));
    }

    public override void Show()
    {
        _camera = Camera.main;
        player = FindObjectOfType<PlayerController>().transform;
        canvasGroup.alpha = 0;

        slider.value = 100 - playerModel.GetFraction;
        SetData(playerModel.GetFraction, "#ff7800", "HUMAN");

        isKeyDown = false;

        //showStateText.text = "<color=#ff7800>HUMAN</color>";
        angelImage.fillAmount = 1;
        devilImage.fillAmount = 1;


        base.Show();
    }
    public override void Hide()
    {
        base.Hide();

    }
    private void AngelEndDistance(object obj)
    {
        angelImage.fillAmount = (float)obj;
        if (angelImage.fillAmount == 0)
        {
            Sequence s = DOTween.Sequence();
            s.AppendInterval(0.5f);
            s.AppendCallback(() =>
            {
                //gameObject.SetActive(false);
                UIManager.Instance.CloseUI(PanelName.HeadStatePanel);
            });
        }
    }
    private void DevilEndDistance(object obj)
    {
        devilImage.fillAmount = (float)obj;
        if (devilImage.fillAmount == 0)
        {
            Sequence s = DOTween.Sequence();
            s.AppendInterval(0.5f);
            s.AppendCallback(() =>
            {
                //gameObject.SetActive(false);
                UIManager.Instance.CloseUI(PanelName.HeadStatePanel);
            });
        }
    }


    public CanvasGroup canvasGroup;
    private void RefreshImage(float arg0)
    {
        int num = (int)(arg0 * 3.065f);
        angelImage_RT.offsetMin = new Vector2(num, 0);
        devilImage_RT.offsetMax = new Vector2(-(306.5f - num), 0);
    }
    private float sliderValue;
    private bool isKeyDown = false;
    // Update is called once per frame
    private void Update()
    {
        if (Input.anyKey && !isKeyDown && !ManagementTool.IsPointerOverGameObject())
        {
            isKeyDown = true;
            canvasGroup.alpha = 1;
        }
        slider.value = Mathf.MoveTowards(slider.value, sliderValue, Time.deltaTime * 100);
        //}

        //private void LateUpdate()
        //{

    }

    // 如果启用 MonoBehaviour，则每个固定帧速率的帧都将调用此函数
    private void FixedUpdate()
    {
        UnityEngine.Vector3 playerHead = player.position + new UnityEngine.Vector3(0, _high, 0);
        playerHead = WorldToUI(scaler, _camera, playerHead);
        playerHead.y = _high * 100;
        headImage.anchoredPosition3D = playerHead;
    }

    public static UnityEngine.Vector3 WorldToUI(CanvasScaler scaler, Camera camera, UnityEngine.Vector3 pos)
    {
        float resolutionX = scaler.referenceResolution.x;
        float resolutionY = scaler.referenceResolution.y;

        UnityEngine.Vector3 viewportPos = camera.WorldToViewportPoint(pos);

        UnityEngine.Vector3 uiPos = new UnityEngine.Vector3(viewportPos.x * resolutionX - resolutionX * 0.5f,
            viewportPos.y * resolutionY - resolutionY * 0.5f, 0);

        return uiPos;
    }
    private void SetData(int value, string colorNum, string textStr)
    {
        sliderValue = 100 - value;
        //slider.DOValue(100 - value, _speed).SetSpeedBased();
        if (value > 50)
        {
            spritesImage.sprite = sprites[0];
        }
        else if (value < 50)
        {
            spritesImage.sprite = sprites[2];
        }
        else
        {
            spritesImage.sprite = sprites[1];
        }
        Color color;
        ColorUtility.TryParseHtmlString(colorNum, out color);
        showStateText.text = textStr;
        showStateText.DOColor(color, _speed).SetSpeedBased();
    }


    private void RefreshHearSlider(object obj)
    {
        string[] str = obj.ToString().Split('|');
        SetData(playerModel.GetFraction, str[0], str[1]);
    }

    private void FORGIVENESS_PUNISHMENT(object obj)
    {
        animator.Play("Show");
        bool flag = (bool)obj;
        FORGIVENESS.SetActive(flag);//宽恕 FORGIVENESS
        PUNISHMENT.SetActive(!flag); //惩罚 PUNISHMENT
    }

}
