using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("按钮")]
    public Button testButton;
    private void OnTestButtonClick()
    {
        DebugSimple.Instance.ChangeMessage("点击了测试按钮");
    }


    #region 按钮方法
    public void Quit()
    {
        Application.Quit();
    }
    #endregion
}
