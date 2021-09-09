using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using ECM.Components;

public class DEBUG_PANEL : MonoBehaviour
{
    public Button 显示或隐藏UI;
    public Button 打开控制面板;
    public Button 关闭控制面板;//关闭控制面板
    public GameObject 控制面板;
    public Toggle 是否开启广告;
    public Dropdown dropdown_Select_Scene;
    public Dropdown dropdown_Select_Player;
    public Text Select_Player_Text;
    public Button goSceneButton;
    private GameDataModel gameData;
    private bool IsShoweUI = true;

    private PlayerModel playerModel;
    public InputField 角色移动速度;
    public InputField 角色拖拽速度;
    public InputField 角色拖拽范围;
    public InputField 角色拖拽延迟;
    public InputField MLSpeed;
    public InputField 角色拖拽速度2;
    public InputField 拖拽分界线;
    public Button 角色速度按钮;



    private void Start()
    {
        gameData = ModelManager.GetModel<GameDataModel>();
        playerModel = ModelManager.GetModel<PlayerModel>();
        显示或隐藏UI.onClick.AddListener(() =>
        {
            IsShoweUI = !IsShoweUI;
            UIManager.Instance.ShowAndHide(IsShoweUI ? 1 : 0);
        });

        打开控制面板.onClick.AddListener(() =>
        {
            控制面板.SetActive(true);
            sceneIndex = gameData.sceneIndex;
            roleDatasIndex = 0;

            dropdown_Select_Scene.value = sceneIndex;
            dropdown_Select_Player.value = roleDatasIndex;
            Select_Player_Text.text = JsonConvert.SerializeObject(gameData.playerRoleModel.playerRoleDatas[roleDatasIndex], Formatting.Indented);

            角色移动速度.text = PlayerSpeed._this.speed.ToString();
            角色拖拽速度.text = PlayerSpeed._this.mSpeed.ToString();
            角色拖拽范围.text = PlayerSpeed._this.mSpeed_Min.ToString();
            角色拖拽延迟.text = PlayerSpeed._this.mSpeed_Max.ToString();
            //if (cm)
            //    MLSpeed.text = cm.maxLateralSpeed.ToString();

            //角色拖拽速度2.text = playerModel.tuSpeed_2.ToString();
            //拖拽分界线.text = playerModel.tuXZ_If.ToString();

        });
        关闭控制面板.onClick.AddListener(() => { 控制面板.SetActive(false); });

        dropdown_Select_Scene.options = new List<Dropdown.OptionData>();
        for (int i = 0; i < gameData.sceneNames.Length; i++)
        {
            dropdown_Select_Scene.options.Add(new Dropdown.OptionData(gameData.sceneNames[i]));
        }
        dropdown_Select_Scene.onValueChanged.AddListener(Dropdown_Select_Scene);

        dropdown_Select_Player.options = new List<Dropdown.OptionData>();
        for (int i = 0; i < gameData.playerRoleModel.playerRoleDatas.Count; i++)
        {
            dropdown_Select_Player.options.Add(new Dropdown.OptionData(gameData.playerRoleModel.playerRoleDatas[i].roleName));
        }
        dropdown_Select_Player.onValueChanged.AddListener(Dropdown_Select_Player);
        goSceneButton.onClick.AddListener(GoSceneButton);
        是否开启广告.isOn = MAX_SDK_Manager.GetInstantiate().isShowAD;
        是否开启广告.onValueChanged.AddListener((v) =>
        {
            MAX_SDK_Manager.GetInstantiate().isShowAD = v;
            if (v) MAX_SDK_Manager.GetInstantiate().ShowBanner();
            else MAX_SDK_Manager.GetInstantiate().HideBanner();
        });

        角色速度按钮.onClick.AddListener(() =>
        {
            PlayerSpeed._this.speed = float.Parse(角色移动速度.text);
            PlayerSpeed._this.mSpeed = float.Parse(角色拖拽速度.text);
            PlayerSpeed._this.mSpeed_Min = float.Parse(角色拖拽范围.text);
            PlayerSpeed._this.mSpeed_Max = float.Parse(角色拖拽延迟.text);
            //CharacterMovement cm = FindObjectOfType<CharacterMovement>();
            //if (cm)
            //    cm.maxLateralSpeed = float.Parse(MLSpeed.text);

            //playerModel.tuSpeed_2 = float.Parse(角色拖拽速度2.text);
            //playerModel.tuXZ_If = float.Parse(拖拽分界线.text);
            FindObjectOfType<PlayerController>().playerSpeed = PlayerSpeed._this;
        });
    }

    private int sceneIndex;
    private int roleDatasIndex;
    private void Dropdown_Select_Scene(int arg0)
    {
        sceneIndex = arg0;
    }

    private void Dropdown_Select_Player(int arg0)
    {
        roleDatasIndex = arg0;
        Select_Player_Text.text = JsonConvert.SerializeObject(gameData.playerRoleModel.playerRoleDatas[roleDatasIndex], Formatting.Indented);
    }
    private void GoSceneButton()
    {
        gameData.sceneIndex = sceneIndex;
        gameData.playerRoleModel.roleDatasIndex = gameData.playerRoleModel.playerRoleDatas[roleDatasIndex]._ID;
        控制面板.SetActive(false);
        SceneLevelManager.Loading(gameData.LoadSceneName);
    }

}
