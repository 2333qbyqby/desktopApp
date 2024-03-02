using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class ChatWithChat : MonoBehaviour
{
    [Header("输入框")]
    [SerializeField] private TMP_InputField inputField;
    [Header("文本显示框")]
    [SerializeField] private TextMeshProUGUI textArea;

    private string userInput;
    public void InitChat()
    {
    }


}
