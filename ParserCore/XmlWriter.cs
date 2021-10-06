using System;
using System.IO;
using System.Text.RegularExpressions;

namespace ParserCore
{
    static class XmlWriter //Класс для записи таблиц характеристик в XML файл
    {
        public static void CreateDirForXml()
        {
            try
            {
                Directory.CreateDirectory(Environment.CurrentDirectory + @"\XML");
            }
            catch (IOException exc)
            {
                Console.WriteLine("I/O Error:\n" + exc.Message);
            }
        }
        public static void SaveToXml(string xmlData, string url)
        {
            const string regExpPattern = @"NN/[^/]+";
            Match match = Regex.Match(url, regExpPattern);
            string xmlName = match.Value.Remove(0, 3);

            try
            {
                if (!File.Exists(Environment.CurrentDirectory + @"\XML\" + xmlName + ".xml")) //Проверяем, нет ли такого файла
                    File.WriteAllText(Environment.CurrentDirectory + @"\XML\" + xmlName + ".xml", xmlData);   //Сохраняем данные в xml файл
            }

            catch (IOException exc)
            {
                Console.WriteLine("I/O Error:\n" + exc.Message);
            }
        }
    }
}
