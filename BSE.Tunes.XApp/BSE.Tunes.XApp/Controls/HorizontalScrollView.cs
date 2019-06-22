using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using Xamarin.Forms;

namespace BSE.Tunes.XApp.Controls
{
    public class HorizontalScrollView : ScrollView
    {
        readonly StackLayout m_itemsStackLayout;

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

        public HorizontalScrollView()
        {
            m_itemsStackLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal
            };

            this.Content = m_itemsStackLayout;
        }

        protected virtual void SetItemsSource(IEnumerable oldCollection)
        {

            //m_itemsStackLayout.Children.Clear();


            //if (ItemsSource == null)
            //{
            //    return;
            //}

            //foreach (var item in ItemsSource)
            //{
            //    m_itemsStackLayout.Children.Add(GetItemView(item));
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


            //        var wraplayout = bindable as WrapLayout;

            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems)
                {
                    m_itemsStackLayout.Children.Remove(GetItemView(item));
                }
            }


            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems)
                {
                    m_itemsStackLayout.Children.Add(GetItemView(item));
                }
            }

            //var oldValueINotifyCollectionChanged = oldValue as INotifyCollectionChanged;

            //if (null != oldValueINotifyCollectionChanged)
            //{
            //    oldValueINotifyCollectionChanged.CollectionChanged -= newValueINotifyCollectionChanged_CollectionChanged;
            //}

            //var newValueINotifyCollectionChanged = newValue as INotifyCollectionChanged;

            //if (null != newValueINotifyCollectionChanged)
            //{
            //    newValueINotifyCollectionChanged.CollectionChanged += newValueINotifyCollectionChanged_CollectionChanged;
            //}
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

            //var gesture = new TapGestureRecognizer
            //{
            //    Command = innerSelectedCommand,
            //    CommandParameter = view
            //};

            //AddGesture(view, gesture);

            return view;
        }
    }
}
