using System.Collections.Generic;
using System.Threading.Tasks;

namespace Xamarin.Forms
{
	public interface INavigation
	{
		IReadOnlyList<Page> ModalStack { get; }

		IReadOnlyList<Page> NavigationStack { get; }

		void InsertPageBefore(Page page, Page before);
		void InsertModalBefore(Page modal, Page before);
		Task<Page> PopAsync();
		Task<Page> PopAsync(bool animated);
		Task<Page> PopModalAsync();
		Task<Page> PopModalAsync(bool animated);
		Task PopToRootAsync();
		Task PopToRootAsync(bool animated);

		Task PushAsync(Page page);

		Task PushAsync(Page page, bool animated);
		Task PushModalAsync(Page page);
		Task PushModalAsync(Page page, bool animated);

		void RemovePage(Page page);
	}
}