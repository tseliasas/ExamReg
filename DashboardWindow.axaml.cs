using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Collections.ObjectModel; // for smart lists
using System;
using Avalonia.Media;

namespace LanguageExamApp
{
    public partial class DashboardWindow : Window
    {
        // these are the 2 smart lists were gonna use
        private ObservableCollection<string> availableExams;
        private ObservableCollection<string> myExams;

        public DashboardWindow()
        {
            InitializeComponent();
            SetupMockDatabase();
        }

        private void SetupMockDatabase()
        {
            // initialize the smart lists
            availableExams = new ObservableCollection<string>
            {
                "ID: 101 | B1 Intermediate | Saturday 9:00 AM | Seats: 15",
                "ID: 102 | B2 Upper-Intermediate | Saturday 1:00 PM | Seats: 3",
                "ID: 103 | C1 Advanced | Sunday 10:00 AM | Seats: 0 (FULL)"
            };
            
            myExams = new ObservableCollection<string>(); // its gonna start fuolly empty

            // connect the UI boxes to our lists
            var topBox = this.FindControl<ListBox>("lstAvailableExams");
            var bottomBox = this.FindControl<ListBox>("lstMyExams");

            topBox.ItemsSource = availableExams;
            bottomBox.ItemsSource = myExams;
        }

        // when user clicks register
        public void BtnRegister_Click(object source, RoutedEventArgs args){
            var txtExamId = this.FindControl<TextBox>("txtExamID"); // To Yahya - ID was written in small here thats what was causing the bug, its working now.
            var lblMessage = this.FindControl<TextBlock>("txtFeedback");

            // also added this so we it doesnt crash whenever we rename things
            // avalonia kept crashing before adding this
            if (txtExamId == null || lblMessage == null)
            {
                Console.WriteLine("CRASH PREVENTED: Still can't find the UI boxes. Check names!");
                return; 
            }

    // 2. GET THE INPUT
    string selectedId = txtExamId.Text ?? ""; 
            if (string.IsNullOrWhiteSpace(selectedId)) // basically checks if its left empty
            {
                lblMessage.Text = "Error: Please enter an Exam ID.";
                lblMessage.Foreground = Brushes.Red; // just makes the error pop out to be red
                return; 
            }

            if (!int.TryParse(selectedId, out int parsedExamId)) // checks if user actually entered a number and not other characters
            {
                lblMessage.Text = "Error: Exam ID must be a number.";
                lblMessage.Foreground = Brushes.Red;
                return;
            }

            if (parsedExamId != 101 && parsedExamId != 102 && parsedExamId != 103) // checks if the exam is in our list, in this case checks if id is 101 102 or 103
            {
                lblMessage.Text = "Error: That Exam ID does not exist.";
                lblMessage.Foreground = Brushes.Red;
                return;
            }

            // 2. THE DATABASE LOGIC
            // if the code gets here it basically means that its passed all our 3 checks above with a valid id
            
            DatabaseHelper dbHelper = new DatabaseHelper();
            
            try 
            {
                using (var conn = dbHelper.GetConnection())
                {
                    conn.Open();
                    
                    // uses stored procedure instead of raw sql strings to prevent sql injection
                    using (var cmd = new Microsoft.Data.SqlClient.SqlCommand("RegisterForExam", conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        
                        // if we were to release this whole app, we will need build proper login
                        // and pass real logged in user's id here
                        // but for this assignment we're hardcoding the UserID to 1 to test the database pipeline

                        cmd.Parameters.AddWithValue("@p_UserID", 1); 
                        cmd.Parameters.AddWithValue("@p_SessionID", parsedExamId); 
                        
                        cmd.ExecuteNonQuery();
                    }
                    
                    // once they click register with a valid id this pops up
                    lblMessage.Text = $"Successfully registered for Exam ID: {parsedExamId}!";
                    lblMessage.Foreground = Brushes.Green;

                    // instantly updated My exams section without restarting the app
                    myExams.Add($"Registered for Exam ID: {parsedExamId} (Status: Confirmed SQL)");
                }
            }
            catch (Exception ex) // to catch network errors (like windows firewall blocking port 1433)
            {
                lblMessage.Text = "Database Error: " + ex.Message;
                lblMessage.Foreground = Brushes.Red;
            }
        }
    }
}