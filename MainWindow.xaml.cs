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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ABtalk_Students_Register
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnCancle_Click(object sender, RoutedEventArgs e)
        {
            FirstNameTextBox.Text = "";
            MiddleNameTextBox.Text = "";
            LastNameTextBox.Text = "";
            SchoolTextBox.Text = "";
            ClassComboBox.SelectedIndex = -1;
        }
    }




    //Textbox Helper Class for gray label text
    public class TextBoxHelper
    {
        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.RegisterAttached("Placeholder", typeof(string), typeof(TextBoxHelper), new PropertyMetadata("", OnPlaceholderChanged));

        public static string GetPlaceholder(TextBox textBox)
        {
            return (string)textBox.GetValue(PlaceholderProperty);
        }

        public static void SetPlaceholder(TextBox textBox, string value)
        {
            textBox.SetValue(PlaceholderProperty, value);
        }

        private static void OnPlaceholderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox textBox)
            {
                textBox.GotFocus += (s, ev) =>
                {
                    if (textBox.Text == e.NewValue.ToString())
                    {
                        textBox.Text = "";
                        textBox.Foreground = Brushes.Black;
                    }
                };

                textBox.LostFocus += (s, ev) =>
                {
                    if (string.IsNullOrWhiteSpace(textBox.Text))
                    {
                        textBox.Text = e.NewValue.ToString();
                        textBox.Foreground = Brushes.Gray;
                    }
                };

                // Initialize placeholder
                textBox.Text = e.NewValue.ToString();
                textBox.Foreground = Brushes.Gray;
            }
        }
    }
}
