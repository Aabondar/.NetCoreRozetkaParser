using System;
using System.Collections.Generic;
using System.IO;

namespace ParserCore
{
    static class Logger
    {
        private static string fileName = Environment.CurrentDirectory + "\\" + "parser.log";

        public static string fileNameGetter() { return fileName; }
        public static void ClearLogFile() //Метод нигде не задействован. Пусть будет на случай, если захотим очистить лог
        {
            try
            {
                File.Create(fileName);
            }

            catch (IOException exc)
            {
                Console.WriteLine("I/O Error:\n" + exc.Message);
            }
        }

        public static void DoLog(string URL, string status)
        {
            var logData = new List<string>(); //List с URL, статусом и датой, который мы запишем в log файл
            logData.Add("URL: " + URL + " - " + status);
            logData.Add("Date: " + DateTime.Now + "\n");

            try
            {
                File.AppendAllLines(fileName, logData); //Дописываем полученные данные в конец файла      
            }

            catch (IOException exc)
            {
                Console.WriteLine("I/O Error:\n" + exc.Message);
            }
        }
    }
}
