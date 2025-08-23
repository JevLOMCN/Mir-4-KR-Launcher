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
  public class ucSpinnerDotCircle : System.Windows.Controls.UserControl, IComponentConnector
  {
    internal ucSpinnerDotCircle UserControl;
    internal Canvas spinner_1;
    internal Ellipse ellipse01;
    internal Ellipse ellipse02;
    internal Ellipse ellipse03;
    internal Ellipse ellipse04;
    internal Ellipse ellipse05;
    internal Ellipse ellipse06;
    internal Ellipse ellipse07;
    internal Ellipse ellipse08;
    internal Ellipse ellipse09;
    internal Ellipse ellipse10;
    internal Ellipse ellipse11;
    internal Ellipse ellipse12;
    private bool _contentLoaded;

    public ucSpinnerDotCircle() => this.InitializeComponent();

    public void aniStop()
    {
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/MIR4_GameRun;component/spinners/ucspinnerdotcircle.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.UserControl = (ucSpinnerDotCircle) target;
          break;
        case 2:
          this.spinner_1 = (Canvas) target;
          break;
        case 3:
          this.ellipse01 = (Ellipse) target;
          break;
        case 4:
          this.ellipse02 = (Ellipse) target;
          break;
        case 5:
          this.ellipse03 = (Ellipse) target;
          break;
        case 6:
          this.ellipse04 = (Ellipse) target;
          break;
        case 7:
          this.ellipse05 = (Ellipse) target;
          break;
        case 8:
          this.ellipse06 = (Ellipse) target;
          break;
        case 9:
          this.ellipse07 = (Ellipse) target;
          break;
        case 10:
          this.ellipse08 = (Ellipse) target;
          break;
        case 11:
          this.ellipse09 = (Ellipse) target;
          break;
        case 12:
          this.ellipse10 = (Ellipse) target;
          break;
        case 13:
          this.ellipse11 = (Ellipse) target;
          break;
        case 14:
          this.ellipse12 = (Ellipse) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
