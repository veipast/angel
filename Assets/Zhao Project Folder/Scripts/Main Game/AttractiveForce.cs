using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttractiveForce : MonoBehaviour
{
    private Transform player;
    private float speed = 10;
    private float tempSpeed = 100;
    public void Init(Transform player)
    {
            this.player = player;
            this.tempSpeed = speed * 10;
    }
    private void Start()
    {
          StartCoroutine(Wait());
    }
    private bool isFloadMove = false;
    IEnumerator Wait()
    {
        yield return new WaitForSeconds(0.5f);
        isFloadMove = true;
    }

    private void Update()
    {
        if (isFloadMove)
        {
            speed = Mathf.MoveTowards(speed, tempSpeed, Time.deltaTime * 20);
        }
        transform.position = UnityEngine.Vector3.MoveTowards(transform.position, player.position + new UnityEngine.Vector3(0, 1, 0), Time.deltaTime * speed);
    }
}
