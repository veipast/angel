using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum PanelType
{
    Normal, Pop
}
public class UIBase : MonoBehaviour
{
    public PanelType panelType;
    public virtual void Init() { }
    public virtual void Show()
    {
        gameObject.SetActive(true);
    }
    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }
    public bool isDebugHide = true;
    private CanvasGroup AH_Group;
    public void ShowAndHide(float value)
    {
        if (isDebugHide)
        {
            if (!AH_Group)
            {
                AH_Group = GetComponent<CanvasGroup>();
                if (!AH_Group)
                    AH_Group = gameObject.AddComponent<CanvasGroup>();
            }
            AH_Group.alpha = value;
        }
    }
}
