using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.Json;
using TheCatApp.Models;

namespace TheCatApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new MainViewModel();
        }

         
    }
}