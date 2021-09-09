using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TurningDirection
{
    Left, Right
}
public class TurnACorner : MonoBehaviour
{
    public TurningDirection turningDirection;
    public Transform nextRoadOrigin;
    public bool isEndTurnACorner = false;
    private bool isTrigger = true;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && isTrigger)
        {
            isTrigger = false;
            MsgManager.Invoke(MsgManagerType.TriggerTurnACorner, this);
            if (isEndTurnACorner)
            {
                MsgManager.Invoke(MsgManagerType.AllEndObject, true);
            }
        }
    }
}
