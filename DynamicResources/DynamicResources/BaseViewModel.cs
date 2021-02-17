namespace DynamicResources
{
    public class BaseViewModel
    {
        public BaseViewModel(Localization.LocalizedResources res)
        {
            AppResources = res;
        }

        public Localization.LocalizedResources AppResources { get; }
    }
}
