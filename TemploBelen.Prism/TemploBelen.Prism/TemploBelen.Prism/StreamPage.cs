using System;
using Xamarin.Forms;
using CodeHollow.FeedReader;
using System.Collections.Generic;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Essentials;

namespace YoutubeChannelStream
{
	public class StreamPage : ContentPage
	{
		#region Fields
		Xamarin.Forms.ListView _listView = new Xamarin.Forms.ListView();
		List<RSSFeedObject> _feeds = new List<RSSFeedObject>();
		#endregion

		#region Constructor
		public StreamPage()
		{
			Title = "Templo Belen Canal en vivo :)";

			// For iPhone X
			On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);

			_listView.ItemSelected += listView_ItemSelected;
			_listView.SelectedItem = null;

			// Default values to display if the feeds aren't loading
			var label = new Label();
			var stack = new StackLayout()
			{
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center
			};
			stack.Children.Add(label);
			Content = stack;
			if (_feeds != null)
				PopulateList();
			else
				label.Text = "Either the feed is empty or the URL is incorrect.";
		}
		#endregion Constructor

		#region Private Functions & Event Handlers
		private void PopulateList()
		{
			_listView.HasUnevenRows = true;

			DataTemplate template = new DataTemplate(typeof(CustomCell));
			_listView.ItemTemplate = template;

			_listView.ItemsSource = _feeds;

			Content = _listView;
		}

		private async void listView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			// To prevent opening multiple pages on double tapping
			_listView.IsEnabled = false;
			var item = e.SelectedItem as RSSFeedObject;
			await Navigation.PushAsync(new StreamDetailPage(item));

			_listView.IsEnabled = true;
		}
		#endregion Private Functions & Event Handlers

		#region LifeCycle Event Overrides
		protected async override void OnAppearing()
		{

			base.OnAppearing();


			var current = Connectivity.NetworkAccess;

			if (current == NetworkAccess.Internet)
			{
				var rssFeeds = new Feed();
				try
				{
					rssFeeds = await FeedReader.ReadAsync($"https://www.youtube.com/feeds/videos.xml?channel_id={Xamarin.Forms.Application.Current.Resources["ChannelId"].ToString()}");
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex);
					_feeds.Add(new RSSFeedObject() { Title = "Test", Date = "January 2099", Link = "www.example.com" });
					PopulateList();
					return;
				}
				foreach (var item in rssFeeds.Items)
				{
					var feed = new RSSFeedObject()
					{
						Title = item.Title,
						Date = item.PublishingDate.Value.ToString("y"),
						Link = item.Link
					};
					_feeds.Add(feed);
				}
				PopulateList();
			}
			else
			{
				var optionSelected = await Xamarin.Forms.Application.Current.MainPage.DisplayActionSheet("Error dev red", "Recargar!", "Cancelar!", "Señor usuario si desea continuar habillite la conexion a internet y seleccione la acción Recargar!");

				if (optionSelected == "Recargar!")
				{
					Xamarin.Forms.Application.Current.MainPage = new Xamarin.Forms.NavigationPage(new StreamPage())
					{
						BarTextColor = Color.FromRgb(255, 255, 255),
						BarBackgroundColor = Color.FromRgb(60, 171, 223)
					};
				}
				else if (optionSelected == "Cancelar!" || string.IsNullOrEmpty(optionSelected))
					return;

			}
		}
		#endregion LifeCycle Event Overrides
	}
}
