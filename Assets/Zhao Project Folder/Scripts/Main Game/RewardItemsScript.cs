using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardItemsScript : MonoBehaviour
{
    private Transform player;
    private GameObject fx_Obj;
    private float speed = 10;
    private float tempSpeed = 100;
    public void Init(Transform player, GameObject fx_Obj, float speed)
    {
        this.player = player;
        this.fx_Obj = fx_Obj;
        this.tempSpeed = speed * 10;
        gameObject.AddComponent<BoxCollider>().isTrigger = true;
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
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameObject obj = Instantiate(fx_Obj, player.position + new UnityEngine.Vector3(0, 1, 0), Quaternion.identity);
            obj.transform.localScale = UnityEngine.Vector3.one * 2;
            Destroy(obj, 1.5f);
            Destroy(gameObject);
        }
    }
}
