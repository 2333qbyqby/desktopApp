using UnityEngine;
using System.Runtime.InteropServices;
using System;
using UnityEngine.EventSystems;
public class MouseEvents : MonoBehaviour
{
    public enum MouseKey
    {
        None,
        Left,
        Right,
        Middle
    }
    public struct POINT
    {
        public int X;
        public int Y;
        public POINT(int x, int y)
        {
            X = x;
            Y = y;
        }


        public override string ToString()
        {
            return ("X" + X + ",Y:" + Y);
        }
    }

    public static MouseEvents Instance { get; private set; }
    [DllImport("user32.dll")]
    public static extern short GetAsyncKeyState(int vKey);

    [DllImport("user32.dll")]
    private static extern IntPtr FindWindow(string IpClassName, string IpWindowName);
    /// <summary>
    /// 获得鼠标再屏幕上的位置
    /// </summary>
    [DllImport("user32.dll")]

    private static extern bool GetCursorPos(ref POINT LpPoint);
    /// <summary>
    /// 设置目标窗体的大小，位置
    /// </summary>
    /// <param name="hWnd">目标句柄</param>
    /// <param name="X">目标窗体新位置X轴坐标</param>
    /// <param name="Y">目标窗体Y轴坐标</param>
    /// <param name="nWidth">目标窗体新宽度</param>
    /// <param name="nHeight">目标窗体新高度</param>
    /// <param name="BRePaint">是否刷新窗体</param>
    /// <returns></returns>
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern int MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool BRePaint);
    private const int VK_LBUTTON = 0x01;//鼠标左键
    private const int VK_RBUTTON = 0x02;//鼠标右键
    private const int VK_MBUTTON = 0x04;//鼠标中键

    private bool isLeftButtonDown;
    private bool isRightButtonDown;
    private bool isMiddleButtonDown;

    public event Action<MouseKey, Vector3> MouseKeyDownEvent;
    public event Action<MouseKey, Vector3> MouseKeyUpEvent;
    public event Action<MouseKey, Vector3> MouseDragEvent;
    public event Action<MouseKey> MouseKeyClickEvent;

    public Vector3 MousePos { get; private set; }

    private bool hasDragged;
    private Vector3 leftDownPos;
    private Vector3 rightDownPos;
    private Vector3 middleDownPos;

    /// <summary>
    /// 记录当前鼠标的位置
    /// </summary>
    public POINT point;
    /// <summary>
    /// 当前窗体句柄
    /// </summary>
    private IntPtr hwnd;
    private bool dragModel;//是否在拖拽模型

    private void Awake()
    {
        Instance = this;

    }

    private void Start()
    {
        Init();
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus && UIManager.Instance.mainMenu.activeSelf)
        {
            UIManager.Instance.HideMainUI();
        }
    }

    private void Update()
    {
        //按下左键
        if (GetAsyncKeyState(VK_LBUTTON) != 0)
        {
            //左键不在UI上，关闭菜单
            if (UIManager.Instance.mainMenu.activeSelf && !EventSystem.current.IsPointerOverGameObject())
            {
                UIManager.Instance.mainMenu.SetActive(false);
            }

            if (!isLeftButtonDown)
            {
                isLeftButtonDown = true;
                leftDownPos = MouseKeyDown(MouseKey.Left);
            }
            else if (MousePos != Input.mousePosition)
            {
                MouseKeyDrag(MouseKey.Left);
                if (!hasDragged)
                {
                    hasDragged = true;
                    // 开启拖拽模型，根据鼠标位置判断是否可拖拽。显示模型时用碰撞体+射线检测，在模型不显示时可用是否在UI上判断

                    if (Physics.Raycast(Camera.main.ScreenPointToRay(MousePos), out RaycastHit hit, 10f, 1 << LayerMask.NameToLayer("Player")) || (EventSystem.current.IsPointerOverGameObject()))

                    {
                        dragModel = true;
                        UIManager.Instance.mainMenu.SetActive(false);
                    }
                }
            }
        }
        // 按下右键
        if (GetAsyncKeyState(VK_RBUTTON) != 0)

        {
            //TODO:sizePage
            //sizePage.SetActive(false);
            DebugSimple.Instance.ChangeMessage("右键按下");
            // 右键模型出现主菜单,并根据鼠标位置设置主菜单出现位置，防止主菜单显示到屏幕外，是把屏幕分为四个象限来判断

            if (Physics.Raycast(Camera.main.ScreenPointToRay(MousePos), out RaycastHit hit, 10f, 1 << LayerMask.NameToLayer("Player")) || (EventSystem.current.IsPointerOverGameObject()))
            {
                DebugSimple.Instance.ChangeMessage("右键按下出现主菜单");
                if (point.X <= Screen.currentResolution.width / 2 && point.Y <= Screen.currentResolution.height / 2)
                {
                    //设置菜单在鼠标位置的左下方
                    UIManager.Instance.SetMainUIPosition(new Vector2(0.5f, 0f));
                }
                else if (point.X > Screen.currentResolution.width / 2 && point.Y <= Screen.currentResolution.height / 2)
                {
                    //设置菜单在鼠标位置的右下方
                    UIManager.Instance.SetMainUIPosition(new Vector2(-0.5f, 0f));
                }
                else if (point.X <= Screen.currentResolution.width / 2 && point.Y > Screen.currentResolution.height / 2)
                {
                    UIManager.Instance.SetMainUIPosition(new Vector2(0.5f, 1.6f));
                    //设置菜单在鼠标位置的左上方
                    //mainMenu.transform.localPosition = new Vector3(Input.mousePosition.x * 3 - 1500, Input.mousePosition.y * 3 - 1800 + 800, 0);
                }
                else if (point.X > Screen.currentResolution.width / 2 && point.Y > Screen.currentResolution.height / 2)
                {
                    //mainMenu.transform.localPosition = new Vector3(Input.mousePosition.x * 3 - 1500 - 600, Input.mousePosition.y * 3 - 1800 + 800, 0);
                   //设置菜单在鼠标位置的右上方
                    UIManager.Instance.SetMainUIPosition(new Vector2(-0.5f, 1.6f));

                }
                UIManager.Instance.ShowMainUI();
            }

            if (!isRightButtonDown)
            {
                isRightButtonDown = true;
                rightDownPos = MouseKeyDown(MouseKey.Right);
            }
            else if (MousePos != Input.mousePosition)
            {
                MouseKeyDrag(MouseKey.Right);
                if (!hasDragged)
                {
                    hasDragged = true;
                }
            }
        }
        // 按下中键
        if (GetAsyncKeyState(VK_MBUTTON) != 0)
        {
            DebugSimple.Instance.ChangeMessage("中键按下");
            if (!isMiddleButtonDown)
            {
                isMiddleButtonDown = true;
                middleDownPos = MouseKeyDown(MouseKey.Middle);
            }
            else if (MousePos != Input.mousePosition)
            {
                MouseKeyDrag(MouseKey.Middle);
                if (!hasDragged)
                {
                    hasDragged = true;
                }
            }
        }
        // 抬起左键
        if (GetAsyncKeyState(VK_LBUTTON) == 0 && isLeftButtonDown)
        {
            isLeftButtonDown = false;
            MouseKeyUp(MouseKey.Left);

            // 无拖拽、down==up
            if (!hasDragged && leftDownPos == MousePos)
            {
                MouseKeyClick(MouseKey.Left);
                //点击模型的时候，随机播放动画
                //if (!anim.GetBool("bow") && !anim.GetBool("exit") && !UIScript.Instance.randomAnimMode && !UIScript.Instance.inGameMode && Physics.Raycast(Camera.main.ScreenPointToRay(MousePos), out RaycastHit hit, 10f, 1 << LayerMask.NameToLayer("showObj")))
                //{
                //    int num = UnityEngine.Random.Range(1, 3); // 1/2
                //    neck.transform.rotation = Quaternion.Euler(0, 0, 0);
                //    switch (num)
                //    {
                //        case 1:
                //            GameObject.FindWithTag("Player").GetComponent<Animator>().SetBool("bow", true); break;
                //        case 2:
                //            GameObject.FindWithTag("Player").GetComponent<Animator>().SetBool("exit", true); break;
                //    }
                //}



            }

            hasDragged = false;
            // 停止拖拽模型
            dragModel = false;
        }

        // 抬起右键
        if (GetAsyncKeyState(VK_RBUTTON) == 0 && isRightButtonDown)
        {
            isRightButtonDown = false;
            MouseKeyUp(MouseKey.Right);

            if (!hasDragged && rightDownPos == MousePos)
            {
                MouseKeyClick(MouseKey.Right);
            }

            hasDragged = false;
        }
        // 抬起中键
        if (GetAsyncKeyState(VK_MBUTTON) == 0 && isMiddleButtonDown)
        {
            isMiddleButtonDown = false;
            MouseKeyUp(MouseKey.Middle);

            if (!hasDragged && middleDownPos == MousePos)
            {
                MouseKeyClick(MouseKey.Middle);
            }

            hasDragged = false;
        }

        if (dragModel)
        {
            DebugSimple.Instance.ChangeMessage("拖拽模型");
            GetCursorPos(ref point);//获取鼠标在屏幕上的位置
            MoveWindow(hwnd, point.X - Screen.width / 2, point.Y - Screen.height / 2, Screen.width, Screen.height, true);
        }
    }

    public void Init()
    {
        isLeftButtonDown = false;
        isRightButtonDown = false;
        isMiddleButtonDown = false;
        hasDragged = false;
        hwnd = FindWindow(null, Application.productName);
        dragModel = false;
        point.X = Screen.currentResolution.width;
        point.Y = Screen.currentResolution.height;
    }

    private Vector3 MouseKeyDown(MouseKey mouseKey)
    {
        RefreshMousePos();
        MouseKeyDownEvent?.Invoke(mouseKey, MousePos);

        return MousePos;
    }
    private Vector3 MouseKeyUp(MouseKey mouseKey)
    {
        RefreshMousePos();
        MouseKeyUpEvent?.Invoke(mouseKey, MousePos);

        return MousePos;
    }

    private Vector3 MouseKeyDrag(MouseKey mouseKey)
    {
        RefreshMousePos();
        MouseDragEvent?.Invoke(mouseKey, MousePos);

        return MousePos;
    }

    private void MouseKeyClick(MouseKey mouseKey)
    {
        MouseKeyClickEvent?.Invoke(mouseKey);
    }

    private Vector3 RefreshMousePos()
    {
        MousePos = Input.mousePosition;
        return MousePos;
    }

    public Vector3 MousePosToWorldPos(Vector3 mousePos)
    {
        var screenInWorldPos = Camera.main.WorldToScreenPoint(mousePos);
        var refPos = new Vector3(mousePos.x, mousePos.y, screenInWorldPos.z);
        var pos = Camera.main.ScreenToWorldPoint(refPos);
        return pos;
    }
}
