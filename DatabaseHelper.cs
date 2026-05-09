using System;
using Microsoft.Data.SqlClient;

namespace LanguageExamApp
{
    public class DatabaseHelper
    {
        // Change this with the exact details to the database youre going to connect it to
        private string connectionString = "Server=YOUR_SERVER_NAME; Database=LanguageExamDB; TrustServerCertificate=True; Integrated Security=True;"; // This is the map and password to the database hence gotta be private ish ig?

        public SqlConnection GetConnection() // Its gonna be used every time another part of the app needs to communicate with the database
        {
            return new SqlConnection(connectionString); // This is the part it returns from tghe connectionString hence the database, and it will get the ready to use SqlConnection
        }
    }
}