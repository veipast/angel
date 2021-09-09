using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;



#if UNITY_IOS || UNITY_IPHONE || UNITY_EDITOR
namespace AudienceNetwork
{
    public static class AdSettings
    {
        [DllImport("__Internal")]
        private static extern void FBAdSettingsBridgeSetAdvertiserTrackingEnabled(bool advertiserTrackingEnabled);

        public static void SetAdvertiserTrackingEnabled(bool advertiserTrackingEnabled)
        {
            FBAdSettingsBridgeSetAdvertiserTrackingEnabled(advertiserTrackingEnabled);
        }
    }
}
#endif




public class MAX_SDK_Manager : MonoBehaviour
{
    public bool isShowAD = true;
    [HideInInspector]
    public bool isShowFlagAD = false;
    private static MAX_SDK_Manager _instantiate;
    public static MAX_SDK_Manager GetInstantiate()
    {

        return _instantiate;
    }
    private void Awake()
    {
        _instantiate = this;
    }

    private const string MaxSdkKey = "zs7_CE8Txsyo1rPdxm1dlTwX6iIA-FQCcsqU2flnOPLv7CS0ClwOTF8m4AOf1FwkpuxvGL2G4Py83Z0hmZ8t3N";//SDK密钥
#if UNITY_ANDROID && !UNITY_EDITOR
    private const string InterstitialAdUnitId = "ac080b7345778d2c";//插屏广告
    private const string RewardedAdUnitId = "8684e58031f3316a";//激励广告 
    private const string BannerAdUnitId = "b81fb765d65e8519";// 横屏广告
#elif UNITY_IOS && !UNITY_EDITOR
    private const string InterstitialAdUnitId = "f5e95f4741117973";//插屏广告
    private const string RewardedAdUnitId = "23bc2a45f83da903";//激励广告 
    private const string BannerAdUnitId = "a3e8dbbf7a815c45";//横屏广告
#elif UNITY_EDITOR
    private const string InterstitialAdUnitId = "f5e95f4741117973";//插屏广告
    private const string RewardedAdUnitId = "23bc2a45f83da903";//激励广告
    private const string BannerAdUnitId = "a3e8dbbf7a815c45";//横屏广告
#endif

    public void Init_AppLovin_SDK(System.Action initAllSDK)
    {
        MaxSdkCallbacks.OnSdkInitializedEvent += sdkConfiguration =>
        {
            Debug.Log("MAX SDK Initialized");
            InitializeInterstitialAds();//插屏广告
            InitializeRewardedAds();//激励广告
            InitializeBannerAds();//横幅广告

            initAllSDK?.Invoke();

#if UNITY_IOS || UNITY_IPHONE
            if (MaxSdkUtils.CompareVersions(UnityEngine.iOS.Device.systemVersion, "14.5") != MaxSdkUtils.VersionComparisonResult.Lesser)
            {
                // Note that App transparency tracking authorization can be checked via `sdkConfiguration.AppTrackingStatus` for Unity Editor and iOS targets
                // 1. Set Facebook ATE flag here, THEN
                //AppsFlyeriOS.waitForATTUserAuthorizationWithTimeoutInterval(60);
                AudienceNetwork.AdSettings.SetAdvertiserTrackingEnabled(sdkConfiguration.AppTrackingStatus == MaxSdkBase.AppTrackingStatus.Authorized);
            }
#endif

            //endOfInitLoadingGame?.Invoke();
        };

        MaxSdk.SetSdkKey(MaxSdkKey);
        MaxSdk.InitializeSdk();
    }
    /// <summary>
    /// 插屏广告时间间隔
    /// </summary>
    [HideInInspector]
    public float _Show_AD_Number = 30;
    private float _CountDown = 0;

    private void Update()
    {
        _CountDown += Time.deltaTime;
    }

    private Action InterstitialCallbackFunction;
    public bool ShowInterstitial(Action callbackFunction, string adName, bool FirstAppearanceAD = false)
    {
        if (!isShowAD) { callbackFunction?.Invoke(); return true; }
        if (_CountDown >= _Show_AD_Number && (Inter_First_Show._this.isShowAD || FirstAppearanceAD))
        {
            if (MaxSdk.IsInterstitialReady(InterstitialAdUnitId))
            {
                Time.timeScale = 0;
                InterstitialCallbackFunction = callbackFunction;
                MaxSdk.ShowInterstitial(InterstitialAdUnitId);
                Firebase.Analytics.FirebaseAnalytics.LogEvent(Firebase.Analytics.FirebaseAnalytics.EventAdImpression, "ShowInterstitial", adName);
                return true;
            }
        }
        InterstitialCallbackFunction = null;
        callbackFunction?.Invoke();
        Debug.LogError("广告还没准备好");
        return false;
    }
    private Action RewardedCallbackFunction;
    public bool ShowRewardedAd(Action callbackFunction, string adName)//点击按钮调用奖励广告
    {
        if (!isShowAD) { callbackFunction?.Invoke(); return true; }
        if (MaxSdk.IsRewardedAdReady(RewardedAdUnitId))
        {
            Time.timeScale = 0;
            RewardedCallbackFunction = callbackFunction;
            MaxSdk.ShowRewardedAd(RewardedAdUnitId);
            Firebase.Analytics.FirebaseAnalytics.LogEvent(Firebase.Analytics.FirebaseAnalytics.EventAdImpression, "ShowRewardedAd", adName);
            return true;
        }
        else
        {
            RewardedCallbackFunction = null;
            Debug.LogError("广告还没准备好");
            return false;
        }
    }

