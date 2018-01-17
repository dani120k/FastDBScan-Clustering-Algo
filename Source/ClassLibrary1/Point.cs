using System;
using System.Collections.Generic;

namespace MyLib
{
    /// <summary>
    /// Класс для точки на плоскости
    /// </summary>
    public class Point
    {
        public double x, y; //абцисса и ордината
        public int cluster; //номер кластера
        public int db = -1;
        public bool IsVisited = false;
        public bool minPTScheck = false;
        public double xnorm, ynorm; //нормализованные координаты
        public int idx;
        public List<Point> findpoint;

        /// <summary>
        /// Конструктор с параметрами
        /// </summary>
        /// <param name="x">Абцисса</param>
        /// <param name="y">Ордината</param>
        public Point(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public Point(double x, double y, int idx)
        {
            this.x = x;
            this.y = y;
            this.idx = idx;
        }

        /// <summary>
        /// Метод, расчитывающий расстояние между двумя точками
        /// </summary>
        /// <param name="first">Первая точка</param>
        /// <returns>Возвращает вещественное расстояние между точкой и заданной</returns>
        public double Distance(Point first)
        {
            return Math.Sqrt(Math.Pow(first.x - this.x, 2) + Math.Pow(first.y - this.y, 2));
        }
    }
}
