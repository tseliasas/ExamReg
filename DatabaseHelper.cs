using System;
using Microsoft.Data.SqlClient;

namespace LanguageExamApp
{
    public class DatabaseHelper
    {
        // 🚨 ATTENTION TEAMMATE: Update this string with your Windows SQL Server details!
        private string connectionString = @"Server=DESKTOP-BUGKGO7\SQLEXPRESS; Database=LanguageExamDB; TrustServerCertificate=True; Integrated Security=True;";

        public SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}