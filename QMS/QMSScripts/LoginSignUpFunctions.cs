using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace QMS.QMSScripts;

public class LoginFunctions
{
    

    #region Modular Functions
    /// <summary>
    /// Returns true if all inputted characters are in ASCII range 32-127
    /// </summary>
    /// <param name="inputted">String to be checked</param>
    /// <returns></returns>
    public static bool ValidationASII(string inputted) //takes string input and returns t/f based on if all characters are in ASCII set
    {
        foreach (char c in inputted) //loops through each character in string
        {
            if (!(c < 128 && c > 31)) //values >= 128 are not ASCII and values < 32 are control characters
            {
                return false; //there is a character not ASCII
            }
        }
        return true; //all characters checked and none are outside of set
    }
    /// <summary>
    /// Returns true if string length is between 5 and 49 inclusive
    /// </summary>
    /// <param name="inputted">String to be checked</param>
    /// <returns></returns>
    public static bool Validation50Chars(string inputted) // takes string input and returns t/f based on if there are between 5 and 49 characters
    {
        if (inputted.Length < 50 && inputted.Length > 4) //using string properties
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    /// <summary>
    /// Returns true if string doesn't contain forbidden characters
    /// </summary>
    /// <param name="inputted">String to be checked</param>
    /// <returns>True if string is safe to pass to database</returns>
    public static bool ValidationSQLProof(string inputted)// returns t/f from banned phrases (*, %, SELECT, CREATE, DELETE, and whitespace)
    {
        string[] forbiddenWords = new string[] { "*", "%", "SELECT", "CREATE", "DELETE", " " }; //unallowed phrases
        foreach (string s in forbiddenWords) //loops through unallowed string array
        {
            if (inputted.ToUpper().Contains(s)) //if unallowed phrase is in string,
            {
                return false; //return false
            }
        }
        return true;
    }
    /// <summary>
    /// Returns true if username exists in the table "LoginAndAccessInfo.dbo.UserInfo"
    /// </summary>
    /// <param name="inputted">Username to be checked</param>
    /// <returns></returns>
    public static bool CheckUsernameExists(string inputted)// accesses database, if it already exists return true, else, return false.
    {
        SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

        builder.DataSource = DatabaseOptions.dataSource; //database login information
        builder.UserID = "Entry";
        builder.Password = "password";
        builder.InitialCatalog = DatabaseOptions.initialCatalog;
        builder.TrustServerCertificate = true;

        using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
        {
            connection.Open();

            String query = "SELECT UserID FROM UserInfo"; //query

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader()) //object reader just looks at an entry of data in the database
                {
                    while (reader.Read()) //moves the object down by one entry, returns true until there are no entries left
                    {
                        if (reader.GetString(0) == inputted) // database formatted by UserID/HashedPassword, so (0) indicates UserID
                        {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }
    /// <summary>
    /// Returns true if username exists in the table "LoginAndAccessInfo.dbo.LoginAttemptInfo"
    /// </summary>
    /// <param name="inputted">Username to be checked</param>
    /// <returns></returns>
    public static bool CheckUsernameExistsInAttempts(string inputted)// accesses database, if it already exists return true, else, return false.
    {
        SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

        builder.DataSource = DatabaseOptions.dataSource; //database login information
        builder.UserID = "Entry";
        builder.Password = "password";
        builder.InitialCatalog = DatabaseOptions.initialCatalog;
        builder.TrustServerCertificate = true;

        using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
        {
            connection.Open();

            String query = "SELECT UserID FROM LoginAttemptInfo"; //query

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader()) //object reader just looks at an entry of data in the database
                {
                    while (reader.Read()) //moves the object down by one entry, returns true until there are no entries left
                    {
                        if (reader.GetString(0) == inputted) // database formatted by UserID/HashedPassword, so (0) indicates UserID
                        {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }
    /// <summary>
    /// Returns string of hexadecimals based on inputs username and (unhashed) password
    /// </summary>
    /// <param name="usernameInput">Username of user wanting to hash password</param>
    /// <param name="passwordInput">Unhashed password</param>
    /// <returns></returns>
    public static string HashAndSalt(string usernameInput, string passwordInput) //returns string of hexadecimals
    {
        string unhashed = passwordInput + ":" + usernameInput; //salted password
        using (HashAlgorithm algorithm = SHA256.Create()) //instance of algorithm used created
        {
            byte[] byteArray = algorithm.ComputeHash(Encoding.UTF8.GetBytes(unhashed)); //hash the salted password
            StringBuilder sb = new StringBuilder();
            foreach (byte b in byteArray)
            {
                sb.Append(b.ToString("X2")); //adds to string builder for every byte, the "X2" means in the format of uppercase hex.
            }
            return sb.ToString(); //returns 64 hex characters.
        }

    }
    /// <summary>
    /// Stores inputs into LoginAndAccessInfo.dbo.UserInfo database
    /// </summary>
    /// <param name="usernameInput">Username of user wanting to store data, verified (for no duplicates)</param>
    /// <param name="hashedPassword">Hashed and salted password directly being stored</param>
    public static void StoreUserInfo(string usernameInput, string hashedPassword)
    {
        SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

        builder.DataSource = DatabaseOptions.dataSource; //database login information
        builder.UserID = "AddUser";
        builder.Password = "password";
        builder.InitialCatalog = DatabaseOptions.initialCatalog;
        builder.TrustServerCertificate = true;

        using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
        {
            connection.Open();

            String query = $"INSERT INTO UserInfo(UserID, hashedPassword) VALUES (@username, @password)"; //query

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@username", usernameInput); //adds parameters to the command to be executed
                command.Parameters.AddWithValue("@password", hashedPassword);

                command.ExecuteNonQuery(); //executes a non-returning query (inserting here)
            }
        }
    }
    /// <summary>
    /// Deletes account entry from LoginAndAccessInfo.dbo.UserInfo.
    /// Stores deletion info
    /// </summary>
    /// <param name="username">Account userID of what you want deleted. Ensure username actually exists else it will cause error</param>
    public static void DeleteAccount(string username)
    {
        SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

        builder.DataSource = DatabaseOptions.dataSource; //database login information
        builder.UserID = "DeleteUser";
        builder.Password = "password";
        builder.InitialCatalog = DatabaseOptions.initialCatalog;
        builder.TrustServerCertificate = true;

        using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
        {
            connection.Open();

            String query = $"DELETE FROM UserInfo WHERE UserID = @user"; //query
            String query2 = $"INSERT INTO DeletedAccounts VALUES (@user, @dt)"; //second query

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@user", username);
                command.ExecuteNonQuery(); //executes a non-returning query (deleting here)
            }
            using (SqlCommand command2 = new SqlCommand(query2, connection))
            {
                command2.Parameters.AddWithValue("@user", username);
                command2.Parameters.AddWithValue("@dt", GetTime());
                command2.ExecuteNonQuery(); //executes a non-returning query (deleting here)
            }
        }
    }
    /// <summary>
    /// Stores attempt into LoginAttemptInfo
    /// </summary>
    /// <param name="UserID">Username used in login request</param>
    /// <param name="AttemptDateTime">Date & Time formatted as YYYY-MM-DD HH:MI:SS</param>
    /// <param name="AttemptSuccessful">0 for unsuccessful, 1 for successful</param>
    public static void StoreAttemptInfo(string UserID, string AttemptDateTime, int AttemptSuccessful)
    {
        SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

        builder.DataSource = DatabaseOptions.dataSource; //database login information
        builder.UserID = "AddUser";
        builder.Password = "password";
        builder.InitialCatalog = DatabaseOptions.initialCatalog;
        builder.TrustServerCertificate = true;

        using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
        {
            connection.Open();

            String query = $"INSERT INTO LoginAttemptInfo(UserID, AttemptDateTime, AttemptSuccessful) VALUES (@user, @datetime, @success)"; //query

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@user", UserID); //adds parameters to the command to be executed
                command.Parameters.AddWithValue("@datetime", AttemptDateTime);
                command.Parameters.AddWithValue("@success", AttemptSuccessful);

                command.ExecuteNonQuery(); //executes a non-returning query (inserting here)
            }
        }
    }
    /// <summary>
    /// Gets most recent consecutive wrong attempts for a user
    /// </summary>
    /// <param name="UserID">User you're trying to find consecutive wrong attempts for</param>
    /// <returns>Integer of how many consecutive wrongs there are</returns>
    public static int GetConsecutiveWrong(string UserID)
    {
        int Consecutives = 0;
        SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

        builder.DataSource = DatabaseOptions.dataSource; //database login information
        builder.UserID = "Entry";
        builder.Password = "password";
        builder.InitialCatalog = DatabaseOptions.initialCatalog;
        builder.TrustServerCertificate = true;

        using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
        {
            connection.Open();

            String query = "SELECT * FROM LoginAttemptInfo WHERE UserID = @username ORDER BY AttemptDateTime DESC";
            //query for most recent user login attempts

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@username", UserID);
                using (SqlDataReader reader = command.ExecuteReader())
                //object reader just looks at an entry of data in the database
                {
                    while (reader.Read() && reader.GetBoolean(2) == false)
                    //moves the object down by one entry, returns true until there are no entries left
                    //second statement finds out whether the attempt was successful
                    //exits once either statement runs to false
                    {
                        Consecutives++; //increment consecutive 'falses'
                    }
                }
            }
        }
        return Consecutives;
    }

    /// <summary>
    /// Requires valid username before use, returns password for any given username.
    /// </summary>
    /// <param name="userID">Username</param>
    /// <returns></returns>
    public static string GetPassword(string UserID)
    {
        SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

        builder.DataSource = DatabaseOptions.dataSource; //database login information
        builder.UserID = "Entry";
        builder.Password = "password";
        builder.InitialCatalog = DatabaseOptions.initialCatalog;
        builder.TrustServerCertificate = true;

        using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
        {
            connection.Open();

            String query = "SELECT HashedPassword FROM UserInfo WHERE UserID = @username"; //query

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@username", UserID);
                return command.ExecuteScalar().ToString();

            }
        }
    }
    /// <summary>
    /// Checks if a username exists in the table of deleted accounts
    /// </summary>
    /// <param name="UserID"></param>
    /// <returns></returns>
    public bool CheckUsernameExistsInAccounts(string UserID)
    {
        SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

        builder.DataSource = DatabaseOptions.dataSource; //database login information
        builder.UserID = "Entry";
        builder.Password = "password";
        builder.InitialCatalog = DatabaseOptions.initialCatalog;
        builder.TrustServerCertificate = true;

        using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
        {
            connection.Open();

            String query = "SELECT UserID FROM DeletedAccounts"; //query

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader()) //object reader just looks at an entry of data in the database
                {
                    while (reader.Read()) //moves the object down by one entry, returns true until there are no entries left
                    {
                        if (reader.GetString(0) == UserID) // database formatted by UserID/HashedPassword, so (0) indicates UserID
                        {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }
    /// <summary>
    /// Gets time for SQL DateTime format
    /// </summary>
    /// <returns>yyyy-MM-dd HH:mm:ss</returns>
    public static string GetTime()
    {
        return (DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
    }
    /// <summary>
    /// Checks if all characters are ASCII, between length 5-49, and that there are no SQL-based keywords
    /// </summary>
    /// <param name="inputted"></param>
    /// <returns>0 - Success, 1 - SQL issue, 2 - Not between 5-49 characters, 3 - Not in ASCII set</returns>
    public static int StandardCheck(string inputted)
    {
        if (ValidationASII(inputted) == true)
        {
            if (Validation50Chars(inputted) == true)
            {
                if (ValidationSQLProof(inputted) == true)
                {
                    return 0;
                }
                else return 1;
            }
            else return 2;
        }
        else return 3;
    }
    /// <summary>
    /// Checks if all characters are ASCII, between length 5-49, and that there are no SQL-based keywords
    /// </summary>
    /// <param name="inputted"></param>
    /// <returns>0 - Success, 1 - SQL issue, 2 - Not between 5-49 characters, 3 - Not in ASCII set, 4 - Username already exists,
    ///  5 - username already exists in DeletedAccounts
    /// </returns>
    public static int SignUpUsernameCheck(string inputted)
    {
        if (ValidationASII(inputted) == true)
        {
            if (Validation50Chars(inputted) == true)
            {
                if (ValidationSQLProof(inputted) == true)
                {
                    if (CheckUsernameExists(inputted) == false)
                    {
                        if (CheckUsernameExistsInAttempts(inputted) == false)
                        {
                            return 0;
                        }
                        else return 5;
                    }
                    else return 4;
                }
                else return 1;
            }
            else return 2;
        }
        else return 3;
    }
    /// <summary>
    /// Used upon sign up function and deletes attempts stored under an already used username. Stores deletion data
    /// </summary>
    /// <param name="UserID">Username (sanitised before entering here)</param>
    public static void DeleteRelevantAttempts(string UserID)
    {
        SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

        builder.DataSource = DatabaseOptions.dataSource; //database login information
        builder.UserID = "DeleteUser";
        builder.Password = "password";
        builder.InitialCatalog = DatabaseOptions.initialCatalog;
        builder.TrustServerCertificate = true;
        string DateTime = GetTime();

        using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
        {
            connection.Open();

            String query = $"DELETE FROM LoginAttemptInfo WHERE UserID = @user"; //query
            String query2 = $"INSERT INTO DeletedAttempts VALUES (@user, @dt)"; //writes into DeletedAttempts file

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@user", UserID);
                command.ExecuteNonQuery(); //executes a non-returning query (deleting here)
            }

            using (SqlCommand command2 = new SqlCommand(query2, connection)) //executes 2nd query
            {
                command2.Parameters.AddWithValue("@user", UserID);
                command2.Parameters.AddWithValue("@dt", DateTime);
                command2.ExecuteNonQuery();
            }
        }
    }
    #endregion

    #region Sign Up and Login functions
    /// <summary>
    /// Once used, user is locked into signing up until it enters valid entries. This function is not for actual use in the final program.
    /// Stores values into database.
    /// </summary>
    public static void SignUp()
    {
        Dictionary<int, string> Problems = new Dictionary<int, string>(); //defines error codes from check functions
        Problems.Add(1, "Invalid SQL-based phrases used");
        Problems.Add(2, "Not in length 5-49 inclusive");
        Problems.Add(3, "Character(s) not in ASCII set");
        Problems.Add(4, "Username already exists");
        Problems.Add(5, "Username already exists inside of DeletedAccounts");
        string wantedUsername = "";
        string wantedPassword = "";

        bool successUsername = false;
        while (!successUsername)
        {
            Console.WriteLine("Username: ");
            wantedUsername = Console.ReadLine();
            if (SignUpUsernameCheck(wantedUsername) != 0)
            {
                Console.WriteLine(Problems[SignUpUsernameCheck(wantedUsername)]);
            }
            else
            {
                successUsername = true;
            }
        }

        bool successPassword = false;
        while (!successPassword)
        {
            Console.WriteLine("Password: ");
            wantedPassword = Console.ReadLine();
            if (StandardCheck(wantedPassword) != 0)
            {
                Console.WriteLine(Problems[SignUpUsernameCheck(wantedPassword)]);

            }
            else
            {
                successPassword = true;
            }
        }
        string hashedPassword = HashAndSalt(wantedUsername, wantedPassword);
        StoreUserInfo(wantedUsername, hashedPassword);
        StoreAttemptInfo(wantedUsername, GetTime(), 1);

        if (CheckUsernameExistsInAttempts(wantedUsername) == true)
        {
            DeleteRelevantAttempts(wantedUsername);
        }
    }

    /// <summary>
    /// Doesn't lock the user in, forces both entries for username and password.
    /// Can return attempts below 5 in format "1[attempts]".
    /// Can return error codes.
    /// Will return string for username once logged in otherwise.
    /// Deletes account if 5 attempts failed consecutively.
    /// </summary>
    /// <returns>
    /// 00 - Invalid input, 01 - Username doesn't exist,
    /// 10 - Deleted account
    /// 1[number] - number=consecutive invalid attempts,
    /// or returns username once logged in
    /// </returns>
    public static string Login()
    {
        Console.WriteLine("Username: ");
        string inputUsername = Console.ReadLine();
        Console.WriteLine("Password: ");
        string inputPassword = Console.ReadLine();

        if (StandardCheck(inputUsername) != 0 || StandardCheck(inputPassword) != 0)
        {
            return ("00"); //invalid input (contains ASCII/invalid length/contains SQL phrases)
        }

        if (CheckUsernameExists(inputUsername) == true)
        {
            string hashedPassword = HashAndSalt(inputUsername, inputPassword);
            string comparisonPassword = GetPassword(inputUsername);

            if (hashedPassword == comparisonPassword)
            {
                StoreAttemptInfo(inputUsername, GetTime(), 1); //stores attempt in DB
                return inputUsername;
            }
            else
            {
                StoreAttemptInfo(inputUsername, GetTime(), 0); //stores attempt in DB
                if (GetConsecutiveWrong(inputUsername) >= 5)
                {
                    DeleteAccount(inputUsername);
                    return ("10");
                }
                else
                {
                    return ("1" + GetConsecutiveWrong(inputUsername).ToString());
                    //logically wont surpass 4, but relies on clearing of DB upon account deletion
                }
            }
        }
        else
        {
            StoreAttemptInfo(inputUsername, GetTime(), 0); //Stores attempt in DB
            return ("01");
        }
    }

    #endregion
}