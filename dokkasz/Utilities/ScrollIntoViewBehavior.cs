using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace dokkasz.Utilities
{
    class ScrollIntoViewBehavior : Behavior<DataGrid>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.SelectionChanged += AssociatedObject_SelectionChanged;
        }

        private void AssociatedObject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AssociatedObject.SelectedItem != null)
            {
                AssociatedObject.Dispatcher.BeginInvoke((Action)(() =>
                {
                    //var info = new DataGridCellInfo(AssociatedObject.Items[AssociatedObject.SelectedIndex], AssociatedObject.Columns[0]);
                    //var cell = (DataGridCell)info.Column.GetCellContent(info.Item)?.Parent;
                    //AssociatedObject.SelectedCells.Clear();
                    //AssociatedObject.SelectedCells.Add(info);
                    //AssociatedObject.CurrentCell = info;

                    AssociatedObject.UpdateLayout();
                    AssociatedObject.ScrollIntoView(AssociatedObject.SelectedItem);
                }));
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.SelectionChanged -= AssociatedObject_SelectionChanged;
        }
    }
}
