using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DevilRoleController : MonoBehaviour
{
    public enum RoleType
    {
        DevilRole, AngelRole, QuestionRole
    }
    public RoleType roleType;
    public DevilRoleState devilRoleState;
    public Transform roleTrans;
    public Transform TextTrans;
    public float speed = 5;
    private bool isMove = false;
    private Vector3 pos_1;
    private Vector3 pos_2;
    private Transform cameraTrann;

    private Vector3 upMovePos;
    private Vector3 downMovePos;
    private bool isUpMove = true;
    // Start is called before the first frame update
    void Start()
    {
        pos_1 = transform.position + transform.forward * 4;
        pos_2 = transform.position + transform.forward * -4;
        isMove = SceneManager.GetActiveScene().name != "temp Player Game Scene";
        cameraTrann = Camera.main.transform;
        if (roleTrans)
        {
            upMovePos = roleTrans.localPosition + new Vector3(0, .4f, 0);
            downMovePos = roleTrans.localPosition;
        }

    }
    private bool isMovePos_1 = true;
    // Update is called once per frame
    void Update()
    {
        if (isMove)
        {
            switch (devilRoleState)
            {
                case DevilRoleState.Idle:
                    break;
                case DevilRoleState.Move:
                    if (isMovePos_1)
                    {
                        transform.position = Vector3.MoveTowards(transform.position, pos_1, Time.deltaTime * speed);
                        transform.LookAt(pos_1);
                        if (Vector3.Distance(pos_1, transform.position) <= 0.3f)
                        {
                            isMovePos_1 = false;
                        }
                    }
                    else
                    {
                        transform.position = Vector3.MoveTowards(transform.position, pos_2, Time.deltaTime * speed);
                        transform.LookAt(pos_2);
                        if (Vector3.Distance(pos_2, transform.position) <= 0.3f)
                        {
                            isMovePos_1 = true;
                        }
                    }
                    break;
                default:
                    break;
            }
            if (roleTrans)
            {
                if (isUpMove)
                {
                    roleTrans.localPosition = Vector3.Lerp(roleTrans.localPosition, upMovePos, Time.deltaTime * 3f);
                    if (Vector3.Distance(roleTrans.localPosition, upMovePos) <= 0.05f)
                    {
                        isUpMove = !isUpMove;
                    }
                }
                else
                {
                    roleTrans.localPosition = Vector3.Lerp(roleTrans.localPosition, downMovePos, Time.deltaTime * 3f);
                    if (Vector3.Distance(roleTrans.localPosition, downMovePos) <= 0.05f)
                    {
                        isUpMove = !isUpMove;
                    }
                }
            }
            if (TextTrans)
                TextTrans.rotation = cameraTrann.rotation;
        }
    }

    private bool isTrigger = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isTrigger)
        {
            VibrateClassScript.GetInstancee.Haptic(MoreMountains.NiceVibrations.HapticTypes.Selection);
            isTrigger = true;
            switch (roleType)
            {
                case RoleType.DevilRole:
                    MsgManager.Invoke(MsgManagerType.TriggerDevilRole);
                    break;
                case RoleType.AngelRole:
                    MsgManager.Invoke(MsgManagerType.TriggerAngelRole);
                    break;
                case RoleType.QuestionRole:
                    MsgManager.Invoke(MsgManagerType.TriggerQuestionRole);
                    break;
                default:
                    break;
            }
            Destroy(gameObject);
        }
    }
}
