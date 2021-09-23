using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace RotationSimulator
{
    /// <summary>
    /// Interaction logic for AddRotationDialog.xaml
    /// </summary>
    public partial class AddRotationDialog : Window
    {
        public AddRotationDialog() {
            InitializeComponent();
        }

        private void b_cancel_Click(object sender, RoutedEventArgs e) {
            Close();
        }

        private void b_add_Click(object sender, RoutedEventArgs e) {
            Close();
        }
    }
}
