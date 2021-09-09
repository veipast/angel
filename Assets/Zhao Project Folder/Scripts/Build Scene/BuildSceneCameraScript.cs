using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
public enum BuildSceneType
{
    //Hell,
}
public class BuildSceneCameraScript : MonoBehaviour
{
    public CinemachineDollyCart dollyCart;
    public float startPosition = 3f;
    public float moveSpeed = 1f;

    // Start is called before the first frame update
    void Start()
    {
        //dollyCart
        dollyCart.m_Position = startPosition;
    }

    private Vector3 startPos;
    private Vector3 pos;
    private bool isMouseDown = false;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !ManagementTool.IsPointerOverGameObject())
        {
            startPos = Input.mousePosition;
            pos = startPos;
            isMouseDown = true;
        }
        if (isMouseDown)
        {
            startPos = Input.mousePosition;

            Vector3 p = startPos - pos;

            dollyCart.m_Position += p.x * Time.deltaTime * moveSpeed;

            pos = startPos;
        }
        if (Input.GetMouseButtonUp(0))
        {
            isMouseDown = false;
        }
    }
}
