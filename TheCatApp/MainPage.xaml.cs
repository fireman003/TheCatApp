using System.Text.Json;

namespace TheCatApp
{
    public partial class MainPage : ContentPage
    {
        public record struct Weight(string imperial, string metric);
        public record struct TheCatInfo(Weight weight, string id, string name, string cfa_url, string vetstreet_url,
            string vcahospitals_url, string temperament, string origin, string country_codes, string country_code,
            string description, string life_span, int indoor, int lap, string alt_names, int adaptability, int affection_level,
            int child_friendly, int dog_friendly, int energy_level, int grooming, int health_issues, int intelligence, int shedding_level,
            int social_needs, int stranger_friendly, int vocalisation, int experimental, int hairless, int natural, int rare, int rex,
            int suppressed_tail, int short_legs, string wikipedia_url, int hypoallergenic, string reference_image_id);

        public record struct getImageUrl(string id, string url, int height, int weight);

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
                        if (json == "You have hit the rate limit, please increase your account package tier or wait a minute")
                        {
                            break;
                        }
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
            }

            //check if there smome error
            if (parsedResponse.Count == 0 || parsedResponse == null)
                {
                    Console.WriteLine("");
                }

                InitializeComponent();
        }
    }
}