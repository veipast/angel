using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ContinuousClickPullUpRatingWindow : MonoBehaviour, IPointerClickHandler
{

    // Start is called before the first frame update
    void Start()
    {

    }
    private bool isClick = false;
    private float CD = 0;
    private int clickCount = 0;
    // Update is called once per frame
    void Update()
    {
        if (isClick)
        {
            CD += Time.deltaTime;
            if (CD >= 0.3f)
            {
                isClick = false;
                CD = 0;
                clickCount = 0;
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        isClick = true;
        CD = 0;
        clickCount++;
        if (clickCount >= 5)
        {
            clickCount = 0;
            isClick = false;
            AppStoreRatingTool.Instance.PullUpRatingWindow();
        }
    }
}
