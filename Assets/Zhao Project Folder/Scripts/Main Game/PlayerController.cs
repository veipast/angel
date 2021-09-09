using System;
using System.Collections;
using System.Collections.Generic;
using DamageNumbersPro;
using DG.Tweening;
using ECM.Components;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum MoveState
{
    //地面，天空 跳跃
    Ground, Jump, AngelSky, Fall
}
[System.Serializable]
public class PlayerSpeed
{
    public static PlayerSpeed _this = new PlayerSpeed();
    public float speed = 12;
    public float mSpeed = 3;
    public float mSpeed_Min = 0.2f;
    public float mSpeed_Max = 30;
}
public class PlayerController : MonoBehaviour
{
    public enum PlayerDirection
    {
        front = 0, left = 90, back = 180, right = 270
    }
    private PlayerModel playerModel;
    public MoveState moveState;
    public PlayerMoveState playerMoveState;
    [HideInInspector]
    public Animator ani;
    private bool isARun;
    public void GetPao(bool flag = false)
    {
        if (playerMoveState != PlayerMoveState.正常路面移动 || isPlayerRotate) return;
        if (flag)
        {
            //A跑状态下 分数大于45
            if (isARun && playerModel.GetFraction >= 45)
            {
                return;
            }
            else if (!isARun && playerModel.GetFraction < 45)
            {
                return;
            }
        }
        if (playerModel.GetFraction >= 45)
        {
            isARun = true;
            ani.CrossFadeInFixedTime("A跑", 0.2f);
        }
        else
        {
            isARun = false;
            ani.CrossFadeInFixedTime("B跑", 0.2f);
        }
    }
    private bool isEndInputStop = false;
    internal void SetTurnACorner()
    {
        playerSpeed.speed = flagSpeed;
        isTurnACorner = false;
        isLimit = true;
        isTriggerEnd = false;
        isEndInputStop = true;
    }

    public CharacterController movement;
    public Transform cameraFollowPoint;
    [Tooltip("中心点")]
    public Transform roadOrigin;
    public PlayerSpeed playerSpeed;
    public float gravity = 50.0F;
    public float rotateNumber = 8;//4.5f
    public float cameraSpeed = 1.2f;//4.5f
    public Transform mistTransform;//云雾
    public DamageNumber damageT;
    public DamageNumber damageE;


    private Vector3 moveDirection = Vector3.zero;
    private Vector3 nowPosition;
    private Vector3 previousPosition;



