using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class RoleDescriptionPanel : UIBase
{
    public Text flagText;
    public Text nameText;
    public Text ageText;
    public Text refText;
    public Transform roleDescriptionBase;
    public Transform endDescriptionBase;
    public Image baseImage;
    public Image bqImage;
    public Sprite[] bp = new Sprite[2];
    private GameDataModel gameData;
    public override void Init()
    {
        base.Init();
        gameData = ModelManager.GetModel<GameDataModel>();
    }
    public override void Show()
    {
        base.Show();
        roleDescriptionBase.localScale = Vector3.zero;
        endDescriptionBase.localScale = Vector3.zero;
        roleDescriptionBase.DOScale(Vector3.one, 0.3f);

        PlayerRoleData data = gameData.playerRoleModel.GetRoleDtat;
        switch (data.levelTargetType)
        {
            case PlayerRoleData.LevelTargetType.Heaven:
                flagText.text = "DESTINY: <color=#55FF00>HEAVEN</color>";
                break;
            case PlayerRoleData.LevelTargetType.Hell:
                flagText.text = "DESTINY: <color=red>HELL</color>";
                break;
            default:
                break;
        }
        bqImage.sprite = bp[(int)data.levelTargetType];
        nameText.text = data.roleName;
        ageText.text = data.roleAge.ToString();
        refText.text = data.roleRef;
        isMouseDown = false;
        switch (data.levelTargetType)
        {
            case PlayerRoleData.LevelTargetType.Heaven:
                Color nowColor1;
                ColorUtility.TryParseHtmlString("#51BBFF", out nowColor1);
                baseImage.color = nowColor1;
                break;
            case PlayerRoleData.LevelTargetType.Hell:
                Color nowColor2;
                ColorUtility.TryParseHtmlString("#FF5157", out nowColor2);
                baseImage.color = nowColor2;
                break;
            default:
                break;
        }
    }
    public override void Hide()
    {
        base.Hide();
    }
    private bool isMouseDown = false;
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !ManagementTool.IsPointerOverGameObject() && !isMouseDown)
        {
            isMouseDown = true;
            Sequence seq = DOTween.Sequence();
            seq.Append(roleDescriptionBase.DOScale(Vector3.zero, 0.3f));
            seq.Append(endDescriptionBase.DOScale(Vector3.one, 0.3f));
        }
    }
}
