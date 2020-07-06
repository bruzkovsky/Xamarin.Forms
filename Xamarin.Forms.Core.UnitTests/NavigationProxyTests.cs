using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Core.UnitTests
{
	[TestFixture]
	public class NavigationProxyTests : BaseTestFixture
	{
		class NavigationTest : INavigation
		{
			public Page LastPushed { get; set; }
			public Page LastPushedModal { get; set; }

			public bool Popped { get; set; }

			public bool PoppedModal { get; set; }

			public bool InsertedModal { get; set; }

			public Task PushAsync (Page root)
			{
				return PushAsync (root, true);
			}

			public Task<Page> PopAsync ()
			{
				return PopAsync (true);
			}

			public Task PopToRootAsync ()
			{
				return PopToRootAsync (true);
			}

			public Task PushModalAsync (Page root)
			{
				return PushModalAsync (root, true);
			}

			public Task<Page> PopModalAsync ()
			{
				return PopModalAsync (true);
			}

			public Task PushAsync (Page root, bool animated)
			{
				LastPushed = root;
				return Task.FromResult (root);
			}

			public Task<Page> PopAsync (bool animated)
			{
				Popped = true;
				return Task.FromResult (LastPushed);
			}

			public Task PopToRootAsync (bool animated)
			{
				return Task.FromResult<Page> (null);
			}

			public Task PushModalAsync (Page root, bool animated)
			{
				LastPushedModal = root;
				return Task.FromResult<object> (null);
			}

			public Task<Page> PopModalAsync (bool animated)
			{
				PoppedModal = true;
				return Task.FromResult<Page> (null);
			}

			public void RemovePage (Page page)
			{
			}

			public void InsertPageBefore (Page page, Page before)
			{
			}

			public void InsertModalBefore(Page modal, Page before)
			{
				InsertedModal = true;
			}

			public System.Collections.Generic.IReadOnlyList<Page> NavigationStack
			{
				get { return new List<Page> (); }
			}

			public System.Collections.Generic.IReadOnlyList<Page> ModalStack
			{
				get { return new List<Page> (); }
			}
		}

		[Test]
		public void Constructor ()
		{
			var proxy = new NavigationProxy ();

			Assert.Null (proxy.Inner);
		}

		[Test]
		public async Task PushesIntoNextInner ()
		{
			var page = new ContentPage ();
			var navProxy = new NavigationProxy ();

			await navProxy.PushAsync (page);

			var navTest = new NavigationTest ();
			navProxy.Inner = navTest;

			Assert.AreEqual (page, navTest.LastPushed);
		}

		[Test]
		public async Task PushesModalIntoNextInner ()
		{
			var page = new ContentPage ();
			var navProxy = new NavigationProxy ();

			await navProxy.PushModalAsync (page);

			var navTest = new NavigationTest ();
			navProxy.Inner = navTest;

			Assert.AreEqual (page, navTest.LastPushedModal);
		}

		[Test]
		public async Task TestPushWithInner ()
		{
			var proxy = new NavigationProxy ();
			var inner = new NavigationTest ();

			proxy.Inner = inner;

			var child = new ContentPage {Content = new View ()};
			await proxy.PushAsync (child);

			Assert.AreEqual (child, inner.LastPushed);
		}

		[Test]
		public async Task TestPushModalWithInner ()
		{
			var proxy = new NavigationProxy ();
			var inner = new NavigationTest ();

			proxy.Inner = inner;

			var child = new ContentPage {Content = new View ()};
			await proxy.PushModalAsync (child);

			Assert.AreEqual (child, inner.LastPushedModal);
		}

		[Test]
		public async Task TestPopWithInner ()
		{
			var proxy = new NavigationProxy ();
			var inner = new NavigationTest ();

			proxy.Inner = inner;

			var child = new ContentPage {Content = new View ()};
			await proxy.PushAsync (child);

			var result = await proxy.PopAsync ();
			Assert.AreEqual (child, result);
			Assert.True (inner.Popped, "Pop was never called on the inner proxy item");
		}

		[Test]
		public async Task TestPopModalWithInner ()
		{
			var proxy = new NavigationProxy ();
			var inner = new NavigationTest ();

			proxy.Inner = inner;

			var child = new ContentPage {Content = new View ()};
			await proxy.PushModalAsync (child);

			await proxy.PopModalAsync ();
			Assert.True (inner.PoppedModal, "Pop was never called on the inner proxy item");
		}

		[Test]
		public void WhenInsertingModal_AndPageIsFound_ThenInsertsModalBeforeThePage()
		{
			var proxy = new NavigationProxy();

			var modal1 = new ContentPage { Content = new View() };
			proxy.PushModalAsync(modal1);
			var modal2 = new ContentPage { Content = new View() };
			proxy.PushModalAsync(modal2);
			var modal3 = new ContentPage { Content = new View() };
			proxy.PushModalAsync(modal3);

			var inserted1 = new ContentPage { Content = new View() };
			proxy.InsertModalBefore(inserted1, modal1);
			var inserted2 = new ContentPage { Content = new View() };
			proxy.InsertModalBefore(inserted2, modal2);
			var inserted3 = new ContentPage { Content = new View() };
			proxy.InsertModalBefore(inserted3, modal3);

			CollectionAssert.AreEqual(new[] { inserted1, modal1, inserted2, modal2, inserted3, modal3 }, proxy.ModalStack);
		}

		[Test]
		public void WhenInsertingModal_AndPageIsNotFound_ThenThrowsArgumentException()
		{
			var proxy = new NavigationProxy();

			var notFoundPage = new ContentPage { Content = new View() };
			var child = new ContentPage { Content = new View() };
			Assert.Throws<ArgumentException>(() => proxy.InsertModalBefore(child, notFoundPage));
		}

		[Test]
		public void WhenInsertingModal_AndInnerIsSet_ThenCallsTheInnerNavigationToo()
		{
			var proxy = new NavigationProxy();
			var inner = new NavigationTest();

			proxy.Inner = inner;

			var topModal = new ContentPage { Content = new View() };
			proxy.PushModalAsync(topModal);

			var child = new ContentPage { Content = new View() };
			proxy.InsertModalBefore(child, topModal);

			Assert.True(inner.InsertedModal, "Insert was never called on the inner proxy item");
		}
	}
}
