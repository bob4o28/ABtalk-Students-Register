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
        // Replace this with your actual search results method
        private List<Students> _searchResults = new List<Students>();
        public MainWindow()
        {
            InitializeComponent();
            // For demonstration, load some dummy data:
            //LoadDummyData();
            dgSearchResults.ItemsSource = _searchResults;
        }

        private void BtnCancle_Click(object sender, RoutedEventArgs e)
        {
            FirstNameTextBox.Text = "";
            MiddleNameTextBox.Text = "";
            LastNameTextBox.Text = "";
            SchoolComboBox.SelectedIndex = -1;
            ClassComboBox.SelectedIndex = -1;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (FirstNameTextBox.Text == "" || LastNameTextBox.Text == "" || SchoolComboBox.SelectedIndex == -1 || ClassComboBox.SelectedIndex == -1)
            {
                MessageBox.Show("Please fill all fields.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            string sql = "INSERT INTO students(FirstName, MidName, LastName, School, Class, RegTime) VALUES (@FirstName, @MidName, @LastName, @School, @Class, @RegTime)";
            MySqlConnection con = dbABtalk.GetConnection();
            MySqlCommand cmd = new MySqlCommand(sql, con);
            cmd.CommandType = CommandType.Text;
            //cmd.Parameters.Add("@idStudents", MySqlDbType.Int32).Value = null;
            cmd.Parameters.Add("@FirstName", MySqlDbType.VarChar).Value = FirstNameTextBox.Text;
            cmd.Parameters.Add("@MidName", MySqlDbType.VarChar).Value = MiddleNameTextBox.Text;
            cmd.Parameters.Add("@LastName", MySqlDbType.VarChar).Value = LastNameTextBox.Text;
            cmd.Parameters.Add("@School", MySqlDbType.VarChar).Value = SchoolComboBox.SelectionBoxItem.ToString();
            cmd.Parameters.Add("@Class", MySqlDbType.VarChar).Value = ClassComboBox.SelectionBoxItem.ToString();
            cmd.Parameters.Add("@RegTime", MySqlDbType.DateTime).Value = DateTime.Now;
            try
            {
                cmd.ExecuteNonQuery();
                MessageBox.Show("Registered Successfully.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                FirstNameTextBox.Text = "";
                MiddleNameTextBox.Text = "";
                LastNameTextBox.Text = "";
                SchoolComboBox.SelectedIndex = -1;
                ClassComboBox.SelectedIndex = -1;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Student not registered! \n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            con.Close();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string searchText = SearchBox.Text.ToLower();
            if (string.IsNullOrWhiteSpace(searchText))
            {
                MessageBox.Show("Please enter a search term.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            _searchResults.Clear();
            string connectionString = "datasource=localhost;port=3306;username=root;password=MySQL.bg.bobo_09!;database=abtalk"; // Replace with your actual connection string
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                string query = "SELECT FirstName, MidName, LastName, School, Class FROM students WHERE LOWER(FirstName) LIKE @searchText OR LOWER(LastName) LIKE @searchText";
                MySqlCommand cmd = new MySqlCommand(query, con);
                cmd.Parameters.AddWithValue("@searchText", "%" + searchText + "%");

                try
                {
                    con.Open();
                    MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        _searchResults.Add(new Students
                        {
                            FirstName = reader["FirstName"].ToString(),
                            MidName = reader["MidName"].ToString(),
                            LastName = reader["LastName"].ToString(),
                            School = reader["School"].ToString(),
                            Class = reader["Class"].ToString()
                        });
                    }
                    reader.Close();
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Error retrieving data from database: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            dgSearchResults.Items.Refresh();
        }

        private void FunctionButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Students student)
            {
                // Implement your function logic for 'student'

                MessageBox.Show($"Function executed for {student.FirstName} {student.LastName}.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }

    ///Textbox Helper Class for gray label text
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


    //public class student
    //{
    //    public int idstudents { get; set; }
    //    public string firstname { get; set; }
    //    public string midname { get; set; }
    //    public string lastname { get; set; }
    //    public string school { get; set; }
    //    public string class { get; set; }
    //    public datetime regtime { get; set; }
    //}

}
