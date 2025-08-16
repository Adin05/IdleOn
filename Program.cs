using System;
using System.Threading;
using System.Windows.Forms;

namespace IdleOn;

static class Program
{
    private static Mutex? mutex = null;

    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        const string appName = "IdleOnSingleInstance";
        bool createdNew;

        mutex = new Mutex(true, appName, out createdNew);

        if (!createdNew)
        {
            // Application is already running
            MessageBox.Show("IdleOn is already running!", "IdleOn", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        try
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }
        finally
        {
            mutex?.ReleaseMutex();
            mutex?.Dispose();
        }
    }    
}