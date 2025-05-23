using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace IdleOn;

public partial class Form1 : Form
{
    private System.Windows.Forms.Timer activityTimer;
    private DateTime lastActivityTime;
    private const int IDLE_THRESHOLD_MINUTES = 2;
    private bool isPreventingSleep = false;
    private bool isMonitoringEnabled = true;

    // Import required Windows API functions
    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);

    [Flags]
    public enum EXECUTION_STATE : uint
    {
        ES_AWAYMODE_REQUIRED = 0x00000040,
        ES_CONTINUOUS = 0x80000000,
        ES_DISPLAY_REQUIRED = 0x00000002,
        ES_SYSTEM_REQUIRED = 0x00000001
    }

    public Form1()
    {
        InitializeComponent();
        InitializeActivityMonitoring();
    }

    private void InitializeActivityMonitoring()
    {
        // Create and configure the timer
        activityTimer = new System.Windows.Forms.Timer();
        activityTimer.Interval = 1000; // Check every second
        activityTimer.Tick += ActivityTimer_Tick;
        activityTimer.Start();

        // Initialize last activity time
        lastActivityTime = DateTime.Now;

        // Set up activity monitoring
        this.MouseMove += Form1_MouseMove;
        this.KeyPress += Form1_KeyPress;
    }

    private void Form1_MouseMove(object sender, MouseEventArgs e)
    {
        if (isMonitoringEnabled)
        {
            UpdateLastActivity();
        }
    }

    private void Form1_KeyPress(object sender, KeyPressEventArgs e)
    {
        if (isMonitoringEnabled)
        {
            UpdateLastActivity();
        }
    }

    private void UpdateLastActivity()
    {
        lastActivityTime = DateTime.Now;
        if (isPreventingSleep)
        {
            StopPreventingSleep();
        }
    }

    private void ActivityTimer_Tick(object sender, EventArgs e)
    {
        if (!isMonitoringEnabled)
        {
            if (isPreventingSleep)
            {
                StopPreventingSleep();
            }
            return;
        }

        TimeSpan idleTime = DateTime.Now - lastActivityTime;
        
        if (idleTime.TotalMinutes >= IDLE_THRESHOLD_MINUTES && !isPreventingSleep)
        {
            StartPreventingSleep();
        }
        else if (idleTime.TotalMinutes < IDLE_THRESHOLD_MINUTES && isPreventingSleep)
        {
            StopPreventingSleep();
        }

        // Update status
        UpdateStatus(idleTime);
    }

    private void StartPreventingSleep()
    {
        SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS | 
                              EXECUTION_STATE.ES_SYSTEM_REQUIRED | 
                              EXECUTION_STATE.ES_DISPLAY_REQUIRED);
        isPreventingSleep = true;
    }

    private void StopPreventingSleep()
    {
        SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS);
        isPreventingSleep = false;
    }

    private void UpdateStatus(TimeSpan idleTime)
    {
        string status = !isMonitoringEnabled ? "Monitoring Disabled" :
                      isPreventingSleep ? "Preventing Sleep" : "Active";
        string idleTimeStr = $"{Math.Floor(idleTime.TotalMinutes)}m {idleTime.Seconds}s";
        
        this.Text = $"IdleOn - {status} (Idle: {idleTimeStr})";
        
        // Update the status labels
        monitoringLabel.Text = $"Monitoring: {(isMonitoringEnabled ? "Enabled" : "Disabled")}";
        sleepLabel.Text = $"Sleep Prevention: {(isPreventingSleep ? "Active" : "Inactive")}";
        idleLabel.Text = $"Idle Time: {idleTimeStr}";
    }

    private void ToggleButton_Click(object sender, EventArgs e)
    {
        isMonitoringEnabled = !isMonitoringEnabled;
        
        if (!isMonitoringEnabled)
        {
            toggleButton.Text = "Enable Monitoring";
            toggleButton.BackColor = System.Drawing.Color.LightCoral;
            if (isPreventingSleep)
            {
                StopPreventingSleep();
            }
        }
        else
        {
            toggleButton.Text = "Disable Monitoring";
            toggleButton.BackColor = System.Drawing.Color.LightGreen;
            lastActivityTime = DateTime.Now;
        }

        UpdateStatus(DateTime.Now - lastActivityTime);
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        StopPreventingSleep();
        base.OnFormClosing(e);
    }
}
