namespace IdleOn;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        this.components = new System.ComponentModel.Container();
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(800, 500);
        this.Text = "IdleOn";
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
        this.MinimizeBox = true;
        this.MaximizeBox = false;
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        this.Icon = new System.Drawing.Icon("assets/screenOn2.ico");

        // Create and configure the main panel
        this.mainPanel = new System.Windows.Forms.Panel();
        this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
        this.mainPanel.Padding = new System.Windows.Forms.Padding(20);
        this.Controls.Add(this.mainPanel);

        // Create and configure the status panel
        this.statusPanel = new System.Windows.Forms.Panel();
        this.statusPanel.Dock = System.Windows.Forms.DockStyle.Fill;
        this.statusPanel.Padding = new System.Windows.Forms.Padding(10);
        this.mainPanel.Controls.Add(this.statusPanel);

        // Create and configure the monitoring status label
        this.monitoringLabel = new System.Windows.Forms.Label();
        this.monitoringLabel.AutoSize = true;
        this.monitoringLabel.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        this.monitoringLabel.Text = "Monitoring: Enabled";
        this.monitoringLabel.Location = new System.Drawing.Point(20, 20);
        this.statusPanel.Controls.Add(this.monitoringLabel);

        // Create and configure the sleep prevention status label
        this.sleepLabel = new System.Windows.Forms.Label();
        this.sleepLabel.AutoSize = true;
        this.sleepLabel.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        this.sleepLabel.Text = "Sleep Prevention: Inactive";
        this.sleepLabel.Location = new System.Drawing.Point(20, 80);
        this.statusPanel.Controls.Add(this.sleepLabel);

        // Create and configure the idle time label
        this.idleLabel = new System.Windows.Forms.Label();
        this.idleLabel.AutoSize = true;
        this.idleLabel.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        this.idleLabel.Text = "Idle Time: 0m 0s";
        this.idleLabel.Location = new System.Drawing.Point(20, 140);
        this.statusPanel.Controls.Add(this.idleLabel);

        // Create and configure the duration dropdown
        this.durationComboBox = new System.Windows.Forms.ComboBox();
        this.durationComboBox.Items.AddRange(new object[] { "30 minutes", "1 hour", "2 hours", "4 hours", "All time" });
        this.durationComboBox.SelectedIndex = 2; // Set default
        this.durationComboBox.Size = new System.Drawing.Size(300, 30);
        this.durationComboBox.Location = new System.Drawing.Point(20, 200);
        this.durationComboBox.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        this.durationComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.statusPanel.Controls.Add(this.durationComboBox);

        // Create and configure the countdown label
        this.countdownLabel = new System.Windows.Forms.Label();
        this.countdownLabel.AutoSize = true;
        this.countdownLabel.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        this.countdownLabel.Text = "Time remaining: --:--:--";
        this.countdownLabel.Location = new System.Drawing.Point(20, 250);
        this.statusPanel.Controls.Add(this.countdownLabel);

        // Create and configure the button panel
        this.buttonPanel = new System.Windows.Forms.Panel();
        this.buttonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
        this.buttonPanel.Height = 90;
        this.buttonPanel.Padding = new System.Windows.Forms.Padding(0, 0, 0, 0);
        this.mainPanel.Controls.Add(this.buttonPanel);

        // Create and configure the toggle button
        this.toggleButton = new System.Windows.Forms.Button();
        this.toggleButton.Text = "Disable Monitoring";
        this.toggleButton.Size = new System.Drawing.Size(200, 80);
        this.toggleButton.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
        this.toggleButton.BackColor = System.Drawing.Color.LightGreen;
        this.toggleButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.toggleButton.Click += new System.EventHandler(this.ToggleButton_Click);
        this.toggleButton.Location = new System.Drawing.Point((this.buttonPanel.Width - this.toggleButton.Width) / 2, 0);
        this.buttonPanel.Controls.Add(this.toggleButton);
    }

    private System.Windows.Forms.Panel mainPanel;
    private System.Windows.Forms.Panel buttonPanel;
    private System.Windows.Forms.Panel statusPanel;
    private System.Windows.Forms.Button toggleButton;
    private System.Windows.Forms.Label monitoringLabel;
    private System.Windows.Forms.Label sleepLabel;
    private System.Windows.Forms.Label idleLabel;
    private System.Windows.Forms.ComboBox durationComboBox;
    private System.Windows.Forms.Label countdownLabel;

    #endregion
}
