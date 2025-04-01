using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ADO.NET_disconnected_3___SteamSpringSale.Windows
{
    /// <summary>
    /// Interaction logic for AllGenresWindow.xaml
    /// </summary>
    public partial class AllGenresWindow : Window
    {
        public AllGenresWindow(List<string> genres)
        {
            InitializeComponent();
            genresListBox.Items.Clear();
            foreach (string genre in genres)
            {
                genresListBox.Items.Add(genre);
            }
        }
    }
}
