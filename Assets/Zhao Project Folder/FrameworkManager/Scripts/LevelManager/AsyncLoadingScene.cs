using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AsyncLoadingScene : MonoBehaviour
{
    public Slider _loadingSlider;
    private float _priority;
    private void Start()
    {
        _loadingSlider.value = 0;
        _priority = 0;
        StartCoroutine(LoadingScene());
    }
    private AsyncOperation _operation;
    private IEnumerator LoadingScene()
    {
        _operation = SceneManager.LoadSceneAsync(SceneLevelManager.SceneName);
        _operation.allowSceneActivation = false;
        yield return null;
    }
    private void Update()
    {
        if (_operation.progress < 0.9f)
            _priority = Mathf.MoveTowards(_priority, _operation.progress, Time.deltaTime * 10f);
        else
            _priority = Mathf.MoveTowards(_priority, 1, Time.deltaTime * 10f);
        _loadingSlider.value = _priority;
        if (_priority == 1 && !_operation.allowSceneActivation)
        {
            _operation.allowSceneActivation = true;
            UIManager.Instance._sceneLoadingPanel.CloasPanel();
        }
    }
}
