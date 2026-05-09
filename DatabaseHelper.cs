using System;
using Microsoft.Data.SqlClient;

namespace LanguageExamApp
{
    public class DatabaseHelper
    {
        // 🚨 ATTENTION TEAMMATE: Update this string with your Windows SQL Server details!
        private string connectionString = @"Server=DESKTOP-BUGKGO7\SQLEXPRESS; Database=LanguageExamDB; TrustServerCertificate=True; Integrated Security=True;";

        public SqlConnection GetConnection() // Its gonna be used every time another part of the app needs to communicate with the database
        {
            return new SqlConnection(connectionString); // This is the part it returns from tghe connectionString hence the database, and it will get the ready to use SqlConnection
        }
    }
}