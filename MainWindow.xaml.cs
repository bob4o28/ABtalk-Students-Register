using System;
using System.Collections.Generic;
using System.Data;
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
using MySql.Data.MySqlClient;

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

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string sql = "INSERT INTO students VALUES (@idStudents, @FirstName, @MidName, @LastName, @School, @Class)";
            MySqlConnection con = dbABtalk.GetConnection();
            MySqlCommand cmd = new MySqlCommand(sql, con);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add("@idStudents", MySqlDbType.Int64).Value = null;
            cmd.Parameters.Add("@FirstName", MySqlDbType.VarChar).Value = FirstNameTextBox.Text;
            cmd.Parameters.Add("@MidName", MySqlDbType.VarChar).Value = MiddleNameTextBox.Text;
            cmd.Parameters.Add("@LastName", MySqlDbType.VarChar).Value = LastNameTextBox.Text;
            cmd.Parameters.Add("@School", MySqlDbType.VarChar).Value = SchoolTextBox.Text;
            cmd.Parameters.Add("@Class", MySqlDbType.VarChar).Value = ClassComboBox.SelectedValue.ToString();
            try
            {
                cmd.ExecuteNonQuery();
                MessageBox.Show("Registered Successfully.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Student not registered! \n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            con.Close();
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
