using Xamarin.Forms;

namespace TestProject
{
	public class Page2 : TabbedPage
	{
		public Page2()
		{
			Children.Add(new Page1 { Title = "First" });
			Children.Add(new Page1 { Title = "Second" });
			Children.Add(new Page1 { Title = "Third" });
		}
	}
}
