using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AngelAndDevilItem : MonoBehaviour
{
    [Tooltip("被选中的")]
    public Button selectItemButton;
    [Tooltip("被选中的显示绿色")]
    public Image selectImage;
    public Image angelImage;
    public Image devilImage;
    public GameObject lockTextObjhect;
    private int _ID;
    private bool isUnlock;
    private AngelAndDevilPanel angelAndDevilPanel;
    public void Init(int _ID, AngelAndDevilPanel angelAndDevilPanel)
    {
        selectItemButton.onClick.AddListener(SelectItemButton);
        this._ID = _ID;
        this.angelAndDevilPanel = angelAndDevilPanel;
        //加载图片
        angelImage.sprite = ResourcesManager.Load<Sprite>(ResourcesType.Sprite, "Angel_" + _ID);
        devilImage.sprite = ResourcesManager.Load<Sprite>(ResourcesType.Sprite, "Devil_" + _ID);
        //
    }

    /// <summary>
    /// 旋转Item 的Button
    /// </summary>
    private void SelectItemButton()
    {
        angelAndDevilPanel.SelectItem(_ID);
        VibrateClassScript.GetInstancee.Haptic(MoreMountains.NiceVibrations.HapticTypes.Selection);
    }
    /// <summary>
    /// 刷新Item 是否解锁 是否被选择
    /// </summary>
    public void RefreshItem(bool isUnlock, float selectID)
    {
        this.isUnlock = isUnlock;
        angelImage.gameObject.SetActive(isUnlock);
        devilImage.gameObject.SetActive(isUnlock);
        lockTextObjhect.SetActive(!isUnlock);
        if (isUnlock && selectID == _ID)
            selectImage.color = Color.green;
        else
            selectImage.color = Color.white;
    }

    internal void SetSelectItemActive(bool value)
    {
        if (value)
            selectImage.color = Color.green;
        else
            selectImage.color = Color.white;
    }
}
