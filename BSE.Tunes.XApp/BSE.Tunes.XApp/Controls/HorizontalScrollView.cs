using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace BSE.Tunes.XApp.Controls
{
    public class HorizontalScrollView : ScrollView
    {
        private readonly StackLayout _itemsStackLayout;
        private ICommand _innerSelectedCommand;

        public event EventHandler SelectedItemChanged;

        public static readonly BindableProperty SelectedItemCommandProperty =
         BindableProperty.Create(
                "SelectedItemCommandProperty", typeof(ICommand),
                typeof(HorizontalScrollView),
                null);

        public ICommand SelectedItemCommand
        {
            get => (ICommand)GetValue(SelectedItemCommandProperty);
            set => SetValue(SelectedItemCommandProperty, value);
        }

        public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create(
                "ItemsSource", typeof(IEnumerable),
                typeof(HorizontalScrollView),
                default(IEnumerable),
                BindingMode.OneWay,
                propertyChanged: ItemsSourceChanged);

        private static void ItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var itemsLayout = (HorizontalScrollView)bindable;
            itemsLayout.SetItemsSource(oldValue as IEnumerable);
        }

        public IEnumerable ItemsSource
        {
            get => GetValue(ItemsSourceProperty) as IEnumerable;
            set => SetValue(ItemsSourceProperty, value);
        }

        public static readonly BindableProperty ItemTemplateProperty =
            BindableProperty.Create(
                "ItemTemplate",
                typeof(DataTemplate),
                typeof(HorizontalScrollView),
                default(DataTemplate));

        public DataTemplate ItemTemplate
        {
            get => GetValue(ItemTemplateProperty) as DataTemplate;
            set => SetValue(ItemTemplateProperty, value);
        }

        public static readonly BindableProperty SelectedItemProperty =
            BindableProperty.Create("SelectedItem", typeof(object), typeof(HorizontalScrollView), null, BindingMode.TwoWay, propertyChanged: OnSelectedItemChanged);

        public object SelectedItem
        {
            get => (object)GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }


        public HorizontalScrollView()
        {
            Orientation = ScrollOrientation.Horizontal;
            _itemsStackLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal
            };

            this.Content = _itemsStackLayout;
        }

        protected virtual void SetItemsSource(IEnumerable oldCollection)
        {

            //m_itemsStackLayout.Children.Clear();
            _innerSelectedCommand = new Command<View>(view =>
            {
                SelectedItem = view.BindingContext;
                SelectedItem = null; // Allowing item second time selection
            });

            //if (ItemsSource == null)
            //{
            //    return;
            //}

            if (oldCollection is INotifyCollectionChanged oldObservableCollection)
            {
                oldObservableCollection.CollectionChanged -= OnObservableCollectionChanged;
            }

            if (ItemsSource is INotifyCollectionChanged observableCollection)
            {
                observableCollection.CollectionChanged += OnObservableCollectionChanged;
            }

            OnObservableCollectionChanged(oldCollection, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        private void OnObservableCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems)
                {
                    _itemsStackLayout.Children.Remove(GetItemView(item));
                }
            }

            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems)
                {
                    _itemsStackLayout.Children.Add(GetItemView(item));
                }
            }
        }

        protected virtual View GetItemView(object item)
        {
            var content = ItemTemplate.CreateContent();

            var view = content is ViewCell cell ? cell.View : content as View;
            //if (!(content is View view))
            //{
            //    return null;
            //}

            view.BindingContext = item;

            var gesture = new TapGestureRecognizer
            {
                Command = _innerSelectedCommand,
                CommandParameter = view
            };

            AddGesture(view, gesture);

            return view;
        }

        void AddGesture(View view, TapGestureRecognizer gesture)
        {
            view.GestureRecognizers.Add(gesture);

            if (!(view is Layout<View> layout))
            {
                return;
            }

            foreach (var child in layout.Children)
            {
                AddGesture(child, gesture);
            }
        }

        static void OnSelectedItemChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var itemsView = (HorizontalScrollView)bindable;
            if (newValue == oldValue && newValue != null)
            {
                return;
            }

            itemsView.SelectedItemChanged?.Invoke(itemsView, EventArgs.Empty);

            if (itemsView.SelectedItemCommand?.CanExecute(newValue) ?? false)
            {
                itemsView.SelectedItemCommand?.Execute(newValue);
            }
        }
    }
}
