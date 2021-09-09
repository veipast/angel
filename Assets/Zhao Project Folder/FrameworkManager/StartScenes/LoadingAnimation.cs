using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingAnimation : MonoBehaviour
{
    public Text LoadingText;
    private int index = 0;
    string[] loading = new string[] { "Loading", "Loading.", "Loading..", "Loading..." };
    // Start is called before the first frame update
    void Start()
    {
        LoadingText.text = loading[index];
        StartCoroutine(Wait());
    }

    IEnumerator Wait()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.4f);
            index++;
            if (index >= loading.Length) index = 0;
            LoadingText.text = loading[index];
        }
    }
}
