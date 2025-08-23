using Microsoft.Win32;
using MIRAGE.Helper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using xamlSpinnersWPF;

namespace MIRAGE
{
  public class MainWindow : Window, IComponentConnector
  {
    private NotifyIcon notify;
    private CallApi _apiWorker;
    private CallApi _apiWorker_btn;
    private string _patchApi_url = "https://api.mir4.co.kr/launcher";
    private string _mainWeb_url = "https://event.mir4.co.kr/launcher/index";
    private string _onestore_url = "";
    private string GAME_PROCESS_NAME = "Mir4";
    private string _btn_gamebutton_off = "pack://application:,,,/Resources/BNT_install_off.png";
    private string _btn_gamebutton_on = "pack://application:,,,/Resources/BNT_install_on.png";
    private string _btn_gamebutton_over = "pack://application:,,,/Resources/BNT_install_over.png";
    private string _btn1_gamebutton_off = "pack://application:,,,/Resources/BNT_install_off.png";
    private string _btn1_gamebutton_on = "pack://application:,,,/Resources/BNT_install_on.png";
    private string _btn1_gamebutton_over = "pack://application:,,,/Resources/BNT_install_over.png";
    private int _last_get_version;
    private bool is_btn_clicked;
    private bool is_game_play_just_now;
    private string _TEMP_PATH = "\\updatetemp\\";
    private bool _IS_TRAY;
    private int _TICK_TERM = 2;
    private DispatcherTimer _timer;
    private DispatcherTimer _timer1;
    private NoticeInfo[] _noticeList;
    private NoticeInfo[] _eventList;
    private string _NOTICE_URL = "https://forum.mir4.co.kr/board/notice";
    private string _EVENT_URL = "https://forum.mir4.co.kr//board/event?event_category=1";
    private string _BRAND_URL = "https://www.mir4.co.kr";
    private string _COMMUNITY_URL = "https://forum.mir4.co.kr";
    private string _YOUTUBE_URL = "https://www.youtube.com/channel/UC7lgXEN_Kc7FcFqEvCPJ8lA";
    private string _CALLCENTER_URL = "https://cs.mir4.co.kr/customer";
    private string _LAUNCHER_REQUIRED_URL = "https://logplatformbeta.blob.core.windows.net/launcher/required/";
    private List<BitmapImage> _MainImages = new List<BitmapImage>();
    private int ImageNumber;
    private DispatcherTimer PictureTimer = new DispatcherTimer();
    private int _MAIN_ANI_TIME = 5;
    private uint message;
    private IntPtr handle;
    public const uint HWND_BROADCAST = 65535;
    private string _PATH_LOCK_FILE = "/MirMobile/SaveData/Saved/SaveGames/temp/Patch.lock";
    private bool _IS_GAME_RUN1;
    private bool _IS_GAME_RUN2;
    private bool _IS_LAUNCHER_UPDATE;
    private bool _IS_GAME_RUN_PROCESS;
    private string _RUN_GAME_INDEX = "";
    internal Grid bgGrid;
    internal ImageBrush BasicBack;
    internal System.Windows.Controls.Image imageView;
    internal System.Windows.Controls.Image headerDiv;
    internal System.Windows.Controls.Image logoDiv;
    internal System.Windows.Controls.Image topDownDiv;
    internal System.Windows.Controls.Image topMaxDiv;
    internal System.Windows.Controls.Image topCloseDiv;
    internal System.Windows.Controls.Image GameControlDiv1;
    internal System.Windows.Controls.Image GameControlDiv;
    internal UCMainMenu MainMenu1;
    internal UCMainMenu MainMenu2;
    internal UCMainMenu MainMenu3;
    internal UCMainMenu MainMenu4;
    internal System.Windows.Controls.Image dirTitle;
    internal System.Windows.Controls.RadioButton radioDir11;
    internal System.Windows.Controls.Image dir11;
    internal System.Windows.Controls.RadioButton radioDir12;
    internal System.Windows.Controls.Image dir12;
    internal System.Windows.Controls.Image dirbtn12;
    internal System.Windows.Controls.RadioButton radioResN;
    internal System.Windows.Controls.RadioButton radioResW;
    internal System.Windows.Controls.Image dir12Tip;
    internal System.Windows.Controls.Image MainTitleDiv;
    internal System.Windows.Controls.Image NoticeDiv;
    internal System.Windows.Controls.Label NoticeTitle;
    internal System.Windows.Controls.Label btnNotice;
    internal System.Windows.Controls.Label Notice1;
    internal System.Windows.Controls.Label Notice2;
    internal System.Windows.Controls.Label Notice3;
    internal System.Windows.Controls.Image EventDiv;
    internal System.Windows.Controls.Label EventTitle;
    internal System.Windows.Controls.Label btnEvent;
    internal System.Windows.Controls.Label Event1;
    internal System.Windows.Controls.Label Event2;
    internal System.Windows.Controls.Label Event3;
    internal System.Windows.Controls.Image FailDiv;
    internal ucSpinnerDotCircle loadSpinner;
    private bool _contentLoaded;

