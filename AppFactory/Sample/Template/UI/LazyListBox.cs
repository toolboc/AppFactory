using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using BuiltToRoam;

namespace Template.UI
{
    public class LazyListBox : ListBox
    {
        /// <summary>
        /// The real source that gets generated
        /// when Source is set and is bound to the
        /// ListBox (ie the ItemsSource property)
        /// </summary>
        private IList realSource;

        /// <summary>
        /// An event that is raised when the user scrolls the listbox
        /// </summary>
        public event EventHandler<ParameterEventArgs<double>> Scrolled;

        /// <summary>
        /// A reference to the vertical scroller within the list
        /// </summary>
        private ScrollBar verticalScroller;


        /// <summary>
        /// Initializes a new instance of the LazyListBox class
        /// </summary>
        public LazyListBox()
        {
            Loaded += LazyListBoxLoaded;
        }

        /// <summary>
        /// Dependency property for the Source of the ListBox. To 
        /// use as a normal listbox use the ItemsSource property. To
        /// have lazy loading, use the Source property
        /// </summary>
        public IEnumerable Source
        {
            get { return (IEnumerable)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Source.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(IEnumerable), typeof(LazyListBox), new PropertyMetadata(null, SourcePropertyChanged));

        /// <summary>
        /// Change handler for the dependency property
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void SourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var list = d as LazyListBox;
            if (list == null) return;

            if (DesignerProperties.IsInDesignTool)
            {
                list.ItemsSource = list.Source;
                return;
            }

            list.SourceChanged(e.OldValue as INotifyCollectionChanged, e.NewValue as INotifyCollectionChanged);
        }

        /// <summary>
        /// On load, get a reference to the vertical scrollbar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LazyListBoxLoaded(object sender, RoutedEventArgs e)
        {
            if (DesignerProperties.IsInDesignTool) return;

            if (verticalScroller == null)
            {
                var viewer = VisualTreeHelper.GetChild(this, 0) as ScrollViewer;
                if (viewer == null) return;
                var scrollViewerRoot = (FrameworkElement)VisualTreeHelper.GetChild(viewer, 0);
                verticalScroller = (ScrollBar)scrollViewerRoot.FindName("VerticalScrollBar");
                if (verticalScroller == null) return;
                verticalScroller.ValueChanged += ScrollChanged;
            }
        }

        /// <summary>
        /// Event handler for when the vertical scrollbar changes position
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScrollChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (DesignerProperties.IsInDesignTool) return;

            var max = verticalScroller.Maximum;
            var newValue = e.NewValue;
            if (max > 0 && newValue > 0)
            {
                // Raise the scroll event passing in the percentage down the list
                Scrolled.SafeRaise(this, (newValue * 100) / max);
            }

        }


        /// <summary>
        /// Handles changing the source (ie when the Source property is set). 
        /// Wires up event handlers for when the collection changes and populates
        /// the list with any existing items
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        private void SourceChanged(INotifyCollectionChanged oldValue, INotifyCollectionChanged newValue)
        {
            if (DesignerProperties.IsInDesignTool) return;

            if (oldValue != null)
            {
                oldValue.CollectionChanged -= CollectionChanged;
                realSource = null;

            }

            if (newValue != null)
            {

                // Create the underlying data source
                var type = newValue.GetType();
                var list = Activator.CreateInstance(type) as IList;
                realSource = list;
                ItemsSource = realSource;

                // Spawn a background operation that will iterate through the existing items
                // jump back to the UI thread in order to add them.
                ThreadPool.QueueUserWorkItem(asyncResult =>
                                                 {
                                                     var wait = new AutoResetEvent(false);
                                                     var newList = (newValue as IList).OfType<object>().ToArray();
                                                     foreach (var item in newList)
                                                     {
                                                         Thread.Sleep(10);
                                                         var itemToAdd = item;
                                                         Dispatcher.BeginInvoke(() =>
                                                                                         {
                                                                                             list.Add(itemToAdd);
                                                                                             wait.Set();
                                                                                         });
                                                         wait.WaitOne();
                                                     }
                                                     newValue.CollectionChanged += CollectionChanged;
                                                 });

            }

        }

        private readonly AutoResetEvent visualUpdateLock = new AutoResetEvent(false);

        /// <summary>
        /// Event handler for when the collection changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (DesignerProperties.IsInDesignTool) return;

            Dispatcher.BeginInvoke(() =>
                                            {
                                                switch (e.Action)
                                                {
                                                    case NotifyCollectionChangedAction.Add:
                                                        // Add/Insert items to the data source
                                                        var idx = e.NewStartingIndex;
                                                        foreach (var item in e.NewItems)
                                                        {
                                                            if (idx > realSource.Count)
                                                            {
                                                                realSource.Add(item);
                                                            }
                                                            else
                                                            {
                                                                realSource.Insert(idx, item);
                                                            }
                                                            idx++;
                                                        }
                                                        break;
                                                    case NotifyCollectionChangedAction.Remove:
                                                        // Remove items from the data source
                                                        foreach (var item in e.OldItems)
                                                        {
                                                            if (realSource.Contains(item))
                                                            {
                                                                realSource.Remove(item);
                                                            }
                                                        }
                                                        break;
                                                }


                                                visualUpdateLock.Set();
                                            });
            visualUpdateLock.WaitOne();
        }


    }


}
