using Avalonia.Controls;
using Avalonia.Interactivity;

namespace LanguageExamApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public void BtnLogin_Click(object source, RoutedEventArgs args)
        {
            // 1. Grab the text boxes from the UI
            var emailBox = this.FindControl<TextBox>("txtEmail");
            var passwordBox = this.FindControl<TextBox>("txtPassword");
            var loginBtn = source as Button;

            // Prevent crashing if the boxes are empty
            if (emailBox == null || passwordBox == null || loginBtn == null) return;

            string email = emailBox.Text ?? "";
            string password = passwordBox.Text ?? "";

            // 2. Fake Login Check (We will connect this to SQL next)
            // Fake Login Check
            if(email.Trim() == "student@test.com" && password.Trim() == "1234")
            {
                // Change button text so we know it worked
                var btn = source as Button;
                btn.Content = "Opening Dashboard...";

                // Create and show the dashboard
                var dashboard = new DashboardWindow();
                dashboard.Show();

                // Wait 1 second so the UI can process the new window, then hide the login
                System.Threading.Tasks.Task.Delay(500).ContinueWith(_ => 
                {
                    Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() => this.Hide());
                });
            }
            else
            {
                var btn = source as Button;
                btn.Content = "Failed. Try again.";
            }
        }
    }
}