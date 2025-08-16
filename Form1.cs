using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Drawing;
using Microsoft.Win32;

namespace IdleOn;

public partial class Form1 : Form
{
    private System.Windows.Forms.Timer activityTimer;
    private System.Windows.Forms.Timer countdownTimer;
    private DateTime lastActivityTime;
    private DateTime endTime;
    private const int IDLE_THRESHOLD_MINUTES = 2;
    private bool isPreventingSleep = false;
    private bool isMonitoringEnabled = true;
    private bool isCountdownActive = false;
    private bool isComputerLocked = false;
    private NotifyIcon trayIcon;
    private ContextMenuStrip trayMenu;

    // Import required Windows API functions
    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);

    [DllImport("user32.dll")]
    static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    static extern int GetWindowText(IntPtr hWnd, System.Text.StringBuilder text, int count);

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
        this.Icon = new Icon("assets/screenOn2.ico");
        InitializeActivityMonitoring();
        InitializeCountdownTimer();
        durationComboBox.SelectedIndexChanged += DurationComboBox_SelectedIndexChanged;
        InitializeTray();
        this.Resize += Form1_Resize;
        SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;
    }

    private void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
    {
        if (e.Reason == SessionSwitchReason.SessionLock)
        {
            isComputerLocked = true;
            if (isPreventingSleep)
            {
                StopPreventingSleep();
            }
        }
        else if (e.Reason == SessionSwitchReason.SessionUnlock)
        {
            isComputerLocked = false;
            lastActivityTime = DateTime.Now;
        }
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

    private void InitializeCountdownTimer()
    {
        countdownTimer = new System.Windows.Forms.Timer();
        countdownTimer.Interval = 1000; // Update every second
        countdownTimer.Tick += CountdownTimer_Tick;
    }

    private void InitializeTray()
    {
        trayMenu = new ContextMenuStrip();
        trayMenu.Items.Add("Restore", null, (s, e) => RestoreFromTray());
        trayMenu.Items.Add("Exit", null, (s, e) => Application.Exit());

        trayIcon = new NotifyIcon();
        trayIcon.Text = "IdleOn";
        trayIcon.Icon = new Icon("assets/screenOn2.ico");
        trayIcon.ContextMenuStrip = trayMenu;
        trayIcon.Visible = false;
        trayIcon.DoubleClick += (s, e) => RestoreFromTray();
    }

    private void DurationComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (!isMonitoringEnabled) return;
        
        // If currently preventing sleep, restart the countdown with new duration
        if (isPreventingSleep)
        {
            StartCountdown();
        }
        else
        {
            StopCountdown();
        }
    }

    private void CountdownTimer_Tick(object sender, EventArgs e)
    {
        if (!isCountdownActive || !isPreventingSleep) return;

        TimeSpan remaining = endTime - DateTime.Now;
        if (remaining.TotalSeconds <= 0)
        {
            countdownTimer.Stop();
            isCountdownActive = false;
            isMonitoringEnabled = false;
            StopPreventingSleep();
            toggleButton.Text = "Enable Monitoring";
            toggleButton.BackColor = System.Drawing.Color.LightCoral;
            countdownLabel.Text = "Time remaining: Expired";
            UpdateStatus(DateTime.Now - lastActivityTime);
            return;
        }

        countdownLabel.Text = $"Time remaining: {remaining.Hours:D2}:{remaining.Minutes:D2}:{remaining.Seconds:D2}";
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
            StopCountdown();
        }
    }

    private void ActivityTimer_Tick(object sender, EventArgs e)
    {
        if (!isMonitoringEnabled || isComputerLocked)
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
            StartCountdown(); // Start countdown when idle is detected
        }
        else if (idleTime.TotalMinutes < IDLE_THRESHOLD_MINUTES && isPreventingSleep)
        {
            StopPreventingSleep();
            StopCountdown(); // Stop countdown when activity resumes
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

    private void StartCountdown()
    {
        string selectedDuration = durationComboBox.SelectedItem.ToString();
        if (selectedDuration == "All time")
        {
            isCountdownActive = false;
            countdownTimer.Stop();
            countdownLabel.Text = "Time remaining: Indefinite";
            return;
        }

        int minutes = selectedDuration switch
        {
            "30 minutes" => 30,
            "1 hour" => 60,
            "2 hours" => 120,
            "4 hours" => 240,
            _ => 30
        };

        endTime = DateTime.Now.AddMinutes(minutes);
        isCountdownActive = true;
        countdownTimer.Start();
        countdownLabel.Text = $"Time remaining: {minutes:D2}:00:00"; // Show initial time immediately
    }

    private void StopCountdown()
    {
        countdownTimer.Stop();
        isCountdownActive = false;
        countdownLabel.Text = "Time remaining: --:--:--";
    }

    private void UpdateStatus(TimeSpan idleTime)
    {
        string status = !isMonitoringEnabled ? "Monitoring Disabled" :
                      isComputerLocked ? "Computer Locked" :
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
            StopCountdown();
        }
        else
        {
            toggleButton.Text = "Disable Monitoring";
            toggleButton.BackColor = System.Drawing.Color.LightGreen;
            lastActivityTime = DateTime.Now;
            StopCountdown();
        }

        UpdateStatus(DateTime.Now - lastActivityTime);
    }

    private void Form1_Resize(object sender, EventArgs e)
    {
        if (this.WindowState == FormWindowState.Minimized)
        {
            HideToTray();
        }
    }

    private void HideToTray()
    {
        this.Hide();
        trayIcon.Visible = true;
    }

    private void RestoreFromTray()
    {
        this.Show();
        this.WindowState = FormWindowState.Normal;
        this.BringToFront();
        trayIcon.Visible = false;
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        StopPreventingSleep();
        countdownTimer.Stop();
        trayIcon.Visible = false;
        SystemEvents.SessionSwitch -= SystemEvents_SessionSwitch;
        base.OnFormClosing(e);
    }
}
