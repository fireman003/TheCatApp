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
            string selecteditem = (string)obj;
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
            List<TheCatInfo> parsedResponse = new List<TheCatInfo>();
            List<string> images = new List<string>();

            //get cat images from API
            using (var client = new HttpClient())
            {
                var endpoint = new Uri("https://api.thecatapi.com/v1/breeds");
                var result = client.GetAsync(endpoint).Result;
                var json = result.Content.ReadAsStringAsync().Result;



                //parse json
                try
                {
                    parsedResponse = JsonSerializer.Deserialize<List<TheCatInfo>>(json);
                }
                catch
                {
                    Debug.WriteLine("too many requests");
                }

            }

            for (int i = 0; i < parsedResponse.Count; i++)
            {
                images.Add($"https://cdn2.thecatapi.com/images/{parsedResponse[i].reference_image_id}.jpg");
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