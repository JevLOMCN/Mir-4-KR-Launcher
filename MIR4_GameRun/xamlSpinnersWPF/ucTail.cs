using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace xamlSpinnersWPF
{
  public class ucTail : System.Windows.Controls.UserControl, IComponentConnector
  {
    internal ucTail UserControl;
    internal Grid LayoutRoot;
    private bool _contentLoaded;

    public ucTail() => this.InitializeComponent();

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/MIR4_GameRun;component/spinners/uctail.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      if (connectionId != 1)
      {
        if (connectionId == 2)
          this.LayoutRoot = (Grid) target;
        else
          this._contentLoaded = true;
      }
      else
        this.UserControl = (ucTail) target;
    }
  }
}
