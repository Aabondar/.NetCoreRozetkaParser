using System;
using System.Collections.Generic;
using System.Text;

namespace ParserCore
{
    class Starter
    {
        private ConsoleKeyInfo cki = default(ConsoleKeyInfo);
        private string URL = @"https://hard.rozetka.com.ua/computers/c80095/";

        public void Exit() //Выход из приложения
        {
            Environment.Exit(0);
        }
        public void Start()   //Временный метод управления запуском приложения или выходом из него
        {
            Console.WriteLine("Press Enter to start or Escape to exit");
            do
            {
                if (Console.KeyAvailable)
                {
                    if (Console.KeyAvailable)
                        cki = Console.ReadKey(true);

                    if (cki.Key == ConsoleKey.Escape)
                        Exit();
                }
            }
            while (cki.Key != ConsoleKey.Enter);

            //DatabaseManager.InitializeConnectionString(); //Внимание! После первого запуска приложения будет создан файл ConnectionString.txt (если его не было),                       
            //DatabaseManager.CreateDB();                   //в котором надо указать строку соединения с базой данных
            //FileParser fp = new FileParser();
            WebPageParser wpp = new WebPageParser(); //Создаем объект парсера файлов и парсера web страниц
            wpp.ParseWebPage(URL);
            wpp.PrintMatches();

            //XmlWriter.CreateDirForXml();
        } 
    }
}
