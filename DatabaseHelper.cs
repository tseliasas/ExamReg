using System;
using Microsoft.Data.SqlClient; // changed to MIcrosoft.Data instead of System.Data for better cross platform support bc Im on linux

namespace LanguageExamApp
{
    public class DatabaseHelper
    {
        // yahya make sure you replace this with whatever your working string is right here
        private string connectionString = @"Server=DESKTOP-BUGKGO7\SQLEXPRESS; Database=LanguageExamDB; TrustServerCertificate=True; Integrated Security=True;";

        public SqlConnection GetConnection() // Its gonna be used every time another part of the app needs to communicate with the database
        {
            // debugging: added this so we can see in the terminal when exactly the app tries to talk to windows
            Console.WriteLine("[Network Log] Attempting to connect to SQL Server");
            
            return new SqlConnection(connectionString); // This is the part it returns from tghe connectionString hence the database, and it will get the ready to use SqlConnection
        }
    }
}