using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
[System.Serializable]
public class PlayerWingObject
{
    public GameObject wingObject;
    private bool _isActive;

    public PlayerWingObject(GameObject gameObject)
    {
        this.wingObject = gameObject;
    }
    private bool jilu = false;
    public bool IsActive { get { return _isActive; } }
    public void SetActive(bool value, bool isAni = false)
    {
        _isActive = value;
        if (isAni)
        {
            if (_isActive)
            {
                wingObject.SetActive(_isActive);
                wingObject.transform.localScale = UnityEngine.Vector3.zero;
                wingObject.transform.DOScale(UnityEngine.Vector3.one, 0.5f);
            }
            else
            {
                Sequence seq = DOTween.Sequence();
                seq.Append(wingObject.transform.DOScale(UnityEngine.Vector3.zero, 0.5f));
                seq.AppendCallback(() => { wingObject.SetActive(_isActive); });
            }
        }
        else
        {
            wingObject.SetActive(_isActive);
        }
        if (jilu != _isActive)
        {
            MsgManager.Invoke(MsgManagerType.TextScale);
            jilu = _isActive;
        }
        
    }
}
