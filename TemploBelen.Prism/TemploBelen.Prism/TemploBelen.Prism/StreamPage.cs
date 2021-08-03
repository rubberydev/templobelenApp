using System;
using Xamarin.Forms;
using CodeHollow.FeedReader;
using System.Collections.Generic;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Essentials;
using System.Linq;

namespace YoutubeChannelStream
{
	public class StreamPage : ContentPage
	{
		#region Fields
		Xamarin.Forms.ListView _listView = new Xamarin.Forms.ListView();
		List<RSSFeedObject> _feeds = new List<RSSFeedObject>();
	    List<RSSFeedObject> _original_feeds = new List<RSSFeedObject>();
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
				VerticalOptions = LayoutOptions.FillAndExpand
			};
			
			Content = stack;
			if (_feeds != null)
            {
				_original_feeds = _feeds;
				this.PopulateList();
			}
			else
				label.Text = "Either the feed is empty or the URL is incorrect.";
		}
		#endregion Constructor

		#region Private Functions & Event Handlers
		private void PopulateList()
		{
			var stack = new StackLayout()
			{
				Orientation = StackOrientation.Vertical,
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.FillAndExpand
			};
			_listView.HasUnevenRows = true;
			var filter = new Xamarin.Forms.Entry
            {
				HorizontalOptions = LayoutOptions.Center,
			    Placeholder = "filtre por tipo de evento e,g Matutino...",
				BackgroundColor = Color.FromRgb(60, 171, 223),
				PlaceholderColor = Color.White
				

			};
			filter.Completed += this.Entry_Completed;
			stack.Children.Add(filter);
			DataTemplate template = new DataTemplate(typeof(CustomCell));
			_listView.ItemTemplate = template;
			_listView.ItemsSource = _feeds;
			stack.Children.Add(_listView);
			Content = stack;
		}

		void Entry_Completed(object sender, EventArgs e)
		{
			var text = ((Xamarin.Forms.Entry)sender).Text; //cast sender to access the properties of the Entry
			this.FilterFeedsAndUpdateListView(text);
		}

		void OnPickerSelectedIndexChanged(object sender, EventArgs e)
		{
			var picker = (Xamarin.Forms.Picker)sender;
			int selectedIndex = picker.SelectedIndex;

			if (selectedIndex != -1)
			{
				string textFilter = (string)picker.ItemsSource[selectedIndex];
				this.FilterFeedsAndUpdateListView(textFilter);
			}
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
				this.LoadFeeds(rssFeeds);
				this.PopulateList();
			}
			else
			{
				var optionSelected = await Xamarin.Forms.Application.Current.MainPage.DisplayActionSheet("Error de red", "Recargar!", "Cancelar!", "Señor usuario si desea continuar habillite la conexion a internet y seleccione la acción Recargar!");

				if (optionSelected == "Recargar!")
				{
					Xamarin.Forms.Application.Current.MainPage = new Xamarin.Forms.NavigationPage(new StreamPage())
					{
						BarTextColor = Color.FromRgb(255, 255, 255),
						BackgroundColor = Color.FromRgb(1, 124, 194),
					};
				}
				else if (optionSelected == "Cancelar!" || string.IsNullOrEmpty(optionSelected))
					return;

			}
		}
        #endregion LifeCycle Event Overrides

        #region other methods
		private void LoadFeeds(Feed feeds)
        {
			foreach (var item in feeds.Items)
			{
				var feed = new RSSFeedObject()
				{
					Title = item.Title,
					Date = item.PublishingDate.Value.ToString("y"),
					Link = item.Link
				};
				_feeds.Add(feed);
			}


		}

		private void FilterFeedsAndUpdateListView(string filterCriteria)
        {
			var updateFeeds = _original_feeds.Where(x => x.Title.Contains(filterCriteria)).ToList();
			_feeds = updateFeeds.Count != 0 ? updateFeeds : _original_feeds;

			if(_feeds.Count != 0 )
				this.PopulateList();
        }
        #endregion
    }
}