    [DllImport("AesTool.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr CreateCBox();

    [DllImport("AesTool.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void DisposeCBox(IntPtr pTestClassObject);

    [DllImport("AesTool.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void SetEncryptKey(IntPtr pTestClassObject, string sek);

    [DllImport("AesTool.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void SetOriginText(IntPtr pTestClassObject, string sek);

    [DllImport("AesTool.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void Encode(IntPtr pTestClassObject);

    [DllImport("AesTool.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
    public static extern IntPtr GetConvertedText(IntPtr pTestClassObject);

    [DllImport("kernel32", SetLastError = true)]
    private static extern bool FreeLibrary(IntPtr hModule);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern uint RegisterWindowMessage(string lpString);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern bool PostMessage(IntPtr hWnd, uint Msg, uint wParam, uint lParam);

    public MainWindow()
    {
      this.InitializeComponent();
      this.handle = new WindowInteropHelper((Window) this).Handle;
      this.message = MainWindow.RegisterWindowMessage("Mir4 Launcher Show Message");
      ComponentDispatcher.ThreadFilterMessage += new ThreadMessageEventHandler(this.ComponentDispatcher_ThreadFilterMessage);
      Console.WriteLine("getInstallPath:" + this.getInstallPath());
      System.Windows.Application.Current.DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(this.Application_DispatcherUnhandledException);
    }

    private void Application_UnhandledException(object sender, UnhandledExceptionEventArgs args)
    {
      Exception exceptionObject = (Exception) args.ExceptionObject;
      int num = (int) System.Windows.MessageBox.Show("An unhandled exception just occurred: " + exceptionObject.Message, "Mir4 Unhandled Exception", MessageBoxButton.OK, MessageBoxImage.Exclamation);
      SentryApi.SendException(exceptionObject);
    }

    private void Application_DispatcherUnhandledException(
      object sender,
      DispatcherUnhandledExceptionEventArgs e)
    {
      int num = (int) System.Windows.MessageBox.Show("An unhandled exception just occurred: " + e.Exception.Message, "Mir4 Dispatcher Unhandled Exception", MessageBoxButton.OK, MessageBoxImage.Exclamation);
      e.Handled = true;
      SentryApi.SendException(e.Exception);
    }

    private void ImageLoadStart()
    {
      try
      {
        this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Delegate) (() =>
        {
          this.PictureTimer.Interval = TimeSpan.FromSeconds((double) this._MAIN_ANI_TIME);
          this.PictureTimer.Tick += new EventHandler(this.ImageTick);
          if (this._MainImages.Count > 0)
          {
            this.imageView.Source = (ImageSource) this._MainImages[0];
            this.PictureTimer.Start();
          }
          this.loadSpinner.Visibility = Visibility.Hidden;
          this.imageView.Visibility = Visibility.Visible;
        }));
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
      }
    }

    private void GetMainImage()
    {
      try
      {
        this._apiWorker = new CallApi(this.getPatchApiUrl() + "/getMainImage", new Dictionary<string, string>()
        {
          {
            "ServiceID",
            App.MIR4_GAME_INDEX
          }
        });
        this._apiWorker.ResultEvent += new CallApi.ResultEventHandler(this._worker_GetMainImage_RunWorkerCompleted);
        this._apiWorker.DoCallPostAsync();
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
        SentryApi.SendException(ex);
      }
    }

    private void _worker_GetMainImage_RunWorkerCompleted()
    {
      try
      {
        if (this._apiWorker._resultString != "-1")
        {
          JObject jobject = JObject.Parse(this._apiWorker._resultString);
          if (int.Parse(jobject["Code"].ToString()) == 200)
          {
            foreach (DownloadFileInfo downloadFileInfo in JsonConvert.DeserializeObject<List<DownloadFileInfo>>(jobject["ImageFileData"].ToString()).ToArray())
              this.ImageDownload(downloadFileInfo.path);
          }
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
        SentryApi.SendException(ex);
      }
      this.ImageLoadStart();
    }

    private void ImageTick(object sender, EventArgs e)
    {
      this.ImageNumber = (this.ImageNumber + 1) % this._MainImages.Count;
      this.ShowNextImage(this.imageView);
    }

    private void ShowNextImage(System.Windows.Controls.Image img)
    {
      Storyboard storyboard = new Storyboard();
      this.BasicBack.ImageSource = this.imageView.Source;
      ObjectAnimationUsingKeyFrames element1 = new ObjectAnimationUsingKeyFrames();
      element1.BeginTime = new TimeSpan?(TimeSpan.FromSeconds(0.7));
      DiscreteObjectKeyFrame keyFrame = new DiscreteObjectKeyFrame((object) this._MainImages[this.ImageNumber], (KeyTime) TimeSpan.Zero);
      element1.KeyFrames.Add((ObjectKeyFrame) keyFrame);
      Storyboard.SetTarget((DependencyObject) element1, (DependencyObject) img);
      Storyboard.SetTargetProperty((DependencyObject) element1, new PropertyPath((object) System.Windows.Controls.Image.SourceProperty));
      storyboard.Children.Add((Timeline) element1);
      DoubleAnimation element2 = new DoubleAnimation(0.0, 1.0, (Duration) TimeSpan.FromSeconds(0.7));
      element2.BeginTime = new TimeSpan?(TimeSpan.FromSeconds(0.7));
      Storyboard.SetTarget((DependencyObject) element2, (DependencyObject) img);
      Storyboard.SetTargetProperty((DependencyObject) element2, new PropertyPath((object) UIElement.OpacityProperty));
      storyboard.Children.Add((Timeline) element2);
      storyboard.Begin((FrameworkElement) img);
    }

    private void ImageDownload(string imgURL)
    {
      try
      {
        BitmapImage image = new BitmapImage();
        int count1 = 100;
        WebRequest webRequest = WebRequest.Create(new Uri(imgURL, UriKind.Absolute));
        webRequest.Timeout = -1;
        BinaryReader binaryReader = new BinaryReader(webRequest.GetResponse().GetResponseStream());
        MemoryStream memoryStream = new MemoryStream();
        byte[] buffer = new byte[count1];
        for (int count2 = binaryReader.Read(buffer, 0, count1); count2 > 0; count2 = binaryReader.Read(buffer, 0, count1))
          memoryStream.Write(buffer, 0, count2);
        image.BeginInit();
        memoryStream.Seek(0L, SeekOrigin.Begin);
        image.StreamSource = (Stream) memoryStream;
        image.EndInit();
        this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Delegate) (() => this._MainImages.Add(image)));
        Console.WriteLine("Image Load : " + imgURL);
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
      }
    }

    private void DoLauncher()
    {
      try
      {
        this.GetMainImage();
        popup popup = new popup(3, "");
        popup.Owner = (Window) this;
        popup.ShowDialog();
        popup.Close();
        App._GAME_STATUS = GAME_STATUS.none;
        App._GAME_STATUS1 = GAME_STATUS.none;
        this.updateGameButton();
        DirectoryInfo directoryInfo = new DirectoryInfo(string.Format("{0}\\MIR4_Launcher", (object) Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)));
        if (!directoryInfo.Exists)
          directoryInfo.Create();
        GameData gameData1 = new GameData();
        (bool flag, GameData gameData2) = new GameInfo().checkGameDataFileAndGetData(App.MIR4_GAME_INDEX);
        if (flag)
        {
          App.m_mapGameData.Add(App.MIR4_GAME_INDEX, gameData2);
          if ("11".Equals(gameData2.dx_version ?? "11"))
            this.radioDir11.IsChecked = new bool?(true);
          else
            this.radioDir12.IsChecked = new bool?(true);
          if (gameData2.resolution.Equals("N"))
            this.radioResN.IsChecked = new bool?(true);
          else
            this.radioResW.IsChecked = new bool?(true);
        }
        if (this.IsGameRun())
          this.GetGameVersionApi_FirstStart();
        else if (flag && int.Parse(gameData2.version) > 0)
        {
          this.DelPatchLock();
          this.GetGameVersionApi_FirstStart();
        }
        else
          this.GetGameVersionApi_OnlyVersion();
      }
      catch (Exception ex)
      {
        SentryApi.SendException(ex);
      }
    }

    private void MirageClose()
    {
      try
      {
        this.ImgAniStop();
        popup popup = new popup(0, "");
        popup.Owner = (Window) this;
        popup.ShowInTaskbar = false;
        bool? nullable = popup.ShowDialog();
        popup.Close();
        if (!nullable.HasValue)
          return;
        if (nullable.GetValueOrDefault())
        {
          MainWindow.UnloadModule("AesTool.dll");
          MainWindow.UnloadModule("AesTool.dll");
          MainWindow.UnloadModule("AesTool.dll");
          System.Windows.Application.Current.Shutdown();
          Process.GetCurrentProcess().Kill();
        }
        else
        {
          this.DoTray();
          this.Hide();
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        SentryApi.SendException(ex);
      }
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

    private void GetGameVersionApi_OnlyVersion()
    {
      this._apiWorker = new CallApi(this.getPatchApiUrl() + "/getGameVersion", new Dictionary<string, string>()
      {
        {
          "ServiceID",
          App.MIR4_GAME_INDEX
        }
      });
      this._apiWorker.ResultEvent += new CallApi.ResultEventHandler(this._worker_Only_RunWorkerCompleted);
      this._apiWorker.DoCallPostAsync();
    }

    private void GetGameVersionApi_FirstStart()
    {
      this._apiWorker = new CallApi(this.getPatchApiUrl() + "/getGameVersion", new Dictionary<string, string>()
      {
        {
          "ServiceID",
          App.MIR4_GAME_INDEX
        }
      });
      this._apiWorker.ResultEvent += new CallApi.ResultEventHandler(this._worker_FirstStart_RunWorkerCompleted);
      this._apiWorker.DoCallPostAsync();
    }

    private void GetGameVersionApi_ButtonClick()
    {
      this._apiWorker_btn = new CallApi(this.getPatchApiUrl() + "/getGameVersion", new Dictionary<string, string>()
      {
        {
          "ServiceID",
          App.MIR4_GAME_INDEX
        }
      });
      this._apiWorker_btn.ResultEvent += new CallApi.ResultEventHandler(this._worker_BtnClick_RunWorkerCompleted);
      this._apiWorker_btn.DoCallPostAsync();
    }

    private void _worker_BtnClick_RunWorkerCompleted()
    {
      try
      {
        if (this._apiWorker_btn._resultString != "-1")
        {
          Console.WriteLine("Game Start BTN - resultString : " + this._apiWorker._resultString);
          JObject jobject = JObject.Parse(this._apiWorker_btn._resultString);
          if (int.Parse(jobject["Code"].ToString()) == 200)
          {
            GameData gameData = new GameData();
            App.m_mapGameData.TryGetValue(App.MIR4_GAME_INDEX, out gameData);
            this._last_get_version = int.Parse(jobject[this.getMode()].ToString());
            this._onestore_url = jobject["OneStoreURL"].ToString();
            Console.WriteLine("_last_get_version ; " + this._last_get_version.ToString());
            Console.WriteLine("localVersion ; " + gameData.version);
            if (this._last_get_version == int.Parse(gameData.version))
            {
              if (this._RUN_GAME_INDEX == "0")
              {
                App._GAME_STATUS1 = GAME_STATUS.run;
                this._IS_GAME_RUN1 = true;
              }
              else
              {
                App._GAME_STATUS = GAME_STATUS.run;
                this._IS_GAME_RUN2 = true;
              }
              this.updateGameButton();
              this.StartGameRun(this._RUN_GAME_INDEX);
            }
            else
            {
              App._GAME_STATUS = GAME_STATUS.update;
              this.gameExitInfo();
              this.updateGameButton();
            }
          }
          else
            this.ShowPopup("패치 정보를 얻어오지 못했습니다.\r\n다시 시도해보세요.");
        }
        else
          this.ShowPopup("패치 정보를 얻어오지 못했습니다.\r\n다시 시도해보세요.");
      }
      catch (Exception ex)
      {
        SentryApi.SendException(ex);
      }
      this._IS_GAME_RUN_PROCESS = false;
    }

    private void _worker_FirstStart_RunWorkerCompleted()
    {
      try
      {
        if (this._apiWorker._resultString != "-1")
        {
          Console.WriteLine("First Check - resultString : " + this._apiWorker._resultString);
          JObject jobject = JObject.Parse(this._apiWorker._resultString);
          if (int.Parse(jobject["Code"].ToString()) == 200)
          {
            GameData gameData = new GameData();
            App.m_mapGameData.TryGetValue(App.MIR4_GAME_INDEX, out gameData);
            this._last_get_version = int.Parse(jobject[this.getMode()].ToString());
            this._onestore_url = jobject["OneStoreURL"].ToString();
            Console.WriteLine("_last_get_version ; " + this._last_get_version.ToString());
            Console.WriteLine("localVersion ; " + gameData.version);
            if (this._last_get_version == int.Parse(gameData.version))
            {
              App._GAME_STATUS = GAME_STATUS.install;
              App._GAME_STATUS1 = GAME_STATUS.install;
              if (this.IsGameRun())
              {
                App._GAME_STATUS = GAME_STATUS.install;
                App._GAME_STATUS1 = GAME_STATUS.install;
                if (this._IS_GAME_RUN1)
                {
                  App._GAME_STATUS1 = GAME_STATUS.run;
                  if (this._timer1 == null)
                  {
                    this._timer1 = new DispatcherTimer()
                    {
                      Interval = TimeSpan.FromSeconds((double) this._TICK_TERM)
                    };
                    this._timer1.Tick += new EventHandler(this.timer1_Tick);
                  }
                  this._timer1.Start();
                }
                if (this._IS_GAME_RUN2)
                {
                  App._GAME_STATUS = GAME_STATUS.run;
                  if (this._timer == null)
                  {
                    this._timer = new DispatcherTimer()
                    {
                      Interval = TimeSpan.FromSeconds((double) this._TICK_TERM)
                    };
                    this._timer.Tick += new EventHandler(this.timer_Tick);
                  }
                  this._timer.Start();
                }
              }
            }
            else
              App._GAME_STATUS = GAME_STATUS.update;
            if (App._GAME_STATUS == GAME_STATUS.update)
              this.gameExitInfo();
            Console.WriteLine("App._GAME_STATUS : " + App._GAME_STATUS.ToString());
            Console.WriteLine("App._GAME_STATUS1 : " + App._GAME_STATUS1.ToString());
            this.updateGameButton();
          }
          else
          {
            this.ShowPopup("패치 정보를 얻어오지 못했습니다.\r\n다시 시도해보세요.");
            this.CloseLauncher();
          }
        }
        else
        {
          this.ShowPopup("패치 정보를 얻어오지 못했습니다.\r\n다시 시도해보세요.");
          this.CloseLauncher();
        }
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

    private void _worker_Only_RunWorkerCompleted()
    {
      try
      {
        if (!(this._apiWorker._resultString != "-1"))
          return;
        JObject jobject = JObject.Parse(this._apiWorker._resultString);
        if (int.Parse(jobject["Code"].ToString()) != 200)
          return;
        this._last_get_version = int.Parse(jobject[this.getMode()].ToString());
        this._onestore_url = jobject["OneStoreURL"].ToString();
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
        SentryApi.SendException(ex);
      }
    }

    private void updateGameButton()
    {
      try
      {
        switch (App._GAME_STATUS)
        {
          case GAME_STATUS.none:
            this._btn_gamebutton_off = "pack://application:,,,/Resources/BNT_install_off.png";
            this._btn_gamebutton_on = "pack://application:,,,/Resources/BNT_install_on.png";
            this._btn_gamebutton_over = "pack://application:,,,/Resources/BNT_install_over.png";
            break;
          case GAME_STATUS.update:
            this._btn_gamebutton_off = "pack://application:,,,/Resources/BNT_update_off.png";
            this._btn_gamebutton_on = "pack://application:,,,/Resources/BNT_update_on.png";
            this._btn_gamebutton_over = "pack://application:,,,/Resources/BNT_update_over.png";
            break;
          case GAME_STATUS.install:
            this._btn_gamebutton_off = "pack://application:,,,/Resources/2_01.png";
            this._btn_gamebutton_on = "pack://application:,,,/Resources/2_02.png";
            this._btn_gamebutton_over = "pack://application:,,,/Resources/2_03.png";
            break;
          case GAME_STATUS.run:
            this._btn_gamebutton_off = "pack://application:,,,/Resources/2_04.png";
            this._btn_gamebutton_on = "pack://application:,,,/Resources/2_04.png";
            this._btn_gamebutton_over = "pack://application:,,,/Resources/2_04.png";
            break;
          case GAME_STATUS.updating:
            this._btn_gamebutton_off = "pack://application:,,,/Resources/BTN_updating.png";
            this._btn_gamebutton_on = "pack://application:,,,/Resources/BTN_updating.png";
            this._btn_gamebutton_over = "pack://application:,,,/Resources/BTN_updating.png";
            break;
          case GAME_STATUS.installing:
            this._btn_gamebutton_off = "pack://application:,,,/Resources/BNT_installing.png";
            this._btn_gamebutton_on = "pack://application:,,,/Resources/BNT_installing.png";
            this._btn_gamebutton_over = "pack://application:,,,/Resources/BNT_installing.png";
            break;
        }
        this.Dispatcher.BeginInvoke(DispatcherPriority.Background, (Delegate) (() =>
        {
          this.GameControlDiv.Visibility = Visibility.Visible;
          this.GameControlDiv.Source = new System.Windows.Controls.Image()
          {
            Source = ((ImageSource) new BitmapImage(new Uri(this._btn_gamebutton_over)))
          }.Source;
        }));
        if (App._GAME_STATUS1 == GAME_STATUS.install || App._GAME_STATUS1 == GAME_STATUS.run)
          this.updateGameButton1();
        else
          this.Dispatcher.BeginInvoke(DispatcherPriority.Background, (Delegate) (() => this.GameControlDiv1.Visibility = Visibility.Hidden));
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
        SentryApi.SendException(ex);
      }
    }

    private void updateGameButton1()
    {
      try
      {
        switch (App._GAME_STATUS1)
        {
          case GAME_STATUS.install:
            this._btn1_gamebutton_off = "pack://application:,,,/Resources/1_01.png";
            this._btn1_gamebutton_on = "pack://application:,,,/Resources/1_02.png";
            this._btn1_gamebutton_over = "pack://application:,,,/Resources/1_03.png";
            break;
          case GAME_STATUS.run:
            this._btn1_gamebutton_off = "pack://application:,,,/Resources/1_04.png";
            this._btn1_gamebutton_on = "pack://application:,,,/Resources/1_04.png";
            this._btn1_gamebutton_over = "pack://application:,,,/Resources/1_04.png";
            break;
        }
        this.Dispatcher.BeginInvoke(DispatcherPriority.Background, (Delegate) (() =>
        {
          this.GameControlDiv1.Visibility = Visibility.Visible;
          this.GameControlDiv1.Source = new System.Windows.Controls.Image()
          {
            Source = ((ImageSource) new BitmapImage(new Uri(this._btn1_gamebutton_over)))
          }.Source;
        }));
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
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

    private static string PtrToStringUtf8(IntPtr ptr)
    {
      if (ptr == IntPtr.Zero)
        return "";
      int length = 0;
      while (Marshal.ReadByte(ptr, length) != (byte) 0)
        ++length;
      if (length == 0)
        return "";
      byte[] numArray = new byte[length];
      Marshal.Copy(ptr, numArray, 0, length);
      return Encoding.UTF8.GetString(numArray);
    }

    private bool StartGameRun(string runIndex)
    {
      try
      {
        string machineName = Environment.MachineName;
        string str1 = Guid.NewGuid().ToString().Replace("-", "");
        string strUTF8_1 = machineName + "DK20EJCDY2UITMVUA983KJF9V8VWN2KT";
        string strUTF8_2 = str1 + "Mir4Mobile";
        IntPtr cbox = MainWindow.CreateCBox();
        MainWindow.SetEncryptKey(cbox, this.UTF82EUCKR(strUTF8_1));
        MainWindow.SetOriginText(cbox, this.UTF82EUCKR(strUTF8_2));
        MainWindow.Encode(cbox);
        string str2 = this.EUCKR2UTF8(MainWindow.PtrToStringUtf8(MainWindow.GetConvertedText(cbox)));
        MainWindow.DisposeCBox(cbox);
        Console.WriteLine(str2);
        GameData gameData = App.m_mapGameData[App.MIR4_GAME_INDEX];
        string fileName = string.Format("{0}/{1}", (object) gameData.install_path, (object) gameData.execname);
        string str3 = "1280";
        bool? isChecked1 = this.radioResW.IsChecked;
        bool flag1 = true;
        if (isChecked1.GetValueOrDefault() == flag1 & isChecked1.HasValue)
          str3 = "1440";
        string arguments = string.Format("-FeatureLevelES31 -Windowed -ResX={3} -ResY=720 -LauncherToken={0} -LaunchGameInstanceID={1} -ClientIndex={2}", (object) str2, (object) str1, (object) runIndex, (object) str3);
        bool? isChecked2 = this.radioDir11.IsChecked;
        bool flag2 = true;
        if (isChecked2.GetValueOrDefault() == flag2 & isChecked2.HasValue)
          arguments += " -dx11";
        Console.WriteLine("RUN PARAM : " + arguments);
        Process process = Process.Start(new ProcessStartInfo(fileName, arguments)
        {
          CreateNoWindow = true,
          RedirectStandardOutput = false,
          UseShellExecute = false
        });
        process.EnableRaisingEvents = true;
        if ("0".Equals(runIndex))
          process.Exited += new EventHandler(this.pGame1_Exited);
        else
          process.Exited += new EventHandler(this.pGame2_Exited);
        return true;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        this.is_btn_clicked = false;
        SentryApi.SendException(ex);
        return false;
      }
    }

    private void pGame1_Exited(object sender, EventArgs e)
    {
      Console.WriteLine("************** 게임 종료 1 ***************");
      this._IS_GAME_RUN1 = false;
      App._GAME_STATUS1 = GAME_STATUS.install;
      this.updateGameButton();
    }

    private void pGame2_Exited(object sender, EventArgs e)
    {
      Console.WriteLine("************** 게임 종료 2 ***************");
      this._IS_GAME_RUN2 = false;
      App._GAME_STATUS = GAME_STATUS.install;
      this.updateGameButton();
    }

    public static void UnloadModule(string moduleName)
    {
      try
      {
        foreach (ProcessModule module in (ReadOnlyCollectionBase) Process.GetCurrentProcess().Modules)
        {
          if (module.ModuleName == moduleName)
          {
            MainWindow.FreeLibrary(module.BaseAddress);
            Thread.Sleep(500);
          }
        }
      }
      catch (Exception ex)
      {
        SentryApi.SendException(ex);
      }
    }

    private void headerDiv_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      if (e.ChangedButton != MouseButton.Left)
        return;
      this.DragMove();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e) => this.DoLauncher();

    private bool IsGameRun()
    {
      bool flag = false;
      try
      {
        this._IS_GAME_RUN1 = false;
        this._IS_GAME_RUN2 = false;
        foreach (Process process in Process.GetProcesses())
        {
          if (process.ProcessName.Equals(this.GAME_PROCESS_NAME))
          {
            flag = true;
            string[] strArray = new string[1]
            {
              process.MainWindowTitle
            };
            for (int index = 0; index < strArray.Length; ++index)
            {
              Console.WriteLine("MainWindowTitle : " + strArray[index]);
              if (strArray[index].IndexOf("(0)") > 0)
                this._IS_GAME_RUN1 = true;
              else if (strArray[index].IndexOf("(1)") > 0)
                this._IS_GAME_RUN2 = true;
            }
          }
        }
      }
      catch (Exception ex)
      {
        SentryApi.SendException(ex);
      }
      return flag;
    }

    private int GetGameProcessCount()
    {
      int gameProcessCount = 0;
      try
      {
        foreach (Process process in Process.GetProcesses())
        {
          if (process.ProcessName.Equals(this.GAME_PROCESS_NAME))
            ++gameProcessCount;
        }
      }
      catch (Exception ex)
      {
      }
      return gameProcessCount;
    }

    private void DoTray()
    {
      try
      {
        if (!this._IS_TRAY)
        {
          System.Windows.Forms.ContextMenu contextMenu = new System.Windows.Forms.ContextMenu();
          this.notify = new NotifyIcon();
          this.notify.Icon = new System.Drawing.Icon("MIR4_Launcher.ico");
          this.notify.Visible = true;
          this.notify.ContextMenu = contextMenu;
          this.notify.Text = "MIR4_Launcher";
          this.notify.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Notify_LeftClick);
          System.Windows.Forms.MenuItem menuItem = new System.Windows.Forms.MenuItem();
          contextMenu.MenuItems.Add(menuItem);
          menuItem.Index = 0;
          menuItem.Text = "MIR4_Launcher 종료";
          menuItem.Click += (EventHandler) ((click, eClick) =>
          {
            System.Windows.Application.Current.Shutdown();
            Process.GetCurrentProcess().Kill();
          });
          this._IS_TRAY = true;
        }
        else
          this.notify.Visible = true;
        this.ImgAniStop();
      }
      catch (Exception ex)
      {
        SentryApi.SendException(ex);
      }
    }

    private void Notify_DoubleClick(object sender, EventArgs e)
    {
      this.Show();
      this.WindowState = WindowState.Normal;
      this.Visibility = Visibility.Visible;
      this.notify.Visible = false;
    }

    private void Notify_LeftClick(object sender, EventArgs e)
    {
      this.Show();
      this.WindowState = WindowState.Normal;
      this.Visibility = Visibility.Visible;
      this.notify.Visible = false;
      this.ImgAniStart();
    }

    protected override void OnStateChanged(EventArgs e)
    {
      if (WindowState.Minimized.Equals((object) this.WindowState))
        this.ImgAniStop();
      else if (WindowState.Normal.Equals((object) this.WindowState))
      {
        this.Show();
        this.WindowState = WindowState.Normal;
        this.Visibility = Visibility.Visible;
        this.ImgAniStart();
      }
      base.OnStateChanged(e);
    }

    protected override void OnClosing(CancelEventArgs e)
    {
    }

    private void topCloseDiv_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.MirageClose();
    }

    private void topDownDiv_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.WindowState = WindowState.Minimized;
    }

    private void topDownDiv_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
    {
      this.Cursor = System.Windows.Input.Cursors.Hand;
    }

    private void topDownDiv_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
    {
      this.Cursor = System.Windows.Input.Cursors.Arrow;
    }

    private void topCloseDiv_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
    {
      this.Cursor = System.Windows.Input.Cursors.Hand;
    }

    private void topCloseDiv_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
    {
      this.Cursor = System.Windows.Input.Cursors.Arrow;
    }

    private void GameControlDiv_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
    {
      this.Cursor = System.Windows.Input.Cursors.Hand;
      this.GameControlDiv.Source = new System.Windows.Controls.Image()
      {
        Source = ((ImageSource) new BitmapImage(new Uri(this._btn_gamebutton_over)))
      }.Source;
    }

    private void GameControlDiv_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
    {
      this.Cursor = System.Windows.Input.Cursors.Arrow;
      this.GameControlDiv.Source = new System.Windows.Controls.Image()
      {
        Source = ((ImageSource) new BitmapImage(new Uri(this._btn_gamebutton_off)))
      }.Source;
    }

    private void GameControlDiv_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.CheckGamePlayCase();
    }

    private void GameControlDiv_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      this.GameControlDiv.Source = new System.Windows.Controls.Image()
      {
        Source = ((ImageSource) new BitmapImage(new Uri(this._btn_gamebutton_on)))
      }.Source;
    }

    private void CheckGamePlayCase()
    {
      try
      {
        switch (App._GAME_STATUS)
        {
          case GAME_STATUS.none:
            App._GAME_STATUS = GAME_STATUS.installing;
            this.updateGameButton();
            this.GetGameUpdate();
            break;
          case GAME_STATUS.update:
            App._GAME_STATUS = GAME_STATUS.updating;
            this.updateGameButton();
            this.GetGameUpdate();
            break;
          case GAME_STATUS.install:
            if (this._IS_GAME_RUN_PROCESS)
              break;
            this._IS_GAME_RUN_PROCESS = true;
            if (this._IS_GAME_RUN2)
              break;
            this._IS_GAME_RUN2 = true;
            this._RUN_GAME_INDEX = "1";
            this.GetGameVersionApi_ButtonClick();
            break;
          default:
            int gameStatus = (int) App._GAME_STATUS;
            break;
        }
      }
      catch (Exception ex)
      {
        SentryApi.SendException(ex);
      }
    }

    private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
      if (e.Key != Key.System || e.SystemKey != Key.F4)
        return;
      this.MirageClose();
      e.Handled = true;
    }

    private void FailDiv_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
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

    private string getMainUrl()
    {
      string mainUrl = "";
      try
      {
        mainUrl = ConfigurationManager.AppSettings["main"];
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
      }
      if (mainUrl == null || mainUrl.Length == 0)
        mainUrl = this._mainWeb_url;
      Console.WriteLine("MainURL : " + mainUrl);
      return mainUrl;
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

    private void timer_Tick(object sender, EventArgs e)
    {
      try
      {
        this._timer.Stop();
        Console.WriteLine("isGameRun : " + this.IsGameRun().ToString());
        if (!this._IS_GAME_RUN2)
        {
          App._GAME_STATUS = GAME_STATUS.install;
          this.updateGameButton();
        }
        else
          this._timer.Start();
      }
      catch (Exception ex)
      {
        SentryApi.SendException(ex);
      }
    }

    private void timer1_Tick(object sender, EventArgs e)
    {
      try
      {
        this._timer1.Stop();
        Console.WriteLine("isGameRun : " + this.IsGameRun().ToString());
        if (!this._IS_GAME_RUN1)
        {
          App._GAME_STATUS1 = GAME_STATUS.install;
          this.updateGameButton();
        }
        else
          this._timer1.Start();
      }
      catch (Exception ex)
      {
        SentryApi.SendException(ex);
      }
    }

    private void TestGameStart()
    {
      App._GAME_STATUS = GAME_STATUS.run;
      if (!this.StartGameRun("1"))
        return;
      this.is_game_play_just_now = true;
      this.WindowState = WindowState.Minimized;
      if (this._timer == null)
      {
        this._timer = new DispatcherTimer()
        {
          Interval = TimeSpan.FromSeconds((double) this._TICK_TERM)
        };
        this._timer.Tick += new EventHandler(this.timer_Tick);
      }
      this._timer.Start();
    }

    private void btnNotice_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
      this.OpenBrowser(this._NOTICE_URL);
    }

    private void OpenBrowser(string URL)
    {
      try
      {
        Process.Start(URL);
      }
      catch (Exception ex)
      {
        SentryApi.SendException(ex);
      }
    }

    private void Notice1_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
      this.OpenBrowser(this._noticeList[0].Url);
    }

    private void Notice2_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
      this.OpenBrowser(this._noticeList[1].Url);
    }

    private void Notice3_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
      this.OpenBrowser(this._noticeList[2].Url);
    }

    private void GetNoticeList()
    {
      try
      {
        this._apiWorker = new CallApi(this.getMainUrl().Replace("index", "") + "getList", new Dictionary<string, string>()
        {
          {
            "Type",
            "notice"
          }
        });
        this._apiWorker.ResultEvent += new CallApi.ResultEventHandler(this._worker_GetNoticeList_RunWorkerCompleted);
        this._apiWorker.DoCallPostAsync();
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
        SentryApi.SendException(ex);
      }
    }

    private void _worker_GetNoticeList_RunWorkerCompleted()
    {
      try
      {
        if (!(this._apiWorker._resultString != "-1"))
          return;
        JObject jobject = JObject.Parse(this._apiWorker._resultString);
        if (int.Parse(jobject["Code"].ToString()) != 200)
          return;
        this._noticeList = JsonConvert.DeserializeObject<List<NoticeInfo>>(jobject["Data"].ToString()).ToArray();
        if (this._noticeList.Length != 0)
          this.Notice1.Content = (object) this.ShowTitle(this._noticeList[0].Title);
        if (this._noticeList.Length > 1)
          this.Notice2.Content = (object) this.ShowTitle(this._noticeList[1].Title);
        if (this._noticeList.Length <= 2)
          return;
        this.Notice3.Content = (object) this.ShowTitle(this._noticeList[2].Title);
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
        SentryApi.SendException(ex);
      }
    }

    private void GetEventList()
    {
      try
      {
        this._apiWorker = new CallApi(this.getMainUrl().Replace("index", "") + "getList", new Dictionary<string, string>()
        {
          {
            "Type",
            "event"
          }
        });
        this._apiWorker.ResultEvent += new CallApi.ResultEventHandler(this._worker_GetEventList_RunWorkerCompleted);
        this._apiWorker.DoCallPostAsync();
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
        SentryApi.SendException(ex);
      }
    }

    private void _worker_GetEventList_RunWorkerCompleted()
    {
      try
      {
        if (!(this._apiWorker._resultString != "-1"))
          return;
        JObject jobject = JObject.Parse(this._apiWorker._resultString);
        if (int.Parse(jobject["Code"].ToString()) != 200)
          return;
        this._eventList = JsonConvert.DeserializeObject<List<NoticeInfo>>(jobject["Data"].ToString()).ToArray();
        if (this._eventList.Length != 0)
          this.Event1.Content = (object) this.ShowTitle(this._eventList[0].Title);
        if (this._eventList.Length > 1)
          this.Event2.Content = (object) this.ShowTitle(this._eventList[1].Title);
        if (this._eventList.Length <= 2)
          return;
        this.Event3.Content = (object) this.ShowTitle(this._eventList[2].Title);
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
        SentryApi.SendException(ex);
      }
    }

    private string ShowTitle(string title)
    {
      string str = title;
      if (title.Length > 20)
        str = title.Substring(0, 20) + "...";
      return str;
    }

    private void btnEvent_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
      this.OpenBrowser(this._EVENT_URL);
    }

    private void Event1_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
      this.OpenBrowser(this._eventList[0].Url);
    }

    private void Event2_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
      this.OpenBrowser(this._eventList[1].Url);
    }

    private void Event3_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
      this.OpenBrowser(this._eventList[2].Url);
    }

