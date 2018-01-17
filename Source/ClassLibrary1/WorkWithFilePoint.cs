using System.Collections.Generic;
using System.IO;
using System;
using System.Globalization;

namespace MyLib
{

    /// <summary>
    /// Класс для работы с файлами
    /// </summary>
    public static class WorkWithFile
    {
        public static int xmin, xmax, ymin, ymax;
        static int number_of_stroke = 1;
        static int count;

        /// <summary>
        /// Статические метод, который получает лист точек из файла
        /// </summary>
        /// <returns>Возвращает лист точек типа Point</returns>
        public static List<Point> GetPointFromFile(string path)
        {
            xmax = int.MinValue;
            xmin = int.MaxValue;
            ymin = int.MaxValue;
            ymax = int.MinValue;
            FileStream stream = new FileStream(path, FileMode.Open); //открываем файл
            StreamReader reader = new StreamReader(stream); //поток для считывания точек
            string s = "";
            List<Point> answer = new List<Point>(0);
            do
            {
                s = reader.ReadLine();
                try
                {
                    if (s != "" && s != null) answer.Add(GetPointFromString(s));
                }
                catch(ArgumentException ex)
                {
                    reader.Close();
                    stream.Close();
                    throw new ArgumentException(ex.Message);
                }
            } while (s != null);
            reader.Close();
            stream.Close();
            return answer;
        }

        /// <summary>
        /// Метод получающий строку, возвращающая точку
        /// </summary>
        /// <param name="s">Строка</param>
        /// <returns>Возвращает точку из двух вещественных чисел из строки</returns>
        static Point GetPointFromString(string s)
        {
            double x, y;
            string[] words = s.Split(' '); //получили массив слов, которые были разделены пробелами
            if (words.Length < 2) words = s.Split('\t');
            string[] answer = new string[2]; //массив для двух вещественных чисел
            try
            {
                int j = 0;
                for (int i = 0; i < words.Length; i++)
                    if (words[i].Length > 0) { answer[j] = words[i]; j++; }

                x = double.Parse(answer[0].Replace('.', ','));
                y = double.Parse(answer[1].Replace('.', ','));
                number_of_stroke++;
            }
            catch
            {
                throw new ArgumentException("Ошибка во входных данных, номер строки: " + number_of_stroke + "\n"+"Cтрока:" + s);
            }
            if (x > xmax) xmax = (int)x;
            if (x < xmin) xmin = (int)x;
            if (y > ymax) ymax = (int)y;
            if (y < ymin) ymin = (int)y;
            return new Point(x, y); //запарсили два вещественных числа и вернули точку, заданную такими координатами
        }
    }
}
