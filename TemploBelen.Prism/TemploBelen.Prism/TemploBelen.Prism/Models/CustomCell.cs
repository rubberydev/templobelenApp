using System;
using Xamarin.Forms;

namespace YoutubeChannelStream
{
	public class CustomCell : ViewCell
	{
		public CustomCell()
		{
			var youTubeFeed = new Label();
			youTubeFeed.LineBreakMode = LineBreakMode.WordWrap;
			youTubeFeed.SetBinding(Label.TextProperty, "Title");
			youTubeFeed.FontSize = 16;
			var youTubeFeed2 = new Label();
			youTubeFeed2.LineBreakMode = LineBreakMode.WordWrap;
			youTubeFeed2.SetBinding(Label.TextProperty, "Date");
			youTubeFeed2.FontSize = 10;
			var customGrid = new Grid()
			{
				BackgroundColor = Color.LightGray,
				RowDefinitions =
				{
					new RowDefinition { Height = new GridLength(80) }
				},
				ColumnDefinitions =
					{
					   new ColumnDefinition(),
					   new ColumnDefinition(),
					   new ColumnDefinition()
					}
			};


			customGrid.Children.Add(new Image()
			{
				Source = "youtube.png",
				HeightRequest = 50,
				MinimumWidthRequest = 70
			});
			Grid.SetColumn(youTubeFeed, 1);
			Grid.SetColumn(youTubeFeed2, 2);

			customGrid.Children.Add(youTubeFeed2);
			customGrid.Children.Add(youTubeFeed);

			var horizontalFrame = new Frame()
			{
				BorderColor = Color.FromHex("#11468a"),
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.Center,
				Content = customGrid,
		 	    MinimumHeightRequest = 50,
				Margin = 10,
				
				
			};

			View = horizontalFrame;
		}
	}
}

