using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace xamlSpinnersWPF
{
  public class ucSpinnerSwirl : System.Windows.Controls.UserControl, IComponentConnector
  {
    internal ucSpinnerSwirl UserControl;
    internal Canvas canvas;
    private bool _contentLoaded;

    public ucSpinnerSwirl() => this.InitializeComponent();

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/MIR4_Launcher;component/spinners/ucspinnerswirl.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      if (connectionId != 1)
      {
        if (connectionId == 2)
          this.canvas = (Canvas) target;
        else
          this._contentLoaded = true;
      }
      else
        this.UserControl = (ucSpinnerSwirl) target;
    }
  }
}
