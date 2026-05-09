using Avalonia.Controls;
using Avalonia.Interactivity;

namespace LanguageExamApp
{
    public partial class MainWindow : Window // partial cos half of the window is xaml
    {
        public MainWindow()
        {
            InitializeComponent(); // basically go read the design and draw the buttons
        }

        public void BtnLogin_Click(object source, RoutedEventArgs args) //this fires up when the user clicks the login button
        {
            // the following just finds the buttons from our ui
            var emailBox = this.FindControl<TextBox>("txtEmail");
            var passwordBox = this.FindControl<TextBox>("txtPassword");
            var loginBtn = source as Button;

            // to prevent crashing if the boxes are empty
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