    private void GetGameUpdate()
    {
      try
      {
        GameData gameData = App.m_mapGameData[App.MIR4_GAME_INDEX];
        this._apiWorker = new CallApi(this.getPatchApiUrl() + "/getGameFileData", new Dictionary<string, string>()
        {
          {
            "ServiceID",
            App.MIR4_GAME_INDEX
          },
          {
            "Version",
            this._last_get_version.ToString()
          }
        });
        this._apiWorker.ResultEvent += new CallApi.ResultEventHandler(this._worker_GameUpdate_RunWorkerCompleted);
        this._apiWorker.DoCallPostAsync();
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
      }
    }

    private void _worker_GameUpdate_RunWorkerCompleted()
    {
      try
      {
        GameData _mirdata = App.m_mapGameData[App.MIR4_GAME_INDEX];
        string installPath = _mirdata.install_path;
        Console.WriteLine("Game install Path : " + installPath);
        if (this._apiWorker._resultString != "-1")
        {
          JObject jobject = JObject.Parse(this._apiWorker._resultString);
          if (int.Parse(jobject["Code"].ToString()) == 200)
          {
            string str1 = "";
            long num = 0;
            try
            {
              str1 = jobject["FileSize"].ToString();
              num = Convert.ToInt64(str1);
            }
            catch (Exception ex)
            {
            }
            string directory = installPath.Substring(0, 3);
            Console.WriteLine("installDiskName : " + directory);
            long diskFreeSpace = new UtilDrive().getDiskFreeSpace(directory);
            Console.WriteLine("Download Size : " + num.ToString());
            Console.WriteLine("Disk Free Size : " + diskFreeSpace.ToString());
            if (num >= diskFreeSpace)
            {
              this.ShowPopup("디스크 용량이 부족합니다.\r\n필요한 용량은 " + str1 + "MB 입니다.\r\n용량 확인 후 다시 시작하세요");
              this.is_btn_clicked = false;
              App._GAME_STATUS = GAME_STATUS.update;
              this.updateGameButton();
            }
            else
            {
              string str2 = jobject["FileData"][(object) "URL"].ToString();
              string path = this.getInstallPath() + this._TEMP_PATH;
              DirectoryInfo directoryInfo = new DirectoryInfo(path);
              if (!directoryInfo.Exists)
                directoryInfo.Create();
              this.ImgAniStop();
              Console.WriteLine("localPath :" + path);
              DownloadGameFile downloadGameFile = new DownloadGameFile();
              downloadGameFile.serverUrl = str2;
              downloadGameFile.localPath = path;
              downloadGameFile.unzipPath = installPath;
              downloadGameFile.Owner = (Window) this;
              bool? nullable1 = downloadGameFile.ShowDialog();
              downloadGameFile.Close();
              this.ImgAniStart();
              bool? nullable2 = nullable1;
              bool flag1 = false;
              if (nullable2.GetValueOrDefault() == flag1 & nullable2.HasValue)
              {
                this.ShowPopup("게임 업데이트 정보를 얻어오지 못했습니다.\r\n다시 시도해보세요.");
                this.is_btn_clicked = false;
                App._GAME_STATUS = GAME_STATUS.update;
                this.updateGameButton();
              }
              else
              {
                GameData gameData1 = new GameData();
                GameInfo gameInfo = new GameInfo();
                _mirdata.version = this._last_get_version.ToString();
                gameInfo.WriteGameData(App.MIR4_GAME_INDEX, _mirdata);
                (bool flag2, GameData gameData2) = gameInfo.checkGameDataFileAndGetData(App.MIR4_GAME_INDEX);
                if (flag2 && int.Parse(gameData2.version) > 0)
                {
                  if (App.m_mapGameData.ContainsKey(App.MIR4_GAME_INDEX))
                    App.m_mapGameData.Remove(App.MIR4_GAME_INDEX);
                  App.m_mapGameData.Add(App.MIR4_GAME_INDEX, gameData2);
                }
                App._GAME_STATUS = GAME_STATUS.install;
                App._GAME_STATUS1 = GAME_STATUS.install;
                this.updateGameButton();
              }
            }
          }
          else
          {
            this.ShowPopup("게임 업데이트 정보를 얻어오지 못했습니다.\r\n다시 시도해보세요.");
            this.is_btn_clicked = false;
            App._GAME_STATUS = GAME_STATUS.update;
            this.updateGameButton();
          }
        }
        else
        {
          this.ShowPopup("게임 업데이트 정보를 얻어오지 못했습니다.\r\n다시 시도해보세요.");
          this.is_btn_clicked = false;
          App._GAME_STATUS = GAME_STATUS.update;
          this.updateGameButton();
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
        this.ShowPopup("게임 업데이트 정보를 얻어오지 못했습니다.\r\n다시 시도해보세요.");
        this.is_btn_clicked = false;
        App._GAME_STATUS = GAME_STATUS.update;
        this.updateGameButton();
      }
    }

    private void MainMenu1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      string URL = "";
      try
      {
        URL = ConfigurationManager.AppSettings["BRAND_URL"];
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
      }
      if (URL == null || URL.Length == 0)
        URL = this._BRAND_URL;
      this.OpenBrowser(URL);
    }

