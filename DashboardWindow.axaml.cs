using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Collections.ObjectModel; // We added this for the smart lists!
using System;

namespace LanguageExamApp
{
    public partial class DashboardWindow : Window
    {
        // These are our smart lists. We declare them here so the whole window can see them.
        private ObservableCollection<string> availableExams;
        private ObservableCollection<string> myExams;

        public DashboardWindow()
        {
            InitializeComponent();
            SetupMockDatabase();
        }

        private void SetupMockDatabase()
        {
            // 1. Initialize the smart lists
            availableExams = new ObservableCollection<string>
            {
                "ID: 101 | B1 Intermediate | Saturday 9:00 AM | Seats: 15",
                "ID: 102 | B2 Upper-Intermediate | Saturday 1:00 PM | Seats: 3",
                "ID: 103 | C1 Advanced | Sunday 10:00 AM | Seats: 0 (FULL)"
            };
            
            myExams = new ObservableCollection<string>(); // Starts completely empty

            // 2. Connect the UI boxes to these lists
            var topBox = this.FindControl<ListBox>("lstAvailableExams");
            var bottomBox = this.FindControl<ListBox>("lstMyExams");

            topBox.ItemsSource = availableExams;
            bottomBox.ItemsSource = myExams;
        }

        public void BtnRegister_Click(object source, RoutedEventArgs args)
        {
            var examIdBox = this.FindControl<TextBox>("txtExamID");
            var feedbackText = this.FindControl<TextBlock>("txtFeedback");
            
            if (examIdBox == null || feedbackText == null) return;

            string selectedId = examIdBox.Text ?? "";

            if (string.IsNullOrWhiteSpace(selectedId))
            {
                feedbackText.Foreground = Avalonia.Media.Brushes.Red;
                feedbackText.Text = "Please enter an Exam ID.";
                return;
            }

            // --- REAL DATABASE CONNECTION STARTS HERE ---
            DatabaseHelper dbHelper = new DatabaseHelper();

            try
            {
                using (var conn = dbHelper.GetConnection())
                {
                    conn.Open();

                    // Tell C# the name of the SQL Stored Procedure
                    using (var cmd = new Microsoft.Data.SqlClient.SqlCommand("RegisterForExam", conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        // Pass the parameters (Assuming hardcoded UserID 1 for now, and the ExamID they typed)
                        cmd.Parameters.AddWithValue("@p_UserID", 1);
                        cmd.Parameters.AddWithValue("@p_SessionID", int.Parse(selectedId));

                        // Execute the SQL!
                        cmd.ExecuteNonQuery();

                        // If SQL doesn't throw an error, it was a success!
                        feedbackText.Foreground = Avalonia.Media.Brushes.Green;
                        feedbackText.Text = $"Successfully registered for Exam ID: {selectedId}!";
                        
                        myExams.Add($"Registered for Exam ID: {selectedId} (Status: Confirmed SQL)");
                        examIdBox.Text = ""; 
                    }
                }
            }
            catch (Exception ex)
            {
                // If the Stored Procedure fails (e.g., 0 seats left), SQL sends an error back to C#
                feedbackText.Foreground = Avalonia.Media.Brushes.Red;
                feedbackText.Text = "Database Error: " + ex.Message;
            }
        }
    }
}