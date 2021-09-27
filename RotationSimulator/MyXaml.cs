using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RotationSimulator
{
    public class MyXaml : DependencyObject
    {
        public static readonly DependencyProperty RotationStepIdProperty = DependencyProperty.RegisterAttached("RotationStepId", typeof(int), typeof(MyXaml), new PropertyMetadata());
        public static readonly DependencyProperty ActionIdProperty = DependencyProperty.RegisterAttached("ActionId", typeof(string), typeof(MyXaml), new PropertyMetadata());

        public static int? GetRotationStepId(DependencyObject d) {
            return (int?)d.GetValue(RotationStepIdProperty);
        }

        public static void SetRotationStepId(DependencyObject d, int id) {
            d.SetValue(RotationStepIdProperty, id);
        }
        public static string GetActionIdProperty(DependencyObject d) {
            return (string)d.GetValue(ActionIdProperty);
        }

        public static void SetActionIdProperty(DependencyObject d, string id) {
            d.SetValue(ActionIdProperty, id);
        }
    }
}
