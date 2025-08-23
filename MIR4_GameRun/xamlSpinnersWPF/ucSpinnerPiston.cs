using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Shapes;

namespace xamlSpinnersWPF
{
  public class ucSpinnerPiston : System.Windows.Controls.UserControl, IComponentConnector
  {
    internal ucSpinnerPiston UserControl;
    internal Ellipse ellipse;
    internal Ellipse ellipse1;
    private bool _contentLoaded;

    public ucSpinnerPiston() => this.InitializeComponent();

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/MIR4_GameRun;component/spinners/ucspinnerpiston.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.UserControl = (ucSpinnerPiston) target;
          break;
        case 2:
          this.ellipse = (Ellipse) target;
          break;
        case 3:
          this.ellipse1 = (Ellipse) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
