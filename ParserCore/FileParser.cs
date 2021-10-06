using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace ParserCore
{
    class FileParser  //Класс для парсинга файлов
    {
        private List<string> refs; //Сюда будем писать ссылки с распаршенными кодами
        private const string refPrefix = "https://otto-zimmermann.com.ua/autoparts/product/ZIMMERMANN/"; // Этот префикс будем прибавлять к распаршенным кодам

        public List<string> ParseFile(string fileName) //Метод для парсинга файла
        {
            string strBuffer;
            StreamReader file = null;
            refs = new List<string>();
            const string regExpPattern = @"\d{7}"; //Паттерн регулярного выражения
            var regExp = new Regex(regExpPattern); //Само регулярное выражение

            try
            {
                file = new StreamReader(Environment.CurrentDirectory + "\\" + fileName); //Инициализируем StreamReader, указываем путь к файлу

                int codeCounter = 0; //Счетчик распаршенных кодов

                while ((strBuffer = file.ReadLine()) != null) //Читаем строки из файла
                {
                    if (regExp.IsMatch(strBuffer)) //Проверяем каждую считанную из файла строку на соответствие регулярному выражению, чтобы найти коды
                    {
                        strBuffer = strBuffer.Trim(); //Перед кодами остались пробелы, убираем их
                        refs.Add(refPrefix + strBuffer + @"/"); //К кодам добавляем префикс и сохраняем в List в виде готовых ссылок
                        codeCounter++;
                    }
                }
                Console.WriteLine("\n" + codeCounter + " codes were successfully formed into references.");
                return refs;
            }

            catch (IOException)
            {
                Console.WriteLine("Для работы приложения файл 'SourceCodes.txt' с исходными кодами для парсинга должен находиться в корневой папке приложения!");
                Environment.Exit(-4);
            }

            finally
            {
                if (file != null) file.Close(); //Не забываем закрыть поток
            }
            return refs;
        }
    }
}

