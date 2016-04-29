using Microsoft.ProjectOxford.Face;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Media.SpeechSynthesis;
using Windows.Networking;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace IOT
{
    
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class VerifyUser : Page
    {
        IFaceServiceClient faceServiceClient;
        MediaCapture captureManager;
        public VerifyUser()
        {
            this.InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            captureManager = new MediaCapture();
            await captureManager.InitializeAsync();

            captureManager = new MediaCapture();
            await captureManager.InitializeAsync();

            capturePreview.Source = captureManager;
            await captureManager.StartPreviewAsync();
        }

        private async void verify_Click(object sender, RoutedEventArgs e)
        {

            ImageEncodingProperties imgFormat = ImageEncodingProperties.CreateJpeg();


            StorageFolder iot = await KnownFolders.PicturesLibrary.CreateFolderAsync("IOT", CreationCollisionOption.OpenIfExists);
            StorageFile file = await iot.CreateFileAsync("verify.jpeg", CreationCollisionOption.ReplaceExisting);
            string path = file.Path;
            await captureManager.CapturePhotoToStorageFileAsync(imgFormat, file);

            string s;
            try
            {

                var client = new HttpClient();
                client.BaseAddress = new Uri("https://api.projectoxford.ai/face/v1.0/detect?returnFaceId=true&returnFaceLandmarks=true");
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "274560707bdb402794943600db24deae");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var randomAccessStream = await file.OpenReadAsync();

                HttpContent content = new StreamContent(randomAccessStream.AsStreamForRead());
                content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/octet-stream");

                var response = await client.PostAsync("https://api.projectoxford.ai/face/v1.0/detect?returnFaceId=true&returnFaceLandmarks=true", content);

                var responseContent = await response.Content.ReadAsStringAsync();
                Debug.Write(responseContent);

                s = responseContent.Substring(12, 36);
                Debug.Write(s);
                if (s == null)
                {
                    SpeechSynthesizer speech = new SpeechSynthesizer();
                    MediaElement MP = new MediaElement();
                    SpeechSynthesisStream stm = await speech.SynthesizeTextToStreamAsync("could not detect face");
                    MP.SetSource(stm, stm.ContentType);
                    MP.Play();
                    return;
                }


            }
            catch
            {
                SpeechSynthesizer speech = new SpeechSynthesizer();
                MediaElement MP = new MediaElement();
                SpeechSynthesisStream stm = await speech.SynthesizeTextToStreamAsync("could not detect face");
                MP.SetSource(stm, stm.ContentType);
                MP.Play();
                return;
            }
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            StorageFile sampleFile = await storageFolder.GetFileAsync("sample.txt");
            string text = await FileIO.ReadTextAsync(sampleFile);

            string[] str = text.Split('\n');

            foreach (var item in str)
            {
                string[] id = item.Split('=');
                var client = new HttpClient();
                client.BaseAddress = new Uri("https://api.projectoxford.ai/face/v1.0/verify");
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "274560707bdb402794943600db24deae");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var randomAccessStream = await file.OpenReadAsync();


                var content = new StringContent("{\"faceId1\":\"" + s + "\",\"faceId2\":" + "\"" + id[0].Trim() + "\"}", Encoding.UTF8, "application/json");

                var response = await client.PostAsync("https://api.projectoxford.ai/face/v1.0/verify", content);

                var responseContent = await response.Content.ReadAsStringAsync();

                Debug.Write(responseContent);

                    if(responseContent.Contains("true"))
                {
                    try
                    {
                        //Create the StreamSocket and establish a connection to the echo server.
                        Windows.Networking.Sockets.StreamSocket socket = new Windows.Networking.Sockets.StreamSocket();

                        //The server hostname that we will be establishing a connection to. We will be running the server and client locally,
                        //so we will use localhost as the hostname.
                        EndpointPair end = new EndpointPair(new HostName("192.168.0.112"), "6767", new HostName("192.168.0.109"), "6767");
                        await socket.ConnectAsync(end);

                        //Every protocol typically has a standard port number. For example HTTP is typically 80, FTP is 20 and 21, etc.
                        //For the echo server/client application we will use a random port 1337.


                        ////Write data to the echo server.
                        //Stream streamOut = socket.OutputStream.AsStreamForWrite();
                        //StreamWriter writer = new StreamWriter(streamOut);
                        //string request = "SUCCESS";
                        //await writer.WriteLineAsync(request);
                        //await writer.FlushAsync();

                        ////Read data from the echo server.
                        //Stream streamIn = socket.InputStream.AsStreamForRead();
                        //StreamReader reader = new StreamReader(streamIn);
                        //string rsponse = await reader.ReadLineAsync();

                        SpeechSynthesizer speech = new SpeechSynthesizer();
                        MediaElement MP = new MediaElement();
                        SpeechSynthesisStream stm = await speech.SynthesizeTextToStreamAsync("Welcome "+id[1]+". The Door has been opened for you");
                        MP.SetSource(stm, stm.ContentType);
                        MP.Play();

                        break;
                    }
                    catch (Exception ex)
                    {
                        var dialog = new MessageDialog("error");
                        await dialog.ShowAsync();            
                    }
                }
            }


        }

       
    }
}
