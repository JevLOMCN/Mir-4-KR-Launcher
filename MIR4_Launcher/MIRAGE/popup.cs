using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MIRAGE
{
  public class popup : Window, IComponentConnector
  {
    private string popup_msg = "";
    private int _type;
    internal Image ImgClose;
    internal Label label_check_string;
    internal Image BtnCancel;
    internal Image BtnClose;
    internal Image Btncheck;
    private bool _contentLoaded;

    public popup(int type, string _text)
    {
      this._type = type;
      this.popup_msg = _text;
      this.InitializeComponent();
    }

    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key != Key.System || e.SystemKey != Key.F4)
        return;
      e.Handled = true;
    }

    private void BtnCancel_MouseLeave(object sender, MouseEventArgs e)
    {
      this.Cursor = Cursors.Arrow;
      this.BtnCancel.Source = new Image()
      {
        Source = ((ImageSource) new BitmapImage(new Uri("pack://application:,,,/Resources/BNT_tray_OFF.png")))
      }.Source;
    }

    private void BtnCancel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
    }

    private void BtnCancel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.DialogResult = new bool?(false);
      this.Close();
    }

    private void BtnCancel_MouseEnter(object sender, MouseEventArgs e)
    {
      this.Cursor = Cursors.Hand;
      this.BtnCancel.Source = new Image()
      {
        Source = ((ImageSource) new BitmapImage(new Uri("pack://application:,,,/Resources/BNT_tray_on.png")))
      }.Source;
    }

    private void BtnClose_MouseEnter(object sender, MouseEventArgs e)
    {
      this.Cursor = Cursors.Hand;
      this.BtnClose.Source = new Image()
      {
        Source = ((ImageSource) new BitmapImage(new Uri("pack://application:,,,/Resources/BNT_close_on.png")))
      }.Source;
    }

    private void BtnClose_MouseLeave(object sender, MouseEventArgs e)
    {
      this.Cursor = Cursors.Arrow;
      this.BtnClose.Source = new Image()
      {
        Source = ((ImageSource) new BitmapImage(new Uri("pack://application:,,,/Resources/BNT_close_off.png")))
      }.Source;
    }

    private void BtnClose_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
    }

    private void BtnClose_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.DialogResult = new bool?(true);
      this.Close();
    }

    private void BtnCheck_MouseEnter(object sender, MouseEventArgs e)
    {
      this.Cursor = Cursors.Hand;
      this.Btncheck.Source = new Image()
      {
        Source = ((ImageSource) new BitmapImage(new Uri("pack://application:,,,/Resources/BNT_check_on.png")))
      }.Source;
    }

    private void BtnCheck_MouseLeave(object sender, MouseEventArgs e)
    {
      this.Cursor = Cursors.Arrow;
      this.Btncheck.Source = new Image()
      {
        Source = ((ImageSource) new BitmapImage(new Uri("pack://application:,,,/Resources/BNT_check_off.png")))
      }.Source;
    }

    private void BtnCheck_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
    }

    private void BtnCheck_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) => this.Close();

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      if (this._type == 3)
      {
        this.label_check_string.Visibility = Visibility.Hidden;
        this.ImgClose.Visibility = Visibility.Hidden;
        this.BtnCancel.Visibility = Visibility.Hidden;
        this.BtnClose.Visibility = Visibility.Hidden;
        this.Btncheck.Visibility = Visibility.Hidden;
        this.Close();
      }
      if (this._type == 0)
      {
        this.label_check_string.Visibility = Visibility.Hidden;
        this.ImgClose.Visibility = Visibility.Visible;
        this.BtnCancel.Visibility = Visibility.Visible;
        this.BtnClose.Visibility = Visibility.Visible;
        this.Btncheck.Visibility = Visibility.Hidden;
      }
      else
      {
        if (this._type != 1)
          return;
        this.label_check_string.Content = (object) this.popup_msg;
        this.label_check_string.Visibility = Visibility.Visible;
        this.ImgClose.Visibility = Visibility.Hidden;
        this.BtnCancel.Visibility = Visibility.Hidden;
        this.BtnClose.Visibility = Visibility.Hidden;
        this.Btncheck.Visibility = Visibility.Visible;
      }
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/MIR4_Launcher;component/popup.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          ((UIElement) target).KeyDown += new KeyEventHandler(this.Window_KeyDown);
          ((FrameworkElement) target).Loaded += new RoutedEventHandler(this.Window_Loaded);
          break;
        case 2:
          this.ImgClose = (Image) target;
          break;
        case 3:
          this.label_check_string = (Label) target;
          break;
        case 4:
          this.BtnCancel = (Image) target;
          this.BtnCancel.MouseLeftButtonUp += new MouseButtonEventHandler(this.BtnCancel_MouseLeftButtonUp);
          this.BtnCancel.MouseEnter += new MouseEventHandler(this.BtnCancel_MouseEnter);
          this.BtnCancel.MouseLeave += new MouseEventHandler(this.BtnCancel_MouseLeave);
          this.BtnCancel.MouseLeftButtonDown += new MouseButtonEventHandler(this.BtnCancel_MouseLeftButtonDown);
          break;
        case 5:
          this.BtnClose = (Image) target;
          this.BtnClose.MouseLeftButtonUp += new MouseButtonEventHandler(this.BtnClose_MouseLeftButtonUp);
          this.BtnClose.MouseEnter += new MouseEventHandler(this.BtnClose_MouseEnter);
          this.BtnClose.MouseLeave += new MouseEventHandler(this.BtnClose_MouseLeave);
          this.BtnClose.MouseLeftButtonDown += new MouseButtonEventHandler(this.BtnClose_MouseLeftButtonDown);
          break;
        case 6:
          this.Btncheck = (Image) target;
          this.Btncheck.MouseLeftButtonUp += new MouseButtonEventHandler(this.BtnCheck_MouseLeftButtonUp);
          this.Btncheck.MouseEnter += new MouseEventHandler(this.BtnCheck_MouseEnter);
          this.Btncheck.MouseLeave += new MouseEventHandler(this.BtnCheck_MouseLeave);
          this.Btncheck.MouseLeftButtonDown += new MouseButtonEventHandler(this.BtnCheck_MouseLeftButtonDown);
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
