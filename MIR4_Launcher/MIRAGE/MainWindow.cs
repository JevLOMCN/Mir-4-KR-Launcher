using Microsoft.Win32;
using MIRAGE.Helper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using xamlSpinnersWPF;

namespace MIRAGE
{
  public class MainWindow : Window, IComponentConnector
  {
    private CallApi _apiWorker;
    private string _patchApi_url = "https://api.mir4.co.kr/launcher";
    private string GAME_PROCESS_NAME = "Mir4";
    private int _LAUNCHER_VER = 5;
    private string _LAUNCHER_NAME = "MIR4_GameRun.exe";
    private string _LAUNCHER_REQUIRED_URL = "https://logplatformbeta.blob.core.windows.net/launcher/required/";
    internal Grid bgGrid;
    internal ImageBrush BasicBack;
    internal ucSpinnerDotCircle loadSpinner;
    private bool _contentLoaded;

    public MainWindow() => this.InitializeComponent();

    private void DoLauncher()
    {
      try
      {
        string installPath = this.getInstallPath();
        Console.WriteLine("App Path : " + installPath);
        string str = installPath + "\\" + this._LAUNCHER_NAME;
        Console.WriteLine("Exec Launcher : " + str);
        if (!File.Exists(str))
        {
          this.ShowPopup("The launcher’s required files are missing.\r\nPlease reinstall the launcher.");
          Application.Current.Shutdown();
        }
        else
        {
          Process.Start(new ProcessStartInfo(str, "-LauncherToken=uwrj2NLZS7teVeYs+3OmL8TwxmiVHmh8AXXrak0mFTIuvVJAJEBzcTo9P+cCtu+i -LaunchGameInstanceID=b60fdd383b28474ca1458b98fc8aed76")
          {
            CreateNoWindow = true,
            RedirectStandardOutput = false,
            UseShellExecute = false
          });
          Application.Current.Shutdown();
        }
      }
      catch (Exception ex)
      {
        SentryApi.SendException(ex);
      }
    }

    private string UTF82EUCKR(string strUTF8)
    {
      ASCIIEncoding asciiEncoding = new ASCIIEncoding();
      return asciiEncoding.GetString(Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding(asciiEncoding.CodePage), Encoding.UTF8.GetBytes(strUTF8)));
    }

    private string EUCKR2UTF8(string strEUCKR)
    {
      ASCIIEncoding asciiEncoding = new ASCIIEncoding();
      return Encoding.UTF8.GetString(Encoding.Convert(Encoding.GetEncoding(asciiEncoding.CodePage), Encoding.UTF8, Encoding.GetEncoding(asciiEncoding.CodePage).GetBytes(strEUCKR)));
    }

    private void ShowPopup(string _msg)
    {
      try
      {
        popup popup = new popup(1, _msg);
        popup.Owner = (Window) this;
        popup.ShowDialog();
        popup.Close();
      }
      catch (Exception ex)
      {
        SentryApi.SendException(ex);
      }
    }

    private void CloseLauncher()
    {
      Environment.Exit(0);
      Process.GetCurrentProcess().Kill();
      this.Close();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      if (this.IsGameRun())
        this.DoLauncher();
      else
        this.GetLauncherRequired();
    }

