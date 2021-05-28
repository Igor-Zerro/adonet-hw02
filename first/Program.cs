using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Microsoft.Extensions.Configuration;
using NickBuhro.Translit;

namespace first
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = ReadConnectionString();
            using (SqlConnection connection =
                new SqlConnection(connectionString))
                // new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=demo2021;Integrated Security=True"))
            {
                connection.Open();

                SqlCommand selectRoleCount = connection.CreateCommand();
                selectRoleCount.CommandText = "SELECT COUNT(*) FROM Roles";
                object roleCount = selectRoleCount.ExecuteScalar();
                Console.WriteLine($"Total row count: {roleCount}");

                if (roleCount == null || (int)roleCount == 0)
                {
                    string user = "UserTest";
                    string admin = "AdminTest";
                    string km = "KontentMenagerTest";
                    SqlCommand initInsertRole = connection.CreateCommand();
                    SqlParameter firstInsertParamUser = new SqlParameter("@UserTest", SqlDbType.VarChar);
                    firstInsertParamUser.Value = user;
                    SqlParameter firstInsertParamAdmin = new SqlParameter("@AdminTest", SqlDbType.VarChar);
                    firstInsertParamAdmin.Value = user;
                    SqlParameter firstInsertParamKM = new SqlParameter("@KontentMenagerTest", SqlDbType.VarChar);
                    firstInsertParamKM.Value = user;
                    initInsertRole.CommandText =
                        "INSERT INTO [dbo].[Roles] ([name]) VALUES (@UserTest), (@AdminTest), (@KontentMenagerTest);";
                    initInsertRole.Parameters.Add(firstInsertParamUser);
                    initInsertRole.Parameters.Add(firstInsertParamAdmin);
                    initInsertRole.Parameters.Add(firstInsertParamKM);
                    int insertedRoleCount = initInsertRole.ExecuteNonQuery();
                    Console.WriteLine($"Total inserted row count: {insertedRoleCount}");
                }
                ShowRoles(connection);
            }
        }
        private static string ReadConnectionString()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("Konfig.json", optional: false);

            IConfiguration config = builder.Build();
            return config.GetSection("ConnectionStrings").GetSection("sqlStrocSoidRoles").Value;
        }
        private static void ShowRoles(SqlConnection connection)
        {
            SqlCommand selectRoles = connection.CreateCommand();
            selectRoles.CommandText = "SELECT * FROM Roles ORDER BY id";
            using (SqlDataReader reader =
                selectRoles.ExecuteReader())
            {
                int fieldCount = reader.FieldCount;
                for (int i = 0; i < fieldCount; i++)
                {
                    Console.Write(reader.GetName(i) + " ");
                }
                Console.WriteLine();
                while (reader.Read())
                {
                    Console.WriteLine(reader.GetInt32(0) + " " + reader.GetString(1));
                    foreach (Char item in reader.GetString(1))
                    {
                        // Console.WriteLine(item + 0);
                        // Console.WriteLine((ushort)item);
                        if ((ushort)item >= 1000)
                        {
                            Console.WriteLine(Transliteration.CyrillicToLatin(reader.GetString(1), Language.Russian));
                            break;
                        }
                    }
                }
            }
        }
    }
}