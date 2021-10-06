using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using System.Net.Http;

namespace ParserCore
{
    class WebPageParser //Класс для парсинга web страниц
    {
        private List<string> refs;
        private int pageCounter = 1;
        private int refCounter = 0;
        private string curRefer;
        HttpClient client;

        public void PrintMatches()
        {
            Console.WriteLine(refCounter + " matches found!");
        }

        public async void ParseWebPage(string refer)
        {
            string result;
            int cutIndex;
            refs = new List<string>();

            if (pageCounter == 1)
                curRefer = refer;

            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(curRefer);
            client = new HttpClient();
            
            
            //request.Method = "GET";

            // поискать библиотеки для работы с json
            try
            {
                HttpResponseMessage response = await client.GetAsync(curRefer);
                response.EnsureSuccessStatusCode();
                //using (var response = request.GetResponse())
                //using (var stream = response.GetResponseStream())
                //using (var reader = new StreamReader(stream))
                
                {
                    //string webPageBuffer = reader.ReadToEnd(); //Переменная для хранения веб страницы
                    string webPageBuffer = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(webPageBuffer);
                    //string searchPattern; //Поисковый паттерн регулярного выражения

                    var searchPattern = @"https://hard.rozetka.com.ua/(\S*?)img";
                    Match match = Regex.Match(webPageBuffer, searchPattern);

                    if (match.Success)
                    {
                        result = match.Value;
                        cutIndex = result.LastIndexOf("/");
                        result = result.Substring(0, cutIndex);
                        refs.Add(result);
                        Console.WriteLine(result);
                        refCounter++;
                        pageCounter++;
                    }

                    while (match.Success)
                    {
                        result = match.NextMatch().Value;
                        if (result == "")
                            break;
                        cutIndex = result.LastIndexOf("/");
                        result = result.Substring(0, cutIndex);
                        Console.WriteLine(result);
                        match = match.NextMatch();
                        refCounter++;
                    }
                }

                while (pageCounter < 5) //Временно установим статичную величину 25, позднее будем сверять не повторяется ли самый первый товар из первой страницы ?
                {
                    curRefer = refer + "/page=" + pageCounter;
                    ParseWebPage(curRefer);
                }
                 
            }
            catch (WebException exc)
            {
                Console.WriteLine(exc.Message);
                Console.WriteLine(exc.Response);
                Console.WriteLine("Запрашиваемая URL не отвечает!");
                Environment.Exit(-5);
            }
        }
    }
}