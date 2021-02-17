using Xamarin.Forms;

namespace DynamicResources
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void Button_Clicked(object sender, System.EventArgs e)
        {
            await ((BaseViewModel)BindingContext).AppResources.SetLanguageAsync(((Button)sender).Text);
        }
    }
}
