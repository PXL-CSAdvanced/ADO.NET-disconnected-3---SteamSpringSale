using ADO.NET_disconnected_3___SteamSpringSale.Windows;
using Microsoft.Win32;
using SteamLibraryClassLibrary;
using System.ComponentModel;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Xps;

namespace ADO.NET_disconnected_3___SteamSpringSale;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private bool _isPlaying;
    private DateTime _lastClickTime;
    private List<string> _videoUrls = new List<string>();
    private bool _isSliderVisible;
    private const string NoFilter = "--None--";
    public MainWindow()
    {
        InitializeComponent();
    }

    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        await Task.Run(() =>
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == true)
            {
                SteamGamesData.ImportFromCsv(ofd.FileName);
            }
        });
        if (SteamGamesData.SteamDataSet != null)
        {
            steamGamesDataGrid.ItemsSource = SteamGamesData.SteamDataSet.Tables["Games"].DefaultView;
            categoryComboBox.Items.Clear();
            categoryComboBox.Items.Add(NoFilter);
            foreach (string category in SteamGamesData.GetAllCategories())
            {
                categoryComboBox.Items.Add(category);
            }
            genreComboBox.Items.Clear();
            genreComboBox.Items.Add(NoFilter);
            foreach (string genre in SteamGamesData.GetAllGenres())
            {
                genreComboBox.Items.Add(genre);
            }
        }
    }

    private void PopularGamesMenuItem_Click(object sender, RoutedEventArgs e)
    {
        PopularGamesWindow popularGamesWindow = new PopularGamesWindow(SteamGamesData.GetAllPopularGames());
        popularGamesWindow.Show();
    }

    private void ProfitableGamesMenuItem_Click(object sender, RoutedEventArgs e)
    {
        ProfitableGamesWindow profitableGamesWindow = new ProfitableGamesWindow(SteamGamesData.GetAllProfitableGames());
        profitableGamesWindow.Show();
    }

    private async void SteamGamesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        DataRowView? rowView = (steamGamesDataGrid.SelectedItem as DataRowView);
        if (rowView != null)
        {
            DataRow row = rowView.Row;
            string steamAppId = row["steam_appid"].ToString() ?? "620";
            gameNameTextBlock.Text = row["name"].ToString();
            positiveReviewTextBlock.Text = row["total_positive"].ToString();
            negativeReviewTextBlock.Text = row["total_negative"].ToString();

            library600x900Image.MaxWidth = library600x900Image.ActualWidth;
            library600x900Image.Source = new BitmapImage(new Uri(GameDetailsData.Library600x900(steamAppId), UriKind.Absolute));
            libraryHeroImage.Source = new BitmapImage(new Uri(GameDetailsData.LibraryHero(steamAppId), UriKind.Absolute));
            logoImage.Source = new BitmapImage(new Uri(GameDetailsData.Logo(steamAppId), UriKind.Absolute));

            _videoUrls = await GameDetailsData.VideoURLsAsync(steamAppId);
            UpdateSlider();

            genresListBox.Items.Clear();    
            categoriesListBox.Items.Clear();
            foreach (string? genre in SteamGamesData.GetAllGenresBySteamAppId(Convert.ToInt32(steamAppId)))
            {
                genresListBox.Items.Add(genre);
            }
            foreach (string? genre in SteamGamesData.GetAllCategoriesBySteamAppId(Convert.ToInt32(steamAppId)))
            {
                categoriesListBox.Items.Add(genre);
            }
        }
    }

    private void UpdateSlider()
    {
        if (_videoUrls.Count > 0)
        {
            videoPlayer.Source = new Uri(_videoUrls[0], UriKind.Absolute);
            CustomSlider.Maximum = _videoUrls.Count - 1;
            CustomSlider.Value = 0;
            Track track = CustomSlider.Template.FindName("PART_Track", CustomSlider) as Track;
            if (track != null)
            {
                track.Thumb.Width = CustomSlider.ActualWidth / _videoUrls.Count;
            }
        }
    }

    private void SearchGrid()
    {
        DataTable filteredDataTable = SteamGamesData.SearchGamesTable(SearchTextBox.Text, SteamGamesData.SteamGamesDataTable);
        if (genreComboBox.SelectedItem != null)
        {
            string genreFilter = genreComboBox.SelectedItem.ToString();
            if (!NoFilter.Equals(genreFilter))
            {
                filteredDataTable = SteamGamesData.FilterGenre(genreFilter, filteredDataTable);
            }
        }
        if (categoryComboBox.SelectedItem != null)
        {
            string categoryFilter = categoryComboBox.SelectedItem.ToString();
            if (!NoFilter.Equals(categoryFilter))
            {
                filteredDataTable = SteamGamesData.FilterCategories(categoryFilter, filteredDataTable);
            }
        }
        steamGamesDataGrid.ItemsSource = filteredDataTable.DefaultView;
    }

    private void Video_Click(object sender, MouseButtonEventArgs e)
    {
        TimeSpan clickGap = DateTime.Now - _lastClickTime;
        _lastClickTime = DateTime.Now;
        if (clickGap.TotalMilliseconds < 400)
        {
            _isPlaying = true;
            videoPlayer.Position = System.TimeSpan.Zero;
            videoPlayer.Play();
        }
        else if (!_isPlaying)
        {
            _isPlaying = true;
            videoPlayer.Play();
        }
        else
        {
            _isPlaying = false;
            videoPlayer.Pause();
        }
    }

    private void SearchButton_Click(object sender, RoutedEventArgs e)
    {
        SearchGrid();
    }

    private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            SearchGrid();
        }
    }
    private void Container_MouseMove(object sender, MouseEventArgs e)
    {
        Point position = e.GetPosition(Container);
        if (position.Y >= Container.ActualHeight - 50 && !_isSliderVisible)
        {
            ShowSlider(SliderPanel);
        }
        else if (position.Y <= Container.ActualHeight - 60 && _isSliderVisible)
        {
            HideSlider(SliderPanel);
        }
    }

    private void Container_MouseLeave(object sender, MouseEventArgs e)
    {
        HideSlider(SliderPanel);
    }

    private void ShowSlider(FrameworkElement element)
    {
        _isSliderVisible = true;
        DoubleAnimation opacityAnim = new DoubleAnimation(1, TimeSpan.FromMilliseconds(200));
        DoubleAnimation translateAnim = new DoubleAnimation(1, TimeSpan.FromMilliseconds(200));

        element.BeginAnimation(OpacityProperty, opacityAnim);
        element.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, translateAnim);
    }

    private void HideSlider(FrameworkElement element)
    {
        _isSliderVisible = false;

        DoubleAnimation opacityAnim = new DoubleAnimation(0, TimeSpan.FromMilliseconds(200));
        DoubleAnimation translateAnim = new DoubleAnimation(0, TimeSpan.FromMilliseconds(200));

        element.BeginAnimation(OpacityProperty, opacityAnim);
        element.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, translateAnim);
    }

    private void CustomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (_videoUrls != null && _videoUrls.Count != 0 && videoPlayer != null)
        {
            int newValue = Convert.ToInt32(e.NewValue);
            int index = newValue < _videoUrls.Count ? newValue : _videoUrls.Count - 1;
            index = index < 0 ? 0 : index;
            videoPlayer.Source = new Uri(_videoUrls[index], UriKind.Absolute);
        }
    }

    private void AllGenresMenuItem_Click(object sender, RoutedEventArgs e)
    {
        AllGenresWindow allGenresWindow = new AllGenresWindow(SteamGamesData.GetAllGenres());
        allGenresWindow.Show();
    }

    private void GenreComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        SearchGrid();
    }

    private void CategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        SearchGrid();
    }

    private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        library600x900Image.MaxWidth = 490;
    }
}