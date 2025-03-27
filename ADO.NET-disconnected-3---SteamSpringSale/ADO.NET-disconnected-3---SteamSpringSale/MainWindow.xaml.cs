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
    private bool _isPlaying;
    private DateTime _lastClickTime;
    private List<string> _videoUrls;

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

    private async void SteamGamesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        DataRowView? rowView = (SteamGamesDataGrid.SelectedItem as DataRowView);
        if (rowView != null)
        {
            DataRow row = rowView.Row;
            string steamAppId = row["steam_appid"].ToString() ?? "620";
            gameNameTextBlock.Text = row["name"].ToString();
            positiveReviewTextBlock.Text = row["total_positive"].ToString();
            negativeReviewTextBlock.Text = row["total_negative"].ToString();

            library600x900Image.Source = new BitmapImage(new Uri(GameDetailsData.Library600x900(steamAppId), UriKind.Absolute));
            libraryHeroImage.Source = new BitmapImage(new Uri(GameDetailsData.LibraryHero(steamAppId), UriKind.Absolute));
            logoImage.Source = new BitmapImage(new Uri(GameDetailsData.Logo(steamAppId), UriKind.Absolute));

            _videoUrls = await GameDetailsData.VideoURLsAsync(steamAppId);

            if (_videoUrls.Count > 0)
            {
                videoPlayer.Source = new Uri(_videoUrls[0], UriKind.Absolute);
            }

            GenresListBox.Items.Clear();    
            CategoriesListBox.Items.Clear();
    
            // TODO vul genres + categories in
        }
    }

    private void Video_Click(object sender, MouseButtonEventArgs e)
    {
        TimeSpan clickGap = DateTime.Now - _lastClickTime;
        _lastClickTime = DateTime.Now;
        if (!_isPlaying)
        {
            _isPlaying = true;
            if (clickGap.TotalMilliseconds < 400)
            {
                videoPlayer.Position = System.TimeSpan.Zero;
                videoPlayer.Play();
            }
            else
            {
                videoPlayer.Play();
            }
        }
        else
        {
            _isPlaying = false;
            videoPlayer.Pause();
        }
    }

    private void SearchButton_Click(object sender, RoutedEventArgs e)
    {
        SteamGamesDataGrid.ItemsSource = SteamGamesData.SearchGamesTable(SearchTextBox.Text).DefaultView;
    }
}