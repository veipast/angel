using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Facebook.Unity;
using GameAnalyticsSDK;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Firebase.Extensions;
using System.Threading.Tasks;
using Newtonsoft.Json;

#if UNITY_IOS
using UnityEngine.iOS;
#endif

using Firebase.RemoteConfig;

public class GameManager : MonoBehaviour
{

    GameDataModel gameData;

    //private const float fpsMeasurePeriod = 0.5f;    //FPS测量间隔
    //private float m_FpsNextPeriod = 0;  //FPS下一段的间隔
    private int FPS = 30;//限帧
    private void Awake()
    {
#if UNITY_IOS
        Application.targetFrameRate = FPS;
        //m_FpsNextPeriod = Time.realtimeSinceStartup + fpsMeasurePeriod; //Time.realtimeSinceStartup获取游戏开始到当前的时间，增加一个测量间隔，计算出下一次帧率计算是要在什么时候

#endif

        DontDestroyOnLoad(gameObject);

        ModelManager.Register();

        gameData = ModelManager.GetModel<GameDataModel>();

        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }


    public MAX_SDK_Manager MAX_SDK_Manager;
    // Start is called before the first frame update
    void Start()
    {
        MAX_SDK_Manager.Init_AppLovin_SDK(InitAllSdk);

        StartCoroutine(WaitLoadScene());
        StartCoroutine(WaitLoadScene10Time());
    }
    private bool isWaitLoadScene = true;
    private IEnumerator WaitLoadScene()
    {
#if UNITY_EDITOR
        yield return new WaitForSeconds(0);
        isWaitFacebookSDK = false;
#else
        yield return new WaitForSeconds(3);
#endif
        isWaitLoadScene = false;
        LoadScene();
    }
    private IEnumerator WaitLoadScene10Time()
    {
        yield return new WaitForSeconds(3f);
        Init_FireBase_Sdk();
        yield return new WaitForSeconds(7f);
        isWaitFacebookSDK = false;
        isWaitLoadScene = false;
        isTenjinSDK = false;
        if (!isLoadScene)
        {
            isLoadScene = true;
            SceneLevelManager.Loading(gameData.LoadSceneName);
            StartCoroutine(WaitShowBanner());
        }
    }
    private void InitAllSdk()
    {
        GameAnalytics.Initialize();
        InitTenjinSDK();
        Init_Facebook_SDK();
    }

    private bool isLoadScene = false;
    private void LoadScene()
    {
        if (isWaitLoadScene) return;
        if (isWaitFacebookSDK) return;
        if (isTenjinSDK) return;
        if (isLoadScene) return;
        isLoadScene = true;

        SceneLevelManager.Loading(gameData.LoadSceneName);
        StartCoroutine(WaitShowBanner());
    }

