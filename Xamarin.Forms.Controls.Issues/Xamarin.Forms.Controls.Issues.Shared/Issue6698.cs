#if !UITEST
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 6698, "crash on TypedBinding.Apply", PlatformAffected.All)]
	public class Issue6698 : TestContentPage
	{
		protected override void Init()
		{
			_container = new AbsoluteLayout();
			var button1 = new Button { Text = "Test 1 (AbsoluteLayout)" };
			button1.Clicked += Button1OnClicked;
			Grid.SetRow(button1, 1);
			var button2 = new Button { Text = "Test 2 (ListView)" };
			button2.Clicked += Button2OnClicked;
			Grid.SetRow(button2, 2);
			var content = new Grid
			{
				RowDefinitions =
				{
					new RowDefinition { Height = GridLength.Star },
					new RowDefinition { Height = GridLength.Auto },
					new RowDefinition { Height = GridLength.Auto }
				},
				Children = { _container, button1, button2 }
			};
			Content = content;
		}

		async void Button1OnClicked(object sender, EventArgs e)
		{
			// Simulation of page transition
			for (var i = 0; i < 1000; i++)
			{
				// Exception triggered by this update
				GetLongLifecycleModel().Next();

				//if (i % 2 == 0)
				//{
				//	ReplaceView(new Issue6698View1(Color.LightSkyBlue) { BindingContext = CreateContainerViewModel() });
				//}
				//else
				//{
					ReplaceView(new Issue6698View2 { BindingContext = CreateContainerViewModel() });
				//}

				await Task.Delay(10);
			}

			Cleanup();
		}

		async void Button2OnClicked(object sender, EventArgs e)
		{
			ReplaceView(GetListView());

			// Simulation of page transition
			for (var i = 0; i < 1000; i++)
			{
				ListOfContainers.Clear();

				// Exception triggered by this update
				GetLongLifecycleModel().Next();

				ListOfContainers.Add(CreateContainerViewModel());

				await Task.Delay(10);
			}

			Cleanup();
		}

		private void ReplaceView(View view)
		{
			Cleanup();

			AbsoluteLayout.SetLayoutFlags(view,
				AbsoluteLayoutFlags.WidthProportional | AbsoluteLayoutFlags.HeightProportional);
			AbsoluteLayout.SetLayoutBounds(view, new Rectangle(0, 0, 1, 1));

			_container.Children.Add(view);
		}

		private void Cleanup()
		{
			// If you clear the BindingContext of the old View, no problem occurs?
			//foreach (var view in Container.Children)
			//{
			//    view.BindingContext = null;
			//}

			_container.Children.Clear();
		}

		static Issue6698LongLifecycleModel _longLifecycleModel;
		AbsoluteLayout _container;
		ListView _listView;

		static Issue6698LongLifecycleModel GetLongLifecycleModel() =>
			_longLifecycleModel ?? (_longLifecycleModel = new Issue6698LongLifecycleModel());

		static Issue6698ContainerViewModel CreateContainerViewModel() =>
			new Issue6698ContainerViewModel(GetLongLifecycleModel());

		ListView GetListView()
		{
			_listView = _listView ?? new ListView(ListViewCachingStrategy.RecycleElement);
			_listView.ItemTemplate = new DataTemplate(() => new ViewCell { View = new Issue6698View2() });
			_listView.ItemsSource = ListOfContainers;
			return _listView;
		}

		public ObservableCollection<Issue6698ContainerViewModel> ListOfContainers { get; } =
			new ObservableCollection<Issue6698ContainerViewModel>();
	}

	public class Issue6698LongLifecycleModel : INotifyPropertyChanged
	{
		private int nextId;

		public Issue6698Entity Entity { get; private set; }

		public void Next()
		{
			nextId++;
			Entity = new Issue6698Entity { Id = nextId, Name = $"Entity-{nextId}", Buffer = new byte[1024] };

			for (var i = 0; i < 80; i++)
				OnPropertyChanged(nameof(Entity));
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}

	public class Issue6698ContainerViewModel
	{
		public Issue6698LongLifecycleModel LongLifecycleModel { get; }

		public Issue6698ContainerViewModel(Issue6698LongLifecycleModel longLifecycle)
		{
			LongLifecycleModel = longLifecycle;
		}
	}

	public class Issue6698Entity
	{
		public int Id { get; set; }

		public string Name { get; set; }

		public byte[] Buffer { get; set; }
	}

	public class Issue6698View1 : ContentView
	{
		public Issue6698View1(Color color)
		{
			var label = new Label { BackgroundColor = color, FontSize = 28 };
			label.SetBinding(Label.TextProperty,
				new TypedBinding<Issue6698ContainerViewModel, string>(vm => (vm.LongLifecycleModel.Entity.Name, true),
					(vm, value) => vm.LongLifecycleModel.Entity.Name = value,
					new Tuple<Func<Issue6698ContainerViewModel, object>, string>[0]));
			Content = new StackLayout { Children = { label } };
		}
	}
}
#endif