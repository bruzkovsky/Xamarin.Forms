using System.Collections.Generic;
using Xamarin.Forms;

namespace TestProject
{
	public class Page1 : ContentPage
	{
		public Page1()
		{
			var itemsSource = new List<string> { "a", "b", "c" };
			var itemTemplate = new DataTemplate(typeof(MyTextCell));
			itemTemplate.SetBinding(TextCell.TextProperty, ".");
			var listView = new ListView
			{
				ItemsSource = itemsSource,
				ItemTemplate = itemTemplate
			};

			Content = listView;
		}
	}

	public class MyTextCell : TextCell
	{
		public MyTextCell()
		{
			ContextActions.Add(new MenuItem { Text = "Do something" });
		}
	}
}
