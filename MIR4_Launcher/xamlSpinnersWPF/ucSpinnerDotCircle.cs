using System.Windows.Controls;

namespace xamlSpinnersWPF
{
    // Public partial, parameterless ctor, calls InitializeComponent
    public partial class ucSpinnerDotCircle : UserControl
    {
        public ucSpinnerDotCircle()
        {
            InitializeComponent();
        }

        public void aniStop()
        {
            // optional: pause/stop storyboard if you wire it up here later
        }
    }
}