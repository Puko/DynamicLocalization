using DynamicResources.Localization;
using Xamarin.Forms;

namespace DynamicResources
{
    public partial class App : Application
    {
        public static LocalizedResources AppResources { get; set; }

        public App()
        {
            InitializeComponent();
            
            MainPage = new MainPage();
        }

        protected override async void OnStart()
        {
            AppResources = new LocalizedResources();
            await AppResources.SetLanguageAsync("fr-FR");

            BaseViewModel vm = new BaseViewModel(AppResources);
            MainPage.BindingContext = vm;
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
