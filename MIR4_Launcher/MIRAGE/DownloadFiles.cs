using MIRAGE.Helper;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace MIRAGE
{
  public class DownloadFiles : Window, IComponentConnector
  {
    private DownloadFileInfo[] _arrDownloadFileInfo;
    private string _type;
    private string _localPath;
    private string _serverUrl;
    private double _downloadTotalByte;
    private double _downloadByte;
    private int _downloadCount;
    private string _tempPath;
    internal Label label_title_string;
    internal Label label_check_string;
    internal ProgressBar pbStatus;
    private bool _contentLoaded;

    public DownloadFiles(
      DownloadFileInfo[] arrDownloadFileInfo,
      string serverUrl,
      string localPath,
      string tempPath,
      string type)
    {
      this._arrDownloadFileInfo = arrDownloadFileInfo;
      this._tempPath = tempPath;
      this._localPath = localPath;
      this._serverUrl = serverUrl;
      this._type = type;
      this._downloadCount = 0;
      this.InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e) => this.Download();

    public void Download()
    {
      try
      {
        string path = this._arrDownloadFileInfo[this._downloadCount].path;
        string uriString = this._serverUrl + path;
        string str1 = this._localPath + "\\" + path;
        string str2 = this._localPath + "\\" + path;
        if (str1.LastIndexOf("/") > 0)
        {
          DirectoryInfo directoryInfo = new DirectoryInfo(str1.Substring(0, str1.LastIndexOf('/')));
          if (!directoryInfo.Exists)
            directoryInfo.Create();
        }
        if ("2".Equals(this._type))
          uriString = this._arrDownloadFileInfo[this._downloadCount].URL;
        this.label_check_string.Content = (object) ("[" + (this._downloadCount + 1).ToString() + " / " + this._arrDownloadFileInfo.Length.ToString() + "]" + path);
        WebClient webClient = new WebClient();
        Uri address = new Uri(uriString);
        webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(this.Completed);
        string fileName = str1.Replace("/", "\\");
        Console.WriteLine("Download URL : " + address?.ToString());
        Console.WriteLine("Download Path : " + fileName);
        webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(this.DownloadProgress);
        webClient.DownloadFileAsync(address, fileName);
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
        this.DialogResult = new bool?(false);
        SentryApi.SendException(ex);
        this.Close();
      }
    }

    private void DownloadProgress(object sender, DownloadProgressChangedEventArgs e)
    {
      this.pbStatus.Value = (double) e.ProgressPercentage;
    }

    private void Completed(object sender, AsyncCompletedEventArgs e)
    {
      ++this._downloadCount;
      this.pbStatus.Value = 100.0;
      if (this._downloadCount >= this._arrDownloadFileInfo.Length)
      {
        this.DialogResult = new bool?(true);
        this.Close();
      }
      else
        this.Download();
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/MIR4_Launcher;component/downloadfiles.xaml", UriKind.Relative));
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
          this.label_title_string = (Label) target;
          break;
        case 3:
          this.label_check_string = (Label) target;
          break;
        case 4:
          this.pbStatus = (ProgressBar) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
