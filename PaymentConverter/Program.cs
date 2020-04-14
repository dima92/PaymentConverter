using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace PaymentConverter
{
    class Program
    {
        // метод принимает  JSON ответ и возвращает коллекцию строк {}
        static List<string> ParseJsonString(string jsonStr)
        {
            List<string> arrObj = new List<string>();

            for (int i = 0; i < jsonStr.Length; i++)
            {
                if (jsonStr[i] == '{')
                {
                    string strTmp = null;
                    for (int j = 0; jsonStr[i] != '}'; i++, j++)
                    {
                        strTmp += jsonStr[i].ToString();
                    }
                    strTmp += "}";
                    arrObj.Add(strTmp);
                }
            }
            return arrObj;
        }

        // поиск самого длинного поля среди полей
        static List<int> LongPosString(List<Currency> myCur)
        {
            List<int> legthsCol = new List<int>();
            int length = 0;

            legthsCol.Add(4); // добавляю 4 символа Cur_ID
            legthsCol.Add(11); // добавляю 11 символа Date
            legthsCol.Add(4); // добавляю 4 символа Cur_Abbreviation

            for (int i = 0; i < myCur.Count; i++)
            {
                if (myCur[i].Cur_Scale.ToString().Length > length)
                {
                    length = myCur[i].Cur_Scale.ToString().Length;
                }
            }
            legthsCol.Add(length); // добавляю x символа Cur_Scale

            length = 0;

            for (int i = 0; i < myCur.Count; i++)
            {
                if (myCur[i].Cur_Name.Length > length)
                {
                    length = myCur[i].Cur_Name.Length;
                }
            }
            legthsCol.Add(length);

            legthsCol.Add(8); // добавляю 8 символа Cur_OfficialRate
            return legthsCol; // возвращаю коллекцию длин полей
        }

        // вывод на экран
        static void ShowCurRate(List<Currency> myCur, List<int> colecеtColumnLength)
        {
            Console.WriteLine("Курсы валют НБ РБ установленные на сегодня: " + DateTime.Now.ToShortDateString());

            // печатается подчеркивающая линия
            int f1_len = colecеtColumnLength[0] + colecеtColumnLength[1] + colecеtColumnLength[2] + colecеtColumnLength[3] + colecеtColumnLength[4] + colecеtColumnLength[5] + 17;

            for (int i = 0; i < f1_len; i++)
            {
                Console.Write("-");
            }
            Console.WriteLine();

            // печатается набор данных курсов
            foreach (var i in myCur)
            {
                // формат-шаблон строки, сначала передается в позицию количество символов на строку
                string _template = String.Format("[{{0, {0}}}] [{{1, {1}}}] [{{2, {2}}}] [{{3, {3}}}] [{{4, {4}}}] [{{5, {5}}}]", colecеtColumnLength[0], colecеtColumnLength[1], colecеtColumnLength[2], colecеtColumnLength[3], colecеtColumnLength[4], colecеtColumnLength[5]);
                // в формат-шаблон подствляются данные с заранее установленной шириной строки
                Console.WriteLine(_template, i.Cur_ID, i.Date.ToShortDateString(), i.Cur_Abbreviation, i.Cur_Scale, i.Cur_Name, i.Cur_OfficialRate);
            }
        }



        static void Main(string[] args)
        {
            WebRequest request = WebRequest.Create("http://www.nbrb.by/API/ExRates/Rates?Periodicity=0");

            WebResponse response = request.GetResponse();

            string jsonStr = null; // строка json массив

            using (Stream stream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(stream);
                jsonStr = reader.ReadToEnd();
                reader.Close();
            }

            // парс ответа
            List<string> colectJS = new List<string>();
            colectJS.AddRange(ParseJsonString(jsonStr));

            // конвертирую json в объект Currency
            List<Currency> myCur = new List<Currency>();
            for (int i = 0; i < colectJS.Count; i++)
            {
                // для каждой строки в коллекции colectJS
                Currency c = JsonConvert.DeserializeObject<Currency>(colectJS[i]);
                // добавляю в коолекцию объектов Currency
                myCur.Add(c);
            }

            // определется самое длинное полей
            List<int> colecеtColumnLength = new List<int>();
            colecеtColumnLength.AddRange(LongPosString(myCur));

            // вызов метода - вывод на печат
            ShowCurRate(myCur, colecеtColumnLength);

            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }
    }
}
