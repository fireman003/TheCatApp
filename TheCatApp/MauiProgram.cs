using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace TheCatApp
{
    public static class MauiProgram
    {
        public record struct Weight(string imperial, string metric);
        public record struct TheCatInfo(Weight weight, string id, string name, string cfa_url, string vetstreet_url,
            string vcahospitals_url, string temperament, string origin, string country_codes, string country_code,
            string description, string life_span, int indoor, int lap, string alt_names, int adaptability, int affection_level,
            int child_friendly, int dog_friendly, int energy_level, int grooming, int health_issues, int intelligence, int shedding_level,
            int social_needs, int stranger_friendly, int vocalisation, int experimental, int hairless, int natural, int rare, int rex, 
            int suppressed_tail, int short_legs, string wikipedia_url, int hypoallergenic, string reference_image_id);

        public record struct getImageUrl(string id, string url, int height, int weight);

        public static MauiApp CreateMauiApp()
        {
            List<TheCatInfo> parsedResponse = new List<TheCatInfo>();
            List<string> images = new List<string>();
            //get cat images from API
            using (var client = new HttpClient()) 
            {
                var endpoint = new Uri("https://api.thecatapi.com/v1/breeds");
                var result = client.GetAsync(endpoint).Result;
                var json = result.Content.ReadAsStringAsync().Result;
                //parse json
                parsedResponse = JsonSerializer.Deserialize<List<TheCatInfo>>(json);
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
                    if(ImageResponse.Count != 0)
                    {
                        images.Add(ImageResponse[0].url.ToString());
                    } else
                    {
                        images.Add("");
                    }

                    ImageResponse.Clear();
                }
            }

            //check if there smome error
            if(parsedResponse.Count == 0 || parsedResponse == null)
            {
                Console.WriteLine("");
            }

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}