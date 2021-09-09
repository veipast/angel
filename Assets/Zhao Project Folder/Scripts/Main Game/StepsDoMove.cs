using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepsDoMove : MonoBehaviour
{
    public Color LineageColor = Color.green;
    public Color InfernalColor = Color.red;
    private bool isAngelColor = false;
    private UnityEngine.Vector3 movePosition;
    //private Material mat;
    public void Init(UnityEngine.Vector3 pos, bool isAngelColor)
    {
        movePosition = pos;
        this.isAngelColor = isAngelColor;
        //this.mat = mat;
        isMove = true;
    }
    private bool isMove = false;
    // Update is called once per frame
    void Update()
    {
        if (isMove)
        {
            transform.position = UnityEngine.Vector3.MoveTowards(transform.position, movePosition, Time.deltaTime * 50);
            if (UnityEngine.Vector3.Distance(transform.position, movePosition) <= 0.01f)
            {
                isMove = false;
                if (isAngelColor)
                    GetComponent<MeshRenderer>().material.color = LineageColor;
                else
                    GetComponent<MeshRenderer>().material.color = InfernalColor;
                //Destroy(this);
            }
        }

    }
}
