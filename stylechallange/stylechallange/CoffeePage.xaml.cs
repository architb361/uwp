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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace stylechallange
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CoffeePage : Page
    {
        string _rost;
        string _sweetner;
        string _cream;
        public CoffeePage()
        {
            this.InitializeComponent();
        }

        private void RostClicked(object sender, RoutedEventArgs e)
        {
            var selected = (MenuFlyoutItem)sender;
            _rost = selected.Text;
            display();
        }

        private void SweetnerClicked(object sender, RoutedEventArgs e)
        {
            var selected = (MenuFlyoutItem)sender;
            _sweetner = selected.Text;
            display();
        }

        private void CreamClicked(object sender, RoutedEventArgs e)
        {
            var selected = (MenuFlyoutItem)sender;
            _cream = selected.Text;
            display();
        }

        void display()
        {
            if (_rost == "None" || string.IsNullOrEmpty(_rost))
            {
                ResultTextBlock.Text = "None";
                return;
            }
            ResultTextBlock.Text = _rost;
            if (_sweetner != "None" && !string.IsNullOrEmpty(_sweetner))
                ResultTextBlock.Text += "+" + _sweetner;
            if (_cream != "None" && !string.IsNullOrEmpty(_cream))
                ResultTextBlock.Text += "+" + _cream;
            

        }
    }
}
