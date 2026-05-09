using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Collections.ObjectModel; // We added this for the smart lists!
using System;
using Avalonia.Media;

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

        // This is the method that triggers when they click the Register button
        public void BtnRegister_Click(object source, RoutedEventArgs args){
            // 1. Explicitly find the boxes using the EXACT names from the .axaml file!
            var txtExamId = this.FindControl<TextBox>("txtExamID"); // Changed to capital ID
            var lblMessage = this.FindControl<TextBlock>("txtFeedback"); // Changed to txtFeedback

            // 🛑 NEW DEFENSE: Let's make sure it doesn't crash if we ever rename things again!
            if (txtExamId == null || lblMessage == null)
            {
                Console.WriteLine("CRASH PREVENTED: Still can't find the UI boxes. Check names!");
                return; 
            }

    // 2. GET THE INPUT
    string selectedId = txtExamId.Text ?? ""; 

    // ... (Keep the rest of your bouncer and database code exactly as it is!) ...

            // ==========================================
            // 🛑 THE BOUNCER STARTS HERE 🛑
            // ==========================================

            // Check 1: Is it empty?
            if (string.IsNullOrWhiteSpace(selectedId))
            {
                lblMessage.Text = "Error: Please enter an Exam ID.";
                lblMessage.Foreground = Brushes.Red;
                return; 
            }
            
            // ... (Keep the rest of your bouncer and database code exactly as it is!) ...

            // Check 2: Is it a real number?
            if (!int.TryParse(selectedId, out int parsedExamId))
            {
                lblMessage.Text = "Error: Exam ID must be a number.";
                lblMessage.Foreground = Brushes.Red;
                return;
            }

            // Check 3: Is it one of our valid exams?
            if (parsedExamId != 101 && parsedExamId != 102 && parsedExamId != 103)
            {
                lblMessage.Text = "Error: That Exam ID does not exist.";
                lblMessage.Foreground = Brushes.Red;
                return;
            }

            // ==========================================
            // ✅ THE BOUNCER ENDS HERE ✅
            // ==========================================


            // 2. THE DATABASE LOGIC
            // If the code makes it down here, it means the data passed all 3 checks!
            // Now it is perfectly safe to open the connection to your friend's database.
            
            DatabaseHelper dbHelper = new DatabaseHelper();
            
            try 
            {
                using (var conn = dbHelper.GetConnection())
                {
                    conn.Open();
                    
                    using (var cmd = new Microsoft.Data.SqlClient.SqlCommand("RegisterForExam", conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        
                        // Use the clean, safe 'parsedExamId' that the bouncer approved
                        cmd.Parameters.AddWithValue("@p_UserID", 1); 
                        cmd.Parameters.AddWithValue("@p_SessionID", parsedExamId); 
                        
                        cmd.ExecuteNonQuery();
                    }
                    
                    // Update the screen to show success!
                    lblMessage.Text = $"Successfully registered for Exam ID: {parsedExamId}!";
                    lblMessage.Foreground = Brushes.Green;
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Database Error: " + ex.Message;
                lblMessage.Foreground = Brushes.Red;
            }
        }
    }
}