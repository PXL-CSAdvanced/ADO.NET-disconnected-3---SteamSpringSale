using Microsoft.Win32;
using SteamLibraryClassLibrary;
using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ADO.NET_disconnected_3___SteamSpringSale;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        OpenFileDialog ofd = new OpenFileDialog();
        if (ofd.ShowDialog() == true)
        {
            SteamGamesData.ImportFromCsv(ofd.FileName);
            SteamGamesDataGrid.ItemsSource = SteamGamesData.SteamDataSet.Tables["Games"].DefaultView;
        }
    }

    private void SteamGamesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        DataRowView? rowView = (SteamGamesDataGrid.SelectedItem as DataRowView);
        if (rowView != null)
        {
            DataRow row = rowView.Row;
            string steamAppId = row["steam_appid"].ToString();
            GameNameTextBlock.Text = row["name"].ToString();
            PositiveReviewTextBlock.Text = row["total_positive"].ToString();
            NegativeReviewTextBlock.Text = row["total_negative"].ToString();

            Library600x900Image.Source = new BitmapImage(new Uri($"https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/{steamAppId}/library_600x900_2x.jpg", UriKind.Absolute));
            LibraryHeroImage.Source = new BitmapImage(new Uri($"https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/{steamAppId}/library_hero.jpg", UriKind.Absolute));
            LogoImage.Source = new BitmapImage(new Uri($"https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/{steamAppId}/logo.png", UriKind.Absolute));

            GenresListBox.Items.Clear();
            CategoriesListBox.Items.Clear();

            // TODO vul genres + categories in
        }
    }

    private void SearchButton_Click(object sender, RoutedEventArgs e)
    {
        SteamGamesDataGrid.ItemsSource = SteamGamesData.SearchGamesTable(SearchTextBox.Text).DefaultView;
    }
}