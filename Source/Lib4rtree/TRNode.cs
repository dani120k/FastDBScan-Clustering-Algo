using System;
using System.Collections.Generic;

namespace Lib4rtree
{
    public class TRNode : ForRstartree
    {

        public TMBR fmbr;// ограничивающий прямоугольник узла
        public int FParent;// индекс в массиве узлов дерева, указывающий на узел-родитель
        public int[] FChildren = new int[0]; // список индексов дочерних детей в массиве узлов дерева
        public MyLib.Point[] FObject = new MyLib.Point[0];// массив с обьектами 
        public bool FisLeaf;// свойство показывающее является ли этот узел конечным(листом)
        public int FLevel;// уровень узла в дереве (0=лист)
        public bool IsVisited = false;

        /// <summary>
        /// Есть ли дети у узла
        /// </summary>
        /// <returns></returns>
        public bool getIsLeaf()
        {
            if (FObject.Length > 0) return true; else return false;
        }

        /// <summary>
        /// метод доступа дочерним узлам
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public int getChild(int index)
        {
            if (FChildren.Length > index) return FChildren[index]; else throw new ArgumentOutOfRangeException("Попытка получения несуществующего объекта");
        }

        /// <summary>
        /// метод доступа к обьектам в узле
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public MyLib.Point getObject(int index)
        {
            if (index < FObject.Length && index >= 0) return FObject[index]; else throw new ArgumentOutOfRangeException("Попытка получения несуществующего объекта");
        }

        /// <summary>
        /// метод присваивания дочернего узла
        /// </summary>
        /// <param name="index"></param>
        /// <param name="node_id"></param>
        public void setChild(int index, int node_id)
        {
            if (FChildren.Length > index && index >= 0)
            {
                //  Console.WriteLine("index "+index +" node_id " + node_id);
                FChildren[index] = node_id;
                FisLeaf = false;
            }
            else
            {
                if (index <= (MAX_M - 1) && index >= 0)
                {
                    // Console.WriteLine("index " + index + " node_id " + node_id);
                    Array.Resize(ref FChildren, index + 1);
                    FChildren[index] = node_id;
                    FisLeaf = false;
                }
            }
        }


        /// <summary>
        ///  метод присваивания обьекта узлу
        /// </summary>
        /// <param name="index"></param>
        /// <param name="obj"></param>
        public void setObject(int index, MyLib.Point obj)
        {
            if (FObject.Length > index && index >= 0)
            {
                FObject[index] = obj;
                FisLeaf = true;
            }
            else
            {
                if (index <= MAX_M - 1 && index >= 0)
                {
                    Array.Resize(ref FObject, index + 1);
                    FObject[index] = obj;
                    FisLeaf = true;
                }
            }

        }

        /// <summary>
        /// метод присваивания узла-родителя
        /// </summary>
        /// <param name="parent_id"></param>
        protected void setParent(int parent_id)
        {
            if (parent_id >= 0) FParent = parent_id;
        }

        /// <summary>
        /// Копирование
        /// </summary>
        /// <param name="node"></param>
        public void copy(TRNode node)// метод копирования узла
        {


            FObject = new MyLib.Point[node.FObject.Length];
            if (FObject.Length > 0)
            {

                for (int i = 0; i < node.FObject.Length; i++)
                {
                    FObject[i] = new MyLib.Point(node.FObject[i].x, node.FObject[i].y, node.FObject[i].idx);
                    FObject[i].cluster = node.FObject[i].cluster;
                }
                FisLeaf = true;
            }
            else
            {
                FChildren = new int[node.FChildren.Length];
                for (int i = 0; i < node.FChildren.Length; i++)
                    FChildren[i] = node.FChildren[i];
                FisLeaf = false;
            }


            fmbr = new TMBR(node.fmbr.Left, node.fmbr.Right);

            FParent = node.FParent;
            FLevel = node.FLevel;
        }

        /// <summary>
        /// Очишает массив объектов
        /// </summary>
        public void clearObjects()
        {
            FisLeaf = false;
            FObject = new MyLib.Point[0];
        }

        public void clearChildren()
        {
            FChildren = new int[0];
        }

        /// <summary>
        /// Конструктор умолчания
        /// </summary>
        public TRNode()
        {
            FParent = -10;
        }

        /// <summary>
        /// Конструктор копирования
        /// </summary>
        /// <param name="node"></param>
        public TRNode(TRNode node)
        {
            FParent = -10;
            copy(node);
        }

        /// <summary>
        /// свойство предоставляющее доступ к полям через соответствующие методы
        /// </summary>
        public TMBR mbr
        {
            get
            {
                return fmbr;
            }
            set
            {
                fmbr = value;
            }
        }

        /// <summary>
        /// Является ли узел листом
        /// </summary>
        public bool isLeaf
        {
            get
            {
                return FisLeaf;
            }
        }

