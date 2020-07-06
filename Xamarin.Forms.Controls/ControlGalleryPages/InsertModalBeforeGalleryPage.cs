namespace Xamarin.Forms.Controls.ControlGalleryPages
{
	class InsertModalBeforeGalleryPage : NavigationPage
	{
		public InsertModalBeforeGalleryPage() : base(CreateRootPage())
		{
		}

		static Page CreateRootPage()
		{
			return new InsertModalRootPage();
		}

		class InsertModalRootPage : ContentPage
		{
			public InsertModalRootPage()
			{
				Content = new StackLayout
				{
					Children =
					{
						new Button
						{
							Text = "Show two modals",
							Command = new Command(async _ =>
							{
								var topmostModal = CreatePage("I am the topmost modal!");
								await Navigation.PushModalAsync(topmostModal);
								Navigation.InsertModalBefore(CreatePage("I am wedged in-between!"), topmostModal);
							})
						}
					}
				};
			}
		}

		static ContentPage CreatePage(string text)
		{
			var page = new ContentPage();
			page.Content = new StackLayout
			{
				Children =
				{
					new Label { Text = text },
					new Button { Text = "Go back", Command = new Command(async () => { await page.Navigation.PopModalAsync();})}
				}
			};
			return page;
		}
	}
}