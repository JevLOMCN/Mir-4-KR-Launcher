using Sentry;
using System;

namespace MIRAGE.Helper
{
  internal class SentryApi
  {
    private static string _SENTRY_DNS = "https://753b20a946854fd189f7da91acb8d808@o464727.ingest.sentry.io/5475019";

    public static void SendException(Exception ex)
    {
      using (SentrySdk.Init(SentryApi._SENTRY_DNS))
        SentrySdk.ConfigureScope((Action<Scope>) (scope => SentrySdk.CaptureException(ex)));
    }
  }
}
