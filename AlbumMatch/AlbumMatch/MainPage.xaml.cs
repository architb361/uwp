using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using Windows.Storage.FileProperties;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using AlbumMatch.Models;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace AlbumMatch
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private ObservableCollection<Song> Songs;
        private ObservableCollection<StorageFile> allSongs;
        bool _MusicPlaying = false;
        int _round = 0;
        int _totalscore = 0;
        public MainPage()
        {
            this.InitializeComponent();
            Songs = new ObservableCollection<Song>();
            allSongs = new ObservableCollection<StorageFile>();
        }

        private async Task RetriveFilesInFolders(ObservableCollection<StorageFile> list,StorageFolder parent)
        {
            foreach(var item in await parent.GetFilesAsync())
            {
                if (item.FileType == ".mp3")
                    list.Add(item);
            }
            foreach (var item in await parent.GetFoldersAsync())
            {
                await RetriveFilesInFolders(list, item);
            }
        }
        private async Task<List<StorageFile>> PickRandomSongs(ObservableCollection<StorageFile> allSongs)
        {
            Random random = new Random();
            var songCount = allSongs.Count;
            var randomSongs = new List<StorageFile>();
            while (randomSongs.Count < 10)
            {
                var randomNumber = random.Next(songCount);
                var randomSong = allSongs[randomNumber];
                MusicProperties prop = await randomSong.Properties.GetMusicPropertiesAsync();

                bool isDup = false;
                foreach (var song in randomSongs)
                {
                    MusicProperties songprop = await song.Properties.GetMusicPropertiesAsync();
                    if (String.IsNullOrEmpty(prop.Album) || prop.Album == songprop.Album)
                        isDup = true;
                }
                if(!isDup)
                    randomSongs.Add(randomSong);
            }
            return randomSongs;
        }
        private async Task PopulateSongList(List<StorageFile> files)
        {
            int id = 0;
            foreach (var file in files)
            {
                MusicProperties songproperties = await file.Properties.GetMusicPropertiesAsync();
                StorageItemThumbnail currentThumb = await file.GetThumbnailAsync(ThumbnailMode.MusicView,200,ThumbnailOptions.UseCurrentScale);
                var albumcover = new BitmapImage();
                albumcover.SetSource(currentThumb);
                var song = new Song();
                song.Id = id;
                song.Title = songproperties.Title;
                song.Artist = songproperties.Artist;
                song.Album = songproperties.Album;
                song.AlbumCover = albumcover;
                song.SongFile = file;
                Songs.Add(song);
                id++;
            }
        }

        private async void SongGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (!_MusicPlaying)
                return;
            CounDown.Pause();
            MyMediaElement.Stop();

            var clickedSong = (Song)e.ClickedItem;
            var correctSong = Songs.FirstOrDefault(p => p.Selected == true);
            int score;
            if (clickedSong.Selected)
            {
                var uri = new Uri("ms-appx:///Assets/correct.png");
                StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(uri);
                var filestream = await file.OpenAsync(FileAccessMode.Read);
                await clickedSong.AlbumCover.SetSourceAsync(filestream);
                score = (int)myProgressBar.Value;
            }
            else
            {
                var uri = new Uri("ms-appx:///Assets/incorrect.png");
                StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(uri);
                var filestream = await file.OpenAsync(FileAccessMode.Read);
                await clickedSong.AlbumCover.SetSourceAsync(filestream);
                score = (int)myProgressBar.Value * -1;
            }
            _totalscore += score;
            ResultTextBlock.Text = String.Format("Score {0} Total Score after {1} Rounds: {2}", score, _round, _totalscore);
            TitleTextBlock.Text = String.Format("Correct Song: {0}", correctSong.Title);
            ArtistTextBlock.Text = String.Format("Performed by: {0}", correctSong.Artist);
            AlbumTextBlock.Text = String.Format("On Album: {0}", correctSong.Album);
            clickedSong.Used = true;
            correctSong.Selected = false;
            correctSong.Used = true;

            _round++;

            if (_round>=5)
            {
                InstructionTextBlock.Text = String.Format("GAME OVER", _totalscore);
                PlayAgainButton.Visibility = Visibility.Visible;
            }
            else
            {
                StarCooldown();
            }
            
        }

        private async void PlayAgainButton_Click(object sender, RoutedEventArgs e)
        {
            await PrepareNewGame();

            PlayAgainButton.Visibility = Visibility.Collapsed;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
                            
        }
        private async Task<ObservableCollection<StorageFile>> SetupMusicList()
        {
            StorageFolder folder = KnownFolders.MusicLibrary;
            var allSongs = new ObservableCollection<StorageFile>();
            await RetriveFilesInFolders(allSongs, folder);
            return allSongs;
        }
        private async Task PrepareNewGame()
        {
            Songs.Clear();

            var randomSongs = await PickRandomSongs(allSongs);

            await PopulateSongList(randomSongs);

            StarCooldown();
            InstructionTextBlock.Text = "Get ready ...";
            ResultTextBlock.Text = "";
            TitleTextBlock.Text = "";
            ArtistTextBlock.Text = "";
            AlbumTextBlock.Text = "";
            _totalscore = 0;
            _round = 0;
        }

        private async void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            StartupProgressRing.IsActive = true;
            allSongs = await SetupMusicList();
            await PrepareNewGame();
            StartupProgressRing.IsActive = false;
            StarCooldown();
        }
        private void StarCooldown()
        {
            _MusicPlaying = false;
            SolidColorBrush brush = new SolidColorBrush(Colors.Blue);
            myProgressBar.Foreground = brush;
            InstructionTextBlock.Text = String.Format("Get ready for round {0}...", _round + 1);
            InstructionTextBlock.Foreground = brush;
            CounDown.Begin();
        }
        private void StartCountdown()
        {
            _MusicPlaying = true;
            SolidColorBrush brush = new SolidColorBrush(Colors.Red);
            myProgressBar.Foreground = brush;
            InstructionTextBlock.Text = "GO!";
            InstructionTextBlock.Foreground = brush;
            CounDown.Begin();
        }
        private async void CounDown_Completed(object sender, object e)
        {
            if (!_MusicPlaying)
            {
                var song = PickSong();
                MyMediaElement.SetSource(await song.SongFile.OpenAsync(FileAccessMode.Read), song.SongFile.ContentType);
                StartCountdown();
            }
        }
        private Song PickSong()
        {
            Random random = new Random();
            var unusedSongs = Songs.Where(p => p.Used == false);
            var randomNumber = random.Next(unusedSongs.Count());
            var randomSong = unusedSongs.ElementAt(randomNumber);
            randomSong.Selected = true;
            return randomSong;
        }
    }
}
