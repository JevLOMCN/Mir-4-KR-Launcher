using System;
using System.Runtime.InteropServices;

namespace MIRAGE
{
  internal class UtilDrive
  {
    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool GetDiskFreeSpaceEx(
      string directory,
      out ulong availableFreeByteCount,
      out ulong totalByteCount,
      out ulong totalFreeByteCount);

    public long getDiskFreeSpace(string directory)
    {
      ulong availableFreeByteCount;
      UtilDrive.GetDiskFreeSpaceEx(directory, out availableFreeByteCount, out ulong _, out ulong _);
      return Convert.ToInt64(availableFreeByteCount / 1024UL / 1024UL);
    }
  }
}