    internal void TerminalPointStop(bool isA)
    {
        playerMoveState = PlayerMoveState.等待;
        isCameraMove = false;
        isEndEA = false;
        transform.DORotate(transform.eulerAngles + new Vector3(0, 180, 0), 0.3f);
        ani.CrossFadeInFixedTime("A转身90", 0.2f);
        StartCoroutine(GameWinsWait());
        Vector3 downPos = transform.position;
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.3f);
        if (isA)
        {
            seq.AppendCallback(() =>
            {
                ani.CrossFadeInFixedTime("A飞升", 0.2f);
                GameObject obj = ResourcesManager.Load<GameObject>(ResourcesType.ElseFolder, "God_ray");
                Instantiate(obj, transform.position - new Vector3(0, 3.35f, 0), Quaternion.Euler(90, 0, 0));
            });
            seq.AppendInterval(1.5f);
            seq.Append(playerTrans.DOMove(downPos + new Vector3(0, 15, 0), 2.5f));
        }
        else
        {
            seq.AppendCallback(() =>
            {
                ani.CrossFadeInFixedTime("A害怕", 0.2f);
                GameObject obj = ResourcesManager.Load<GameObject>(ResourcesType.ElseFolder, "heidong");
                Transform t = Instantiate(obj, transform.position + new Vector3(0, 0.1f, 0), Quaternion.identity).transform;
                t.localScale = Vector3.zero;
                t.DOScale(new Vector3(3.4f, 3.4f, 3.4f), 0.4f);
            });
            seq.AppendInterval(1.5f);
            seq.Append(playerTrans.DOMove(downPos + new Vector3(0, -5, 0), 2));
        }
        angelFX_Up.gameObject.SetActive(false);
        devilFX_Up.gameObject.SetActive(false);
    }

    private float H_Control = 0;
    private bool isPlayGame = false;
    private float limitFloat = 4.5f;
    /// <summary>
    /// 是否限制范围
    /// </summary>
    private bool isLimit = true;
    /// <summary>
    /// true前后方形 false左右方向
    /// </summary>
    private bool isFront = true;

    private GameDataModel gameData;
    private MainGameSceneScript mainGameSceneScript;
    private void Start()
    {
        flagSpeed = playerSpeed.speed;
        mainGameSceneScript = FindObjectOfType<MainGameSceneScript>();
        angelFX.gameObject.SetActive(false);
        devilFX.gameObject.SetActive(false);
        angelFX_Up.gameObject.SetActive(false);
        devilFX_Up.gameObject.SetActive(false);
        InitPlayer();
        playerSpeed = PlayerSpeed._this;
    }
    [HideInInspector]
    public Transform playerTrans;
    private void InitPlayer()
    {
        //创建角色
        PlayerRoleData data = gameData.playerRoleModel.GetRoleDtat;

        playerTrans = Instantiate(ResourcesManager.Load<GameObject>(ResourcesType.Roles, "player" + data._ID.ToString()), transform, false).transform;
        playerTrans.localPosition = Vector3.zero;
        playerTrans.localEulerAngles = Vector3.zero;
        playerTrans.localScale = new Vector3(2.8f, 2.8f, 2.8f);
        playerTrans.name = "Player Game Object";
        //playerTrans = transform.Find();
        ani = playerTrans.GetComponent<Animator>();
        //[Tooltip("天使翅膀")]
        angelWingObject.Add(new PlayerWingObject(GameObject.Find("天使翅膀上")));
        angelWingObject.Add(new PlayerWingObject(GameObject.Find("天使翅膀下")));
        angelWingObject.Add(new PlayerWingObject(GameObject.Find("天使环")));
        foreach (var item in angelWingObject) item.SetActive(false);
        //[Tooltip("恶魔翅膀")]
        devilWingObject.Add(new PlayerWingObject(GameObject.Find("恶魔翅膀")));
        devilWingObject.Add(new PlayerWingObject(GameObject.Find("恶魔尾巴")));
        devilWingObject.Add(new PlayerWingObject(GameObject.Find("角")));
        foreach (var item in devilWingObject) item.SetActive(false);
        //[Tooltip("变色")]
        for (int i = 0; i < playerTrans.childCount; i++)
        {
            SkinnedMeshRenderer temp = playerTrans.GetChild(i).GetComponent<SkinnedMeshRenderer>();
            if (temp)
                playerMtr.Add(temp);
        }

        //初始化分数
        playerModel.InitFraction = data.startNumber;
        RefreshFraction();

        if (data.levelTargetType == PlayerRoleData.LevelTargetType.Heaven)
        {
            playerModel.LittleAngelWings = 1;
            playerModel.LittleDevilWings = -1;
        }
        else
        {
            playerModel.LittleAngelWings = 1;
            playerModel.LittleDevilWings = -1;
        }
    }


    private void Awake()
    {
        MsgManager.Add(MsgManagerType.RefreshFraction, RefreshFraction);
        MsgManager.Add(MsgManagerType.TriggerTurnACorner, TriggerTurnACorner);//触发了转弯机制
        MsgManager.Add(MsgManagerType.TriggerJumpState, TriggerJumpState);
        MsgManager.Add(MsgManagerType.TriggerAngelSkyState, TriggerAngelSkyState);
        MsgManager.Add(MsgManagerType.TriggerFallState, TriggerFallState);
        MsgManager.Add(MsgManagerType.DoorTriggerEnter, DoorTriggerEnter);//限制
        MsgManager.Add(MsgManagerType.DoorTriggerExit, DoorTriggerExit);//结束限制 结算分数
        MsgManager.Add(MsgManagerType.TriggerEnterBridge, TriggerEnterBridge);//独木桥
        MsgManager.Add(MsgManagerType.TriggerDownBridge, TriggerExitBridge);//独木桥
        MsgManager.Add(MsgManagerType.ShowDamageNumber, ShowDamageNumber);
        MsgManager.Add(MsgManagerType.TriggerDevilRole, TriggerDevilRole);
        MsgManager.Add(MsgManagerType.TriggerAngelRole, TriggerAngelRole);
        MsgManager.Add(MsgManagerType.TriggerQuestionRole, TriggerQuestionRole);
        MsgManager.Add(MsgManagerType.ChangeSky, ChangeSky);
        MsgManager.Add(MsgManagerType.LockRefreshScore, LockRefreshScore);
        MsgManager.Add(MsgManagerType.ShowGiftBoxPanelType, ShowGiftBoxPanelType);
        MsgManager.Add(MsgManagerType.DestroyGiftBox, DestroyGiftBox);
        MsgManager.Add(MsgManagerType.SetPlayerMove, SetPlayerMove);
        MsgManager.Add(MsgManagerType.TextScale, TextScale);
        playerModel = ModelManager.GetModel<PlayerModel>();
        gameData = ModelManager.GetModel<GameDataModel>();
    }

    private void OnDestroy()
    {
        MsgManager.Remove(MsgManagerType.RefreshFraction, RefreshFraction);
        MsgManager.Remove(MsgManagerType.TriggerTurnACorner, TriggerTurnACorner);//触发了转弯机制
        MsgManager.Remove(MsgManagerType.TriggerJumpState, TriggerJumpState);
        MsgManager.Remove(MsgManagerType.TriggerAngelSkyState, TriggerAngelSkyState);
        MsgManager.Remove(MsgManagerType.TriggerFallState, TriggerFallState);
        MsgManager.Remove(MsgManagerType.DoorTriggerEnter, DoorTriggerEnter);//限制
        MsgManager.Remove(MsgManagerType.DoorTriggerExit, DoorTriggerExit);//结束限制 结算分数
        MsgManager.Remove(MsgManagerType.TriggerEnterBridge, TriggerEnterBridge);//独木桥
        MsgManager.Remove(MsgManagerType.TriggerDownBridge, TriggerExitBridge);//独木桥
        MsgManager.Remove(MsgManagerType.ShowDamageNumber, ShowDamageNumber);
        MsgManager.Remove(MsgManagerType.TriggerDevilRole, TriggerDevilRole);
        MsgManager.Remove(MsgManagerType.TriggerAngelRole, TriggerAngelRole);
        MsgManager.Remove(MsgManagerType.TriggerQuestionRole, TriggerQuestionRole);
        MsgManager.Remove(MsgManagerType.ChangeSky, ChangeSky);
        MsgManager.Remove(MsgManagerType.LockRefreshScore, LockRefreshScore);
        MsgManager.Remove(MsgManagerType.ShowGiftBoxPanelType, ShowGiftBoxPanelType);
        MsgManager.Remove(MsgManagerType.DestroyGiftBox, DestroyGiftBox);
        MsgManager.Remove(MsgManagerType.SetPlayerMove, SetPlayerMove);
        MsgManager.Remove(MsgManagerType.TextScale, TextScale);
    }
    private bool isPlayerRotate = false;
    private void TextScale(object obj)
    {
        if (playerModel.GetFraction >= 45 && playerModel.isPulsNumber)
        {
            isPlayerRotate = true;
            ani.CrossFadeInFixedTime("A旋转", 0.2f);
            StartCoroutine(WaitRotate());
        }
    }
    IEnumerator WaitRotate()
    {
        yield return new WaitForSeconds(0.5f);
        isPlayerRotate = false;
        GetPao();
    }
    private void SetPlayerMove(object obj)
    {
        if ((bool)obj)
        {
            playerMoveState = PlayerMoveState.正常路面移动;
            GetPao();
        }
        else
        {
            ani.CrossFadeInFixedTime("A等待1", 0.2f);
            playerMoveState = PlayerMoveState.等待;
        }
    }

    public void ShowDamageNumber(object value)
    {
        int num = (int)value;
        if (num > 0)
        {
            damageT.CreateNew(num, transform.position + new Vector3(0, 1, 0) + transform.right);
        }
        else if (num < 0)
        {
            damageE.CreateNew(-num, transform.position + new Vector3(0, 1, 0) - transform.right);
        }
    }

    private void DestroyGiftBox(object obj)
    {
        playerMoveState = PlayerMoveState.正常路面移动;
        GetPao();
    }

    private void ShowGiftBoxPanelType(object obj)
    {
        ani.CrossFadeInFixedTime("A等待1", 0.2f);
        playerMoveState = PlayerMoveState.等待;
    }

    private void LockRefreshScore(object obj)
    {
        if (!(bool)obj)
        {
            playerMoveState = PlayerMoveState.正常路面移动;
            GetPao();
        }
        else
        {
            GetPao();
            playerMoveState = PlayerMoveState.等待;
        }
    }

    private void Update()
    {

        if (!isPlayGame && Input.anyKey && !ManagementTool.IsPointerOverGameObject())//点击任意键 开始游戏
        {
            playerMoveState = PlayerMoveState.正常路面移动;
            isPlayGame = true;
            GetPao();
            CM_Vcam3.SetActive(false);
        }



        if (Input.GetKeyDown(KeyCode.Space))//点击空格重置关卡
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }



        if (isPlayerStop) return;


        InputController();


        switch (playerMoveState)
        {
            case PlayerMoveState.等待:
                movement.Move(Vector3.zero);
                break;
            case PlayerMoveState.正常路面移动:
                正常路面移动();
                break;
            case PlayerMoveState.跳过悬崖:
                跳过悬崖();
                break;
            case PlayerMoveState.行走云朵:
                行走云朵();
                break;
            case PlayerMoveState.上升云朵:
                上升云朵();
                break;
            case PlayerMoveState.必然下落:
                必然下落();
                break;
            case PlayerMoveState.独木桥:
                独木桥();
                break;
            case PlayerMoveState.上楼:
                上楼();
                break;
            case PlayerMoveState.下楼:
                下楼();
                break;
            case PlayerMoveState.楼梯踩空:
                楼梯踩空();
                break;
        }

        if (isEndEA)
        {
            transform.eulerAngles = endEA;
            CM_Vcam1_Follow.eulerAngles = endEA;
        }

        TurnACornerUpdate();

        ChuFaXiaLuo();

        ScopeLimitation();
        RayPool();

        //MagnetWings();

        CM_Vcam1_Follow.position = Vector3.Lerp(CM_Vcam1_Follow.position, transform.position, Time.deltaTime * 10);
    }
    private float CLAMP_DELTA_X = 50;
    /// <summary>
    /// 输入控制器
    /// </summary>
    private void InputController()
    {
        if (Input.GetMouseButtonDown(0))
        {
            nowPosition = Input.mousePosition;
            previousPosition = Input.mousePosition;
            H_Control = 0;
        }
        if (Input.GetMouseButton(0) && (!isEndInputStop || !isTurnACorner))
        {
            if (isLimit)
            {
                nowPosition = Input.mousePosition;
                H_Control = (previousPosition - nowPosition).x;
                previousPosition = nowPosition;
            }
            else
            {
                nowPosition = Input.mousePosition;
                previousPosition = Input.mousePosition;
                H_Control = 0;
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            H_Control = 0;
        }
    }

    #region 计算结束弯道初的位置点
    /// <summary>
    /// 地面层级
    /// </summary>
    public LayerMask groundLayerMask;
    private float h = 0;

    private Vector3 playerForwardPosition = Vector3.zero;
    private Vector3 lookAtForwardPosition = Vector3.zero;
    public CharacterController lookAtTransform;
    float pCD = 0;
    /// <summary>
    /// 正常移动
    /// </summary>
    private void 正常路面移动()
    {
        {
            //////计算最前方的位置
            //Vector3 forwardPosition = CM_Vcam1_Follow.forward * 1f;
            //////计算侧面的的位置
            //float sideFloat = -H_Control * 0.05f;
            //if (sideFloat > 1) sideFloat = 1; else if (sideFloat < -1) sideFloat = -1;
            //Vector3 sidePosition = CM_Vcam1_Follow.right * sideFloat;
            ////看相某个方向
            //float rotatrSpeed = sideFloat * 10;
            //if (Mathf.Abs(rotatrSpeed) == 0) rotatrSpeed = 5;
            //else if (Mathf.Abs(rotatrSpeed) < 0.5) rotatrSpeed = 4;

            //lookAtForwardPosition = Vector3.Lerp(lookAtForwardPosition, sidePosition, Time.deltaTime * Mathf.Abs(rotatrSpeed));
            //playerTrans.LookAt(CM_Vcam1_Follow.position + lookAtForwardPosition + forwardPosition);


            ////计算最前方的位置
            Vector3 forwardPosition = Vector3.forward * 1.8f;
            ////计算侧面的的位置
            float sideFloat = -H_Control * 0.05f;
            if (sideFloat > 1) sideFloat = 1; else if (sideFloat < -1) sideFloat = -1;
            Vector3 sidePosition = Vector3.right * sideFloat;
            //看相某个方向
            float rotatrSpeed = sideFloat * 10;
            if (Mathf.Abs(rotatrSpeed) == 0) rotatrSpeed = 5;
            else if (Mathf.Abs(rotatrSpeed) < 0.5) rotatrSpeed = 4;

            lookAtForwardPosition = Vector3.Lerp(lookAtForwardPosition, sidePosition, Time.deltaTime * Mathf.Abs(rotatrSpeed));
            //playerTrans.LookAt(CM_Vcam1_Follow.position + lookAtForwardPosition + forwardPosition);


            float _angle = Vector3.Angle(forwardPosition, forwardPosition + lookAtForwardPosition) * (lookAtForwardPosition.x >= 0 ? 1 : -1);
            playerTrans.localEulerAngles = new Vector3(0, _angle, 0);
        }
        float a = playerSpeed.mSpeed;
        {
            ////计算最前方的位置
            Vector3 forwardPosition = CM_Vcam1_Follow.forward * playerSpeed.speed;
            ////计算侧面的的位置
            float sideFloat = -H_Control * 0.1f;
            if (sideFloat != 0)
            {
                if (sideFloat < playerSpeed.mSpeed_Min && sideFloat > 0)
                    sideFloat = 0;
                else if (sideFloat > -playerSpeed.mSpeed_Min && sideFloat < 0)
                    sideFloat = 0;
                else if (sideFloat > 15 && sideFloat < playerSpeed.mSpeed_Max)
                {

                    a = playerSpeed.mSpeed + 2;
                }
                else if (sideFloat < -15 && sideFloat > -playerSpeed.mSpeed_Max)
                {
                    a = playerSpeed.mSpeed + 2;
                }
                else if (sideFloat >= playerSpeed.mSpeed_Max)
                {
                    sideFloat = playerSpeed.mSpeed_Max;
                }
                else if (sideFloat <= -playerSpeed.mSpeed_Max)
                {
                    sideFloat = -playerSpeed.mSpeed_Max;
                }
            }

            Vector3 sidePosition = CM_Vcam1_Follow.right * sideFloat * a;
            ////看相某个方向
            playerForwardPosition = sidePosition + forwardPosition;
            playerForwardPosition.y = 0;
        }

        //h = Mathf.MoveTowards(h, -H_Control * 0.1f, Time.deltaTime * 3);
        //lookAtTransform.Move(lookAtTransform.transform.right * -H_Control * Time.deltaTime);

        //if (h == 0 && H_Control == 0)
        //{
        //    //pCD += Time.deltaTime;
        //    //if (pCD >= 0.1)
        //    lookAtTransform.transform.position = Vector3.MoveTowards(lookAtTransform.transform.position
        //        , CM_Vcam1_Follow.position + forwardPosition
        //        , Time.deltaTime * 10);
        //}

        //ScopeLimitationLookAt();
        //Vector3 tempPositioin = lookAtTransform.transform.localPosition;
        //tempPositioin = new Vector3(tempPositioin.x, 0, mSpeed);
        //lookAtTransform.transform.localPosition = tempPositioin;

        if (movement.isGrounded)
        {
            moveDirection = playerForwardPosition;
        }
        moveDirection.y -= gravity * Time.deltaTime;
        movement.Move(moveDirection * Time.deltaTime);

    }
    private Vector3 endEA;
    private bool isEndEA = false;
    public void SetRoadOrigin(Transform roadOrigin, Vector3 vector3)
    {
        this.roadOrigin = roadOrigin;
        isTriggerEnd = false;
        endEA = vector3;
        isEndEA = true;
    }
    //private void ScopeLimitationLookAt()
    //{
    //    if (isLimit && isTriggerEnd)
    //    {
    //        if (isFront)//前后限制
    //        {
    //            //roadOrigin.right * limitFloat

    //            if (lookAtTransform.transform.position.x > roadOrigin.position.x + limitFloat)
    //            {
    //                lookAtTransform.transform.position = new Vector3(roadOrigin.position.x + limitFloat, lookAtTransform.transform.position.y, lookAtTransform.transform.position.z);
    //            }
    //            if (lookAtTransform.transform.position.x < roadOrigin.position.x - limitFloat)
    //            {
    //                lookAtTransform.transform.position = new Vector3(roadOrigin.position.x - limitFloat, lookAtTransform.transform.position.y, lookAtTransform.transform.position.z);
    //            }
    //        }
    //        else//左右限制
    //        {
    //            if (lookAtTransform.transform.position.z > roadOrigin.position.z + limitFloat)
    //            {
    //                lookAtTransform.transform.position = new Vector3(lookAtTransform.transform.position.x, lookAtTransform.transform.position.y, roadOrigin.position.z + limitFloat);
    //            }
    //            if (lookAtTransform.transform.position.z < roadOrigin.position.z - limitFloat)
    //            {
    //                lookAtTransform.transform.position = new Vector3(lookAtTransform.transform.position.x, lookAtTransform.transform.position.y, roadOrigin.position.z - limitFloat);
    //            }
    //        }
    //    }

    //}
    private bool isTriggerEnd = true;
    /// <summary>
    /// 角色左右范围限制 限制在路面范围内
    /// </summary>
    private void ScopeLimitation()
    {
        if (isLimit && isTriggerEnd)
        {
            if (isFront)//前后限制
            {
                //roadOrigin.right * limitFloat

                if (transform.position.x > roadOrigin.position.x + limitFloat)
                {
                    transform.position = new Vector3(roadOrigin.position.x + limitFloat, transform.position.y, transform.position.z);
                }
                if (transform.position.x < roadOrigin.position.x - limitFloat)
                {
                    transform.position = new Vector3(roadOrigin.position.x - limitFloat, transform.position.y, transform.position.z);
                }
            }
            else//左右限制
            {
                if (transform.position.z > roadOrigin.position.z + limitFloat)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y, roadOrigin.position.z + limitFloat);
                }
                if (transform.position.z < roadOrigin.position.z - limitFloat)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y, roadOrigin.position.z - limitFloat);
                }
            }
        }

        mistTransform.position = transform.position + new Vector3(0, -3, 0);
    }

    #endregion



    #region 触发器
    [HideInInspector]
    public bool isTurnACorner = false;
    private Vector3 playerMoveRotation;
    private float playerRotateSpeed;
    private void OnTriggerEnter(Collider other)
    {
        if (isPlayerStop) return;

        //跌落触发结束游戏
        if (other.CompareTag("GameOver"))
        {
            Sequence sequence = DOTween.Sequence();
            sequence.AppendInterval(1f);
            sequence.AppendCallback(() =>
            {
                UIManager.Instance.OpenUI(PanelName.GameOverPanel);
            });
            VibrateClassScript.GetInstancee.Haptic(MoreMountains.NiceVibrations.HapticTypes.Failure);
        }

        //翅膀触发部分
        if (other.CompareTag("angel"))// 小天使翅膀: 增加10分
        {
            playerModel.SetFraction(PlayerModel.FractionType.LittleAngelWings, playerModel.LittleAngelWings);
            Destroy(other.gameObject);
            VibrateClassScript.GetInstancee.Haptic(MoreMountains.NiceVibrations.HapticTypes.LightImpact);
        }
        if (other.CompareTag("d_angel"))// 大天使翅膀: 增加80分
        {
            playerModel.SetFraction(PlayerModel.FractionType.BigAngelWings, playerModel.BigAngelWings);
            Destroy(other.gameObject);
            VibrateClassScript.GetInstancee.Haptic(MoreMountains.NiceVibrations.HapticTypes.LightImpact);
        }
        if (other.CompareTag("devil"))// 小恶魔翅膀: 减少10分
        {
            playerModel.SetFraction(PlayerModel.FractionType.LittleDevilWings, playerModel.LittleDevilWings);
            Destroy(other.gameObject);
            VibrateClassScript.GetInstancee.Haptic(MoreMountains.NiceVibrations.HapticTypes.LightImpact);
        }
        if (other.CompareTag("d_devil"))// 大恶魔翅膀: 减少80分
        {
            playerModel.SetFraction(PlayerModel.FractionType.BigDevilWings, playerModel.BigDevilWings);
            Destroy(other.gameObject);
            VibrateClassScript.GetInstancee.Haptic(MoreMountains.NiceVibrations.HapticTypes.LightImpact);
        }
    }


    #endregion 

    #region 触发转弯机制
    private float flagSpeed;
    /// <summary>
    /// 触发转弯机制
    /// </summary>
    /// <param name="obj"></param>
    private void TriggerTurnACorner(object obj)
    {
        //锁定左右位移
        TurnACorner turnACorner = obj as TurnACorner;
        //锁定位置限制模块
        isLimit = false;
        //更改限制方向
        isFront = !isFront;
        //计算旋转角度 和移动速度
        //speed = 2f * flagSpeed;
        playerMoveRotation = transform.eulerAngles;
        playerRotateSpeed = rotateNumber * playerSpeed.speed;
        switch (turnACorner.turningDirection)
        {
            case TurningDirection.Left:
                playerMoveRotation += new Vector3(0, -90, 0);
                playerRotateSpeed *= GetPlayerPoint(true);
                break;
            case TurningDirection.Right:
                playerMoveRotation += new Vector3(0, 90, 0);
                playerRotateSpeed *= GetPlayerPoint(false);
                break;
            default:
                break;
        }
        roadOrigin = turnACorner.nextRoadOrigin;
        isTurnACorner = true;//开启旋转
    }

    /// <summary>
    /// 转弯Update
    /// </summary>
    private void TurnACornerUpdate()
    {
        if (isTurnACorner)
        {
            //playerMoveState = PlayerMoveState.等待;
            Vector3 tempPlayerRotation = transform.eulerAngles;
            playerRotateSpeed = playerRotateSpeed >= 0 ? 1 : -1;
            tempPlayerRotation.y = Mathf.MoveTowards(tempPlayerRotation.y, playerMoveRotation.y, Time.deltaTime * playerRotateSpeed * 250f);

            Vector3 CM_Vcam1_FollowRotation = CM_Vcam1_Follow.eulerAngles;
            CM_Vcam1_FollowRotation.y = Mathf.MoveTowards(CM_Vcam1_FollowRotation.y, playerMoveRotation.y, Time.deltaTime * playerRotateSpeed * 250f * 1f);

            transform.eulerAngles = tempPlayerRotation;
            //movement.rotation = Quaternion.Euler(tempPlayerRotation);
            CM_Vcam1_Follow.eulerAngles = CM_Vcam1_FollowRotation;
            if (tempPlayerRotation.y == playerMoveRotation.y)
            {
                playerSpeed.speed = flagSpeed;
                isTurnACorner = false;
                isLimit = true;
                //playerMoveState = PlayerMoveState.正常路面移动;
            }
        }
    }
    /// <summary>
    /// 获取角色限制区域的值
    /// </summary>
    /// <param name="isLeft"></param>
    /// <returns></returns>
    private float GetPlayerPoint(bool isLeft)
    {
        float point = 0;
        PlayerDirection playerDirection = (PlayerDirection)transform.eulerAngles.y;
        switch (playerDirection)
        {
            case PlayerDirection.front:
                ReckonPlayerPoint(out point, roadOrigin.position.x, transform.position.x, false);
                break;
            case PlayerDirection.left:
                ReckonPlayerPoint(out point, roadOrigin.position.z, transform.position.z, true);
                break;
            case PlayerDirection.back:
                ReckonPlayerPoint(out point, roadOrigin.position.x, transform.position.x, true);
                break;
            case PlayerDirection.right:
                ReckonPlayerPoint(out point, roadOrigin.position.z, transform.position.z, true);
                break;
        }
        if (isLeft)
            return (limitFloat * 2 - point) / (limitFloat * 2.5f) + 1f;
        else
            return point / (limitFloat * 2.5f) + 1f;
    }

    private void ReckonPlayerPoint(out float point, float originPoint, float playerPoint, bool isPlus)
    {
        float startPoint;
        if (isPlus)
        {
            startPoint = originPoint + limitFloat;
        }
        else
        {
            startPoint = originPoint - limitFloat;
        }
        point = startPoint - playerPoint;
    }

    #endregion

    #region 跳跃机制

    private Vector3 move_OriginPoint;
    private Vector3 move_EndPoint;
    private float jumpTime;
    private void TriggerJumpState(object obj)
    {
        //判断是否可以起飞
        TakeOff takeOff = obj as TakeOff;
        moveState = takeOff.moveState;

        //计算出结束点
        move_EndPoint = -takeOff.transform.forward * 10;
        Ray ray = new Ray(takeOff.transform.position, -takeOff.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayerMask))//检测是否在地面上
        {
            move_EndPoint = hit.point - takeOff.transform.forward;
            Debug.DrawLine(transform.position, hit.point, Color.red);
        }
        move_EndPoint.y = transform.position.y;
        move_OriginPoint = new Vector3(takeOff.transform.position.x, transform.position.y, takeOff.transform.position.z);
        jumpTime = Vector3.Distance(move_EndPoint, takeOff.transform.position) / playerSpeed.speed;

        ani.CrossFadeInFixedTime("A跳起", 0.1f);
        Sequence seq = DOTween.Sequence();

        //判断是否有翅膀可以跳过去
        if (playerModel.GetFraction == 50)
        {
            MsgManager.Invoke(MsgManagerType.DestroyCollider);//云消散
            seq.AppendCallback(() =>
            {
                transform.DOMove(transform.forward * 1.5f + transform.position + new Vector3(0, 2, 0), 0.2f);
            });
            seq.AppendInterval(0.3f);
            seq.AppendCallback(() =>
            {
                playerMoveState = PlayerMoveState.必然下落;
                ani.CrossFadeInFixedTime("A自由落体", 0.25f);
                shangshengYun(true);
            });
            return;
        }

        seq.AppendCallback(() =>
        {
            playerMoveState = PlayerMoveState.跳过悬崖;
        });
        seq.AppendInterval(0.3f);
        seq.AppendCallback(() =>
        {
            ani.CrossFadeInFixedTime("A飞", 0.25f);
        });
    }

    private void 跳过悬崖()
    {
        transform.Translate(Vector3.forward * playerSpeed.speed * 0.025f * jumpTime);
        if (JumpPercentage() >= 0.5f)
        {
            transform.Translate(Vector3.up * playerSpeed.speed * 0.01f * -jumpTime);
            if (transform.position.y < move_OriginPoint.y)
            {
                transform.position = new Vector3(transform.position.x, move_OriginPoint.y, transform.position.z);
            }
        }
        else
        {
            transform.Translate(Vector3.up * playerSpeed.speed * 0.008f * jumpTime);
        }
        if (JumpPercentage() >= 0.9f)
        {
            ani.CrossFadeInFixedTime("A落地", 0.05f);
            playerMoveState = PlayerMoveState.正常路面移动;
            moveState = MoveState.Ground;
        }

        transform.Translate(Vector3.right * H_Control * -0.01f);
    }

    private float JumpPercentage()
    {
        if (isFront)
        {
            float a = Mathf.Abs(move_EndPoint.z - move_OriginPoint.z);
            float b = Mathf.Abs(transform.position.z - move_OriginPoint.z);
            return b / a;
        }
        else
        {
            float a = Mathf.Abs(move_EndPoint.x - move_OriginPoint.x);
            float b = Mathf.Abs(transform.position.x - move_OriginPoint.x);
            return b / a;
        }
    }
    #endregion

    #region 云上行走

    private void TriggerAngelSkyState(object obj)
    {
        //判断是否可以起飞
        TakeOff takeOff = obj as TakeOff;
        moveState = takeOff.moveState;

        //计算出结束点
        move_EndPoint = -takeOff.transform.forward * 10;
        Ray ray = new Ray(takeOff.transform.position, -takeOff.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayerMask))//检测是否在地面上
        {
            move_EndPoint = hit.point - takeOff.transform.forward;
            Debug.DrawLine(transform.position, hit.point, Color.red);
        }
        move_EndPoint.y = transform.position.y;
        move_OriginPoint = new Vector3(takeOff.transform.position.x, transform.position.y, takeOff.transform.position.z);
        jumpTime = Vector3.Distance(move_EndPoint, takeOff.transform.position) / playerSpeed.speed;

        //判断是否有翅膀可以跳过去
        if (playerModel.GetFraction < 50)
        {
            transform.DOMove(transform.forward * 1.5f + transform.position, 0.2f);
            Sequence seq = DOTween.Sequence();
            seq.AppendInterval(0.3f);
            seq.AppendCallback(() =>
            {
                IsTriggerDownMove = true;
            });
            return;
        }
        playerMoveState = PlayerMoveState.行走云朵;
    }

    private void 行走云朵()
    {
        if (JumpPercentage() >= 0.9f)
        {
            playerMoveState = PlayerMoveState.正常路面移动;
            moveState = MoveState.Ground;
        }
        moveDirection = new Vector3(H_Control * -15f, 0, 26f);
        moveDirection = transform.TransformDirection(moveDirection);
        //moveDirection *=playerSpeed.speed;
        //roleController.Move(moveDirection * Time.deltaTime);
        movement.Move(transform.forward * playerSpeed.speed);
    }

    #endregion

    private void TriggerFallState(object obj)
    {
        transform.DOMove(transform.forward * 1.5f + transform.position, 0.2f);
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.3f);
        seq.AppendCallback(() =>
        {
            playerMoveState = PlayerMoveState.必然下落;
            ani.CrossFadeInFixedTime("A自由落体", 0.25f);
            shangshengYun(true);
        });
    }

    private bool isLimits = false;
    private Vector3 limitsVector3;
    /// <summary>
    /// 碰到门 限制移动范围
    /// </summary>
    /// <param name="obj"></param>
    private void DoorTriggerEnter(object obj)
    {
        isLimits = true;
        limitsVector3 = (obj as Transform).position;
    }
    /// <summary>
    /// 离开门 取消限制移动范围
    /// </summary>
    /// <param name="obj"></param>
    private void DoorTriggerExit(object obj)
    {
        isLimits = false;
        DoorBase doorBase = obj as DoorBase;
        DoorData doorData = doorBase.GetDoorData();
        playerModel.SetFraction(PlayerModel.FractionType.DoorNumber, doorData.score);
        switch (doorData.situationalState)
        {
            case SituationalState.暴食:
                暴食(doorBase.getRewards, doorData.score >= 0);
                break;
            case SituationalState.色欲:
                色欲(doorBase.getRewards, doorData.score);
                break;
            case SituationalState.贪念:
                贪念(doorBase.getRewards, doorData.score >= 0);
                break;
            case SituationalState.懒惰:
                懒惰(doorData.score >= 0);
                break;
        }
    }

    #region
    private bool is暴食 = false;
    private void 暴食(List<Transform> 交互物体, bool flag)
    {
        if (is暴食) return;
        is暴食 = true;
        //食物飞向玩家
        foreach (var item in 交互物体)
        {
            item.gameObject.AddComponent<RewardItemsScript>().Init(transform, Resources.Load<GameObject>("Eat_FX"), playerSpeed.speed);
            if (flag) return;
        }
    }
    private bool is色欲 = false;
    private void 色欲(List<Transform> 交互物体, int score)
    {
        if (is色欲) return;
        is色欲 = true;
        if (交互物体.Count == 0 || score >= 0) return;
        //玩家停止运动
        playerMoveState = PlayerMoveState.等待;

        isCameraMove = false;

        //女人走向晚间
        Animator woman = 交互物体[0].GetComponent<Animator>();
        woman.CrossFadeInFixedTime("A走", 0.2f);
        Sequence seq = DOTween.Sequence();
        seq.AppendCallback(() =>
        {
            Vector3 pos = transform.position;//+ 交互物体[0].forward * 1.5f;
            交互物体[0].DOMove(pos + new Vector3(1.4f, 0, 0), 0.7f);
            //transform.DOMove(pos + new Vector3(-0.7f, 0, 0), 0.7f);
        });
        seq.AppendInterval(0.5f);
        seq.AppendCallback(() =>
        {
            交互物体[0].DORotate(new Vector3(0, -90, 0), 0.2f);
            transform.DORotate(new Vector3(0, 90, 0), 0.2f);
            woman.CrossFadeInFixedTime("A女人接吻", 0.05f);
            ani.CrossFadeInFixedTime("A男人接吻", 0.05f);
        });
        seq.AppendInterval(0.5f);
        seq.AppendCallback(() =>
        {
            GameObject obj = Instantiate(Resources.Load<GameObject>("Heart_FX"), transform.position + transform.forward * 0.7f + new Vector3(0, 3f, 0), Quaternion.identity);
            obj.transform.localScale = Vector3.one * 2.5f;
            Destroy(obj, 2f);
        });
        seq.AppendInterval(3.5f);
        seq.AppendCallback(() =>
        {
            playerMoveState = PlayerMoveState.正常路面移动;
            //woman.CrossFadeInFixedTime("A等待1", 0.2f);
            GetPao();
            transform.DOLocalRotate(new Vector3(0, 180, 0), 0.2f);
        });
        seq.AppendInterval(0.2f);
        seq.AppendCallback(() =>
        {
            isCameraMove = true;
        });
    }
    private bool is贪念 = false;
    private void 贪念(List<Transform> 交互物体, bool flag)
    {
        //钱飞向玩家
        if (is贪念) return;
        is贪念 = true;
        //食物飞向玩家
        foreach (var item in 交互物体)
        {
            item.gameObject.AddComponent<RewardItemsScript>().Init(transform, Resources.Load<GameObject>("Money_FX"), playerSpeed.speed);
            if (flag) return;
        }
    }

    private void 懒惰(bool v)
    {
        //动画勤劳
        if (!v)
        {
            ani.CrossFadeInFixedTime("A躺平", 0.25f);
            Sequence seq = DOTween.Sequence();
            seq.AppendInterval(3f);
            seq.AppendCallback(() =>
            {
                GetPao();
            });
        }
    }
    #endregion

    private float downSpeed = 5;
    private void 上升云朵() { }
    private void 必然下落()
    {
        //moveDirection = new Vector3(0, 0, 0f);
        //moveDirection = transform.TransformDirection(moveDirection);
        //moveDirection *=playerSpeed.speed;

        //downSpeed = Mathf.MoveTowards(downSpeed, 20, Time.deltaTime * 4);
        //moveDirection.y -= gravity * downSpeed * Time.deltaTime;

        //  movement.Move(transform.forward,playerSpeed.speed);
        //roleController.Move(moveDirection * Time.deltaTime);
        movement.Move(transform.up * playerSpeed.speed);

        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1f, groundLayerMask))//检测是否在地面上
        {
            downSpeed = 5;
            playerMoveState = PlayerMoveState.等待;
            ani.CrossFadeInFixedTime("A落地", 0.25f);
            Sequence seq = DOTween.Sequence();
            seq.AppendInterval(0.3f);
            seq.AppendCallback(() =>
            {
                playerMoveState = PlayerMoveState.正常路面移动;
                shangshengYun(false);
                moveState = MoveState.Ground;
            });
            Debug.DrawLine(transform.position, hit.point, Color.red);
        }
    }





    private void TriggerEnterBridge(object obj)
    {
        //变换为走独木桥
        playerMoveState = PlayerMoveState.独木桥;
        ani.CrossFadeInFixedTime("A走独木桥", 0.25f);
    }

    private void TriggerExitBridge(object obj)
    {
        if (playerMoveState == PlayerMoveState.独木桥)
        {
            playerMoveState = PlayerMoveState.正常路面移动;
            GetPao();
            playerTrans.DOLocalRotate(new Vector3(0, 0, 0), 0.1f);
        }
    }

    private void 独木桥()
    {
        //检测脚下是否有路
        moveDirection = new Vector3(0, 0, 0.65f);
        moveDirection = transform.TransformDirection(moveDirection);
        moveDirection *= playerSpeed.speed;


        //H_Control * -0.1f
        //Transform playerTrans = playerTrans;

        Ray ray = new Ray(transform.position + new Vector3(0, 1, 0), -transform.up);
        RaycastHit hit;
        if (!Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayerMask))//检测是否在地面上
        {
            playerMoveState = PlayerMoveState.等待;
            Sequence seq = DOTween.Sequence();
            seq.Append(transform.DOMove(transform.forward * 1.3f + transform.position, 0.3f));
            seq.AppendCallback(() =>
            {
                ani.CrossFadeInFixedTime("A自由落体", 0.25f);
                playerMoveState = PlayerMoveState.必然下落;
                shangshengYun(true);
            });

            playerTrans.DOLocalRotate(new Vector3(0, 0, 0), 0.2f);
            MsgManager.Invoke(MsgManagerType.TriggerDownBridge);
        }
        playerTrans.Rotate(Vector3.forward * H_Control * 30 * Time.deltaTime);
        if (Vector3.Angle(transform.up, playerTrans.up) >= 60)
        {
            playerMoveState = PlayerMoveState.等待;
            Sequence seq = DOTween.Sequence();
            seq.Append(transform.DOMove(transform.forward * 1.3f + transform.position, 0.3f));
            seq.AppendCallback(() =>
            {
                playerMoveState = PlayerMoveState.必然下落;
                ani.CrossFadeInFixedTime("A自由落体", 0.25f);
                shangshengYun(true);
            });
            MsgManager.Invoke(MsgManagerType.TriggerDownBridge);

        }
        movement.Move(transform.forward);

        //roleController.Move(moveDirection * Time.deltaTime);
    }





    #region  射线检测水池 上楼 下楼
    public GameObject CM_Vcam1;
    public GameObject CM_Vcam2;
    public GameObject CM_Vcam3;
    public GameObject CM_Vcam4;
    public Transform CM_Vcam1_Follow;

    private Vector3 upStairsPosition, downStairsPosition, endPosition;
    private bool isMakeMisstep = false;
    /// <summary>
    /// 跑楼梯 台阶
    /// </summary>
    /// <param name="upPosition">向上的位置</param>
    /// <param name="downPosition">向下的位置</param>
    /// <param name="value">是否大于等50分 天使 </param>
    public void SetClimbStairs(Vector3 upPosition, Vector3 downPosition, bool value)
    {
        upStairsPosition = upPosition;
        downStairsPosition = downPosition;
        endPosition = transform.position;
        isMakeMisstep = value;
        playerMoveState = PlayerMoveState.上楼;
        movement.Move(Vector3.zero);
        MsgManager.Invoke(MsgManagerType.ChangeSky, true);
        if (!value)
        {
            Sequence seq = DOTween.Sequence();
            seq.AppendInterval(0.1f);
            seq.AppendCallback(() =>
            {
                MsgManager.Invoke(MsgManagerType.StepMove);
            });
            seq.AppendInterval(0.3f);
            seq.AppendCallback(() =>
            {
                gravity *= 10f;
                ani.CrossFadeInFixedTime("A自由落体", 0.25f);
                shangshengYun(true);
                playerMoveState = PlayerMoveState.楼梯踩空;
                MsgManager.Invoke(MsgManagerType.ChangeSky, false);
            });
        }
    }

    public void 上楼()
    {
        transform.position = Vector3.MoveTowards(transform.position, upStairsPosition, Time.deltaTime * playerSpeed.speed * 2);
        if (Vector3.Distance(transform.position, upStairsPosition) == 0)
        {
            playerMoveState = PlayerMoveState.等待;
            if (playerModel.GetFraction > 99)
            {
                isCameraMove = false;
                CM_Vcam2.transform.position = CM_Vcam1.transform.position;
                CM_Vcam2.gameObject.SetActive(true);
                MsgManager.Invoke(MsgManagerType.GameEndReward_Lineage, transform);
            }
            else
            {
                ani.CrossFadeInFixedTime("A等待1", 0.25f);
                StartCoroutine(GameWinsWait());
            }
        }
        CalculateEndDistance();
    }
    public void 下楼()
    {
        transform.position = Vector3.MoveTowards(transform.position, downStairsPosition, Time.deltaTime * playerSpeed.speed * 2);
        if (Vector3.Distance(transform.position, downStairsPosition) == 0)
        {
            playerMoveState = PlayerMoveState.等待;
            if (playerModel.GetFraction < 1)
            {
                isCameraMove = false;
                CM_Vcam2.transform.position = CM_Vcam1.transform.position;
                CM_Vcam2.gameObject.SetActive(true);
                MsgManager.Invoke(MsgManagerType.GameEndReward_Infernal, transform);//地狱
            }
            else
            {
                ani.CrossFadeInFixedTime("A等待1", 0.25f);
                StartCoroutine(GameWinsWait());
            }
        }
        CalculateEndDistance();
    }
    IEnumerator GameWinsWait()
    {
        yield return new WaitForSeconds(4f);
        UIManager.Instance.OpenUI(PanelName.GameWinsPanel);
    }
    /// <summary>
    /// 计算终点距离
    /// </summary>
    private void CalculateEndDistance()
    {
        float num = 1;
        if (isMakeMisstep)
        {
            float a = Mathf.Abs(transform.position.z) - Mathf.Abs(endPosition.z);
            float b = Mathf.Abs(upStairsPosition.z) - Mathf.Abs(endPosition.z);
            num = 1 - a / b;
            MsgManager.Invoke(MsgManagerType.AngelEndDistance, num);
        }
        else
        {
            float a = Mathf.Abs(transform.position.z) - Mathf.Abs(endPosition.z);
            float b = Mathf.Abs(downStairsPosition.z) - Mathf.Abs(endPosition.z);
            num = 1 - a / b;
            MsgManager.Invoke(MsgManagerType.DevilEndDistance, num);
        }
    }

    private Vector3 DLAC_3;
    private float DL_CD = 0;
    private float DL_CD_Temp = 0;
    public void 楼梯踩空()
    {
        //if (DLV3 == transform.position)
        //    DL_CD += Time.deltaTime;
        //else DL_CD = 0;
        //DLV3 = transform.position;
        ////moveDirection = new Vector3(0, 0, 0);
        ////moveDirection = transform.TransformDirection(moveDirection);
        ////moveDirection *=playerSpeed.speed;
        ////moveDirection.y -= gravity * 1 * Time.deltaTime;
        ////roleController.Move(moveDirection * Time.deltaTime);

        ////if (roleController.isGrounded)
        //DL_CD_Temp += Time.deltaTime;
        //if (DL_CD >= 0.2f || DL_CD_Temp >= 2f)
        //{
        //    DL_CD = 0;
        //    DL_CD_Temp = 0;
        //    playerMoveState = PlayerMoveState.等待;
        //    
        //    Sequence seq = DOTween.Sequence();
        //    seq.AppendInterval(0.25f);
        //    seq.AppendCallback(() =>
        //    {
        //        movement.Move(Vector3.zero,playerSpeed.speed);
        //        playerMoveState = PlayerMoveState.下楼;
        //    });
        //}

        movement.Move(transform.up * playerSpeed.speed);
        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 0.5f, groundLayerMask))//检测是否在地面上
        {
            playerMoveState = PlayerMoveState.等待;
            GetPao();
            Sequence seq = DOTween.Sequence();
            seq.AppendInterval(0.25f);
            seq.AppendCallback(() =>
            {
                movement.Move(Vector3.zero);
                playerMoveState = PlayerMoveState.下楼;
            });
        }
    }
    private bool isPlayerStop = false;
    public void PlayerStopMove()
    {
        playerMoveState = PlayerMoveState.等待;
        //isPlayerStop = true;
    }
    public void PlayerPlayMove()
    {
        playerMoveState = PlayerMoveState.正常路面移动;
        isEndInputStop = true;
        //isPlayerStop = false;
    }
    public LayerMask poolLM;
    private float poolCD = 0;
    private bool isPoolSlowDown = false;
    /// <summary>
    /// 射线检测水池
    /// </summary>
    private void RayPool()
    {
        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 15, poolLM))
        {
            poolCD += Time.deltaTime;
            if (!isPoolSlowDown)
            {
                playerSpeed.speed /= 1.2f;
                isPoolSlowDown = true;
            }
            Debug.DrawLine(transform.position, hit.point, Color.red);
            if (poolCD >= 0.1f)
            {
                poolCD = 0;
                if (hit.transform.CompareTag("angel"))
                {
                    playerModel.SetFraction(PlayerModel.FractionType.PoolNumber, playerModel.PoolNumber);
                }
                else if (hit.transform.CompareTag("devil"))
                {
                    playerModel.SetFraction(PlayerModel.FractionType.PoolNumber, -playerModel.PoolNumber);
                }
                VibrateClassScript.GetInstancee.Haptic(MoreMountains.NiceVibrations.HapticTypes.LightImpact);
            }
        }
        else
        {
            poolCD = 0;
            if (isPoolSlowDown)
            {
                playerSpeed.speed *= 1.2f;
                isPoolSlowDown = false;
            }
            {
                //speed = playerModel.speed;
            }
        }
    }

    #endregion


    #region 相机跟随部分
    public Transform lightT;
    private bool isCameraMove = true;
    /// <summary>
    /// 相机跟随部分
    /// </summary>
    private void LateUpdate()
    {
        cameraFollowPoint.position = Vector3.Lerp(cameraFollowPoint.position, transform.position, Time.deltaTime * playerSpeed.speed); ;
        if (isCameraMove)
        {
            cameraFollowPoint.rotation = transform.rotation;
            lightT.rotation = transform.rotation;
            //lightT.eulerAngles = new Vector3(lightT.eulerAngles.x, transform.eulerAngles.x + 180, lightT.eulerAngles.z);
        }
    }
    #endregion


    #region 分数操控部分

    [Header("分数操控部分")]
    /// <summary>
    /// 天使特效
    /// </summary>
    public ParticleSystem angelFX;
    /// <summary>
    /// 恶魔特效
    /// </summary>
    public ParticleSystem devilFX;
    public ParticleSystem angelFX_Up;
    public ParticleSystem devilFX_Up;
    [Tooltip("天使翅膀")]
    public List<PlayerWingObject> angelWingObject = new List<PlayerWingObject>();
    [Tooltip("恶魔翅膀")]
    public List<PlayerWingObject> devilWingObject = new List<PlayerWingObject>();
    [Tooltip("变色")]
    public List<SkinnedMeshRenderer> playerMtr = new List<SkinnedMeshRenderer>();
    /// <summary>
    /// 刷新分数时
    /// </summary>
    /// <param name="obj"></param>
    private void RefreshFraction(object obj = null)
    {
        //根据分数刷新翅膀
        if (playerModel.GetFraction >= 100)
        {
            // ANGEL= 100分, 头顶光环, 42ff00
            ShowProp(2, -1, "#42ff00", "ANGEL");
        }
        if (playerModel.GetFraction >= 80 && playerModel.GetFraction < 100)
        {
            // HALF-ANGEL, 80~99分, 两对翅膀, 90ff00
            ShowProp(1, -1, "#90ff00", "HALF-ANGEL");
        }
        if (playerModel.GetFraction > 55 && playerModel.GetFraction < 80)
        {
            // KINDNESS, 50~65分, 一对翅膀, ffba00
            ShowProp(0, -1, "#ffba00", "KINDNESS");
        }
        if (playerModel.GetFraction >= 45 && playerModel.GetFraction <= 55)
        {
            //  HUMAN=50分, 无任何装扮, ff7800
            ShowProp(-1, -1, "#ff7800", "HUMAN");
        }
        if (playerModel.GetFraction > 20 && playerModel.GetFraction < 45)
        {
            // BAD GUY, 50~35分, 一对翅膀, ff4800
            ShowProp(-1, 0, "#ff4800", "BAD GUY");
        }
        if (playerModel.GetFraction > 0 && playerModel.GetFraction <= 20)
        {
            // HALF-DEMON2, 35~20分, 一条尾巴, ff0000
            ShowProp(-1, 1, "#ff0000", "HALF-DEMON");
        }
        if (playerModel.GetFraction <= 0)
        {
            // DEMON＜20分, 一对犄角, e00000
            ShowProp(-1, 2, "#e00000", "DEMON");
        }
        GetPao(true);
        ani.SetBool("Angel", playerModel.GetFraction >= 100);
        ani.SetBool("Devil", playerModel.GetFraction <= 0);
    }
    private void MagnetWings()
    {
        if (playerModel.GetFraction >= 100)
        {
            for (int i = mainGameSceneScript.gsdManager.wings.Count - 1; i >= 0; i--)
            {
                GameObject item = mainGameSceneScript.gsdManager.wings[i];
                if (item)
                {
                    if (item.CompareTag("angel"))
                    {
                        Transform t = item.transform;
                        if (Vector3.Distance(t.position, transform.position) <= 10f &&
                           Vector3.Angle(transform.forward, t.position - transform.position) <= 100f)
                        {
                            item.AddComponent<AttractiveForce>().Init(transform);
                            mainGameSceneScript.gsdManager.wings.RemoveAt(i);
                        }
                    }
                }
                else
                {
                    mainGameSceneScript.gsdManager.wings.RemoveAt(i);
                }
            }
        }
        else if (playerModel.GetFraction <= 0)
        {
            for (int i = mainGameSceneScript.gsdManager.wings.Count - 1; i >= 0; i--)
            {
                GameObject item = mainGameSceneScript.gsdManager.wings[i];
                if (item)
                {
                    if (item.CompareTag("devil"))
                    {
                        Transform t = item.transform;
                        if (Vector3.Distance(t.position, transform.position) <= 10f &&
                           Vector3.Angle(transform.forward, t.position - transform.position) <= 100f)
                        {
                            item.AddComponent<AttractiveForce>().Init(transform);
                            mainGameSceneScript.gsdManager.wings.RemoveAt(i);
                        }
                    }
                }
                else
                {
                    mainGameSceneScript.gsdManager.wings.RemoveAt(i);
                }
            }
        }
    }
    private void ShowProp(int angelIndex, int devilIndex, string colorNum, string textStr)
    {
        for (int i = 0; i < angelWingObject.Count; i++)
        {
            if (i <= angelIndex)
            {
                if (angelWingObject[i].IsActive) continue;
                angelWingObject[i].SetActive(true, true);
                angelFX.gameObject.SetActive(true);
                angelFX.Play();

                Color nowColor;
                ColorUtility.TryParseHtmlString("#9F7700", out nowColor);
                foreach (var item in playerMtr)
                {
                    //Material[] m = item.sharedMaterials;
                    for (int k = 0; k < item.materials.Length; k++)
                    {
                        item.materials[k].SetColor("_Color", nowColor);
                        item.materials[k].SetFloat("_Yellow", 1);
                    }
                    //item.sharedMaterials = m;
                }
                Sequence s = DOTween.Sequence();
                s.AppendInterval(0.5f);
                s.AppendCallback(() =>
                {
                    ColorUtility.TryParseHtmlString("#FF0000", out nowColor);
                    foreach (var item in playerMtr)
                    {
                        for (int k = 0; k < item.materials.Length; k++)
                        {
                            item.materials[k].SetColor("_Color", nowColor);
                            item.materials[k].SetFloat("_Yellow", 0);
                        }
                    }
                });
            }
            else
            {
                angelWingObject[i].SetActive(false, true);
            }
        }
        for (int i = 0; i < devilWingObject.Count; i++)
        {
            if (i <= devilIndex)
            {
                if (devilWingObject[i].IsActive) continue;
                devilWingObject[i].SetActive(true, true);
                devilFX.gameObject.SetActive(true);
                devilFX.Play();

                Color nowColor;
                ColorUtility.TryParseHtmlString("#FF0000", out nowColor);
                foreach (var item in playerMtr)
                {
                    for (int k = 0; k < item.materials.Length; k++)
                    {
                        item.materials[k].SetColor("_Color", nowColor);
                        item.materials[k].SetFloat("_Yellow", 1);
                    }
                }
                Sequence s = DOTween.Sequence();
                s.AppendInterval(0.5f);
                s.AppendCallback(() =>
                {
                    ColorUtility.TryParseHtmlString("#9F7700", out nowColor);
                    foreach (var item in playerMtr)
                    {
                        for (int k = 0; k < item.materials.Length; k++)
                        {
                            item.materials[k].SetColor("_Color", nowColor);
                            item.materials[k].SetFloat("_Yellow", 0);
                        }
                    }
                });
            }
            else
            {
                devilWingObject[i].SetActive(false, true);
            }
        }
        if (playerModel.GetFraction >= 100)
        {
            angelFX_Up.gameObject.SetActive(true);
            angelFX_Up.Play();
        }
        else angelFX_Up.Stop();
        if (playerModel.GetFraction <= 0)
        {
            devilFX_Up.gameObject.SetActive(true);
            devilFX_Up.Play();
        }
        else devilFX_Up.Stop();
        MsgManager.Invoke(MsgManagerType.RefreshHearSlider, colorNum + "|" + textStr);
        UpdateRoleStatus();
    }
    /// <summary>
    /// 分数判断时使用
    /// </summary>
    /// <returns></returns>
    private bool UpdateRoleStatus()
    {
        switch (moveState)
        {
            case MoveState.Ground:
                return true;
            case MoveState.Jump:
                if (playerModel.GetFraction == 50)//只允许飞过去
                {
                    GameOver();
                    //speed = 1;
                    return true;
                }
                else return false;
            case MoveState.AngelSky:
                if (playerModel.GetFraction < 50)//只允许天使飞过去
                {
                    GameOver();
                    return true;
                }
                else return false;
            case MoveState.Fall:
                GameOver();
                gravity *= 1.2f;
                return true;
        }
        return true;
    }
    //private bool isGameOver = false;
    private void GameOver()
    {
        //speed /= 2;
        ani.CrossFadeInFixedTime("A自由落体", 0.25f);
        shangshengYun(true);
        playerMoveState = PlayerMoveState.必然下落;
        MsgManager.Invoke(MsgManagerType.DestroyCollider);//云消散
    }
    #endregion
    public void shangshengYun(bool isUp)
    {
        CM_Vcam1.SetActive(!isUp);
        CM_Vcam4.SetActive(isUp);
    }

    private bool isTriggerDownMove = false;
    private bool IsTriggerDownMove
    {
        get { return IsTriggerDownMove; }
        set
        {
            isTriggerDownMove = value;
            if (isTriggerDownMove)
            {
                List<ParticleSystem> MoveClouds = mainGameSceneScript.gsdManager.MoveClouds;
                int index = 0;
                for (int i = 0; i < MoveClouds.Count; i++)
                {
                    float a = Vector3.Distance(MoveClouds[index].transform.position, transform.position);
                    float b = Vector3.Distance(MoveClouds[i].transform.position, transform.position);
                    if (a > b)
                    {
                        index = i;
                    }
                }
                MoveClouds[index].Stop();
            }
        }
    }
    private float downMoveCD = 0;
    private void ChuFaXiaLuo()
    {
        if (!isTriggerDownMove) return;
        downMoveCD += Time.deltaTime;
        if (downMoveCD >= 0.4f)
        {
            Ray ray = new Ray(transform.position, -transform.up);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1f, groundLayerMask) || playerModel.GetFraction >= 50)//检测是否在地面上
            {
                isTriggerDownMove = false;
                downMoveCD = 0;
                playerMoveState = PlayerMoveState.行走云朵;
                shangshengYun(false);
                moveState = MoveState.Ground;
            }
            else
            {
                MsgManager.Invoke(MsgManagerType.DestroyCollider);//云消散
                isTriggerDownMove = false;
                downMoveCD = 0;
                playerMoveState = PlayerMoveState.必然下落;
                ani.CrossFadeInFixedTime("A自由落体", 0.25f);
                shangshengYun(true);
                moveState = MoveState.Ground;
            }
        }
    }


    private void TriggerDevilRole(object obj)
    {
        playerMoveState = PlayerMoveState.等待;
        ani.CrossFadeInFixedTime("hit", 0.2f);
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.5f);
        seq.AppendCallback(() =>
        {
            playerMoveState = PlayerMoveState.正常路面移动;
            GetPao();
        });
        MsgManager.Invoke(MsgManagerType.FORGIVENESS_PUNISHMENT, false);
        playerModel.SetFraction(PlayerModel.FractionType.DevilRoleNumber, playerModel.DevilRoleNumber);
    }

    private void TriggerAngelRole(object obj)
    {
        playerMoveState = PlayerMoveState.等待;
        ani.CrossFadeInFixedTime("A欢呼", 0.2f);
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(1f);
        seq.AppendCallback(() =>
        {
            playerMoveState = PlayerMoveState.正常路面移动;
            GetPao();
        });
        MsgManager.Invoke(MsgManagerType.FORGIVENESS_PUNISHMENT, true);
        playerModel.SetFraction(PlayerModel.FractionType.AngelRoleNumber, playerModel.AngelRoleNumber);
    }
    /// <summary>
    /// 随机？
    /// </summary>
    /// <param name="obj"></param>
    private void TriggerQuestionRole(object obj)
    {
        int num = UnityEngine.Random.Range(1, 16);
        if (UnityEngine.Random.Range(0, 2) == 1)//随机到天使
        {
            playerMoveState = PlayerMoveState.等待;
            ani.CrossFadeInFixedTime("A欢呼", 0.2f);
            Sequence seq = DOTween.Sequence();
            seq.AppendInterval(1f);
            seq.AppendCallback(() =>
            {
                playerMoveState = PlayerMoveState.正常路面移动;
                GetPao();
            });
            MsgManager.Invoke(MsgManagerType.FORGIVENESS_PUNISHMENT, true);
        }
        else//随机到恶魔
        {
            playerMoveState = PlayerMoveState.等待;
            ani.CrossFadeInFixedTime("hit", 0.2f);
            Sequence seq = DOTween.Sequence();
            seq.AppendInterval(0.5f);
            seq.AppendCallback(() =>
            {
                playerMoveState = PlayerMoveState.正常路面移动;
                GetPao();
            });
            num = -num;
            MsgManager.Invoke(MsgManagerType.FORGIVENESS_PUNISHMENT, false);
        }
        playerModel.SetFraction(PlayerModel.FractionType.QuestionRoleNumber, num, num);
    }

    private void ChangeSky(object obj)
    {
        mistTransform.gameObject.SetActive((bool)obj);
    }

}
public enum PlayerMoveState
{
    等待,
    正常路面移动,
    跳过悬崖,
    行走云朵,
    上升云朵,
    必然下落,
    独木桥,
    上楼,
    下楼,
    楼梯踩空
}
