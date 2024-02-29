using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemUI : MonoBehaviour
{
    public GameObject SystemPic;
    private bool isShow = false;
    private void Start()
    {
        HideSystemUI();
    }
    public void ShowSystemUI()
    {
        SystemPic.SetActive(true);
    }
    public void HideSystemUI()
    {
        SystemPic.SetActive(false);
    }

    public void OnClickSystemUI()
    {
        if (isShow)
        {
            HideSystemUI();
            isShow = false;
        }
        else
        {
            ShowSystemUI();
            isShow = true;
        }
    }
}
