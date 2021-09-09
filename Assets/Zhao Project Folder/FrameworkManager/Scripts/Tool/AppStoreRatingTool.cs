using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_ANDROID && !UNITY_EDITOR
using Google.Play.Review;
#elif UNITY_IOS && !UNITY_EDITOR
using UnityEngine.iOS;
#endif




public class AppStoreRatingTool : MonoBehaviour
{
    private static AppStoreRatingTool _instance;
    public static AppStoreRatingTool Instance { get { return _instance; } }
    private void Awake() { _instance = this; }

    public void PullUpRatingWindow()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        StartCoroutine(GooglePlayReviewWait());
#elif UNITY_IOS && !UNITY_EDITOR
        Device.RequestStoreReview();
#endif
        Debug.Log("Pull Up Rating Window");
    }


#if UNITY_ANDROID && !UNITY_EDITOR
    private IEnumerator GooglePlayReviewWait()
    {
        ReviewManager rm = new ReviewManager();
        var requestFlowOperation = rm.RequestReviewFlow();
        yield return requestFlowOperation;
        if (requestFlowOperation.Error != ReviewErrorCode.NoError)
        {
            Debug.LogFormat("requestFlowOperation:{0}", requestFlowOperation.Error);
            yield break;
        }
        PlayReviewInfo playReviewInfo = requestFlowOperation.GetResult();
        var launchFlowOperation = rm.LaunchReviewFlow(playReviewInfo);
        yield return launchFlowOperation;
        playReviewInfo = null;
        if (launchFlowOperation.Error != ReviewErrorCode.NoError)
        {
            yield break;
        }
        Debug.Log("open review over !");
    }
#endif


}