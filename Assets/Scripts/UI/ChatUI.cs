using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatUI : MonoBehaviour
{
    public GameObject chatUIObj;
    private bool isShow = false;
    private void Start()
    {
        this.HideChatUI();
    }
    
    public void ShowChatUI()
    {
        chatUIObj.SetActive(true);
    }

    public void HideChatUI()
    {
        chatUIObj.SetActive(false);
    }

    public void OnClickChatUI()
    {
        if (isShow)
        {
            HideChatUI();
            isShow = false;
        }
        else
        {
            ShowChatUI();
            isShow = true;
        }
    }
}
