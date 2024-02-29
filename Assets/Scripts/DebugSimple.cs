using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using fivuvuvUtil;
using TMPro;
public class DebugSimple : MonoSingleton<DebugSimple>
{
    public TextMeshProUGUI text;
    public void ChangeMessage(string msg)
    {
        text.text = msg;
    }
}

