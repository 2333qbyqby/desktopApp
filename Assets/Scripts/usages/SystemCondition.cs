using UnityEngine;
using System.Runtime.InteropServices;
using System;
using TMPro;
using UnityEngine.Profiling;
using System.Threading;
using System.Diagnostics;
using System.Collections;
using System.Threading.Tasks;
using System.Linq;
public class SystemCondition : MonoBehaviour
{
    // -----------CPU---------------
    private float updateInterval = 1;
    private int processorCount; // The amount of physical CPU cores
    private float CpuUsage; // output
    private Thread _cpuThread;
    private float _lasCpuUsage;
    [Header("cpu文本")]
    public TextMeshProUGUI cpu1;
    //-----------Memory---------------
    #region Memory
    public struct MEMORYSTATUSEX
    {
        public uint dwLength;
        public uint dwMemoryLoad;
        public ulong ullTotalPhys;
        public ulong ullAvailPhys;
        public ulong ullTotalPageFile;
        public ulong ullAvailPageFile;
        public ulong ullTotalVirtual;
        public ulong ullAvailVirtual;
        public ulong ullAvailExtendedVirtual;
    }

    [DllImport("kernel32.dll")]
    protected static extern void GlobalMemoryStatus(ref MEMORYSTATUSEX lpBuff);
    //
    protected ulong GetWinAvailMemory()
    {
        MEMORYSTATUSEX ms = new MEMORYSTATUSEX();
        ms.dwLength = 64;
        GlobalMemoryStatus(ref ms);
        return ms.ullAvailPhys;
    }

    protected ulong GetWinTotalMemory()
    {
        MEMORYSTATUSEX ms = new MEMORYSTATUSEX();
        ms.dwLength = 64;
        GlobalMemoryStatus(ref ms);
        return ms.ullTotalPhys;
    }

    protected long GetWinUsedMemory()
    {
        return Profiler.GetTotalReservedMemoryLong();
    }
    #endregion

    [Header("内存文本")]
    public TextMeshProUGUI memory1;
    public TextMeshProUGUI memory2;

    private double memoryTotal;
    private double memoryUsed;

    // -----------GraphicsDevice-------------
    public TextMeshProUGUI graphicsDeviceName;
    public TextMeshProUGUI graphicsDeviceVendorAndId;
    public TextMeshProUGUI graphicsDeviceVersion;
    private void Awake()
    {
        #region CPU
        CpuUsage = 0;
        _cpuThread = new Thread(UpdateCPUUsage)
        {
            IsBackground = true,
            Priority = System.Threading.ThreadPriority.BelowNormal
        };
        _cpuThread.Start();
        #endregion

        #region GraphicsDevice
        graphicsDeviceName.text = "显卡 : " + SystemInfo.graphicsDeviceName;
        graphicsDeviceVendorAndId.text = "供应商ID :" + SystemInfo.graphicsDeviceVendorID.ToString();
        graphicsDeviceVersion.text = "版本 : " + SystemInfo.graphicsDeviceVersion;
        #endregion
    }
    void Update()
    {
        // =======================CPU=======================
#if !UNITY_EDITOR
            processorCount = SystemInfo.processorCount / 2;
#endif
        // for more efficiency skip if nothing has changed
        if (Mathf.Approximately(_lasCpuUsage, CpuUsage)) return;

        // the first two values will always be "wrong"
        // until _lastCpuTime is initialized correctly
        // so simply ignore values that are out of the possible range
        if (CpuUsage < 0 || CpuUsage > 100) return;

        // I used a float instead of int for the % so use the ToString you like for displaying it

        cpu1.text = "CPU : " + CpuUsage.ToString("F1") + "%";

        // Update the value of _lasCpuUsage
        _lasCpuUsage = CpuUsage;


        // =======================Memory=======================
        memoryTotal = GetWinTotalMemory() / 1024.0 / 1024 / 1024;
        memoryUsed = memoryTotal - GetWinAvailMemory() / 1024.0 / 1024 / 1024;
        memory1.text = "Memory : " + (memoryUsed / (float)memoryTotal * 100).ToString("#.#") + "%";
        memory2.text = memoryUsed.ToString("#.#") + " / " + memoryTotal.ToString("#.#") + " G";

    }

    private void UpdateCPUUsage()
    {
        var lastCpuTime = new TimeSpan(0);

        // This is ok since this is executed in a background thread
        while (true)
        {
            var cpuTime = new TimeSpan(0);

            // Get a list of all running processes in this PC
            var AllProcesses = Process.GetProcesses();
            // Sum up the total processor time of all running processes
            cpuTime = AllProcesses.Aggregate(cpuTime, (current, process) => current + process.TotalProcessorTime);

            // get the difference between the total sum of processor times
            // and the last time we called this
            var newCPUTime = cpuTime - lastCpuTime;

            // update the value of _lastCpuTime
            lastCpuTime = cpuTime;

            // The value we look for is the difference, so the processor time all processes together used
            // since the last time we called this divided by the time we waited
            // Then since the performance was optionally spread equally over all physical CPUs
            // we also divide by the physical CPU count


            CpuUsage = 100f * (float)newCPUTime.TotalSeconds / updateInterval / processorCount;

            // Wait for UpdateInterval
            Thread.Sleep(Mathf.RoundToInt(updateInterval * 1000));
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // We want only the physical cores but usually
        // this returns the twice as many virtual core count
        //
        // if this returns a wrong value for you comment this method out
        // and set the value manually
        processorCount = SystemInfo.processorCount / 2;

    }
#endif
    private void OnDisable()
    {
        _cpuThread?.Abort();
        Destroy(GetComponent<SystemCondition>());
    }
}
