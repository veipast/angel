using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    private Transform _canvas;
    private Transform _normal;
    private Transform _pop;
    public SceneLoadingPanel _sceneLoadingPanel;
    public UIManager()
    {
        GameObject temp = GameObject.Find("UI_Canvas");
        if (temp == null)
        {
            temp = GameObject.Instantiate(ResourcesManager.Load<GameObject>(ResourcesType.UI, "UI_Canvas"));
            temp.transform.eulerAngles = new UnityEngine.Vector3(0, 180, 0);
        }
        GameObject.DontDestroyOnLoad(temp);
        _canvas = temp.transform.Find("UIManagerCanvas");
        _normal = _canvas.Find("Normal").transform;
        _pop = _canvas.Find("Pop").transform;
        _sceneLoadingPanel = _canvas.Find("SceneLoadingPanel/CloudAnimationPanel").GetComponent<SceneLoadingPanel>();
    }
    Dictionary<PanelName, UIBase> dicUI = new Dictionary<PanelName, UIBase>();
    Dictionary<PanelName, UIBase> tempUI = new Dictionary<PanelName, UIBase>();
    Stack<UIBase> stackUI = new Stack<UIBase>();

    public void OpenUI(PanelName name)
    {
        UIBase temp = LoadUIBase(name);
        switch (temp.panelType)
        {
            case PanelType.Normal:
                temp.transform.SetParent(_normal, false);
                if (!tempUI.ContainsKey(name))
                {
                    tempUI.Add(name, temp);
                }
                break;
            case PanelType.Pop:
                temp.transform.SetParent(_pop, false);
                stackUI.Push(temp);
                break;
            default:
                break;
        }
        try
        {
            if (temp)
                temp.Show();
            else
                ShowUI_EX(name);
        }
        catch (Exception ex)
        {
            ShowUI_EX(name);
        }

    }
    private void ShowUI_EX(PanelName name)
    {
        UIBase temp;
        if (dicUI.TryGetValue(name, out temp))
        {
            dicUI.Remove(name);
        }
        temp = LoadUIBase(name);
        switch (temp.panelType)
        {
            case PanelType.Normal:
                temp.transform.SetParent(_normal, false);
                if (!tempUI.ContainsKey(name))
                {
                    tempUI.Add(name, temp);
                }
                break;
            case PanelType.Pop:
                temp.transform.SetParent(_pop, false);
                stackUI.Push(temp);
                break;
            default:
                break;
        }
        temp.Show();
    }
    private UIBase LoadUIBase(PanelName name)
    {
        UIBase temp;
        if (!dicUI.TryGetValue(name, out temp))
        {
            GameObject obj = GameObject.Instantiate(ResourcesManager.Load<GameObject>(ResourcesType.UI, "Panel/" + name.ToString()), _normal, false);
            temp = obj.GetComponent<UIBase>();
            if (temp != null)
            {
                dicUI.Add(name, temp);
                temp.Init();
            }
        }
        return temp;
    }
    public void CloseUI(PanelName name)
    {
        UIBase temp;//= LoadUIBase(name);
        if (dicUI.TryGetValue(name, out temp))
        {
            switch (temp.panelType)
            {
                case PanelType.Normal:
                    if (tempUI.ContainsKey(name))
                    {
                        if (!dicUI.ContainsKey(name))
                            dicUI.Add(name, tempUI[name]);
                        tempUI.Remove(name);
                    }
                    break;
                case PanelType.Pop:
                    if (stackUI.Count > 1)
                    {
                        if (!dicUI.ContainsKey(name))
                            dicUI.Add(name, stackUI.Pop());
                        else
                            stackUI.Pop();
                    }
                    break;
                default:
                    break;
            }
            if (dicUI.ContainsKey(name))
                temp.Hide();
        }
    }

    public void ShowAndHide(float value)
    {
        foreach (KeyValuePair<PanelName, UIBase> item in dicUI)
        {
            item.Value.ShowAndHide(value);
        }
    }
    public CanvasScaler GetCanvasScaler { get { return _canvas.GetComponent<CanvasScaler>(); } }
}
