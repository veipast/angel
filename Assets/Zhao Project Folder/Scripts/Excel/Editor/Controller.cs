//using CrowdRun;
//using MyUtilites;
//using Modules.Saves;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class Controller : MonoBehaviour
//{
//   public static bool BlockingMoveSite { get; set; }
//   public static Vector3 ClampBounds;
//   public float SpeedZ => _speedZ;

//   private const float DELAY_RUN_AFTER_FIGHT = .17f;
//   private const float DELAY_DANCE_AFTER_KILL_BOSS = 2f;
//   private const float CLAMP_DELTA_X = 6000f;

//   [SerializeField] private float _senceX;
//   [SerializeField] private float _speedZ;
//   [SerializeField] private Crowd _crowd;
//   [SerializeField] private AnimationCurve _coeffCountHumanSence;
//   [SerializeField] private Vector3 _clampBounds;

//   private Action _updater;
//   private Vector3 _lastMousePos;


//   public void MultiplySpeedZ (float newSpeed)
//   {
//      _speedZ *= newSpeed;
//   }

//   public void NewSpeedZ(float newSpeed)
//   {
//      _speedZ = newSpeed;
//   }

//   private void Awake()
//   {
//      ClampBounds = _clampBounds;
//      BlockingMoveSite = false;
//      OnUpdateVariables(E.NOTHING);
//   }

//   private void Start()
//   {
//      _crowd.SetupLeader();
//   }

//   private void Update()
//   {
//#if (UNITY_EDITOR || UNITY_STANDALONE)
//      if (Input.GetMouseButtonDown(0))
//      {
//         _lastMousePos = Input.mousePosition;
//      }

//      _updater?.Invoke();
//#else 
//      if (Input.touches.Length > 0 && Input.touches[0].phase == TouchPhase.Began)
//      {
//         _lastMousePos = Input.touches[0].position;
//      }
//      _updater?.Invoke();
//#endif
//   }

//   private void HandlerMouse()
//   {
//#if (UNITY_EDITOR || UNITY_STANDALONE)
//      if (Input.GetMouseButton(0) && !BlockingMoveSite)
//      {
//         Vector3 delta = Input.mousePosition - _lastMousePos;
         
//         if ( delta.x >= 0 )
//         {
            
//            delta.x = Mathf.Clamp(delta.x, 0, CLAMP_DELTA_X * Time.deltaTime);
//         } else
//         {
//            delta.x = Mathf.Clamp(delta.x, -CLAMP_DELTA_X * Time.deltaTime, 0);
//         }
//         Vector3 newPos = new Vector3(transform.position.x + delta.x * _senceX * Time.deltaTime, transform.position.y, transform.position.z);

//         if (BoundsControllerChecker.left && delta.x < 0 && BoundsControllerChecker.left.transform.position.x <= -_clampBounds.x
//            || BoundsControllerChecker.right && delta.x > 0 && BoundsControllerChecker.right.transform.position.x >= _clampBounds.x)
//         {
//            _lastMousePos = Input.mousePosition;
//            return;
//         }

//         transform.position = new Vector3(Mathf.Clamp(newPos.x, -_clampBounds.x, _clampBounds.x), newPos.y, newPos.z);
//         _lastMousePos = Input.mousePosition;
//      }
//#else
//      if (Input.touches.Length > 0 && !BlockingMoveSite)
//      {
//         Vector3 delta = ((Vector3)Input.touches[0].position) - _lastMousePos;
//         if ( delta.x >= 0 )
//         {
//            delta.x = Mathf.Clamp(delta.x, 0, CLAMP_DELTA_X * Time.deltaTime);
//         } else
//         {
//            delta.x = Mathf.Clamp(delta.x, -CLAMP_DELTA_X * Time.deltaTime, 0);
//         }
//         Vector3 newPos = new Vector3(transform.position.x + delta.x * _senceX * Time.deltaTime, transform.position.y, transform.position.z);

//         if (BoundsControllerChecker.left && delta.x < 0 && BoundsControllerChecker.left.transform.position.x <= -_clampBounds.x
//            || BoundsControllerChecker.right && delta.x > 0 && BoundsControllerChecker.right.transform.position.x >= _clampBounds.x)
//         {
//            _lastMousePos = Input.touches[0].position;
//            return;
//         }

//         transform.position = new Vector3(Mathf.Clamp(newPos.x, -_clampBounds.x, _clampBounds.x), newPos.y, newPos.z);
//         _lastMousePos = Input.touches[0].position;
//      }
//#endif
//   }

