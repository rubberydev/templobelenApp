using Prism;
using Prism.Ioc;
using TemploBelen.Prism.ViewModels;
using Xamarin.Essentials.Implementation;
using Xamarin.Essentials.Interfaces;
using Xamarin.Forms;
using YoutubeChannelStream;

namespace TemploBelen.Prism
{
    public partial class App
    {
        public App(IPlatformInitializer initializer)
            : base(initializer)
        {
        }

        protected override async void OnInitialized()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new StreamPage())
            {
                BarTextColor = Color.FromRgb(255, 255, 255),
                BarBackgroundColor = Color.FromRgb(60, 171, 223)
            };

            await NavigationService.NavigateAsync("NavigationPage/MainPage");
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IAppInfo, AppInfoImplementation>();

            containerRegistry.RegisterForNavigation<NavigationPage>();
            //containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>();
        }
    }
}
