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

            // fake login check kinda like a mock authentication
            // we hardcoded this as well because our priority was getting the cross platform db pipeline working
            // to fully finish the build, we will need to write a new sql query to check the actual users

            if(email.Trim() == "student@test.com" && password.Trim() == "1234")
            {
                // changes the button text to show it works
                var btn = source as Button;
                btn.Content = "Opening Dashboard...";

                // displays the dashboard window
                var dashboard = new DashboardWindow();
                dashboard.Show();

                // wait 1 sec so the UI can process the new window, then hide the login
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