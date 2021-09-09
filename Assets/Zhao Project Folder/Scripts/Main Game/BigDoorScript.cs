using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
public class BigDoorScript : MonoBehaviour
{
    public Material aMaterial;
    public GameObject aObject;
    public Material dMaterial;
    public GameObject dObject;
    public MeshRenderer doorMR;
    public TMP_Text showText;
    private GameDataModel gameData;
    // Start is called before the first frame update
    void Start()
    {
        gameData = ModelManager.GetModel<GameDataModel>();
        PlayerRoleData data = gameData.playerRoleModel.GetRoleDtat;
        switch (data.levelTargetType)
        {
            case PlayerRoleData.LevelTargetType.Heaven:
                showText.text = "DESTINY TO HEAVEN";
                doorMR.material = aMaterial;
                aObject.SetActive(true);
                break;
            case PlayerRoleData.LevelTargetType.Hell:
                showText.text = "DESTINY TO HELL";
                doorMR.material = dMaterial;
                dObject.SetActive(true);
                break;
            default:
                break;
        }
    }

    private bool isKeyDown = true;
    // Update is called once per frame
    void Update()
    {
        if (Input.anyKey && isKeyDown)
        {
            isKeyDown = false;
            PlayerRoleData data = gameData.playerRoleModel.GetRoleDtat;
            switch (data.levelTargetType)
            {
                case PlayerRoleData.LevelTargetType.Heaven:
                    aObject.transform.GetChild(0).GetComponent<Animator>().SetTrigger("Yes Up");
                    break;
                case PlayerRoleData.LevelTargetType.Hell:
                    dObject.transform.GetChild(0).GetComponent<Animator>().SetTrigger("Yes Down");
                    break;
                default:
                    break;
            }
        }
    }
    private bool isTrigger = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isTrigger)
        {
            isTrigger = true;
            PlayerRoleData data = gameData.playerRoleModel.GetRoleDtat;
            switch (data.levelTargetType)
            {
                case PlayerRoleData.LevelTargetType.Heaven:

                    //变天
                    MsgManager.Invoke(MsgManagerType.ChangeSky, true);

                    Color nowColor;
                    ColorUtility.TryParseHtmlString("#51BBFF", out nowColor);
                    GameObject.Find("LEVELIMAGE").GetComponent<Image>().color = nowColor;
                    GameObject.Find("LEVELTEXT").GetComponent<Text>().text = "HEAVEN";
                    break;
                case PlayerRoleData.LevelTargetType.Hell:

                    //变天
                    MsgManager.Invoke(MsgManagerType.ChangeSky, false);
                    Color nowColor1;
                    ColorUtility.TryParseHtmlString("#FF5157", out nowColor1);
                    GameObject.Find("LEVELIMAGE").GetComponent<Image>().color = nowColor1;
                    GameObject.Find("LEVELTEXT").GetComponent<Text>().text = "HELL";
                    break;
                default:
                    break;
            }

            Transform t = GameObject.Find("LEVELIMAGE").transform.parent;
            Sequence seq = DOTween.Sequence();
            seq.Append(t.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.3f));
            seq.Append(t.DOScale(Vector3.one, 0.3f));
            Destroy(gameObject, 0.5f);
        }
    }
}
