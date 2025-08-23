using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace MIRAGE.Properties
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class Resources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal Resources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (MIRAGE.Properties.Resources.resourceMan == null)
          MIRAGE.Properties.Resources.resourceMan = new ResourceManager("MIRAGE.Properties.Resources", typeof (MIRAGE.Properties.Resources).Assembly);
        return MIRAGE.Properties.Resources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => MIRAGE.Properties.Resources.resourceCulture;
      set => MIRAGE.Properties.Resources.resourceCulture = value;
    }

    internal static Bitmap basic_bg
    {
      get
      {
        return (Bitmap) MIRAGE.Properties.Resources.ResourceManager.GetObject(nameof (basic_bg), MIRAGE.Properties.Resources.resourceCulture);
      }
    }

    internal static Bitmap BNT_check_off
    {
      get
      {
        return (Bitmap) MIRAGE.Properties.Resources.ResourceManager.GetObject(nameof (BNT_check_off), MIRAGE.Properties.Resources.resourceCulture);
      }
    }

    internal static Bitmap BNT_check_on
    {
      get
      {
        return (Bitmap) MIRAGE.Properties.Resources.ResourceManager.GetObject(nameof (BNT_check_on), MIRAGE.Properties.Resources.resourceCulture);
      }
    }
  }
}
