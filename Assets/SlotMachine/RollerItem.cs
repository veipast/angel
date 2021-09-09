using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RollerItem : MonoBehaviour
{
    private List<RewardItemData> rewardItemDatas = new List<RewardItemData>();
    private Image[] showItem = new Image[3];
    private int ID;
    private int index;
    public void Init(List<RewardItemData> rewardItemDatas, int _ID)
    {
        this.rewardItemDatas = rewardItemDatas;
        ID = _ID;
        index = _ID * 5 % rewardItemDatas.Count;
        for (int i = 0; i < showItem.Length; i++)
        {
            showItem[i] = transform.GetChild(i).GetComponent<Image>();
        }
        RefreshShowItem();
    }

    public void StartTurning(System.Action<int> EndRotation, int moveIndex)
    {
        StartCoroutine(RotateWait(EndRotation, moveIndex));
    }
    private IEnumerator RotateWait(System.Action<int> EndRotation, int moveIndex)
    {
        //计算旋转时间
        int timeCount = rewardItemDatas.Count * 3 + rewardItemDatas.Count * ID;

        for (int i = 0; i < timeCount; i++)
        {
            index++;
            if (index >= rewardItemDatas.Count) index = 0;
            RefreshShowItem();
            if (i < (ID + 1) * 3)
                yield return new WaitForSeconds(0.01f);
            else if (i < timeCount - 20)
                yield return new WaitForSeconds(0.05f);
            else if (i < timeCount - 10)
                yield return new WaitForSeconds(0.1f);
            else
                yield return new WaitForSeconds(0.15f);



            VibrateClassScript.GetInstancee.Haptic(MoreMountains.NiceVibrations.HapticTypes.LightImpact);
        }
        for (int i = 0; i < rewardItemDatas.Count + 5; i++)
        {
            if (moveIndex == index)
            {
                EndRotation?.Invoke(index);
                break;
            }
            index++;
            if (index >= rewardItemDatas.Count) index = 0;
            RefreshShowItem();
            yield return new WaitForSeconds(0.15f);


            VibrateClassScript.GetInstancee.Haptic(MoreMountains.NiceVibrations.HapticTypes.LightImpact);
        }
    }

    private void RefreshShowItem()
    {
        if (index - 1 <= 0)
            showItem[0].sprite = rewardItemDatas[rewardItemDatas.Count - 1].rewardSprite;
        else
            showItem[0].sprite = rewardItemDatas[index - 1].rewardSprite;

        showItem[1].sprite = rewardItemDatas[index].rewardSprite;

        if (index + 1 >= rewardItemDatas.Count)
            showItem[2].sprite = rewardItemDatas[0].rewardSprite;
        else
            showItem[2].sprite = rewardItemDatas[index + 1].rewardSprite;
    }
}
