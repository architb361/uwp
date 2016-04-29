using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace rasp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private const int LED_PIN = 6;
        private GpioPin ledPin;
        private GpioPinValue ledPinValue = GpioPinValue.High;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var gpio = GpioController.GetDefault();
            ledPin = gpio.OpenPin(LED_PIN);
            ledPin.Write(GpioPinValue.High);
            ledPin.SetDriveMode(GpioPinDriveMode.Output);
            Windows.Networking.Sockets.StreamSocketListener socketListener = new Windows.Networking.Sockets.StreamSocketListener();
            socketListener.ConnectionReceived += SocketListener_ConnectionReceived;
            await socketListener.BindServiceNameAsync("6767");
        }

        private async void SocketListener_ConnectionReceived(Windows.Networking.Sockets.StreamSocketListener sender, Windows.Networking.Sockets.StreamSocketListenerConnectionReceivedEventArgs args)
        {
            //Stream inStream = args.Socket.InputStream.AsStreamForRead();
            //StreamReader reader = new StreamReader(inStream);
            //string request = await reader.ReadLineAsync();

            ledPinValue = (ledPinValue == GpioPinValue.Low) ? GpioPinValue.High : GpioPinValue.Low;
            ledPin.Write(ledPinValue);

            await Task.Delay(TimeSpan.FromMilliseconds(70));

            ledPinValue = (ledPinValue == GpioPinValue.Low) ? GpioPinValue.High : GpioPinValue.Low;
            ledPin.Write(ledPinValue);

            //Stream outStream = args.Socket.OutputStream.AsStreamForWrite();
            //StreamWriter writer = new StreamWriter(outStream);
            //await writer.WriteLineAsync("Door opened");
            //await writer.FlushAsync();

        }
    }
}