    IEnumerator WaitShowBanner()
    {
        yield return new WaitForSeconds(5f);
        MAX_SDK_Manager.GetInstantiate().ShowBanner();
    }
    //public bool isWaitFireBaseSDK = true;
    private Firebase.FirebaseApp firebaseApp;
    private void Init_FireBase_Sdk()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                firebaseApp = Firebase.FirebaseApp.DefaultInstance;
                GetFirebaseValue();
                Debug.Log("FireBase初始化完成！");
                // Set a flag here to indicate whether Firebase is ready to use by your app.
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
            //isWaitFireBaseSDK = false;
            //LoadScene();
        });
    }

    private bool isWaitFacebookSDK = true;
    private void Init_Facebook_SDK()
    {
        if (!FB.IsInitialized)
        {
            FB.Init(() =>
            {
                if (FB.IsInitialized)
                {
                    FB.ActivateApp();
                    isWaitFacebookSDK = false;
                    LoadScene();
                }
                else
                    Debug.LogError("Couldn't Initialized Facebook!");
            },
            isGameShown =>
            {
                if (!isGameShown)
                    Time.timeScale = 0;
                else
                    Time.timeScale = 1;
            });
        }
        else
        {
            Debug.LogError("Facebook Activate App");
            FB.ActivateApp();
        }
    }

    private BaseTenjin TJ_Instance;
    private bool isTenjinSDK = true;
    private void InitTenjinSDK()
    {
        TJ_Instance = Tenjin.getInstance("1USEZHM7XMSJYYYAVTJ8MGTNCQVUPI1X");

#if UNITY_IOS && !UNITY_EDITOR

        //instance.SetAppStoreType(AppStoreType.other);//???

        if (new Version(Device.systemVersion).CompareTo(new Version("14.0")) >= 0)
        {
            // Tenjin wrapper for requestTrackingAuthorization
            TJ_Instance.RequestTrackingAuthorizationWithCompletionHandler((status) =>
            {
                Debug.Log("===> App Tracking Transparency Authorization Status: " + status);

                //instance.Init("1USEZHM7XMSJYYYAVTJ8MGTNCQVUPI1X");
                TJ_Instance.Connect();
            });
        }
        else
        {
            TJ_Instance.Connect();
        }
#elif UNITY_ANDROID && !UNITY_EDITOR
        TJ_Instance.Connect();
#elif UNITY_EDITOR
        TJ_Instance.Connect();
#endif

        TJ_Instance.RegisterAppForAdNetworkAttribution();

        //TJ_Instance.UpdateConversionValue();

        isTenjinSDK = false;
        LoadScene();
    }
    void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus)
        {
            InitTenjinSDK();
        }

    }



    //private void aaaaaaa()
    //{
    //    // Log an event with no parameters.记录一个没有参数的事件。
    //    Firebase.Analytics.FirebaseAnalytics.LogEvent(Firebase.Analytics.FirebaseAnalytics.EventAdImpression);

    //    // Log an event with a float parameter使用float参数记录事件
    //    Firebase.Analytics.FirebaseAnalytics.LogEvent("progress", "percent", 0.4f);

    //    // Log an event with an int parameter.使用int参数记录事件。
    //    Firebase.Analytics.FirebaseAnalytics.LogEvent(
    //        Firebase.Analytics.FirebaseAnalytics.EventPostScore,
    //        Firebase.Analytics.FirebaseAnalytics.ParameterScore,
    //        42
    //      );

    //    // Log an event with a string parameter.使用字符串参数记录事件。
    //    Firebase.Analytics.FirebaseAnalytics.LogEvent(Firebase.Analytics.FirebaseAnalytics.EventJoinGroup, Firebase.Analytics.FirebaseAnalytics.ParameterGroupId, "spoon_welders");

    //    // Log an event with multiple parameters, passed as a struct:使用多个参数记录事件，并作为结构传递：
    //    Firebase.Analytics.Parameter[] LevelUpParameters =
    //    {
    //        new Firebase.Analytics.Parameter(
    //        Firebase.Analytics.FirebaseAnalytics.ParameterLevel, 5),
    //        new Firebase.Analytics.Parameter(
    //        Firebase.Analytics.FirebaseAnalytics.ParameterCharacter, "mrspoon"),
    //        new Firebase.Analytics.Parameter(
    //        "hit_accuracy", 3.14f)
    //    };

    //    Firebase.Analytics.FirebaseAnalytics.LogEvent(
    //      Firebase.Analytics.FirebaseAnalytics.EventLevelUp,
    //      LevelUpParameters);
    //}

    private void GetFirebaseValue()
    {
        Dictionary<string, object> defaults = new Dictionary<string, object>();
        defaults.Add("interTestA", @"{""InterTestA"":false,""InterTime"":30,""RVFreeTimes"":3}");
        defaults.Add("InterFirstShow", @"{""InterFirstShow"":1}");

        //gameData.sceneNames
        //foreach (string item in gameData.sceneNames)
        //{
        //    TextAsset text = Resources.Load<TextAsset>("ConfigFolder/" + item);
        //    string[] s = item.Split(' ');
        //    string key = s[s.Length - 1];
        //    if (!defaults.ContainsKey(key))
        //        defaults.Add(key, text.text);
        //    Debug.LogError(key);
        //}

        Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults).ContinueWithOnMainThread(task =>
        {
            FetchDataAsync();
        });
    }
    // Start a fetch request.
    // FetchAsync only fetches new data if the current data is older than the provided
    // timespan.  Otherwise it assumes the data is "recent enough", and does nothing.
    // By default the timespan is 12 hours, and for production apps, this is a good
    // number. For this example though, it's set to a timespan of zero, so that
    // changes in the console will always show up immediately.
    private Task FetchDataAsync()
    {
        //DebugLog("Fetching data...");

        System.Threading.Tasks.Task fetchTask =
        Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.FetchAndActivateAsync();
        return fetchTask.ContinueWithOnMainThread((t) =>
        {
            //FirebaseRemoteConfig b = FirebaseRemoteConfig.DefaultInstance;
            FirebaseRemoteConfig a = FirebaseRemoteConfig.DefaultInstance;
            ConfigValue config_1 = a.GetValue("interTestA");
            try
            {
                Debug.Log(config_1.StringValue);
                FirebaseConfigData_AD._this = JsonUtility.FromJson<FirebaseConfigData_AD>(config_1.StringValue);
                MAX_SDK_Manager.GetInstantiate().isShowFlagAD = FirebaseConfigData_AD._this.InterTestA;
                MAX_SDK_Manager.GetInstantiate()._Show_AD_Number = FirebaseConfigData_AD._this.InterTime;
                Debug.Log(MAX_SDK_Manager.GetInstantiate().isShowFlagAD.ToString() + "|" +
                          MAX_SDK_Manager.GetInstantiate()._Show_AD_Number.ToString());
            }
            catch (Exception ex)
            {
                Debug.LogError("获取FirebaseRemoteConfig出错:\n" + ex.ToString());
                MAX_SDK_Manager.GetInstantiate().isShowFlagAD = false;
                MAX_SDK_Manager.GetInstantiate()._Show_AD_Number = 30;
            }
            ConfigValue config_2 = a.GetValue("InterFirstShow");
            try
            {
                Debug.Log(config_2.StringValue);
                Inter_First_Show._this = JsonUtility.FromJson<Inter_First_Show>(config_2.StringValue);
            }
            catch { }

            //foreach (string item in gameData.sceneNames)
            //{
            //    try
            //    {
            //        string[] s = item.Split(' ');
            //        ConfigValue configTemp = a.GetValue(s[s.Length - 1]);
            //        Debug.LogError(configTemp.StringValue);
            //        GameSceneData tempData = JsonConvert.DeserializeObject<GameSceneData>(configTemp.StringValue);
            //        AllGameSceneData.SetData(item, tempData);
            //    }
            //    catch (Exception ex)
            //    {

            //    }

            //}
        });
    }


}
[System.Serializable]
public class FirebaseConfigData_AD
{
    public static FirebaseConfigData_AD _this = new FirebaseConfigData_AD();
    public bool InterTestA;
    public float InterTime = 30;
    public int RVFreeTimes = 3;
}
[System.Serializable]
public class Inter_First_Show
{
    public static Inter_First_Show _this = new Inter_First_Show();
    public float InterFirstShow = 1;
    /// <summary>
    /// LEVEL超多InterFirstShow 时 isShowAD = true
    /// </summary>
    public bool isShowAD
    {
        get { return PlayerPrefs.GetInt("Inter_First_Show", 0) == 1; }
        set { PlayerPrefs.SetInt("Inter_First_Show", value ? 1 : 0); }
    }
}


//public class NwtWork : MonoBehaviour
//{
//    private float time = 0f;
//    void Update()
//    {
//        time += Time.deltaTime * 1;
//        if (time >= 1)
//        {
//            time = 0f;
//            switch (Application.internetReachability)
//            {
//                case NetworkReachability.NotReachable:
//                    Debug.LogError("网络断开");
//                    break;
//                case NetworkReachability.ReachableViaLocalAreaNetwork:
//                    Debug.LogError("WIFI");
//                    break;
//                case NetworkReachability.ReachableViaCarrierDataNetwork:
//                    Debug.LogError("4G/3G");
//                    break;
//            }
//        }
//    }
//}
