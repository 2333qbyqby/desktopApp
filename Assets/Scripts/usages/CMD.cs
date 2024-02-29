using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMD : MonoBehaviour
{
    public void OpenCMD()
    {
        System.Diagnostics.Process.Start("cmd.exe");
    }
}
