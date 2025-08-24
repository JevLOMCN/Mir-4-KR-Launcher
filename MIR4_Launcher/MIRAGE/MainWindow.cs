using Microsoft.Win32;
using MIRAGE.Helper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;

namespace MIRAGE
{
    public partial class MainWindow : Window
    {
        private CallApi _apiWorker;
        private string _patchApi_url = "https://api.mir4.co.kr/launcher";
        private string GAME_PROCESS_NAME = "Mir4";
        private int _LAUNCHER_VER = 5;
        private string _LAUNCHER_NAME = "MIR4_GameRun.exe";
        private string _LAUNCHER_REQUIRED_URL = "https://logplatformbeta.blob.core.windows.net/launcher/required/";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (IsGameRun())
                DoLauncher();
            else
                GetLauncherRequired();
        }

        private void DoLauncher()
        {
            try
            {
                string installPath = getInstallPath();
                Console.WriteLine("App Path : " + installPath);

                string exePath = Path.Combine(installPath, _LAUNCHER_NAME);
                Console.WriteLine("Exec Launcher : " + exePath);

                if (!File.Exists(exePath))
                {
                    ShowPopup("The launcher’s required files are missing.\r\nPlease reinstall the launcher.");
                    Application.Current.Shutdown();
                    return;
                }

                var psi = new ProcessStartInfo(
                    exePath,
                    "-LauncherToken=uwrj2NLZS7teVeYs+3OmL8TwxmiVHmh8AXXrak0mFTIuvVJAJEBzcTo9P+cCtu+i -LaunchGameInstanceID=b60fdd383b28474ca1458b98fc8aed76"
                )
                {
                    CreateNoWindow = true,
                    RedirectStandardOutput = false,
                    UseShellExecute = false
                };

                Process.Start(psi);
                Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                SentryApi.SendException(ex);
            }
        }

        private string UTF82EUCKR(string strUTF8)
        {
            ASCIIEncoding asciiEncoding = new ASCIIEncoding();
            return asciiEncoding.GetString(
                Encoding.Convert(
                    Encoding.UTF8,
                    Encoding.GetEncoding(asciiEncoding.CodePage),
                    Encoding.UTF8.GetBytes(strUTF8)
                )
            );
        }

        private string EUCKR2UTF8(string strEUCKR)
        {
            ASCIIEncoding asciiEncoding = new ASCIIEncoding();
            return Encoding.UTF8.GetString(
                Encoding.Convert(
                    Encoding.GetEncoding(asciiEncoding.CodePage),
                    Encoding.UTF8,
                    Encoding.GetEncoding(asciiEncoding.CodePage).GetBytes(strEUCKR)
                )
            );
        }