    private void MainMenu2_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      string URL = "";
      try
      {
        URL = ConfigurationManager.AppSettings["COMMUNITY_URL"];
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
      }
      if (URL == null || URL.Length == 0)
        URL = this._COMMUNITY_URL;
      this.OpenBrowser(URL);
    }

    private void MainMenu3_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      string URL = "";
      try
      {
        URL = ConfigurationManager.AppSettings["YOUTUBE_URL"];
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
      }
      if (URL == null || URL.Length == 0)
        URL = this._YOUTUBE_URL;
      this.OpenBrowser(URL);
    }

    private void MainMenu4_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      string URL = "";
      try
      {
        URL = ConfigurationManager.AppSettings["CALLCENTER_URL"];
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
      }
      if (URL == null || URL.Length == 0)
        URL = this._CALLCENTER_URL;
      this.OpenBrowser(URL);
    }

    private void ImgAniStart()
    {
      if (this._MainImages.Count <= 0 || this.PictureTimer.IsEnabled)
        return;
      this.PictureTimer.Start();
    }

    private void ImgAniStop()
    {
      if (!this.PictureTimer.IsEnabled)
        return;
      this.PictureTimer.Stop();
    }

    private void ComponentDispatcher_ThreadFilterMessage(ref System.Windows.Interop.MSG msg, ref bool handled)
    {
      if ((long) msg.message != (long) this.message || !(msg.wParam != this.handle))
        return;
      this.Show();
      this.WindowState = WindowState.Normal;
      this.Visibility = Visibility.Visible;
      this.Topmost = true;
      this.Topmost = false;
      this.Focus();
      this.ImgAniStart();
    }

    private void GameControlDiv1_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
    {
      this.Cursor = System.Windows.Input.Cursors.Hand;
      this.GameControlDiv1.Source = new System.Windows.Controls.Image()
      {
        Source = ((ImageSource) new BitmapImage(new Uri(this._btn1_gamebutton_over)))
      }.Source;
    }

    private void GameControlDiv1_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
    {
      this.Cursor = System.Windows.Input.Cursors.Arrow;
      this.GameControlDiv1.Source = new System.Windows.Controls.Image()
      {
        Source = ((ImageSource) new BitmapImage(new Uri(this._btn1_gamebutton_off)))
      }.Source;
    }

    private void GameControlDiv1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      this.GameControlDiv1.Source = new System.Windows.Controls.Image()
      {
        Source = ((ImageSource) new BitmapImage(new Uri(this._btn1_gamebutton_on)))
      }.Source;
    }

    private void GameControlDiv1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.CheckGamePlayCase1();
    }

    private void CheckGamePlayCase1()
    {
      try
      {
        if (App._GAME_STATUS1 == GAME_STATUS.install)
        {
          if (this._IS_GAME_RUN_PROCESS)
            return;
          this._IS_GAME_RUN_PROCESS = true;
          if (this._IS_GAME_RUN1)
            return;
          this._IS_GAME_RUN1 = true;
          this._RUN_GAME_INDEX = "0";
          this.GetGameVersionApi_ButtonClick();
        }
        else
        {
          int gameStatuS1 = (int) App._GAME_STATUS1;
        }
      }
      catch (Exception ex)
      {
        SentryApi.SendException(ex);
      }
    }

    private void DelPatchLock()
    {
      try
      {
        string path = string.Format("{0}{1}", (object) App.m_mapGameData[App.MIR4_GAME_INDEX].install_path, (object) this._PATH_LOCK_FILE);
        Console.WriteLine("patch.lock Path : " + path);
        if (!System.IO.File.Exists(path))
          return;
        System.IO.File.Delete(path);
      }
      catch (Exception ex)
      {
        SentryApi.SendException(ex);
      }
    }

    private void Window_Closing(object sender, CancelEventArgs e)
    {
      Console.WriteLine("Close Event");
      e.Cancel = true;
    }

    private void gameExitInfo()
    {
      if (this.IsGameRun())
      {
        this.ShowPopup("업데이트가 필요합니다.\r\n업데이트를 위해 실행중인\r\n게임 프로세스를 종료합니다");
        this.gameKill();
        this._IS_GAME_RUN1 = false;
        this._IS_GAME_RUN2 = false;
      }
      if (this._timer1 != null)
        this._timer1.Stop();
      if (this._timer != null)
        this._timer.Stop();
      App._GAME_STATUS = GAME_STATUS.update;
      App._GAME_STATUS1 = GAME_STATUS.none;
    }

    private void gameKill()
    {
      try
      {
        foreach (Process process in Process.GetProcesses())
        {
          if (process.ProcessName.Equals(this.GAME_PROCESS_NAME))
          {
            process.Kill();
            Thread.Sleep(2000);
          }
        }
      }
      catch (Exception ex)
      {
        SentryApi.SendException(ex);
      }
    }

    private void dirbtn12_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
    {
      this.dir12Tip.Visibility = Visibility.Visible;
    }

    private void dirbtn12_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
    {
      this.dir12Tip.Visibility = Visibility.Hidden;
    }

    private void dir12_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.radioDir12.IsChecked = new bool?(true);
      this.setDirect_Version("12");
    }

    private void dir11_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.radioDir11.IsChecked = new bool?(true);
      this.setDirect_Version("11");
    }

    private void setDirect_Version(string dx_version)
    {
      try
      {
        GameData gameData;
        if (!App.m_mapGameData.TryGetValue(App.MIR4_GAME_INDEX, out gameData))
          return;
        GameInfo gameInfo = new GameInfo();
        gameData.dx_version = dx_version;
        string miR4GameIndex = App.MIR4_GAME_INDEX;
        GameData _mirdata = gameData;
        gameInfo.WriteGameData(miR4GameIndex, _mirdata);
        if (App.m_mapGameData.ContainsKey(App.MIR4_GAME_INDEX))
          App.m_mapGameData.Remove(App.MIR4_GAME_INDEX);
        App.m_mapGameData.Add(App.MIR4_GAME_INDEX, gameData);
      }
      catch (Exception ex)
      {
        SentryApi.SendException(ex);
      }
    }

    private void radioDir11_Checked(object sender, RoutedEventArgs e)
    {
      this.setDirect_Version("11");
    }

    private void radioDir12_Checked(object sender, RoutedEventArgs e)
    {
      this.setDirect_Version("12");
    }

    private void setRes(string _res)
    {
      try
      {
        GameData gameData;
        if (!App.m_mapGameData.TryGetValue(App.MIR4_GAME_INDEX, out gameData))
          return;
        GameInfo gameInfo = new GameInfo();
        gameData.resolution = _res;
        string miR4GameIndex = App.MIR4_GAME_INDEX;
        GameData _mirdata = gameData;
        gameInfo.WriteGameData(miR4GameIndex, _mirdata);
        if (App.m_mapGameData.ContainsKey(App.MIR4_GAME_INDEX))
          App.m_mapGameData.Remove(App.MIR4_GAME_INDEX);
        App.m_mapGameData.Add(App.MIR4_GAME_INDEX, gameData);
      }
      catch (Exception ex)
      {
        SentryApi.SendException(ex);
      }
    }

    private void radioResN_Checked(object sender, RoutedEventArgs e)
    {
      bool? isChecked = this.radioResN.IsChecked;
      bool flag = true;
      if (!(isChecked.GetValueOrDefault() == flag & isChecked.HasValue))
        this.radioResN.IsChecked = new bool?(true);
      this.setRes("N");
    }

    private void radioResW_Checked(object sender, RoutedEventArgs e)
    {
      bool? isChecked = this.radioResW.IsChecked;
      bool flag = true;
      if (!(isChecked.GetValueOrDefault() == flag & isChecked.HasValue))
        this.radioResW.IsChecked = new bool?(true);
      this.setRes("W");
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      System.Windows.Application.LoadComponent((object) this, new Uri("/MIR4_GameRun;component/mainwindow.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    internal Delegate _CreateDelegate(System.Type delegateType, string handler)
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
          ((UIElement) target).KeyDown += new System.Windows.Input.KeyEventHandler(this.Window_KeyDown);
          break;
        case 2:
          this.bgGrid = (Grid) target;
          break;
        case 3:
          this.BasicBack = (ImageBrush) target;
          break;
        case 4:
          this.imageView = (System.Windows.Controls.Image) target;
          break;
        case 5:
          this.headerDiv = (System.Windows.Controls.Image) target;
          this.headerDiv.MouseLeftButtonDown += new MouseButtonEventHandler(this.headerDiv_MouseLeftButtonDown);
          break;
        case 6:
          this.logoDiv = (System.Windows.Controls.Image) target;
          break;
        case 7:
          this.topDownDiv = (System.Windows.Controls.Image) target;
          this.topDownDiv.MouseLeftButtonUp += new MouseButtonEventHandler(this.topDownDiv_MouseLeftButtonUp);
          this.topDownDiv.MouseEnter += new System.Windows.Input.MouseEventHandler(this.topDownDiv_MouseEnter);
          this.topDownDiv.MouseLeave += new System.Windows.Input.MouseEventHandler(this.topDownDiv_MouseLeave);
          break;
        case 8:
          this.topMaxDiv = (System.Windows.Controls.Image) target;
          break;
        case 9:
          this.topCloseDiv = (System.Windows.Controls.Image) target;
          this.topCloseDiv.MouseLeftButtonUp += new MouseButtonEventHandler(this.topCloseDiv_MouseLeftButtonUp);
          this.topCloseDiv.MouseEnter += new System.Windows.Input.MouseEventHandler(this.topCloseDiv_MouseEnter);
          this.topCloseDiv.MouseLeave += new System.Windows.Input.MouseEventHandler(this.topCloseDiv_MouseLeave);
          break;
        case 10:
          this.GameControlDiv1 = (System.Windows.Controls.Image) target;
          this.GameControlDiv1.MouseLeftButtonUp += new MouseButtonEventHandler(this.GameControlDiv1_MouseLeftButtonUp);
          this.GameControlDiv1.MouseEnter += new System.Windows.Input.MouseEventHandler(this.GameControlDiv1_MouseEnter);
          this.GameControlDiv1.MouseLeave += new System.Windows.Input.MouseEventHandler(this.GameControlDiv1_MouseLeave);
          this.GameControlDiv1.MouseLeftButtonDown += new MouseButtonEventHandler(this.GameControlDiv1_MouseLeftButtonDown);
          break;
        case 11:
          this.GameControlDiv = (System.Windows.Controls.Image) target;
          this.GameControlDiv.MouseLeftButtonUp += new MouseButtonEventHandler(this.GameControlDiv_MouseLeftButtonUp);
          this.GameControlDiv.MouseEnter += new System.Windows.Input.MouseEventHandler(this.GameControlDiv_MouseEnter);
          this.GameControlDiv.MouseLeave += new System.Windows.Input.MouseEventHandler(this.GameControlDiv_MouseLeave);
          this.GameControlDiv.MouseLeftButtonDown += new MouseButtonEventHandler(this.GameControlDiv_MouseLeftButtonDown);
          break;
        case 12:
          this.MainMenu1 = (UCMainMenu) target;
          break;
        case 13:
          this.MainMenu2 = (UCMainMenu) target;
          break;
        case 14:
          this.MainMenu3 = (UCMainMenu) target;
          break;
        case 15:
          this.MainMenu4 = (UCMainMenu) target;
          break;
        case 16:
          this.dirTitle = (System.Windows.Controls.Image) target;
          break;
        case 17:
          this.radioDir11 = (System.Windows.Controls.RadioButton) target;
          this.radioDir11.Checked += new RoutedEventHandler(this.radioDir11_Checked);
          break;
        case 18:
          this.dir11 = (System.Windows.Controls.Image) target;
          this.dir11.MouseLeftButtonUp += new MouseButtonEventHandler(this.dir11_MouseLeftButtonUp);
          break;
        case 19:
          this.radioDir12 = (System.Windows.Controls.RadioButton) target;
          this.radioDir12.Checked += new RoutedEventHandler(this.radioDir12_Checked);
          this.radioDir12.MouseEnter += new System.Windows.Input.MouseEventHandler(this.dirbtn12_MouseEnter);
          this.radioDir12.MouseLeave += new System.Windows.Input.MouseEventHandler(this.dirbtn12_MouseLeave);
          break;
        case 20:
          this.dir12 = (System.Windows.Controls.Image) target;
          this.dir12.MouseLeftButtonUp += new MouseButtonEventHandler(this.dir12_MouseLeftButtonUp);
          this.dir12.MouseEnter += new System.Windows.Input.MouseEventHandler(this.dirbtn12_MouseEnter);
          this.dir12.MouseLeave += new System.Windows.Input.MouseEventHandler(this.dirbtn12_MouseLeave);
          break;
        case 21:
          this.dirbtn12 = (System.Windows.Controls.Image) target;
          this.dirbtn12.MouseEnter += new System.Windows.Input.MouseEventHandler(this.dirbtn12_MouseEnter);
          this.dirbtn12.MouseLeave += new System.Windows.Input.MouseEventHandler(this.dirbtn12_MouseLeave);
          break;
        case 22:
          this.radioResN = (System.Windows.Controls.RadioButton) target;
          this.radioResN.Checked += new RoutedEventHandler(this.radioResN_Checked);
          break;
        case 23:
          ((UIElement) target).MouseLeftButtonUp += new MouseButtonEventHandler(this.radioResN_Checked);
          break;
        case 24:
          this.radioResW = (System.Windows.Controls.RadioButton) target;
          this.radioResW.Checked += new RoutedEventHandler(this.radioResW_Checked);
          break;
        case 25:
          ((UIElement) target).MouseLeftButtonUp += new MouseButtonEventHandler(this.radioResW_Checked);
          break;
        case 26:
          this.dir12Tip = (System.Windows.Controls.Image) target;
          break;
        case 27:
          this.MainTitleDiv = (System.Windows.Controls.Image) target;
          break;
        case 28:
          this.NoticeDiv = (System.Windows.Controls.Image) target;
          break;
        case 29:
          this.NoticeTitle = (System.Windows.Controls.Label) target;
          break;
        case 30:
          this.btnNotice = (System.Windows.Controls.Label) target;
          this.btnNotice.PreviewMouseDown += new MouseButtonEventHandler(this.btnNotice_PreviewMouseDown);
          break;
        case 31:
          this.Notice1 = (System.Windows.Controls.Label) target;
          this.Notice1.PreviewMouseDown += new MouseButtonEventHandler(this.Notice1_PreviewMouseDown);
          break;
        case 32:
          this.Notice2 = (System.Windows.Controls.Label) target;
          this.Notice2.PreviewMouseDown += new MouseButtonEventHandler(this.Notice2_PreviewMouseDown);
          break;
        case 33:
          this.Notice3 = (System.Windows.Controls.Label) target;
          this.Notice3.PreviewMouseDown += new MouseButtonEventHandler(this.Notice3_PreviewMouseDown);
          break;
        case 34:
          this.EventDiv = (System.Windows.Controls.Image) target;
          break;
        case 35:
          this.EventTitle = (System.Windows.Controls.Label) target;
          break;
        case 36:
          this.btnEvent = (System.Windows.Controls.Label) target;
          this.btnEvent.PreviewMouseDown += new MouseButtonEventHandler(this.btnEvent_PreviewMouseDown);
          break;
        case 37:
          this.Event1 = (System.Windows.Controls.Label) target;
          this.Event1.PreviewMouseDown += new MouseButtonEventHandler(this.Event1_PreviewMouseDown);
          break;
        case 38:
          this.Event2 = (System.Windows.Controls.Label) target;
          this.Event2.PreviewMouseDown += new MouseButtonEventHandler(this.Event2_PreviewMouseDown);
          break;
        case 39:
          this.Event3 = (System.Windows.Controls.Label) target;
          this.Event3.PreviewMouseDown += new MouseButtonEventHandler(this.Event3_PreviewMouseDown);
          break;
        case 40:
          this.FailDiv = (System.Windows.Controls.Image) target;
          this.FailDiv.MouseLeftButtonUp += new MouseButtonEventHandler(this.FailDiv_MouseLeftButtonUp);
          break;
        case 41:
          this.loadSpinner = (ucSpinnerDotCircle) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
