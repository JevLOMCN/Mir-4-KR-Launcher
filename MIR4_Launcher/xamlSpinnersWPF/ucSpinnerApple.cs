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
  public class ucSpinnerApple : System.Windows.Controls.UserControl, IComponentConnector
  {
    internal ucSpinnerApple UserControl;
    internal Canvas spinner_1;
    internal Grid LayoutRoot;
    internal Rectangle Rectangle01;
    internal Rectangle Rectangle02;
    internal Rectangle Rectangle03;
    internal Rectangle Rectangle04;
    internal Rectangle Rectangle05;
    internal Rectangle Rectangle06;
    internal Rectangle Rectangle07;
    internal Rectangle Rectangle08;
    internal Rectangle Rectangle09;
    internal Rectangle Rectangle10;
    internal Rectangle Rectangle11;
    internal Rectangle Rectangle12;
    private bool _contentLoaded;

    public ucSpinnerApple() => this.InitializeComponent();

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/MIR4_Launcher;component/spinners/ucspinnerapple.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.UserControl = (ucSpinnerApple) target;
          break;
        case 2:
          this.spinner_1 = (Canvas) target;
          break;
        case 3:
          this.LayoutRoot = (Grid) target;
          break;
        case 4:
          this.Rectangle01 = (Rectangle) target;
          break;
        case 5:
          this.Rectangle02 = (Rectangle) target;
          break;
        case 6:
          this.Rectangle03 = (Rectangle) target;
          break;
        case 7:
          this.Rectangle04 = (Rectangle) target;
          break;
        case 8:
          this.Rectangle05 = (Rectangle) target;
          break;
        case 9:
          this.Rectangle06 = (Rectangle) target;
          break;
        case 10:
          this.Rectangle07 = (Rectangle) target;
          break;
        case 11:
          this.Rectangle08 = (Rectangle) target;
          break;
        case 12:
          this.Rectangle09 = (Rectangle) target;
          break;
        case 13:
          this.Rectangle10 = (Rectangle) target;
          break;
        case 14:
          this.Rectangle11 = (Rectangle) target;
          break;
        case 15:
          this.Rectangle12 = (Rectangle) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
