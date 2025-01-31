using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.Json;
using TheCatApp.Models;
using System.Diagnostics;
using System.Windows.Input;

namespace TheCatApp.ViewModels
{
public class MainViewModel : BindableObject, INotifyPropertyChanged
{
    private ObservableCollection<LinkedData> _linkedDatas = new();
    public ObservableCollection<LinkedData> linkedDatas
    {
        get => _linkedDatas;
        set
        {
            _linkedDatas = value;
        }
    }
    public MainViewModel()
    {
        LoadData();

        Detail = new Command(ShowDetail);
    }
    public ICommand Detail { get; }

    private void ShowDetail(object obj)
    {
        string selecteditem = (string) obj;
        int i = 0;
        foreach (var item in linkedDatas)
        {
            if (item.TheCatInfo.name == selecteditem)
            {
                break;
            }
            i++;
        }

        string msg = $"🐱 {linkedDatas[i].TheCatInfo.name} - {linkedDatas[i].TheCatInfo.origin}\n\n" +
           $"📌 Země původu: {linkedDatas[i].TheCatInfo.origin} ({linkedDatas[i].TheCatInfo.country_code})\n" +
           $"❤️ Temperament: {linkedDatas[i].TheCatInfo.temperament}\n" +
           $"📜 Popis: {linkedDatas[i].TheCatInfo.description}\n" +
           $"📆 Délka života: {linkedDatas[i].TheCatInfo.life_span} let\n" +
           $"🐾 Úroveň přizpůsobivosti: {linkedDatas[i].TheCatInfo.adaptability}/5\n" +
           $"👶 Přátelskost k dětem: {linkedDatas[i].TheCatInfo.child_friendly}/5\n" +
           $"🐶 Přátelskost ke psům: {linkedDatas[i].TheCatInfo.dog_friendly}/5\n" +
           $"🔋 Úroveň energie: {linkedDatas[i].TheCatInfo.energy_level}/5\n" +
           $"🧹 Náročnost na péči: {linkedDatas[i].TheCatInfo.grooming}/5\n" +
           $"🧠 Inteligence: {linkedDatas[i].TheCatInfo.intelligence}/5\n" +
           $"🌍 Wikipedia: {linkedDatas[i].TheCatInfo.wikipedia_url}";

        Application.Current.MainPage.DisplayAlert("cat detail", msg, "Ok");
    }

    private void LoadData()
    {
        bool reloadCatData = false;
        bool isOnline = Connectivity.NetworkAccess == NetworkAccess.Internet;
        string complete = "C:\\ProgramData\\CatInfo\\CatInfo.json";
        string ImagesFile = "C:\\ProgramData\\CatInfo\\CatImages.txt";
        List<TheCatInfo> parsedResponse = new List<TheCatInfo>();
        List<string> images = new List<string>();
        List<getImageUrl> ImageResponse = new List<getImageUrl>();

        if ((!File.Exists(complete) && isOnline) || reloadCatData)
        {
            string dir = @"C:\ProgramData\CatInfo";
            // If directory does not exist, create it
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            using (StreamWriter sw = new StreamWriter(complete, false))
            {
                //get cat images from API
                using (var client = new HttpClient())
                {
                    var endpoint = new Uri("https://api.thecatapi.com/v1/breeds");
                    var result = client.GetAsync(endpoint).Result;
                    var json = result.Content.ReadAsStringAsync().Result;

                    sw.Write(json);

                    //parse json
                    parsedResponse = JsonSerializer.Deserialize<List<TheCatInfo>>(json);
                }
            }

            using (StreamWriter sw = new StreamWriter(ImagesFile, false))
            {
                using (var client = new HttpClient())
                {
                    //get image urls for cats

                    for (int i = 0; i < parsedResponse.Count; i++)
                    {
                        var endpoint = new Uri($"https://api.thecatapi.com/v1/images/search?breed_id={parsedResponse[i].id}");
                        var result = client.GetAsync(endpoint).Result;
                        var json = result.Content.ReadAsStringAsync().Result;
                        try
                        {
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
                        }
                        catch
                        {
                            images.Add("https://www.shutterstock.com/shutterstock/photos/2059817444/display_1500/stock-vector-no-image-available-photo-coming-soon-illustration-vector-2059817444.jpg");
                        }


                        sw.WriteLine(images[i]);
                        ImageResponse.Clear();
                    }
                }
            }

        }
        else if (File.Exists(complete) && images.Count == 0 && parsedResponse.Count == 0)
        {
            parsedResponse.Clear();
            images.Clear();

            using (StreamReader sr = new StreamReader(complete))
            {
                var json = sr.ReadToEnd();
                parsedResponse = JsonSerializer.Deserialize<List<TheCatInfo>>(json);

            }
            using (StreamReader sr = new StreamReader(ImagesFile))
            {
                for (int i = 0; i < parsedResponse.Count; i++)
                {
                    images.Add((string)sr.ReadLine());
                }
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


        for (int i = 0; i < parsedResponse.Count; i++)
        {
            LinkedData temp = new LinkedData(images[i], parsedResponse[i]);
            linkedDatas.Add(temp);
        }
    }
}

}