        public int Parent
        {
            get
            {
                return FParent;
            }
            set
            {
                setParent(value);
            }
        }

        public int Level
        {
            get
            {
                return FLevel;
            }
            set
            {
                FLevel = value;
            }
        }

        /// <summary>
        /// метод определяющий пересекаются ли две области mbr1-объект и mbr2 - область поиска
        /// </summary>
        /// <param name="mbr1">Объект</param>
        /// <param name="mbr2">Область</param>
        /// <returns></returns>
        public static bool isIntersected(TMBR mbr1, TMBR mbr2)
        {
            double d1, d2, d3, d4;
            d1 = Math.Min(mbr1.Left.X, mbr1.Right.X);
            d2 = Math.Max(mbr1.Left.Y, mbr1.Right.Y);
            d3 = Math.Max(mbr1.Left.X, mbr1.Right.X);
            d4 = Math.Min(mbr1.Left.Y, mbr1.Right.Y);

            mbr1.Left.X = d1;
            mbr1.Left.Y = d2;
            mbr1.Right.X = d3;
            mbr1.Right.Y = d4;

            d1 = Math.Min(mbr2.Left.X, mbr2.Right.X);
            d2 = Math.Max(mbr2.Left.Y, mbr2.Right.Y);
            d3 = Math.Max(mbr2.Left.X, mbr2.Right.X);
            d4 = Math.Min(mbr2.Left.Y, mbr2.Right.Y);

            mbr2.Left.X = d1;
            mbr2.Left.Y = d2;
            mbr2.Right.X = d3;
            mbr2.Right.Y = d4;

            if (mbr1.Left.X >= mbr2.Left.X && mbr1.Right.X <= mbr2.Right.X && mbr1.Left.Y <= mbr2.Left.Y && mbr1.Right.Y >= mbr2.Right.Y) return true; else return false;

        }

        /// <summary>
        /// метод определяющий пересекается ли два прямоугольника
        /// </summary>
        /// <param name="mbr"></param>
        /// <returns></returns>
        public bool IsIntersected(TMBR mbr)
        {
            double d1, d2, d3, d4;
            d1 = Math.Min(fmbr.Left.X, fmbr.Right.X);
            d2 = Math.Max(fmbr.Left.Y, fmbr.Right.Y);
            d3 = Math.Max(fmbr.Left.X, fmbr.Right.X);
            d4 = Math.Min(fmbr.Left.Y, fmbr.Right.Y);

            fmbr.Left.X = d1;
            fmbr.Left.Y = d2;
            fmbr.Right.X = d3;
            fmbr.Right.Y = d4;

            d1 = Math.Min(mbr.Left.X, mbr.Right.X);
            d2 = Math.Max(mbr.Left.Y, mbr.Right.Y);
            d3 = Math.Max(mbr.Left.X, mbr.Right.X);
            d4 = Math.Min(mbr.Left.Y, mbr.Right.Y);



            mbr.Left.X = d1;
            mbr.Left.Y = d2;
            mbr.Right.X = d3;
            mbr.Right.Y = d4;

            return !(mbr.Left.Y < fmbr.Right.Y || mbr.Right.Y > fmbr.Left.Y || mbr.Right.X < fmbr.Left.X || mbr.Left.X > fmbr.Right.X);
        }

        /// <summary>
        /// Возвращает площадь перекрытия MBR узла и заданной области
        /// </summary>
        /// <param name="mbr_ovrl"></param>
        /// <returns></returns>
        public double Overlap(TMBR mbr_ovrl)
        {
            double x, y;
            x = Math.Min(mbr_ovrl.Right.X, fmbr.Right.X) - Math.Max(mbr_ovrl.Left.X, fmbr.Left.X);
            if (x <= 0) return 0;
            y = Math.Min(mbr_ovrl.Left.Y, fmbr.Left.Y) - Math.Max(mbr_ovrl.Right.Y, fmbr.Right.Y);
            if (y <= 0) return 0;
            return x * y;
        }

        /// <summary>
        /// Возвращает площадь MBR узла
        /// </summary>
        /// <returns></returns>
        public double Area()
        {
            return (fmbr.Right.X - fmbr.Left.X) * (fmbr.Left.Y - fmbr.Right.Y);
        }

        /// <summary>
        /// Возвращает площадь MBR узла
        /// </summary>
        /// <returns></returns>
        public double Area(TMBR mbr)
        {
            return (mbr.Right.X - mbr.Left.X) * (mbr.Left.Y - mbr.Right.Y);
        }

        /// <summary>
        /// Возвращает периметр MBR
        /// </summary>
        /// <returns></returns>
        public double margin()
        {
            return 2 * (fmbr.Right.X - fmbr.Left.X + fmbr.Left.Y - fmbr.Right.Y);
        }
    }
}
