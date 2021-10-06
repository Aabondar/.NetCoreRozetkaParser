using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace ParserCore
{
    static class DatabaseManager
    {
        private static string connectionString = null; //Строка соединения с базой данных
        private const string configFileName = @"\ConnectionString.txt";
        private const string configFileDefaultText = @"Connection String = ";

        public static void InitializeConnectionString() //Создаем конфигурационный файл для connection string-а, в случае если его нет
        {
            try
            {
                if (!File.Exists(Environment.CurrentDirectory + configFileName)) //Создаем файл по шаблону, сообщаем пользователю о необходимости ввести connection string и закрываем приложение
                {
                    File.WriteAllText(Environment.CurrentDirectory + configFileName, configFileDefaultText);
                    Console.WriteLine("File 'ConnectionString.txt' created in the application root directory, but it still has to be filled, please.");
                    Environment.Exit(-2);
                }
                else //Если connection string в файле не пустой, извлекаем его. Иначе сообщаем о проблеме и выходим из приложения
                {
                    try
                    {
                        string strBuffer;
                        var file = new StreamReader(Environment.CurrentDirectory + configFileName);
                        strBuffer = file.ReadLine();
                        strBuffer = strBuffer.Remove(0, configFileDefaultText.Length - 1);
                        connectionString = strBuffer.Trim();
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Proper work of the application requires You to set the SQL server connection string in the file 'ConnectionString.txt' please.");
                        Environment.Exit(-1);
                    }

                    if (connectionString == null || connectionString == string.Empty)
                    {
                        Console.WriteLine("Proper work of the application requires You to set the SQL server connection string in the file 'ConnectionString.txt' please.");
                        Environment.Exit(-1);
                    }
                }
            }
            catch (IOException)
            {
                Console.WriteLine("Failed to create or access file 'ConnectionString.txt'");
            }
        }
        public static void CreateDB() //Метод для создания локальной базы данных с пустыми таблицами и хранимыми процедурами
        {
            string queryString = "IF NOT EXISTS(SELECT * FROM sys.databases WHERE NAME = 'AutoSpareParts') CREATE DATABASE AutoSpareParts";

            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader(); //Создаем базу данных AutoSpareParts, если она не была создана ранее
                connection.Close();

                queryString = @"
                USE AutoSpareParts
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Parts') 
                CREATE TABLE Parts(Id int IDENTITY(1,1) NOT NULL PRIMARY KEY, URL nvarchar(99), VenCode nvarchar(30), Brand nvarchar(30), Name nvarchar(30))
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'LinkParts')
                CREATE TABLE LinkParts(Id int IDENTITY(1,1) NOT NULL PRIMARY KEY, PartsFk int NOT NULL, Name nvarchar(30), Number nvarchar(30), Type nvarchar(30))
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE TYPE = 'P' AND NAME = 'InsertToParts')
                EXEC('
                CREATE PROCEDURE InsertToParts
                @url nvarchar(99),
                @vencode nvarchar(30),
                @brand nvarchar(30),
                @name nvarchar(30)
                AS
                BEGIN
                INSERT INTO Parts
                (URL, VenCode, Brand, Name)
                VALUES
                (@url, @vencode, @brand, @name)
                END
                ')
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE TYPE = 'P' AND NAME = 'InsertToLinkParts')
                EXEC('
                CREATE PROCEDURE InsertToLinkParts
                @name nvarchar(30),
                @number nvarchar(30),
                @type nvarchar(30)
                AS
                BEGIN
                INSERT INTO LinkParts
                (PartsFk, Name, Number, Type)
                VALUES
                ((SELECT TOP 1 Id FROM Parts ORDER BY Id DESC), @name, @number, @type)
                END
                ')
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE TYPE = 'P' AND NAME = 'ReadFromDB')
                EXEC('
                CREATE PROCEDURE ReadFromDB
                AS
                BEGIN
                SELECT Vencode, Brand, Parts.Name, Number, LinkParts.Name AS LinkedName
                FROM Parts
                JOIN LinkParts
                ON Parts.Id = LinkParts.PartsFk
                END
                ')
                ";

                command = new SqlCommand(queryString, connection);
                connection.Open();
                reader = command.ExecuteReader(); //Создаем таблицы и хранимые процедуры в базе, если они не были созданы ранее
                connection.Close();
            }
            catch (SqlException)
            {
                Console.WriteLine("Database access error. Please check if the connection string is properly installed.");
            }
        }
        public static void WriteToPartsTable(string URL, string venCode, string brand, string name) //Метод для вызова процедуры записи в таблицу Parts
        {
            string queryString = @"USE AutoSpareParts
                                   EXEC InsertToParts @url, @vencode, @brand, @name";

            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                command.Parameters.Add("@url", SqlDbType.NVarChar).Value = URL;
                command.Parameters.Add("@vencode", SqlDbType.NVarChar).Value = venCode;
                command.Parameters.Add("@brand", SqlDbType.NVarChar).Value = brand;
                command.Parameters.Add("@name", SqlDbType.NVarChar).Value = name;
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (SqlException)
            {
                Console.WriteLine("Database access error. Please check if the connection string is properly installed.");
            }
        }
        public static void WriteToLinkPartsTable(string name, string number, string type) //Метод для вызова процедуры записи в таблицу LinkParts
        {
            string queryString = @"USE AutoSpareParts
                                   EXEC InsertToLinkParts @name, @number, @type";

            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                command.Parameters.Add("@name", SqlDbType.NVarChar).Value = name;
                command.Parameters.Add("@number", SqlDbType.NVarChar).Value = number;
                command.Parameters.Add("@type", SqlDbType.NVarChar).Value = type;
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (SqlException)
            {
                Console.WriteLine("Database access error. Please check if the connection string is properly installed.");
            }
        }
    }
}
