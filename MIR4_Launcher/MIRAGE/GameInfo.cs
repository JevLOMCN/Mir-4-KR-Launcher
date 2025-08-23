using MIRAGE.Helper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;

namespace MIRAGE
{
  internal class GameInfo
  {
    public (bool, GameData) checkGameDataFileAndGetData(string _gameno)
    {
      GameData gameData = new GameData();
      try
      {
        Thread.Sleep(10);
        Console.WriteLine(nameof (checkGameDataFileAndGetData));
        Application current = Application.Current;
        string path = string.Format("{0}\\MIR4_Launcher\\{1}_gamedata.mgd", (object) Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), (object) _gameno);
        Console.WriteLine(path);
        if (!File.Exists(path))
          return (false, gameData);
        string end;
        using (StreamReader streamReader = new StreamReader(path, Encoding.UTF8))
          end = streamReader.ReadToEnd();
        Console.WriteLine("MGD : " + end);
        if (end == "")
          return (false, gameData);
        JObject jobject = JObject.Parse(end.Replace('\\', '/'));
        gameData.install_path = jobject["install_path"].ToString();
        gameData.version = jobject["version"].ToString();
        gameData.execname = jobject["execname"].ToString();
        try
        {
          gameData.launcher_version = jobject["launcher_version"].ToString();
        }
        catch (Exception ex)
        {
          gameData.launcher_version = "";
        }
        return (true, gameData);
      }
      catch (Exception ex)
      {
        SentryApi.SendException(ex);
        return (false, gameData);
      }
    }

    public bool WriteGameData(string _gameno, GameData _mirdata)
    {
      bool flag = true;
      try
      {
        Thread.Sleep(10);
        Application current = Application.Current;
        string path = string.Format("{0}\\MIR4_Launcher\\{1}_gamedata.mgd", (object) Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), (object) _gameno);
        Console.WriteLine(path);
        if (File.Exists(path))
        {
          Console.WriteLine("MGD : " + JsonConvert.SerializeObject((object) _mirdata));
          JsonSerializer jsonSerializer = new JsonSerializer();
          jsonSerializer.NullValueHandling = NullValueHandling.Ignore;
          using (StreamWriter streamWriter = new StreamWriter(path))
          {
            using (JsonWriter jsonWriter = (JsonWriter) new JsonTextWriter((TextWriter) streamWriter))
              jsonSerializer.Serialize(jsonWriter, (object) _mirdata);
          }
        }
        else
          flag = false;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
        SentryApi.SendException(ex);
        flag = false;
      }
      return flag;
    }
  }
}
