using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YanJiangGenSuo : MonoBehaviour
{
    private Transform player;
    void Update()
    {
        if (player)
        {
            transform.position = new Vector3(player.position.x, transform.position.y, player.position.z);
        }
        else
        {
            PlayerController temp = FindObjectOfType<PlayerController>();
            if (temp) player = temp.transform;
        }
    }
}
