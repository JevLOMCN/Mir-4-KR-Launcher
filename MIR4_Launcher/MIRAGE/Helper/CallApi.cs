using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace MIRAGE.Helper
{
  internal class CallApi
  {
    private string _hosturl;
    private Dictionary<string, string> _param;
    private WebClient _wc;
    public string _resultString;
    private HttpWebRequest httpWebRequest;

    public event CallApi.ResultEventHandler ResultEvent;

    public CallApi(string host, Dictionary<string, string> param)
    {
      this._resultString = "";
      this._hosturl = host;
      this._param = new Dictionary<string, string>((IDictionary<string, string>) param);
      this._wc = new WebClient();
    }

    public Dictionary<string, string> param => this._param;

    public void DoCallPostAsync() => this._worker_DoWork();

    private void FN(IAsyncResult result)
    {
      try
      {
        HttpWebResponse response = (HttpWebResponse) this.httpWebRequest.EndGetResponse(result);
        StreamReader streamReader = new StreamReader(response.GetResponseStream(), Encoding.Default, true);
        this._resultString = streamReader.ReadToEnd();
        streamReader.Close();
        response.Close();
      }
      catch (Exception ex)
      {
        SentryApi.SendException(ex);
      }
      if (this.ResultEvent == null)
        return;
      this.ResultEvent();
    }

    private void _worker_DoWork()
    {
      try
      {
        string str = "";
        foreach (KeyValuePair<string, string> keyValuePair in this._param)
          str += string.Format("{0}={1}&", (object) keyValuePair.Key, (object) keyValuePair.Value);
        string s = str.Remove(str.Length - 1);
        this.httpWebRequest = (HttpWebRequest) WebRequest.Create(this._hosturl);
        byte[] bytes = Encoding.UTF8.GetBytes(s);
        this.httpWebRequest.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
        this.httpWebRequest.Method = "POST";
        this.httpWebRequest.ContentLength = (long) bytes.Length;
        Stream requestStream = this.httpWebRequest.GetRequestStream();
        requestStream.Write(bytes, 0, bytes.Length);
        requestStream.Close();
        this.httpWebRequest.BeginGetResponse(new AsyncCallback(this.FN), (object) null);
      }
      catch (Exception ex)
      {
        this._resultString = "-1";
        if (this.ResultEvent != null)
          this.ResultEvent();
        SentryApi.SendException(ex);
      }
    }

    public delegate void ResultEventHandler();
  }
}
