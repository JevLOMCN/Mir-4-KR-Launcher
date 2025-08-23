using MIRAGE.Helper;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading;
using System.Windows;

namespace MIRAGE
{
  public class App : Application
  {
    public const uint HWND_BROADCAST = 65535;
    public static string MIR4_GAME_INDEX = "720";
    public static Dictionary<string, GameData> m_mapGameData = new Dictionary<string, GameData>();
    public static GAME_STATUS _GAME_STATUS;
    public static GAME_STATUS _GAME_STATUS1;
    private Mutex mutex;
    private static string _UPDATER_PROCESS = "MIR4_Launcher";
    private static string _LAUNCHER_PROCESS = "MIR4_GameRun";
    private static string _LAUNCHER_TITLE = "미르4";

    [DllImport("User32")]
    private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll")]
    public static extern void SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("User32.dll")]
    public static extern bool MoveWindow(
      IntPtr handle,
      int x,
      int y,
      int width,
      int height,
      bool redraw);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool GetWindowRect(IntPtr hwnd, out App.RECT lpRect);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern uint RegisterWindowMessage(string lpString);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern bool PostMessage(IntPtr hWnd, uint Msg, uint wParam, uint lParam);

    protected override void OnStartup(StartupEventArgs e)
    {
      bool createdNew;
      this.mutex = new Mutex(true, App._UPDATER_PROCESS, out createdNew);
      if (createdNew)
      {
        if (!App.IsRunningAsAdministrator())
        {
          Console.WriteLine("Not running as administrator");
          Process.Start(new ProcessStartInfo(Assembly.GetEntryAssembly().CodeBase)
          {
            UseShellExecute = true,
            Verb = "runas"
          });
          Application.Current.Shutdown();
        }
        Console.WriteLine("Running as administrator");
        if (App.IsLauncherRun())
          Environment.Exit(0);
        else
          base.OnStartup(e);
      }
      else
      {
        try
        {
          Environment.Exit(0);
        }
        catch (Exception ex)
        {
          SentryApi.SendException(ex);
        }
      }
    }

    public static bool IsRunningAsAdministrator()
    {
      return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
    }

    private static bool IsLauncherRun()
    {
      bool flag = false;
      try
      {
        foreach (Process process in Process.GetProcesses())
        {
          if (process.ProcessName.Equals(App._LAUNCHER_PROCESS))
          {
            IntPtr window = App.FindWindow((string) null, App._LAUNCHER_TITLE);
            if (window != IntPtr.Zero)
              App.PostMessage((IntPtr) (long) ushort.MaxValue, App.RegisterWindowMessage("Mir4 Launcher Show Message"), (uint) (int) window, 100U);
            flag = true;
            break;
          }
        }
      }
      catch (Exception ex)
      {
        SentryApi.SendException(ex);
      }
      return flag;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      this.StartupUri = new Uri("MainWindow.xaml", UriKind.Relative);
    }

    [STAThread]
    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public static void Main()
    {
      App app = new App();
      app.InitializeComponent();
      app.Run();
    }

    public struct RECT
    {
      public int Left;
      public int Top;
      public int Right;
      public int Bottom;

      public RECT(int left, int top, int right, int bottom)
      {
        this.Left = left;
        this.Top = top;
        this.Right = right;
        this.Bottom = bottom;
      }

      public RECT(Rectangle r)
        : this(r.Left, r.Top, r.Right, r.Bottom)
      {
      }

      public int X
      {
        get => this.Left;
        set
        {
          this.Right -= this.Left - value;
          this.Left = value;
        }
      }

      public int Y
      {
        get => this.Top;
        set
        {
          this.Bottom -= this.Top - value;
          this.Top = value;
        }
      }

      public int Height
      {
        get => this.Bottom - this.Top;
        set => this.Bottom = value + this.Top;
      }

      public int Width
      {
        get => this.Right - this.Left;
        set => this.Right = value + this.Left;
      }

      public System.Drawing.Point Location
      {
        get => new System.Drawing.Point(this.Left, this.Top);
        set
        {
          this.X = value.X;
          this.Y = value.Y;
        }
      }

      public System.Drawing.Size Size
      {
        get => new System.Drawing.Size(this.Width, this.Height);
        set
        {
          this.Width = value.Width;
          this.Height = value.Height;
        }
      }

      public static implicit operator Rectangle(App.RECT r)
      {
        return new Rectangle(r.Left, r.Top, r.Width, r.Height);
      }

      public static implicit operator App.RECT(Rectangle r) => new App.RECT(r);

      public static bool operator ==(App.RECT r1, App.RECT r2) => r1.Equals(r2);

      public static bool operator !=(App.RECT r1, App.RECT r2) => !r1.Equals(r2);

      public bool Equals(App.RECT r)
      {
        return r.Left == this.Left && r.Top == this.Top && r.Right == this.Right && r.Bottom == this.Bottom;
      }

      public override bool Equals(object obj)
      {
        switch (obj)
        {
          case App.RECT r1:
            return this.Equals(r1);
          case Rectangle r2:
            return this.Equals(new App.RECT(r2));
          default:
            return false;
        }
      }

      public override int GetHashCode() => ((Rectangle) this).GetHashCode();

      public override string ToString()
      {
        return string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{{Left={0},Top={1},Right={2},Bottom={3}}}", (object) this.Left, (object) this.Top, (object) this.Right, (object) this.Bottom);
      }
    }
  }
}