        private void ShowPopup(string message)
        {
            try
            {
                var p = new popup(1, message) { Owner = this };
                p.ShowDialog();
                p.Close();
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
            Close();
        }

        private bool IsGameRun()
        {
            try
            {
                foreach (Process process in Process.GetProcesses())
                {
                    if (process.ProcessName.Equals(GAME_PROCESS_NAME, StringComparison.OrdinalIgnoreCase))
                        return true;
                }
            }
            catch (Exception ex)
            {
                SentryApi.SendException(ex);
            }
            return false;
        }

        private void GetLauncherRequired()
        {
            try
            {
                string installPath = getInstallPath();
                Console.WriteLine("getInstallPath : " + installPath);

                string[] required = new[]
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

                var toDownload = new List<DownloadFileInfo>();
                foreach (string file in required)
                {
                    string path = Path.Combine(installPath, file);
                    if (!File.Exists(path))
                    {
                        Console.WriteLine("Required file missing: " + path);
                        toDownload.Add(new DownloadFileInfo
                        {
                            path = file,
                            URL = _LAUNCHER_REQUIRED_URL + file
                        });
                    }
                    else
                    {
                        Console.WriteLine("Required file found: " + path);
                    }
                }

                if (toDownload.Count > 0)
                {
                    Console.WriteLine("Required files to download: " + toDownload.Count);
                    var dlg = new DownloadFiles(toDownload.ToArray(), "", installPath, "\\", "2")
                    {
                        Owner = this
                    };
                    dlg.ShowDialog();
                    dlg.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                SentryApi.SendException(ex);
            }

            GetLauncherVersion();
        }

        private void GetLauncherVersion()
        {
            try
            {
                _apiWorker = new CallApi(
                    getPatchApiUrl() + "/getLauncherInfo",
                    new Dictionary<string, string> { { "ServiceID", App.MIR4_GAME_INDEX } }
                );
                _apiWorker.ResultEvent += _worker_LauncherVersion_RunWorkerCompleted;
                _apiWorker.DoCallPostAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                SentryApi.SendException(ex);
                DoLauncher();
            }
        }

        private void _worker_LauncherVersion_RunWorkerCompleted()
        {
            try
            {
                string installPath = getInstallPath();
                Console.WriteLine("App Path : " + installPath);
                Console.WriteLine("_resultString : " + _apiWorker._resultString);

                if (_apiWorker._resultString != "-1")
                {
                    JObject jo = JObject.Parse(_apiWorker._resultString);
                    JToken codeTok = jo["Code"];
                    if (codeTok != null && int.Parse(codeTok.ToString()) == 200)
                    {
                        getLauncherVersion();

                        int serverVer = 0;
                        JToken verTok = jo["LauncherVersion"];
                        if (verTok != null) int.TryParse(verTok.ToString(), out serverVer);

                        string serverUrl = "";
                        JToken urlTok = jo["URL"];
                        if (urlTok != null) serverUrl = urlTok.ToString();

                        Console.WriteLine("Launcher Server Version : " + serverVer);
                        Console.WriteLine("Launcher Local Version : " + _LAUNCHER_VER);

                        if (serverVer > _LAUNCHER_VER)
                        {
                            string sizeStr = "";
                            long sizeBytes = 0;
                            try
                            {
                                JToken sizeTok = jo["FileSize"];
                                if (sizeTok != null)
                                {
                                    sizeStr = sizeTok.ToString();
                                    long.TryParse(sizeStr, out sizeBytes);
                                }
                            }
                            catch { /* ignore size parse errors */ }

                            string root = installPath.Length >= 3 ? installPath.Substring(0, 3) : "C:\\";
                            long free = new UtilDrive().getDiskFreeSpace(root);

                            Console.WriteLine("Download Size : " + sizeBytes);
                            Console.WriteLine("Disk Free Size : " + free);

                            if (sizeBytes >= free)
                            {
                                ShowPopup("Not enough disk space.\r\nYou need " + sizeStr + " MB.\r\nPlease check your disk space and restart.");
                                Environment.Exit(0);
                                Process.GetCurrentProcess().Kill();
                                Close();
                                return;
                            }

                            ShowPopup("A launcher update is available.\r\nThe launcher will restart after the update.");

                            var dir = new DirectoryInfo(installPath);
                            if (!dir.Exists) dir.Create();

                            var filesToken = jo["FileData"];
                            List<DownloadFileInfo> files = new List<DownloadFileInfo>();
                            if (filesToken != null)
                                files = JsonConvert.DeserializeObject<List<DownloadFileInfo>>(filesToken.ToString());

                            var dlg = new DownloadFiles(files.ToArray(), serverUrl, installPath, "", "1")
                            {
                                Owner = this
                            };
                            bool? ok = dlg.ShowDialog();
                            dlg.Close();

                            if (ok.HasValue && ok.Value)
                            {
                                GameData gd = App.m_mapGameData[App.MIR4_GAME_INDEX];
                                gd.launcher_version = serverVer.ToString();

                                var gi = new GameInfo();
                                gi.WriteGameData(App.MIR4_GAME_INDEX, gd);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                SentryApi.SendException(ex);
            }

            DoLauncher();
        }

        private string getInstallPath()
        {
            try
            {
                const string subkey = @"SOFTWARE\WEMADE\MIR4_Launcher";
                using (var rk = Registry.CurrentUser.OpenSubKey(subkey, false))
                {
                    if (rk != null)
                    {
                        object v = rk.GetValue("ProgramFolder");
                        if (v != null) return v.ToString();
                    }
                }
            }
            catch { /* ignore */ }
            return "";
        }

        private string getPatchApiUrl()
        {
            string url = "";
            try
            {
                url = ConfigurationManager.AppSettings["patch"];
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            if (string.IsNullOrWhiteSpace(url))
                url = _patchApi_url;

            Console.WriteLine("PatchURL : " + url);
            return url;
        }

        private string getMode()
        {
            string s = "";
            try
            {
                s = ConfigurationManager.AppSettings["mode"];
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            if (string.IsNullOrWhiteSpace(s))
                s = "Release";

            string mode = s + "GameVersion";
            Console.WriteLine("Launche Mode : " + mode);
            return mode;
        }

        private void getLauncherVersion()
        {
            var gi = new GameInfo();
            var result = gi.checkGameDataFileAndGetData(App.MIR4_GAME_INDEX); // tuple

            bool ok = result.Item1;
            GameData saved = result.Item2;

            if (ok)
            {
                int verParsed;
                if (int.TryParse(saved.version, out verParsed) && verParsed > 0)
                {
                    if (!App.m_mapGameData.ContainsKey(App.MIR4_GAME_INDEX))
                        App.m_mapGameData.Add(App.MIR4_GAME_INDEX, saved);
                }
            }

            if (string.IsNullOrEmpty(saved.launcher_version))
            {
                saved.launcher_version = _LAUNCHER_VER.ToString();
                gi.WriteGameData(App.MIR4_GAME_INDEX, saved);
            }
            else
            {
                int parsed;
                if (int.TryParse(saved.launcher_version, out parsed))
                    _LAUNCHER_VER = parsed;
            }
        }
    }
}
