using System;
using System.Collections.Generic;

namespace Lib4rtree
{
    /// <summary>
    /// Класс с основными компонентами в виде структур
    /// </summary>
    public class ForRstartree
    {
        public static int MAX_M = 100; // Максимальное количество объектов в узле
        public static int MIN_M = (int)(MAX_M * 0.4); // Минимальное количество объектов в узле

        public enum TAxis { X, Y };// для chooseSplitAxis - по какой оси будет разделение

        protected enum TBound { Left, Right };// граница по какой будет идти сортировка (левая\правая)

        public struct Point// структура описывающая точку
        {
            public Point(double x, double y)
            {
                X = x;
                Y = y;
            }
            public double X, Y;
        }

        public struct TMBR// структура описывающая область покрытия узла MBR = Minimum Bounding Rectangle
        {
            public TMBR(Point l, Point r)
            {
                Left = l;
                Right = r;
            }
            public Point Left, Right;// left - координаты верхнего левого угла right - координаты нижнего правого угла
        }
        

    }
}