//   private Vector3 ClampedLimeter(Vector3 targetPosition)
//   {
//      if (Limiter.TouchLeftLimiter && targetPosition.x < transform.position.x)
//      {
//         return new Vector3(transform.position.x, targetPosition.y, targetPosition.z);
//      }
//      else if (Limiter.TouchRightLimiter && targetPosition.x > transform.position.x)
//      {
//         return new Vector3(transform.position.x, targetPosition.y, targetPosition.z);
//      }

//      return targetPosition;
//   }

//   private void MoveForward()
//   {
//      transform.position += Vector3.forward * _speedZ * Time.deltaTime;
//   }

//   private void OnTapToPlay(object _)
//   {
//      EventManager.EmitEvent(GameEventType.INIT_CAMERA_TARGET_SMOOTH_DAMP, transform);//初始化相机target;
//#if (UNITY_EDITOR || UNITY_STANDALONE)
//      if (Input.GetMouseButton(0))
//      {
//         _lastMousePos = Input.mousePosition;
//      }
//#else
//      if (Input.touches.Length > 0)
//      {
//         _lastMousePos = Input.touches[0].position;
//      }
//#endif
//      _crowd.Run();
//      _updater += MoveForward;
//      _updater += HandlerMouse;
//   }

//   private void StopCrowd(object _)
//   {
//      _crowd.Idle();
//      _updater -= MoveForward;
//      _updater -= HandlerMouse;
//   }

//   private void OnDestroyAllEnemyInCrowd(object _)
//   {
//#if (UNITY_EDITOR || UNITY_STANDALONE)
//      if (Input.GetMouseButton(0))
//      {
//         _lastMousePos = Input.mousePosition;
//      }
//#else
//      if (Input.touches.Length > 0)
//      {
//         _lastMousePos = Input.touches[0].position;
//      }
//#endif
//      StartCoroutine(DelayDestroyAllEnemyInCrowd());
//   }

//   private IEnumerator DelayDestroyAllEnemyInCrowd()
//   {
//      yield return new WaitForSeconds(DELAY_RUN_AFTER_FIGHT);
//      _crowd.Run();
//      _updater += MoveForward;
//      _updater += HandlerMouse;
//      _crowd.AllEnemyInCrowdDestroy();
//   }

//   private void OnUpdateVariables(object _)
//   {
//      _senceX = Saves.MouseSenceX.Value;
//      _speedZ = Saves.MoveSpeedZ.Value;
//   }

//   private void SelfDisabel(object _)
//   {
//      _updater -= HandlerMouse;
//   }

//   private void OnWin(object _)
//   {
//      _updater = null;
//      _crowd.Idle();
//   }

//   private void OnKillBoss(object _)
//   {
//      _updater = null;

//      StartCoroutine(DelayDance());

//      IEnumerator DelayDance()
//      {
//         yield return new WaitForSeconds(DELAY_DANCE_AFTER_KILL_BOSS);
//         _crowd.BreakTargetMove();
//         _crowd.Dance();
//      }
//   }

//   private void OnEnable()
//   {
//      EventManager.StartListening(GameEventType.TAP_TO_PLAY, OnTapToPlay);
//      EventManager.StartListening(GameEventType.ALL_HUMAN_IN_CROWD_ENEMY_DESTROY, OnDestroyAllEnemyInCrowd);
//      EventManager.StartListening(GameEventType.FIRST_CONTACT_WITH_ENEMY, StopCrowd);
//      EventManager.StartListening(GameEventType.UPDATE_CONSTANTS, OnUpdateVariables);
//      EventManager.StartListening(GameEventType.WIN, OnWin);
//      EventManager.StartListening(GameEventType.ALL_HUMAN_IN_CROWD_BOSS_DESTROY, OnKillBoss);
//      EventManager.StartListening(GameEventType.START_BOSS_FIGHT, SelfDisabel);
//   }

//   private void OnDisable()
//   {
//      EventManager.StopListening(GameEventType.TAP_TO_PLAY, OnTapToPlay);
//      EventManager.StopListening(GameEventType.ALL_HUMAN_IN_CROWD_ENEMY_DESTROY, OnDestroyAllEnemyInCrowd);
//      EventManager.StopListening(GameEventType.FIRST_CONTACT_WITH_ENEMY, StopCrowd);
//      EventManager.StopListening(GameEventType.UPDATE_CONSTANTS, OnUpdateVariables);
//      EventManager.StopListening(GameEventType.WIN, OnWin);
//      EventManager.StopListening(GameEventType.ALL_HUMAN_IN_CROWD_BOSS_DESTROY, OnKillBoss);
//      EventManager.StopListening(GameEventType.START_BOSS_FIGHT, SelfDisabel);
//   }
//}
