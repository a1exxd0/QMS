using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using QMS.QMSScripts;
using QMS.ViewModels;
using Windows.Devices.Enumeration;

namespace QMS.Views;
public sealed partial class MainPage : Page
{
    public EventHandler<RoutedEventArgs>? LoginPressedRecieved; //Event flags
    public EventHandler<RoutedEventArgs>? RegisterPressedRecieved;
    public EventHandler<RoutedEventArgs>? SubmittedRegisterRecieved;
    public EventHandler<RoutedEventArgs?> SubmittedLoginRecieved;

    public MainViewModel ViewModel
    {
        get;
    }

    public MainPage()
    {
        ViewModel = App.GetService<MainViewModel>();
        Resources.Add("LeftBorderWidth", 300);
        Resources.Add("TopBorderHeight", 240);
        Resources.Add("LeftBorderColour", "#C3C3C3");
        Resources.Add("PurpleColour", "#C293FF");
        Resources.Add("LightGrey", "#FFDCDCDE");

        LoginPressedRecieved += loginPressedRecievedFunction;
        RegisterPressedRecieved += registerPressedRecievedFunction;
        SubmittedRegisterRecieved += RegisterPressedFunction;
        SubmittedLoginRecieved += LoginPressedFunction;

        
        
        InitializeComponent();


    }
    private void loginPressedRecievedFunction(object sender, RoutedEventArgs e)
    {
        SubmitRegister.Visibility = Visibility.Collapsed;
        RegisterForm.Visibility = Visibility.Collapsed;
        ErrorBoxLogin.Text = "";
        SuccessBoxRegister.Text = "";
        SubmitLogin.Visibility = Visibility.Visible;
        LoginForm.Visibility = Visibility.Visible;
    }
    private void registerPressedRecievedFunction(object sender, RoutedEventArgs e)
    {
        SubmitLogin.Visibility = Visibility.Collapsed;
        LoginForm.Visibility = Visibility.Collapsed;
        ErrorBoxRegister.Text = "";
        SuccessBoxRegister.Text = "";
        SubmitRegister.Visibility = Visibility.Visible;
        RegisterForm.Visibility = Visibility.Visible;
    }
    /// <summary>
    /// Once used, user is locked into signing up until it enters valid entries.
    /// Stores values into database.
    /// </summary>
    /// <returns>returns true for success and false for faliure</returns>
    private void RegisterPressedFunction(object sender, RoutedEventArgs e)
    {
        Dictionary<int, string> Problems = new Dictionary<int, string>();
        Problems.Add(1, "Invalid SQL-based phrases used");
        Problems.Add(2, "Not in length 5-49 inclusive");
        Problems.Add(3, "Character(s) not in ASCII set");
        Problems.Add(4, "Username already exists");
        Problems.Add(5, "Username already exists inside of DeletedAccounts");

        var wantedUsername = UsernameRegister.Text;
        var wantedPassword = PasswordRegister.Password.ToString();

        if (LoginFunctions.SignUpUsernameCheck(wantedUsername) != 0)
        {
            ErrorBoxRegister.Text = Problems
                [LoginFunctions.SignUpUsernameCheck(wantedUsername)];
            return;
        }
        if (LoginFunctions.StandardCheck(wantedPassword) != 0)
        {
            ErrorBoxRegister.Text = Problems
                [LoginFunctions.StandardCheck(wantedPassword)];
            return;
        }
        var hashedPassword = LoginFunctions.HashAndSalt(wantedUsername, wantedPassword);
        LoginFunctions.StoreUserInfo(wantedUsername, hashedPassword);
        LoginFunctions.StoreAttemptInfo(wantedUsername, LoginFunctions.GetTime(), 1);
        SuccessBoxRegister.Text = "Successful registration";
        ErrorBoxRegister.Text = "";
        if (LoginFunctions.CheckUsernameExistsInAttempts(wantedUsername))
        {
            LoginFunctions.DeleteRelevantAttempts(wantedUsername);
        }
        return;
    }
    /// <summary>
    /// Submits login attempt to database
    /// </summary>
    /// <returns>true if succesful, false if not</returns>
    private void LoginPressedFunction(object sender, RoutedEventArgs e)
    {
        Dictionary<int, string> Problems = new Dictionary<int, string>();
        Problems.Add(1, "Invalid entry/ies");
        Problems.Add(2, "Username doesnt exist");
        Problems.Add(3, "Too many attempts - account now deleted");

        var inputUsername = UsernameLogin.Text;
        var inputPassword = PasswordLogin.Password.ToString();
        var userCheck = LoginFunctions.StandardCheck(inputUsername);
        var passCheck = LoginFunctions.StandardCheck(inputPassword);
        if (userCheck != 0 || passCheck != 0)
        {
            ErrorBoxLogin.Text = Problems[1]; return;
        }
        if (!LoginFunctions.CheckUsernameExists(inputUsername))
        {
            LoginFunctions.StoreAttemptInfo(inputUsername, LoginFunctions.GetTime(), 0);
            ErrorBoxLogin.Text = Problems[2]; return;
        }
        else
        {
            var hashedPassword = LoginFunctions.HashAndSalt(inputUsername, inputPassword);
            var comparisonPassword = LoginFunctions.GetPassword(inputUsername);

            if (hashedPassword == comparisonPassword)
            {
                LoginFunctions.StoreAttemptInfo(inputUsername, LoginFunctions.GetTime(), 1);
                KeyVarFunc.username = inputUsername;

                List<Border> MainItems = new();
                MainItems.Add(MainLeftBorder);
                MainItems.Add(MainTopBorder);
                MainItems.Add(LoginForm);
                MainItems.Add(RegisterForm);

                frame.Navigate(typeof(MessagingPage));

                for (var i = 0; i < MainItems.Count; i++)
                {
                    MainItems[i].Visibility = Visibility.Collapsed;
                }

                //ErrorBoxLogin.Text = "logged in as " + inputUsername;

                return;
            }
            else
            {
                LoginFunctions.StoreAttemptInfo(inputUsername, LoginFunctions.GetTime(), 0);
                if (LoginFunctions.GetConsecutiveWrong(inputUsername) >= 5)
                {
                    ErrorBoxLogin.Text = Problems[3];
                    LoginFunctions.DeleteAccount(inputUsername); return;
                }
                {
                    ErrorBoxLogin.Text = "Incorrect password on Attempt " +
                        LoginFunctions.GetConsecutiveWrong(inputUsername).ToString();
                    return;
                }
            }
        }
    }

    //Invokers
    private void LoginPressed(object sender, RoutedEventArgs e)
    {
        LoginPressedRecieved?.Invoke(this, e);
    }
    private void RegisterPressed(object sender, RoutedEventArgs e)
    {
        RegisterPressedRecieved?.Invoke(this, e);
    }
    private void SubmittedRegister(object sender, RoutedEventArgs e)
    {
        SubmittedRegisterRecieved?.Invoke(this, e);
    }
    private void SubmittedLogin(object sender, RoutedEventArgs e)
    {
        SubmittedLoginRecieved?.Invoke(this, e);
    }
}
