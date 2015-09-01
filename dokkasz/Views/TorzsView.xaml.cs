using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace dokkasz.Views
{
    /// <summary>
    /// Interaction logic for TorzsView.xaml
    /// </summary>
    public partial class TorzsView : UserControl
    {
        public ObservableCollection<DataGridColumn> DataGridColumns
        {
            get { return dataGrid.Columns; }
        }

        public TorzsView()
        {
            InitializeComponent();
        }
    }
}
