using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VibrateClassScript : MonoBehaviour
{
    private static VibrateClassScript _instance;
    public static VibrateClassScript GetInstancee { get { return _instance; } }
    private void Awake()
    {
        _instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        //VibrateClassScript.GetInstancee.Haptic
    }
    private float _CD = 0;
    private bool isVibrate = false;
    // Update is called once per frame
    void Update()
    {
        _CD += Time.deltaTime;
        if (_CD >= 0.1f)
        {
            isVibrate = true;
        }
    }
    public void Haptic(MoreMountains.NiceVibrations.HapticTypes hapticTypes)
    {
        if (isVibrate)
        {
            MoreMountains.NiceVibrations.MMVibrationManager.Haptic(hapticTypes);
            isVibrate = false;
            _CD = 0;
        }
    }

}
