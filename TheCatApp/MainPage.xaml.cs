using System.Diagnostics;
using System.Text.Json;
using TheCatApp.Models;

namespace TheCatApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            bool reloadCatData = false;
            bool isOnline = Connectivity.NetworkAccess == NetworkAccess.Internet;
            string catLocalData = "catData.json";
            List<TheCatInfo> parsedResponse = new List<TheCatInfo>();
            List<string> images = new List<string>();
            if ((!File.Exists(catLocalData) && isOnline) || reloadCatData)
            {
                //get cat images from API
                using (var client = new HttpClient())
                {
                    var endpoint = new Uri("https://api.thecatapi.com/v1/breeds");
                    var result = client.GetAsync(endpoint).Result;
                    var json = result.Content.ReadAsStringAsync().Result;
                    //parse json
                    parsedResponse = JsonSerializer.Deserialize<List<TheCatInfo>>(json);
                    //save data to local file
                    File.WriteAllText(catLocalData, json);
                }

                using (var client = new HttpClient())
                {
                    //get image urls for cats
                    List<getImageUrl> ImageResponse = new List<getImageUrl>();
                    for (int i = 0; i < parsedResponse.Count; i++)
                    {
                        var endpoint = new Uri($"https://api.thecatapi.com/v1/images/search?breed_id={parsedResponse[i].id}");
                        var result = client.GetAsync(endpoint).Result;
                        var json = result.Content.ReadAsStringAsync().Result;
                        ImageResponse = JsonSerializer.Deserialize<List<getImageUrl>>(json);

                        //handle if image doesnt exists
                        if (ImageResponse.Count != 0)
                        {
                            images.Add(ImageResponse[0].url.ToString());
                        }
                        else
                        {
                            images.Add("https://www.shutterstock.com/shutterstock/photos/2059817444/display_1500/stock-vector-no-image-available-photo-coming-soon-illustration-vector-2059817444.jpg");
                        }

                        ImageResponse.Clear();
                    }
                }
            }
            else if (File.Exists(catLocalData))
            {
                //get cat images from local file
                string json = File.ReadAllText(catLocalData);
                parsedResponse = JsonSerializer.Deserialize<List<TheCatInfo>>(json);
                for (int i = 0; i < parsedResponse.Count; i++)
                {
                    images.Add(parsedResponse[i].reference_image_id);
                }
            }
            else
            {
                //some error mostly no internet
                Debug.WriteLine("No internet connection and no data is localy stored");
            }

            //check if there smome error
            if (parsedResponse.Count == 0 || parsedResponse == null)
            {
                Debug.WriteLine("Parsed data is empty");
            }

            LinkedData linkedData = new LinkedData(images, parsedResponse);
            

            InitializeComponent();
            BindingContext = linkedData;
        }
    }
}