    public void ShowBanner()
    {
        MaxSdk.ShowBanner(BannerAdUnitId);
        try
        {
            Firebase.Analytics.FirebaseAnalytics.LogEvent(Firebase.Analytics.FirebaseAnalytics.EventAdImpression, "ShowBanner", "Banner");
        }
        catch { }
    }
    public void HideBanner()
    {
        MaxSdk.HideBanner(BannerAdUnitId);
    }
    #region   

    private int retryAttempt_nterstitial;

    private void InitializeInterstitialAds()
    {
        // Attach callback
        MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
        MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialLoadFailedEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialDisplayedEvent;
        MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnInterstitialClickedEvent;
        MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialHiddenEvent;//广告隐藏事件
        MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnInterstitialAdFailedToDisplayEvent;//加载失败

        // Load the first interstitial
        LoadInterstitial();
    }

    private void LoadInterstitial()
    {
        MaxSdk.LoadInterstitial(InterstitialAdUnitId);
    }

    private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad is ready for you to show. MaxSdk.IsInterstitialReady(adUnitId) now returns 'true'

        // Reset retry attempt
        retryAttempt_nterstitial = 0;
        Debug.Log("OnInterstitialLoadedEvent");
    }

    private void OnInterstitialLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Interstitial ad failed to load 
        // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds)

        retryAttempt_nterstitial++;
        double retryDelay = Math.Pow(2, Math.Min(6, retryAttempt_nterstitial));

        Invoke("LoadInterstitial", (float)retryDelay);
        Debug.Log("OnInterstitialLoadFailedEvent");
    }

    private void OnInterstitialDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("OnInterstitialDisplayedEvent");
    }

    private void OnInterstitialAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad failed to display. AppLovin recommends that you load the next ad.
        LoadInterstitial();
        Debug.Log("OnInterstitialAdFailedToDisplayEvent");
    }

    private void OnInterstitialClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {

        Debug.Log("OnInterstitialClickedEvent");
    }

    private void OnInterstitialHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad is hidden. Pre-load the next ad.
        Time.timeScale = 1;

        _CountDown = 0;
        InterstitialCallbackFunction?.Invoke();

        LoadInterstitial();
        Debug.Log("OnInterstitialHiddenEvent???");
    }


    #endregion

    #region   


    private int retryAttempt_Rewarded;

    private void InitializeRewardedAds()
    {
        // Attach callback
        MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
        MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdLoadFailedEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
        MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
        MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;
        MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdHiddenEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;

        // Load the first rewarded ad
        LoadRewardedAd();
    }

    private void LoadRewardedAd()
    {
        MaxSdk.LoadRewardedAd(RewardedAdUnitId);
    }

    private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad is ready for you to show. MaxSdk.IsRewardedAdReady(adUnitId) now returns 'true'.

        // Reset retry attempt
        retryAttempt_Rewarded = 0;
    }

    private void OnRewardedAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Rewarded ad failed to load 
        // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds).
        Time.timeScale = 1;
        retryAttempt_Rewarded++;
        double retryDelay = Math.Pow(2, Math.Min(6, retryAttempt_Rewarded));

        Invoke("LoadRewardedAd", (float)retryDelay);
    }

    private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {

    }

    private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad failed to display. AppLovin recommends that you load the next ad.
        LoadRewardedAd();
    }

    private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {

    }

    private void OnRewardedAdHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Time.timeScale = 1;
        // Rewarded ad is hidden. Pre-load the next ad
        LoadRewardedAd();
    }

    private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
    {
        Time.timeScale = 1;
        _CountDown = 0;
        RewardedCallbackFunction?.Invoke();
        // The rewarded ad displayed and the user should receive the reward.
    }

    private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Ad revenue paid. Use this callback to track user revenue.
    }


    #endregion





    #region   


    private void InitializeBannerAds()
    {
        MaxSdk.CreateBanner(BannerAdUnitId, MaxSdkBase.BannerPosition.BottomCenter);
        MaxSdk.SetBannerBackgroundColor(BannerAdUnitId, Color.black);
    }
    #endregion



}