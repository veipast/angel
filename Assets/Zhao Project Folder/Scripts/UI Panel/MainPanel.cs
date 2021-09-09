using System;
using System.Collections;
using System.Collections.Generic;
using GameAnalyticsSDK;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class MainPanel : UIBase
{
    public Image levelImage;
    public Text levelText;
    public GameObject flagImage;

    public Image setupImage;
    public Button setupButton;
    public Button audioButton;
    public Button vibroonButton;
    public Button infoButton;


    private GameDataModel gameData;
    public override void Init()
    {
        base.Init();
        gameData = ModelManager.GetModel<GameDataModel>();
        setupButton.onClick.AddListener(SetupButton);
        audioButton.onClick.AddListener(AudioButton);
        vibroonButton.onClick.AddListener(VibroonButton);
        infoButton.onClick.AddListener(OpenInfoLinkButton);
    }
    private bool isPlayerGameScene;
    public override void Show()
    {
        base.Show();
        isMouseDown = false;

        isPlayerGameScene = (SceneManager.GetActiveScene().name == "Player Game Scene");
        flagImage.SetActive(isPlayerGameScene);
        Color nowColor;
        ColorUtility.TryParseHtmlString("#695C77", out nowColor);
        levelImage.color = nowColor;
        levelText.text = "LEVEL " + gameData.LevelNnmber;
        setupImage.fillAmount = 0;
        isOpenSetupImage = false;
        setupImage.gameObject.SetActive(isOpenSetupImage);
        SetupAllImage();
    }
    public override void Hide()
    {
        base.Hide();
    }
    private bool isMouseDown = false;
    void Update()
    {
        if (!isMouseDown && Input.anyKey && !ManagementTool.IsPointerOverGameObject())
        {
            if (isPlayerGameScene)
            {
                isMouseDown = true;
                gameData = ModelManager.GetModel<GameDataModel>();
                GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, gameData.LevelNnmber.ToString("00000"));
                flagImage.SetActive(false);
            }
            if (isOpenSetupImage)
            {
                isOpenSetupImage = false;
                Sequence seq = DOTween.Sequence();
                seq.Append(setupImage.DOFillAmount(0, 0.3f));
                seq.AppendCallback(() => { setupImage.gameObject.SetActive(false); });
            }
        }

    }

    private bool isOpenSetupImage = false;

    private void SetupButton()
    {
        isOpenSetupImage = !isOpenSetupImage;
        if (isOpenSetupImage)
        {
            setupImage.gameObject.SetActive(true);
            setupImage.DOFillAmount(1, 0.3f);
        }
        else
        {
            Sequence seq = DOTween.Sequence();
            seq.Append(setupImage.DOFillAmount(0, 0.3f));
            seq.AppendCallback(() => { setupImage.gameObject.SetActive(false); });
        }

        VibrateClassScript.GetInstancee.Haptic(MoreMountains.NiceVibrations.HapticTypes.Selection);
    }

    private bool _setupAudio { get { return PlayerPrefs.GetInt("SetupAudio", 1) != 0; } set { PlayerPrefs.SetInt("SetupAudio", value ? 1 : 0); } }
    public Sprite[] audioSprite = new Sprite[2];
    private void AudioButton()
    {
        bool flag = _setupAudio;
        _setupAudio = !flag;
        //Debug.Log(_setupAudio.ToString() + flag.ToString() + PlayerPrefs.GetInt("SetupAudio", 0));
        SetupAllImage();

        VibrateClassScript.GetInstancee.Haptic(MoreMountains.NiceVibrations.HapticTypes.Selection);
    }
    private bool _setupVibroon { get { return PlayerPrefs.GetInt("VibroonAudio", 1) != 0; } set { PlayerPrefs.SetInt("VibroonAudio", value ? 1 : 0); } }
    public Sprite[] vibroonSprite = new Sprite[2];
    private void VibroonButton()
    {
        bool flag = _setupVibroon;
        _setupVibroon = !flag;
        SetupAllImage();

        VibrateClassScript.GetInstancee.Haptic(MoreMountains.NiceVibrations.HapticTypes.Selection);
    }

    private void OpenInfoLinkButton()
    {
        Application.OpenURL("https://millecrepestudios.com/privacy-policy");
    }
    private void SetupAllImage()
    {
        audioButton.GetComponent<Image>().sprite = audioSprite[_setupAudio ? 1 : 0];
        vibroonButton.GetComponent<Image>().sprite = vibroonSprite[_setupVibroon ? 1 : 0];
        MoreMountains.NiceVibrations.MMVibrationManager.SetHapticsActive(_setupVibroon);
    }

}
