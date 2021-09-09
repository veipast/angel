using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoadingPanel : MonoBehaviour
{
    public Animator animator;
    public void OpenPanel()
    {
        transform.parent.gameObject.SetActive(true);
        animator.Play("聚拢");
    }
    public void CloasPanel()
    {
        animator.Play("散开");
    }

    public void AnimatorOpenPanel_()
    {
        //切换场景
        StartCoroutine(ToLoadingScene());
    }
    IEnumerator ToLoadingScene()
    {
        yield return new WaitForSeconds(0.25f);
        SceneLevelManager.ToLoadingScene();
    }
    public void AnimatorCloasPanel_()
    {
        transform.parent.gameObject.SetActive(false);
    }
}
