using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace NeteaseMusicDownloader.Behaviors
{
    public sealed class ScrollIntoViewBehavior : Behavior<ListView>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.SelectionChanged += ScrollIntoView;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.SelectionChanged -= ScrollIntoView;
            base.OnDetaching();
        }

        private void ScrollIntoView(object o, SelectionChangedEventArgs e)
        {
            ListView b = (ListView)o;
            if (b == null) return;
            if (b.SelectedItem == null) return;

            ListViewItem item = (ListViewItem)((ListView)o).ItemContainerGenerator.ContainerFromItem(((ListView)o).SelectedItem);
            if (item != null) item.BringIntoView();
        }
    }
}