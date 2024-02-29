using fivuvuvUtil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoSingleton<UIManager>
{
    [Header("主UI")]
    public GameObject mainMenu;
    private void Start()
    {
        Init();
    }
    public void Init()
    {
        mainMenu.SetActive(false);
    }

    public void ShowMainUI()
    {
        mainMenu.SetActive(true);
    }
    public void HideMainUI()
    {
        mainMenu.SetActive(false);
    }

    public void SetMainUIPosition(Vector2 vector2)
    {
        Vector3 curPos=mainMenu.transform.position;
        mainMenu.transform.position = new Vector3(vector2.x, vector2.y, curPos.z);
    }
    
}
