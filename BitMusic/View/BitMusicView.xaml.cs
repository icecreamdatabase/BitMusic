using System.Windows;
using BitMusic.ViewModel;

namespace BitMusic.View
{
    /// <summary>
    /// Interaction logic for BitMusicView.xaml
    /// </summary>
    public partial class BitMusicView : Window
    {
        public BitMusicView()
        {
            DataContext = new BitMusicViewModel();
            InitializeComponent();
        }
    }
}
