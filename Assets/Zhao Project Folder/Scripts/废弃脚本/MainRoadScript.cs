using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEditor;

public class MainRoadScript : MonoBehaviour
{
    public List<StepsData> steps = new List<StepsData>();
    [System.Serializable]
    public class StepsData
    {
        public Transform steps;
        public UnityEngine.Vector3 movePosition;
        public bool isMaterial = false;
        public StepsData(Transform steps)
        {
            this.steps = steps;
        }
    }

    public GameObject baise;
    public GameObject heise;
    public int stepsCount = 100;
    private bool colorFlag = true;

    public float endPos;
    public Transform StepsBase;

    private void Awake()
    {
        for (int i = 0; i < stepsCount; i++)
        {
            GameObject temp;
            if (colorFlag)
            {
                temp = Instantiate(baise);
            }
            else
            {
                temp = Instantiate(heise);
            }
            colorFlag = !colorFlag;
            temp.transform.SetParent(transform);
            temp.transform.position = new UnityEngine.Vector3(0, -1, -2 * i - 2);
        }
        for (int i = 0; i < StepsBase.childCount; i++)
        {
            GameObject temp = StepsBase.GetChild(i).gameObject;
            //temp.transform.SetParent(transform);
            temp.transform.position = transform.position + new UnityEngine.Vector3(0, -1, -2 * i);
            //temp.AddComponent<StepsDoMove>();
            temp.tag = "Stairs";
            steps.Add(new StepsData(temp.transform));
            endPos = temp.transform.position.z;
        }
    }
    [ContextMenu("楼梯重置")]
    public void 楼梯重置()
    {
        foreach (var item in steps)
        {
            item.steps.position = new UnityEngine.Vector3(item.steps.position.x, -1, item.steps.position.z);
            item.steps.GetComponent<MeshRenderer>().material = null;
        }
        //isPlay = false;
    }
    [ContextMenu("楼梯上升")]
    public void 楼梯上升()
    {
        StartCoroutine(楼梯上升Wait());
    }
    private IEnumerator 楼梯上升Wait()
    {
        for (int i = steps.Count - 1; i >= 0; i--)
        {
            Transform t = steps[i].steps;
            UnityEngine.Vector3 pos = new UnityEngine.Vector3(t.position.x, i, t.position.z);
            steps[i].movePosition = pos;
            //Sequence seq = DOTween.Sequence();
            //t.DOMove(pos, 20).SetSpeedBased();
            //seq.AppendCallback(() => { t.GetComponent<MeshRenderer>().material = angelMaterial; });
            t.gameObject.GetComponent<StepsDoMove>().Init(pos, true);
            yield return new WaitForSeconds(0.05f);
        }
        //isUp = true;
        //isPlay = true;
    }
    [ContextMenu("楼梯下降")]
    public void 楼梯下降()
    {
        StartCoroutine(楼梯下降Wait());
    }
    private IEnumerator 楼梯下降Wait()
    {
        for (int i = 0; i < steps.Count; i++)
        {
            Transform t = steps[i].steps;
            UnityEngine.Vector3 pos = new UnityEngine.Vector3(t.position.x, i * -1 - 2, t.position.z);
            steps[i].movePosition = pos;
            //t.DOMove(pos, 20).SetSpeedBased();
            //seq.AppendCallback(() => { t.GetComponent<MeshRenderer>().material = angelMaterial; });
            t.GetComponent<StepsDoMove>().Init(pos, false);
            yield return new WaitForSeconds(0.05f);
        }
        //isUp = false;
        //isPlay = true;
    }
}
