using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Shapes;

namespace xamlSpinnersWPF
{
  public class ucSpinnerCogs : System.Windows.Controls.UserControl, IComponentConnector
  {
    internal ucSpinnerCogs UserControl;
    internal Canvas uc_SpinnerCogs;
    internal Path topCog;
    internal Path bottomCog;
    private bool _contentLoaded;

    public ucSpinnerCogs() => this.InitializeComponent();

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/MIR4_Launcher;component/spinners/ucspinnercogs.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.UserControl = (ucSpinnerCogs) target;
          break;
        case 2:
          this.uc_SpinnerCogs = (Canvas) target;
          break;
        case 3:
          this.topCog = (Path) target;
          break;
        case 4:
          this.bottomCog = (Path) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
