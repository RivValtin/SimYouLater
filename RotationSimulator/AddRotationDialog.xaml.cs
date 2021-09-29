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
        public bool confirmed = false;
        public AddRotationDialog() {
            InitializeComponent();
        }

        private void b_cancel_Click(object sender, RoutedEventArgs e) {
            confirmed = false;
            Close();
        }

        private void b_add_Click(object sender, RoutedEventArgs e) {
            confirmed = true;
            Close();
        }

        private void tb_rotationName_TextChanged(object sender, TextChangedEventArgs e) {
            if (b_add != null) {
                SetAddButtonState();
            }
        }

        private void cb_jobSelector_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (b_add != null) {
                SetAddButtonState();
            }
        }

        private void SetAddButtonState() {
            b_add.IsEnabled = !string.IsNullOrEmpty(tb_rotationName.Text) && ActionBank.actionSets.ContainsKey(cb_jobSelector.Text);
        }
    }
}
