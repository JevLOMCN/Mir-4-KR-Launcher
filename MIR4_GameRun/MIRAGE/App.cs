using Microsoft.Win32;
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
      this.CheckWindowsVersion();
      bool createdNew;
      this.mutex = new Mutex(true, App._LAUNCHER_PROCESS, out createdNew);
      if (createdNew)
      {
        if (e.Args != null && e.Args.Length < 2)
        {
          this.ShowPopup("런처에서 실행해 주세요");
          Application.Current.Shutdown();
        }
        else if (!e.Args[0].Contains("LauncherToken") || !e.Args[1].Contains("LaunchGameInstanceID"))
        {
          this.ShowPopup("런처에서 실행해 주세요");
          Application.Current.Shutdown();
        }
        else if (!App.IsRunningAsAdministrator())
        {
          Console.WriteLine("관리자 권한이 아님");
          Process.Start(new ProcessStartInfo(Assembly.GetEntryAssembly().CodeBase)
          {
            UseShellExecute = true,
            Verb = "runas"
          });
          Application.Current.Shutdown();
        }
        else
        {
          Console.WriteLine("관리자 권한임");
          base.OnStartup(e);
        }
      }
      else
      {
        try
        {
          foreach (Process process in Process.GetProcesses())
          {
            Console.WriteLine(process.ProcessName);
            if (process.ProcessName.Equals(App._LAUNCHER_PROCESS))
            {
              Console.WriteLine("=========> Process Name : " + process.ProcessName);
              IntPtr window = App.FindWindow((string) null, App._LAUNCHER_TITLE);
              if (window != IntPtr.Zero)
              {
                Console.WriteLine("=========> Title Find : " + App._LAUNCHER_TITLE);
                App.PostMessage((IntPtr) (long) ushort.MaxValue, App.RegisterWindowMessage("Mir4 Launcher Show Message"), (uint) (int) window, 100U);
              }
              Application.Current.Shutdown();
              break;
            }
          }
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

    private void ShowPopup(string _msg)
    {
      try
      {
        popup popup = new popup(1, _msg);
        popup.ShowDialog();
        popup.Close();
      }
      catch (Exception ex)
      {
        SentryApi.SendException(ex);
      }
    }

    private void CheckWindowsVersion()
    {
      float num1 = 6.3f;
      string name = "SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion";
      bool flag = true;
      RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(name, true);
      if (registryKey != null)
      {
        try
        {
          if ((double) float.Parse(registryKey.GetValue("CurrentVersion").ToString()) >= (double) num1)
            flag = false;
        }
        catch
        {
        }
        registryKey.Close();
      }
      if (!flag)
        return;
      int num2 = (int) MessageBox.Show("보안 컴플라이언스를 준수하기 위해 TLS 1.2로 전환됩니다.\r\nTLS 1.2가 지원되는 Windows 8.1이상으로 OS를 업그레이드를 권장합니다.\r\nWindows 8.0이하의 OS사용자는 TLS 1.2가 지원되도록 직접 세팅이 필요합니다.", App._LAUNCHER_TITLE);
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
