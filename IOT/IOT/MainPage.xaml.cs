using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Devices.Gpio;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Media.SpeechSynthesis;
using Windows.Networking.Sockets;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using System;
using System.Net.Http.Headers;
using System.Text;
using System.Net.Http;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace IOT
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        IFaceServiceClient faceServiceClient;
        private const uint BufferSize = 8192;
        MediaCapture captureManager;
        StorageFile file;
        string pre;
        public MainPage()
        {
            this.InitializeComponent();
        }

        //private const int LED_PIN = 6;
        //private const int BUTTON_PIN = 5;
        //private GpioPin ledPin;
        //private GpioPin buttonPin;
        //private GpioPinValue ledPinValue = GpioPinValue.High;

        private  async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            captureManager = new MediaCapture();
            await captureManager.InitializeAsync();

            captureManager = new MediaCapture();
            await captureManager.InitializeAsync();

            capturePreview.Source = captureManager;
            await captureManager.StartPreviewAsync();

            SpeechSynthesizer speech = new SpeechSynthesizer();
            MediaElement MP = new MediaElement();
            SpeechSynthesisStream stm = await speech.SynthesizeTextToStreamAsync("You are going to add user");
            MP.SetSource(stm, stm.ContentType);
            MP.Play();

        }

        async private void StopCapturePreview_Click(object sender, RoutedEventArgs e)
        {
            await captureManager.StopPreviewAsync();
        }



        async private void CapturePhoto_Click(object sender, RoutedEventArgs e)
        {
            string s = name.Text;
            if (s!="")
            {
                ImageEncodingProperties imgFormat = ImageEncodingProperties.CreateJpeg();


                StorageFolder iot = await Windows.Storage.KnownFolders.PicturesLibrary.CreateFolderAsync("IOT", CreationCollisionOption.OpenIfExists);
                StorageFile file = await iot.CreateFileAsync("test.jpeg", CreationCollisionOption.ReplaceExisting);
                string path = file.Path;
                await captureManager.CapturePhotoToStorageFileAsync(imgFormat, file);


                // Get photo as a BitmapImage
                BitmapImage bmpImage = new BitmapImage(new Uri(file.Path));

                // imagePreview is a <Image> object defined in XAML
                imagePreview.Source = bmpImage;


                try
                {
                    await detectface(file, path);
                }
                catch
                {
                    SpeechSynthesizer speech = new SpeechSynthesizer();
                    MediaElement MP = new MediaElement();
                    SpeechSynthesisStream stm = await speech.SynthesizeTextToStreamAsync("Could not detect face please try again");
                    MP.SetSource(stm, stm.ContentType);
                    MP.Play();
                    var dialog = new MessageDialog("Could not detect face please try again");
                    await dialog.ShowAsync();
                }
            }
            else
            {
                SpeechSynthesizer speech = new SpeechSynthesizer();
                MediaElement MP = new MediaElement();
                SpeechSynthesisStream stm = await speech.SynthesizeTextToStreamAsync("name not entered");
                MP.SetSource(stm, stm.ContentType);
                MP.Play();
                var dialog = new MessageDialog("name not entered");
                await dialog.ShowAsync();

            }

        }

        async private Task detectface(StorageFile file, String path)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://api.projectoxford.ai/face/v1.0/detect?returnFaceId=true&returnFaceLandmarks=true");
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "274560707bdb402794943600db24deae");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var randomAccessStream = await file.OpenReadAsync();
        
            HttpContent content = new StreamContent(randomAccessStream.AsStreamForRead());
            content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/octet-stream");

            var response = await client.PostAsync("https://api.projectoxford.ai/face/v1.0/detect?returnFaceId=true&returnFaceLandmarks=true",content);

            var responseContent = await response.Content.ReadAsStringAsync();
            Debug.Write(responseContent);

            String s = responseContent.Substring(12,36);
            pre = s;
            Debug.Write(s);

            StorageFolder storageFolder =ApplicationData.Current.LocalFolder;
            StorageFile sampleFile =  await storageFolder.CreateFileAsync("sample.txt",CreationCollisionOption.OpenIfExists);
            await FileIO.AppendTextAsync(sampleFile,s +" = "+name.Text+"\n");

            var dialog = new MessageDialog("User Added");
            await dialog.ShowAsync();
            SpeechSynthesizer speech = new SpeechSynthesizer();
            MediaElement MP = new MediaElement();
            SpeechSynthesisStream stm = await speech.SynthesizeTextToStreamAsync("name not entered");
            MP.SetSource(stm, stm.ContentType);
            MP.Play();
        

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(VerifyUser));
        }

        //async private void button_Click(object sender, RoutedEventArgs e)
        //{
        //    ImageEncodingProperties imgFormat = ImageEncodingProperties.CreateJpeg();

        //    // create storage file in local app storage
        //    //file = await ApplicationData.Current..CreateFileAsync(
        //    //    "TestPhoto.jpg",
        //    //    CreationCollisionOption.GenerateUniqueName);

        //    StorageFolder iot = await Windows.Storage.KnownFolders.PicturesLibrary.CreateFolderAsync("IOT", CreationCollisionOption.OpenIfExists);
        //    StorageFile file = await iot.CreateFileAsync("verify.jpeg", CreationCollisionOption.ReplaceExisting);
        //    string path = file.Path;


        //    // take photo
        //    await captureManager.CapturePhotoToStorageFileAsync(imgFormat, file);

        //    var client = new HttpClient();
        //    client.BaseAddress = new Uri("https://api.projectoxford.ai/face/v1.0/detect?returnFaceId=true&returnFaceLandmarks=true");
        //    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "274560707bdb402794943600db24deae");
        //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //    var randomAccessStream = await file.OpenReadAsync();

        //    HttpContent content = new StreamContent(randomAccessStream.AsStreamForRead());
        //    content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/octet-stream");

        //    var response = await client.PostAsync("https://api.projectoxford.ai/face/v1.0/detect?returnFaceId=true&returnFaceLandmarks=true", content);

        //    var responseContent = await response.Content.ReadAsStringAsync();

        //    String s = responseContent.Substring(12, 36);



        //    client = new HttpClient();
        //    client.BaseAddress = new Uri("https://api.projectoxford.ai/face/v1.0/verify");
        //    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "274560707bdb402794943600db24deae");
        //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //    randomAccessStream = await file.OpenReadAsync();


        //    content = new StringContent("{\"faceId1\":\"" + s + "\",\"faceId2\":" + "\"" + pre + "\"}",Encoding.UTF8,"application/json");

        //    response = await client.PostAsync("https://api.projectoxford.ai/face/v1.0/verify", content);

        //    responseContent = await response.Content.ReadAsStringAsync();

        //    Debug.Write(responseContent);

        //}





        /////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////


    }
}
