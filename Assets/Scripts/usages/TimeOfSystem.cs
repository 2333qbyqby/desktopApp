using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimeOfSystem : MonoBehaviour
{
    [Header("所需要组件")]
    [SerializeField]private TextMeshProUGUI tmp;
    private int hour;
    private int minute;
    private int second;
    private void Update()
    {
        hour = DateTime.Now.Hour;
        minute = DateTime.Now.Minute;
        second = DateTime.Now.Second;

        tmp.text = hour.ToString("D2") + ":" + minute.ToString("D2") + ":" + second.ToString("D2");

    }
}
