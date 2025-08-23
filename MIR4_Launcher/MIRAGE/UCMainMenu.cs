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
  public class UCMainMenu : UserControl, IComponentConnector
  {
    private string _OnImageUrl = "";
    private string _OffImageUrl = "";
    private string _ImgTitle = "";
    internal StackPanel MenuDiv;
    internal Image ImgMenu;
    internal Image ImgTitle;
    private bool _contentLoaded;

    public string OnImage
    {
      get => this._OnImageUrl;
      set => this._OnImageUrl = value;
    }

    public string OffImage
    {
      get => this._OffImageUrl;
      set
      {
        this._OffImageUrl = value;
        this.OffImageDisp();
      }
    }

    public string TitleImage
    {
      get => this._ImgTitle;
      set
      {
        this._ImgTitle = value;
        this.TitleImageDisp();
      }
    }

    public UCMainMenu() => this.InitializeComponent();

    private void MenuDiv_MouseEnter(object sender, MouseEventArgs e) => this.OnImageDisp();

    private void MenuDiv_MouseLeave(object sender, MouseEventArgs e) => this.OffImageDisp();

    private void OnImageDisp()
    {
      try
      {
        if (this._OnImageUrl.Length <= 0)
          return;
        this.ImgMenu.Source = new Image()
        {
          Source = ((ImageSource) new BitmapImage(new Uri("pack://application:,,," + this._OnImageUrl)))
        }.Source;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
      }
    }

    private void OffImageDisp()
    {
      try
      {
        if (this._OffImageUrl.Length <= 0)
          return;
        this.ImgMenu.Source = new Image()
        {
          Source = ((ImageSource) new BitmapImage(new Uri("pack://application:,,," + this._OffImageUrl)))
        }.Source;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
      }
    }

    private void TitleImageDisp()
    {
      try
      {
        if (this._ImgTitle.Length <= 0)
          return;
        this.ImgTitle.Source = new Image()
        {
          Source = ((ImageSource) new BitmapImage(new Uri("pack://application:,,," + this._ImgTitle)))
        }.Source;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
      }
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/MIR4_Launcher;component/ucmainmenu.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.MenuDiv = (StackPanel) target;
          this.MenuDiv.MouseEnter += new MouseEventHandler(this.MenuDiv_MouseEnter);
          this.MenuDiv.MouseLeave += new MouseEventHandler(this.MenuDiv_MouseLeave);
          break;
        case 2:
          this.ImgMenu = (Image) target;
          break;
        case 3:
          this.ImgTitle = (Image) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
