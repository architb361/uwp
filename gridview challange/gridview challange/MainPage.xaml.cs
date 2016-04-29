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
using gridview_challange.Model;
using System.Collections.ObjectModel;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace gridview_challange
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        ObservableCollection<NewsItem> NewsItems;
        public MainPage()
        {
            this.InitializeComponent();
            NewsItems = new ObservableCollection<NewsItem>();
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;   
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(Finanacial.IsSelected)
            {
                NewFactory.GetNews("Financial", NewsItems);
                TitleTextBlock.Text = "Financial";
            }
            else if(Food.IsSelected)
            {
                NewFactory.GetNews("Food", NewsItems);
                TitleTextBlock.Text = "Food";
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Finanacial.IsSelected = true;
        }
    }
}
