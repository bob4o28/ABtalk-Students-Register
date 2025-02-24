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
        private Students _selectedStudent;
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
            EmailTextBox.Text = "";
            SchoolComboBox.SelectedIndex = -1;
            ClassComboBox.SelectedIndex = -1;
            ClassLetterComboBox.SelectedIndex = -1;
            
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (FirstNameTextBox.Text == "" || LastNameTextBox.Text == "" || SchoolComboBox.SelectedIndex == -1 || ClassComboBox.SelectedIndex == -1)
            {
                MessageBox.Show("Please fill all fields.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            string sql = "INSERT INTO students(FirstName, MidName, LastName, Email, School, Class, ClassLetter, RegTime, PauseTime, PauseLast) VALUES (@FirstName, @MidName, @LastName, @Email, @School, @Class, @ClassLetter, @RegTime, @PauseTime, @PauseLast)";
            MySqlConnection con = dbABtalk.GetConnection();
            MySqlCommand cmd = new MySqlCommand(sql, con);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add("@FirstName", MySqlDbType.VarChar).Value = FirstNameTextBox.Text;
            cmd.Parameters.Add("@MidName", MySqlDbType.VarChar).Value = MiddleNameTextBox.Text;
            cmd.Parameters.Add("@LastName", MySqlDbType.VarChar).Value = LastNameTextBox.Text;
            cmd.Parameters.Add("@Email", MySqlDbType.VarChar).Value = EmailTextBox.Text;
            cmd.Parameters.Add("@School", MySqlDbType.VarChar).Value = SchoolComboBox.SelectionBoxItem.ToString();
            cmd.Parameters.Add("@Class", MySqlDbType.VarChar).Value = ClassComboBox.SelectionBoxItem.ToString();
            cmd.Parameters.Add("@ClassLetter", MySqlDbType.VarChar).Value = ClassLetterComboBox.SelectionBoxItem.ToString();
            cmd.Parameters.Add("@RegTime", MySqlDbType.DateTime).Value = DateTime.Now;
            cmd.Parameters.Add("@PauseTime", MySqlDbType.DateTime).Value = DateTime.MinValue;
            cmd.Parameters.Add("@PauseLast", MySqlDbType.DateTime).Value = DateTime.MinValue;
            try
            {
                cmd.ExecuteNonQuery();
                MessageBox.Show("Registered Successfully.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                FirstNameTextBox.Text = "";
                MiddleNameTextBox.Text = "";
                LastNameTextBox.Text = "";
                EmailTextBox.Text = "";
                SchoolComboBox.SelectedIndex = -1;
                ClassComboBox.SelectedIndex = -1;
                ClassLetterComboBox.SelectedIndex = -1;
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
                string query = "SELECT idStudents, FirstName, MidName, LastName, School, Class, ClassLetter FROM students WHERE LOWER(FirstName) LIKE @searchText OR LOWER(LastName) LIKE @searchText";
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
                            idStudents = Convert.ToInt32(reader["idStudents"]),
                            FirstName = reader["FirstName"].ToString(),
                            MidName = reader["MidName"].ToString(),
                            LastName = reader["LastName"].ToString(),
                            School = reader["School"].ToString(),
                            Class = reader["Class"].ToString(),
                            ClassLetter = reader["ClassLetter"].ToString()
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
                _selectedStudent = student;
                LoadStudentData(student.idStudents);
                //MessageBox.Show($"FunctionButton_Click called for student ID: {student.idStudents}", "Debug", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private void LoadStudentData(int studentId)
        {
            string connectionString = "datasource=localhost;port=3306;username=root;password=MySQL.bg.bobo_09!;database=abtalk"; // Replace with your actual connection string
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                string query = "SELECT * FROM students WHERE idStudents = @idStudents";
                MySqlCommand cmd = new MySqlCommand(query, con);
                cmd.Parameters.AddWithValue("@idStudents", studentId);

                try
                {
                    con.Open();
                    MySqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        _selectedStudent = new Students
                        {
                            idStudents = Convert.ToInt32(reader["idStudents"]),
                            FirstName = reader["FirstName"].ToString(),
                            MidName = reader["MidName"].ToString(),
                            LastName = reader["LastName"].ToString(),
                            School = reader["School"].ToString(),
                            Class = reader["Class"].ToString(),
                            ClassLetter = reader["ClassLetter"].ToString(),
                            RegTime = reader["RegTime"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["RegTime"]),
                            LastTime = reader["LastTime"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["LastTime"]),
                            PauseTime = reader["PauseTime"] == DBNull.Value ? (DateTime?)null : (reader["PauseTime"] is TimeSpan ? DateTime.MinValue.Add((TimeSpan)reader["PauseTime"]) : Convert.ToDateTime(reader["PauseTime"])),
                            PauseLast = reader["PauseLast"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["PauseLast"]),
                            Status = reader["Status"].ToString()
                        };

                        txName.Text = $"{_selectedStudent.FirstName} {_selectedStudent.MidName} {_selectedStudent.LastName}";
                        txSchool.Text = _selectedStudent.School;
                        txClass.Text = _selectedStudent.Class;
                        txID.Text = _selectedStudent.idStudents.ToString();
                        txStatus.Text = reader["Status"].ToString();
                        if (_selectedStudent.LastTime.HasValue)
                        {
                            TimeSpan timeDifference = _selectedStudent.LastTime.Value - _selectedStudent.RegTime;
                            txTime.Text = $"{timeDifference.Hours} hours {timeDifference.Minutes} minutes";
                        }
                        else
                        {
                            txTime.Text = "N/A";
                        }
                    }
                    reader.Close();
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Error retrieving data from database: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            if (_selectedStudent.Status == "Pause")
            {
                btnPauseTime.Content = "Resume Time";
            }
            else
            {
                btnPauseTime.Content = "Pause Time";
            }
        }

        private void btnEndTime_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedStudent == null)
            {
                MessageBox.Show("No student selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string connectionString = "datasource=localhost;port=3306;username=root;password=MySQL.bg.bobo_09!;database=abtalk"; // Replace with your actual connection string
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                string query = "UPDATE students SET LastTime = @LastTime, InTime = @InTime, Status = @Status WHERE idStudents = @idStudents";
                MySqlCommand cmd = new MySqlCommand(query, con);
                DateTime leaveTime = DateTime.Now;
                TimeSpan timeDifference = leaveTime - _selectedStudent.RegTime;
                DateTime NullTime = DateTime.MinValue;
                //TimeSpan totalPauseTime = _selectedStudent.PauseTime.HasValue ? (_selectedStudent.PauseTime.Value - NullTime) : TimeSpan.Zero;
                DateTime TimeDiff = DateTime.MinValue + timeDifference;
                TimeSpan totalTime = TimeDiff - _selectedStudent.PauseTime.Value;
                string status = "Left";

                cmd.Parameters.AddWithValue("@LastTime", leaveTime);
                cmd.Parameters.AddWithValue("@InTime", totalTime.TotalMinutes);
                cmd.Parameters.AddWithValue("@Status", status);
                cmd.Parameters.AddWithValue("@idStudents", _selectedStudent.idStudents);

                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    _selectedStudent.LastTime = leaveTime;
                    txStatus.Text = status;
                    txTime.Text = $"{totalTime.Hours} hours {totalTime.Minutes} minutes";
                    MessageBox.Show("Leave time and status updated successfully.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Error updating leave time and status: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SchoolComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SchoolComboBox.SelectedItem is ComboBoxItem selectedItem && selectedItem.Content.ToString() == "Other")
            {
                string newSchool = PromptForNewValue("Enter new school name:");
                if (!string.IsNullOrEmpty(newSchool))
                {
                    SchoolComboBox.Items.Insert(SchoolComboBox.Items.Count - 1, new ComboBoxItem { Content = newSchool });
                    SchoolComboBox.SelectedItem = SchoolComboBox.Items[SchoolComboBox.Items.Count - 2];
                }
                else
                {
                    SchoolComboBox.SelectedIndex = -1;
                }
            }
        }

        private string PromptForNewValue(string message)
        {
            InputDialog inputDialog = new InputDialog(message);
            if (inputDialog.ShowDialog() == true)
            {
                return inputDialog.ResponseText;
            }
            return null;
        }

        private void ClassLetterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ClassLetterComboBox.SelectedItem is ComboBoxItem selectedItem && selectedItem.Content.ToString() == "Other")
            {
                string newClassLetter = PromptForNewValue("Enter new Class Letter:");
                if (!string.IsNullOrEmpty(newClassLetter))
                {
                    ClassLetterComboBox.Items.Insert(ClassLetterComboBox.Items.Count - 1, new ComboBoxItem { Content = newClassLetter });
                    ClassLetterComboBox.SelectedItem = ClassLetterComboBox.Items[ClassLetterComboBox.Items.Count - 2];
                }
                else
                {
                    ClassLetterComboBox.SelectedIndex = -1;
                }
            }
        }

        private void btnPauseTime_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedStudent == null)
            {
                MessageBox.Show("No student selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string connectionString = "datasource=localhost;port=3306;username=root;password=MySQL.bg.bobo_09!;database=abtalk"; // Replace with your actual connection string
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                
                if (_selectedStudent.Status == "Registered")
                {
                    string query = "UPDATE students SET PauseLast = @PauseLast, Status = @Status WHERE idStudents = @idStudents";
                    MySqlCommand cmd = new MySqlCommand(query, con);
                    DateTime PauseTimeNow = DateTime.Now;
                    string status = "Pause";
                    cmd.Parameters.AddWithValue("@PauseLast", PauseTimeNow);
                    cmd.Parameters.AddWithValue("@Status", status);
                    cmd.Parameters.AddWithValue("@idStudents", _selectedStudent.idStudents);
                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        txStatus.Text = status;
                        btnPauseTime.Content = "Resume Time";
                        MessageBox.Show("Pause time and status updated successfully.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show("Error updating pause time and status: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else if (_selectedStudent.Status == "Pause")
                {
                    string query = "UPDATE students SET PauseTime = @PauseTime, Status = @Status WHERE idStudents = @idStudents";
                    MySqlCommand cmd = new MySqlCommand(query, con);
                    DateTime PauseTimeNow = DateTime.Now;
                    TimeSpan timeDifference = PauseTimeNow - _selectedStudent.PauseLast.Value;
                    TimeSpan totalTime = timeDifference;
                    string status = "Registered";
                    cmd.Parameters.AddWithValue("@PauseTime", totalTime.TotalMinutes);
                    cmd.Parameters.AddWithValue("@Status", status);
                    cmd.Parameters.AddWithValue("@idStudents", _selectedStudent.idStudents);
                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        txStatus.Text = status;
                        btnPauseTime.Content = "Pause Time";
                        MessageBox.Show("Pause time and status updated successfully.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show("Error updating pause time and status: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
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
