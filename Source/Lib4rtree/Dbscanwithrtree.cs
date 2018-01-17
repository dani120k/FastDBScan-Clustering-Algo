using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyLib;

namespace Lib4rtree
{
    public static  class Dbscanwithrtree
    {
        public static double eps;//расстояние для dbscan'a
        static int minPTS;//минимальное количество точек в заданном радиусе для dbscan'a

        public static void Clustering(List<MyLib.Point> list, double e, int mpts, out int cluster_number)
        {
            //сохраним переменные в статические чтобы не передавать их лишний раз в методы
            eps = e;
            minPTS = mpts;
            TRtree tree = new TRtree();
            Console.WriteLine("count " + list.Count);
            for (int i = 0; i < list.Count; i++)
            { list[i].idx = i; tree.insertObject(list[i]); }

            cluster_number = 0;//начинаем счет кластеров с нуля
            CheckAllPoints(tree);

            for (int i = 0; i < tree.FNodeArr.Length; i++)//пройдем по всем узлам дерева
            {
                for (int j = 0; j < tree.FNodeArr[i].FObject.Length; j++)//проходим по всем точкам в узле дерева
                {
                    if (tree.FNodeArr[i].FObject[j].IsVisited == false && tree.FNodeArr[i].FObject[j].minPTScheck == true)//если точка не была посещена и выполнено условие на количество точек в eps-окрестности
                    {
                        tree.FNodeArr[i].FObject[j].IsVisited = true;//помечаем как посещенная
                        tree.FNodeArr[i].FObject[j].db = cluster_number;//помечаем текущую точку как текущий кластер
                        Startclust(tree, tree.FNodeArr[i].FObject[j].findpoint, cluster_number);//запускаем рекурсивную функцию для заполнения кластера
                        cluster_number++;//прибавляем номер кластера
                    }
                    else
                    {
                        tree.FNodeArr[i].FObject[j].IsVisited = true;//иначе помечаем как посещенную(точка не прошла проверку на плотность в eps-окрестности)
                    }
                }
            }

            for (int i = 0; i < tree.FNodeArr.Length; i++)
                for (int j = 0; j < tree.FNodeArr[i].FObject.Length; j++)
                    list[tree.FNodeArr[i].FObject[j].idx].db = tree.FNodeArr[i].FObject[j].db;

        }

        static void CheckAllPoints(TRtree tree)
        {
            for (int i = 0; i < tree.FNodeArr.Length; i++)
                for (int j = 0; j < tree.FNodeArr[i].FObject.Length; j++)
                    if (Check(tree, tree.FNodeArr[i].FObject[j])) tree.FNodeArr[i].FObject[j].minPTScheck = true; else tree.FNodeArr[i].FObject[j].minPTScheck = false;

        }

        static bool Check(TRtree tree, MyLib.Point point)
        {

            ForRstartree.Point lefttop = new ForRstartree.Point(point.x - eps, point.y + eps);
            ForRstartree.Point rightdown = new ForRstartree.Point(point.x + eps, point.y - eps);
            ForRstartree.TMBR findzone = new ForRstartree.TMBR(lefttop, rightdown);//Получили квадрат описывающий круг с центром в текущей точке
            List<MyLib.Point> list = new List<MyLib.Point>(0);
            List<MyLib.Point> find = new List<MyLib.Point>(0);
            find = tree.findobjectinarea(findzone, tree.FRoot, list, point.idx);//получили лист точек, которые находятся в eps-окрестности текущей(ее не берем)
            for (int i = 0; i < tree.FNodeArr.Length; i++)
                tree.FNodeArr[i].IsVisited = false;
            point.findpoint = find;

            if (find.Count >= minPTS) return true; else return false;
        }




        /// <summary>
        /// Рекурсивная функция для расширеия кластера
        /// </summary>
        /// <param name="tree">Дерево</param>
        /// <param name="findpoint">Лист найденных точек</param>
        /// <param name="cluster">Номер кластера</param>
        static void Startclust(TRtree tree, List<MyLib.Point> findpoint, int cluster)
        { 
            for (int i = 0; i < findpoint.Count; i++)
            {
                if (findpoint[i].IsVisited == false && findpoint[i].minPTScheck==true) { findpoint[i].db = cluster; findpoint[i].IsVisited = true; Startclust(tree, findpoint[i].findpoint, cluster); }
            }

        }
    }
}
