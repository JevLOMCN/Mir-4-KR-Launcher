using MIRAGE.Helper;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace MIRAGE
{
  public class DownloadGameFile : Window, IComponentConnector
  {
    private long _DownloadFileLength;
    private long _TotalBytesToReceive;
    private int _DownloadPer;
    private string _DownloadFullName = "";
    internal Label LaDispPer;
    internal Label LaDispPerUnit;
    internal Label LaDispPerTxt;
    internal Label LaDownloadFile;
    internal Label LaDownloadLength;
    internal ProgressBar ImgProgress;
    private bool _contentLoaded;

    public string localPath { get; set; }

    public string serverUrl { get; set; }

    public string unzipPath { get; set; }

    public DownloadGameFile() => this.InitializeComponent();

    private void Window_Loaded(object sender, RoutedEventArgs e) => this.Download();

    public void Download()
    {
      try
      {
        string fileName = this.GetFileName(this.serverUrl);
        this.LaDispPer.Content = (object) "0";
        this.LaDispPerTxt.Content = (object) "다운로드중...";
        this.LaDownloadFile.Content = (object) ("다운로드 파일 : " + fileName);
        this.ImgProgress.Value = 0.0;
        this._DownloadFullName = this.localPath + fileName;
        this.DispByte();
        Thread.Sleep(10);
        WebClient webClient = new WebClient();
        Uri address = new Uri(this.serverUrl);
        webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(this.Completed);
        webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(this.DownloadProgress);
        webClient.DownloadFileAsync(address, this._DownloadFullName);
      }
      catch (Exception ex)
      {
        this.DialogResult = new bool?(false);
        SentryApi.SendException(ex);
        this.Close();
      }
    }

    public string GetFileName(string url)
    {
      string fileName = "";
      try
      {
        fileName = ((IEnumerable<string>) new Uri(url).Segments).Last<string>();
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
        SentryApi.SendException(ex);
      }
      return fileName;
    }

    public void DispByte()
    {
      this.LaDownloadLength.Content = (object) ("( " + string.Format("{0:#,##0}", (object) this._DownloadFileLength) + " / " + string.Format("{0:#,##0}", (object) this._TotalBytesToReceive) + " )");
    }

    public void DispFIleCount(int totalFileCount, int unzipFileCount)
    {
      this.LaDownloadLength.Content = (object) ("( " + string.Format("{0:#,##0}", (object) totalFileCount) + " / " + string.Format("{0:#,##0}", (object) unzipFileCount) + " )");
    }

    private void DownloadProgress(object sender, DownloadProgressChangedEventArgs e)
    {
      this._DownloadPer = e.ProgressPercentage;
      this._TotalBytesToReceive = e.BytesReceived;
      this._DownloadFileLength = e.TotalBytesToReceive;
      this.DispByte();
      this.ImgProgress.Value = (double) this._DownloadPer;
      this.LaDispPer.Content = (object) this._DownloadPer;
    }

    private void Completed(object sender, AsyncCompletedEventArgs e)
    {
      if (e.Error != null)
      {
        this.DialogResult = new bool?(false);
        this.Close();
      }
      else
      {
        this._DownloadPer = 100;
        this._TotalBytesToReceive = this._DownloadFileLength;
        this.ImgProgress.Value = 100.0;
        this.LaDispPer.Content = (object) "100";
        this.DispByte();
        Thread.Sleep(100);
        try
        {
          FileInfo file = new FileInfo(this._DownloadFullName);
          while (this.IsFileLocked(file))
            Thread.Sleep(1000);
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.ToString());
          SentryApi.SendException(ex);
        }
        this.ExtractZIPFileAsync(this._DownloadFullName, this.unzipPath);
      }
    }

    protected virtual bool IsFileLocked(FileInfo file)
    {
      FileStream fileStream = (FileStream) null;
      try
      {
        fileStream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
      }
      catch (IOException ex)
      {
        return true;
      }
      finally
      {
        fileStream?.Close();
      }
      return false;
    }

    public async Task ExtractZIPFileAsync(string zipFilePath, string backupFolder)
    {
      DownloadGameFile downloadGameFile = this;
      int totalFileCount = 0;
      int unzipFileCount = 0;
      downloadGameFile.LaDispPer.Content = (object) "0";
      downloadGameFile.LaDispPerTxt.Content = (object) "압축 푸는중...";
      downloadGameFile.LaDownloadFile.Content = (object) "파일명 : ";
      downloadGameFile.ImgProgress.Value = 0.0;
      try
      {
        using (ZipArchive zipArchive = ZipFile.OpenRead(zipFilePath))
        {
          foreach (ZipArchiveEntry entry in zipArchive.Entries)
            ++totalFileCount;
          downloadGameFile.DispFIleCount(totalFileCount, unzipFileCount);
          foreach (ZipArchiveEntry entry in zipArchive.Entries)
          {
            ZipArchiveEntry zipArchiveEntry = entry;
            string directoryName = Path.GetDirectoryName(Path.Combine(backupFolder, zipArchiveEntry.FullName));
            if (!Directory.Exists(directoryName))
              Directory.CreateDirectory(directoryName);
            if (!(zipArchiveEntry.Name == string.Empty))
            {
              downloadGameFile.LaDownloadFile.Content = (object) ("파일명 : " + zipArchiveEntry.Name);
              await Task.Run((Action) (() => zipArchiveEntry.ExtractToFile(Path.Combine(backupFolder, zipArchiveEntry.FullName), true)));
              ++unzipFileCount;
              int int32 = Convert.ToInt32(100 * unzipFileCount / totalFileCount);
              downloadGameFile.DispFIleCount(totalFileCount, unzipFileCount);
              downloadGameFile.ImgProgress.Value = (double) int32;
              downloadGameFile.LaDispPer.Content = (object) int32;
            }
          }
        }
        downloadGameFile.DialogResult = new bool?(true);
        try
        {
          FileInfo file = new FileInfo(downloadGameFile._DownloadFullName);
          while (downloadGameFile.IsFileLocked(file))
            Thread.Sleep(1000);
          file.Delete();
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.ToString());
          SentryApi.SendException(ex);
        }
      }
      catch (Exception ex)
      {
        downloadGameFile.DialogResult = new bool?(false);
        SentryApi.SendException(ex);
      }
      downloadGameFile.Close();
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/MIR4_Launcher;component/downloadgamefile.xaml", UriKind.Relative));
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
          this.LaDispPer = (Label) target;
          break;
        case 3:
          this.LaDispPerUnit = (Label) target;
          break;
        case 4:
          this.LaDispPerTxt = (Label) target;
          break;
        case 5:
          this.LaDownloadFile = (Label) target;
          break;
        case 6:
          this.LaDownloadLength = (Label) target;
          break;
        case 7:
          this.ImgProgress = (ProgressBar) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