    private bool IsGameRun()
    {
      bool flag = false;
      try
      {
        foreach (Process process in Process.GetProcesses())
        {
          if (process.ProcessName.Equals(this.GAME_PROCESS_NAME))
          {
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

    private void GetLauncherRequired()
    {
      try
      {
        string installPath = this.getInstallPath();
        Console.WriteLine("getInstallPath : " + installPath);
        string[] strArray = new string[8]
        {
          "AesTool.dll",
          "Newtonsoft.Json.dll",
          "Newtonsoft.Json.xml",
          "Sentry.dll",
          "Sentry.PlatformAbstractions.dll",
          "Sentry.Protocol.dll",
          "Sentry.Protocol.xml",
          "Sentry.xml"
        };
        List<DownloadFileInfo> downloadFileInfoList = new List<DownloadFileInfo>();
        for (int index = 0; index < strArray.Length; ++index)
        {
          string path = installPath + "\\" + strArray[index];
          if (!File.Exists(path))
          {
            Console.WriteLine("Required file missing: " + path);
            downloadFileInfoList.Add(new DownloadFileInfo()
            {
              path = strArray[index],
              URL = this._LAUNCHER_REQUIRED_URL + strArray[index]
            });
          }
          else
            Console.WriteLine("Required file found: " + path);
        }
        if (downloadFileInfoList.Count > 0)
        {
          Console.WriteLine("Required files to download: " + downloadFileInfoList.Count.ToString());
          DownloadFiles downloadFiles = new DownloadFiles(downloadFileInfoList.ToArray(), "", installPath, "\\", "2");
          downloadFiles.Owner = (Window) this;
          downloadFiles.ShowDialog();
          downloadFiles.Close();
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
        SentryApi.SendException(ex);
      }
      this.GetLauncherVersion();
    }

    private void GetLauncherVersion()
    {
      try
      {
        this._apiWorker = new CallApi(this.getPatchApiUrl() + "/getLauncherInfo", new Dictionary<string, string>()
        {
          {
            "ServiceID",
            App.MIR4_GAME_INDEX
          }
        });
        this._apiWorker.ResultEvent += new CallApi.ResultEventHandler(this._worker_LauncherVersion_RunWorkerCompleted);
        this._apiWorker.DoCallPostAsync();
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
        SentryApi.SendException(ex);
        this.DoLauncher();
      }
    }

    private void _worker_LauncherVersion_RunWorkerCompleted()
    {
      try
      {
        string installPath = this.getInstallPath();
        Console.WriteLine("App Path : " + installPath);
        Console.WriteLine("_resultString : " + this._apiWorker._resultString);
        if (this._apiWorker._resultString != "-1")
        {
          JObject jobject = JObject.Parse(this._apiWorker._resultString);
          if (int.Parse(jobject["Code"].ToString()) == 200)
          {
            this.getLauncherVersion();
            int num1 = int.Parse(jobject["LauncherVersion"].ToString());
            string serverUrl = jobject["URL"].ToString();
            Console.WriteLine("Launcher Server Version : " + num1.ToString());
            Console.WriteLine("Launcher Local Version : " + this._LAUNCHER_VER.ToString());
            if (num1 > this._LAUNCHER_VER)
            {
              string str = "";
              long num2 = 0;
              try
              {
                str = jobject["FileSize"].ToString();
                num2 = Convert.ToInt64(str);
              }
              catch (Exception ex)
              {
              }
              string directory = installPath.Substring(0, 3);
              Console.WriteLine("installDiskName : " + directory);
              long diskFreeSpace = new UtilDrive().getDiskFreeSpace(directory);
              Console.WriteLine("Download Size : " + num2.ToString());
              Console.WriteLine("Disk Free Size : " + diskFreeSpace.ToString());
              if (num2 >= diskFreeSpace)
              {
                this.ShowPopup($"Not enough disk space.\r\nYou need {str} MB.\r\nPlease check your disk space and restart.");
                Environment.Exit(0);
                Process.GetCurrentProcess().Kill();
                this.Close();
              }
              this.ShowPopup("A launcher update is available.\r\nThe launcher will restart after the update.");
              DirectoryInfo directoryInfo = new DirectoryInfo(installPath);
              if (!directoryInfo.Exists)
                directoryInfo.Create();
              DownloadFiles downloadFiles = new DownloadFiles(JsonConvert.DeserializeObject<List<DownloadFileInfo>>(jobject["FileData"].ToString()).ToArray(), serverUrl, installPath, "", "1");
              downloadFiles.Owner = (Window) this;
              bool? nullable1 = downloadFiles.ShowDialog();
              downloadFiles.Close();
              bool? nullable2 = nullable1;
              bool flag = true;
              if (nullable2.GetValueOrDefault() == flag & nullable2.HasValue)
              {
                GameData gameData = App.m_mapGameData[App.MIR4_GAME_INDEX];
                GameInfo gameInfo = new GameInfo();
                gameData.launcher_version = num1.ToString();
                string miR4GameIndex = App.MIR4_GAME_INDEX;
                GameData _mirdata = gameData;
                gameInfo.WriteGameData(miR4GameIndex, _mirdata);
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
        SentryApi.SendException(ex);
      }
      this.DoLauncher();
    }

    private string getInstallPath()
    {
      string installPath = "";
      try
      {
        string name = "SOFTWARE\\WEMADE\\MIR4_Launcher";
        RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(name, true);
        if (registryKey != null)
        {
          installPath = registryKey.GetValue("ProgramFolder").ToString();
          registryKey.Close();
        }
      }
      catch (Exception ex)
      {
      }
      return installPath;
    }

    private string getPatchApiUrl()
    {
      string patchApiUrl = "";
      try
      {
        patchApiUrl = ConfigurationManager.AppSettings["patch"];
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
      }
      if (patchApiUrl == null || patchApiUrl.Length == 0)
        patchApiUrl = this._patchApi_url;
      Console.WriteLine("PatchURL : " + patchApiUrl);
      return patchApiUrl;
    }

    private string getMode()
    {
      string str = "";
      try
      {
        str = ConfigurationManager.AppSettings["mode"];
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
      }
      if (str == null || str.Length == 0)
        str = "Release";
      string mode = str + "GameVersion";
      Console.WriteLine("Launche Mode : " + mode);
      return mode;
    }

    private void getLauncherVersion()
    {
      GameData gameData = new GameData();
      GameInfo gameInfo = new GameInfo();
      (bool flag, GameData _mirdata) = gameInfo.checkGameDataFileAndGetData(App.MIR4_GAME_INDEX);
      if (flag && int.Parse(_mirdata.version) > 0)
        App.m_mapGameData.Add(App.MIR4_GAME_INDEX, _mirdata);
      if (_mirdata.launcher_version.Length == 0)
      {
        _mirdata.launcher_version = this._LAUNCHER_VER.ToString() ?? "";
        gameInfo.WriteGameData(App.MIR4_GAME_INDEX, _mirdata);
      }
      else
        this._LAUNCHER_VER = int.Parse(_mirdata.launcher_version);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/MIR4_Launcher;component/mainwindow.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    internal Delegate _CreateDelegate(Type delegateType, string handler)
    {
      return Delegate.CreateDelegate(delegateType, (object) this, handler);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          ((FrameworkElement) target).Loaded += new RoutedEventHandler(this.Window_Loaded);
          break;
        case 2:
          this.bgGrid = (Grid) target;
          break;
        case 3:
          this.BasicBack = (ImageBrush) target;
          break;
        case 4:
          this.loadSpinner = (ucSpinnerDotCircle) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
