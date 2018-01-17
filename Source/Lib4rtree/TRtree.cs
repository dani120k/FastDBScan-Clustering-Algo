using System;
using System.Collections.Generic;
using MyLib;



namespace Lib4rtree
{
    public class TRtree : ForRstartree
    {
        public TRNode[] FNodeArr = new TRNode[0]; //массив узлов дерева
        public int FRoot;// ссылка на положение корневого узла в массиве узлов
        private int FHeight; // высота дерева

        /// <summary>
        /// Бытсрая сортировка массива точек по заданной оси
        /// </summary>
        /// <param name="a">Массив точек</param>
        /// <param name="l">Левая граница</param>
        /// <param name="r">Правая граница</param>
        /// <param name="axe">Ось по которой происходит сортировка</param>
        static void QuickSort(MyLib.Point[] a, int l, int r, TAxis axe)
        {
            MyLib.Point temp;
            double mid = 0;

            switch (axe)
            {
                case TAxis.X: mid = a[l + (r - l) / 2].x; break;
                case TAxis.Y: mid = a[l + (r - l) / 2].y; break;
            }

            //запись эквивалентна (l+r)/2, 
            //но не вызввает переполнения на больших данных
            int i = l;
            int j = r;
            //код в while обычно выносят в процедуру particle
            while (i <= j)
            {
                switch (axe)
                {
                    case TAxis.X:
                        {
                            while (a[i].x < mid) i++;
                            while (a[j].x > mid) j--;
                        }
                        break;
                    case TAxis.Y:
                        {
                            while (a[i].y < mid) i++;
                            while (a[j].y > mid) j--;
                        }
                        break;
                }


                if (i <= j)
                {
                    temp = a[i];
                    a[i] = a[j];
                    a[j] = temp;
                    i++;
                    j--;
                }
            }
            if (i < r)
                QuickSort(a, i, r, axe);

            if (l < j)
                QuickSort(a, l, j, axe);
        }


        /// <summary>
        ///  быстрая сортировка для узлов по их MBR. axe - ось по которой происходит сортировка, bound - граница по которой происходит сортировка (левая/правая)
        /// </summary>
        /// <param name="List"></param>
        /// <param name="iLo"></param>
        /// <param name="iHi"></param>
        /// <param name="axe"></param>
        /// <param name="bound"></param>
        #region
        private void QuickSort(int[] List, int iLo, int iHi, TAxis axe, TBound bound)
        {
            //Console.WriteLine("QuickSort2 work");
            int Lo = iLo, Hi = iHi;
            int T;
            double Mid = 0.0;

            switch (bound)
            {
                case TBound.Left:
                    {
                        switch (axe)
                        {
                            case TAxis.X: Mid = FNodeArr[List[(Lo + Hi) / 2]].mbr.Left.X; break;
                            case TAxis.Y: Mid = FNodeArr[List[(Lo + Hi) / 2]].mbr.Left.Y; break;
                        }
                    }
                    break;
                case TBound.Right:
                    {
                        switch (axe)
                        {
                            case TAxis.X: Mid = FNodeArr[List[(Lo + Hi) / 2]].mbr.Right.X; break;
                            case TAxis.Y: Mid = FNodeArr[List[(Lo + Hi) / 2]].mbr.Right.Y; break;
                        }
                    }
                    break;
            }

            do
            {
                switch (bound)
                {
                    case TBound.Left:
                        {
                            switch (axe)
                            {
                                case TAxis.X:
                                    {
                                        while (FNodeArr[List[Lo]].mbr.Left.X < Mid)
                                            Lo++;
                                        while (FNodeArr[List[Hi]].mbr.Left.X > Mid)
                                            Hi--;
                                    }
                                    break;
                                case TAxis.Y:
                                    {
                                        while (FNodeArr[List[Lo]].mbr.Left.Y < Mid)
                                            Lo++;
                                        while (FNodeArr[List[Hi]].mbr.Left.Y > Mid)
                                            Hi--;
                                    }
                                    break;
                            }
                        }
                        break;
                    case TBound.Right:
                        {
                            switch (axe)
                            {
                                case TAxis.X:
                                    {
                                        while (FNodeArr[List[Lo]].mbr.Right.X < Mid)
                                            Lo++;
                                        while (FNodeArr[List[Hi]].mbr.Right.X > Mid)
                                            Hi--;
                                    }
                                    break;
                                case TAxis.Y:
                                    {
                                        while (FNodeArr[List[Lo]].mbr.Right.Y < Mid)
                                            Lo++;
                                        while (FNodeArr[List[Hi]].mbr.Right.Y > Mid)
                                            Hi--;
                                    }
                                    break;
                            }
                        }
                        break;
                }

                if (Lo <= Hi)
                {
                    T = List[Lo];
                    List[Lo] = List[Hi];
                    List[Hi] = T;
                    Lo++;
                    Hi--;
                }
            } while (Lo <= Hi);

            if (Hi > iLo) QuickSort(List, iLo, Hi, axe, bound);
            if (Lo < iHi) QuickSort(List, Lo, iHi, axe, bound);
        }
        #endregion

        /// <summary>
        /// разделяет узел на 2 в соответствии с алгоритмами R*-tree (node_id = ссылка на узел для разделения obj = обьект для вставки)
        /// </summary>
       #region
        private void splitNodeRStar(int node_id, MyLib.Point obj)
        {
            //Console.WriteLine("splitNodeRStar work1 " + FHeight);
            TAxis axe;
            int parent_id, new_child_id;
            TRNode node_1, node_2, node_1_min, node_2_min;
            int i, j, k;
            MyLib.Point[] arr_obj;
            double area_min, Area;

            if (!FNodeArr[node_id].isLeaf) return;

            if (isRoot(node_id))
            {
                //Console.WriteLine("it works");
                parent_id = newNode();
                FNodeArr[FRoot].Parent = parent_id;
                FNodeArr[parent_id].setChild(0, FRoot);
                FNodeArr[parent_id].Level = FNodeArr[FNodeArr[parent_id].getChild(0)].Level + 1;
                FRoot = parent_id;
                FHeight++;
            }
            else
            {

                parent_id = newNode();
                FNodeArr[parent_id].Parent = FNodeArr[node_id].Parent;
                FNodeArr[FNodeArr[parent_id].Parent].setChild(FNodeArr[FNodeArr[parent_id].Parent].FChildren.Length, parent_id);
                FNodeArr[node_id].Parent = parent_id;
                FNodeArr[parent_id].setChild(0, node_id);
                /*
                parent_id = FNodeArr[node_id].Parent;
                Console.WriteLine("fuck); " + parent_id);
                FNodeArr[FNodeArr[node_id].Parent].FisLeaf = false;
                */
            }


            arr_obj = new MyLib.Point[MAX_M + 1];

            for (i = 0; i < arr_obj.Length - 1; i++)
                arr_obj[i] = FNodeArr[node_id].getObject(i);

            arr_obj[arr_obj.Length - 1] = obj;

            node_1_min = new TRNode();
            node_2_min = new TRNode();
            node_1 = new TRNode();
            node_2 = new TRNode();

            axe = chooseSplitAxis(obj, node_id);

            area_min = double.MaxValue;


            for (i = 0; i <= 1; i++)
            {
                QuickSort(arr_obj, 0, arr_obj.Length - 1, axe);

                for (k = MIN_M - 1; k <= MAX_M - MIN_M; k++)
                {
                    node_1.clearObjects();
                    node_2.clearObjects();

                    j = 0;

                    while (j <= k)
                    {
                        node_1.setObject(j, arr_obj[j]);
                        j++;
                    }

                    for (j = k; j < arr_obj.Length - 1; j++)
                        node_2.setObject(j - k, arr_obj[j + 1]);


                    updateMBR(node_1);
                    updateMBR(node_2);

                    Area = node_1.Area() + node_2.Area();
                    if (Area < area_min)
                    {
                        node_1_min.copy(node_1);
                        node_2_min.copy(node_2);
                        area_min = Area;
                    }
                }
            }

            node_1_min.Level = 0;
            node_2_min.Level = 0;

            FNodeArr[node_id].copy(node_1_min);
            FNodeArr[node_id].Parent = parent_id;

            updateMBR(node_id);

            new_child_id = newNode();
            FNodeArr[new_child_id].copy(node_2_min);
            FNodeArr[new_child_id].Parent = parent_id;
            updateMBR(new_child_id);

            if (FNodeArr[parent_id].FChildren.Length < MAX_M)
            {
                FNodeArr[parent_id].setChild(FNodeArr[parent_id].FChildren.Length, new_child_id);
                updateMBR(parent_id);
                FNodeArr[parent_id].FisLeaf = false;
            }
            else
                splitNodeRStar(parent_id, new_child_id);
        }
        #endregion

        /// <summary>
        /// разделяет узел на 2 в соответствии с алгоритмами R*-tree splited_Node_Id = ссылка на узел для разделения, inserted_Node_Id = узел для вставки 
        /// </summary>
        /// <param name="splited_Node_Id"></param>
        /// <param name="inserted_Node_Id"></param>
        #region
        private void splitNodeRStar(int splited_Node_Id, int inserted_Node_Id)
        {
            //Console.WriteLine("splitNodeRSter2 work");
            TAxis axe;
            int parent_id, new_child_id;
            TRNode node_1, node_2, node_1_min, node_2_min;
            int i, j, k;
            int[] arr_node;
            double area_min, Area;

            if (FNodeArr[splited_Node_Id].isLeaf) return;

            if (isRoot(splited_Node_Id))
            {
                parent_id = newNode();
                FNodeArr[FRoot].Parent = parent_id;
                FNodeArr[parent_id].setChild(0, FRoot);
                FNodeArr[parent_id].Level = FNodeArr[FNodeArr[parent_id].getChild(0)].Level + 1;
                FRoot = parent_id;
                FNodeArr[parent_id].FisLeaf = false;
                FHeight = FHeight + 1;
            }
            else
            {
                
                parent_id = newNode();
                FNodeArr[parent_id].Parent = FNodeArr[splited_Node_Id].Parent;
                FNodeArr[FNodeArr[parent_id].Parent].setChild(FNodeArr[FNodeArr[parent_id].Parent].FChildren.Length, parent_id);
                FNodeArr[splited_Node_Id].Parent = parent_id;
                FNodeArr[parent_id].setChild(0, splited_Node_Id);
                //parent_id = FNodeArr[splited_Node_Id].Parent;
            }

            arr_node = new int[MAX_M + 1];
            for (i = 0; i < arr_node.Length - 1; i++)
                arr_node[i] = FNodeArr[splited_Node_Id].getChild(i);

            arr_node[arr_node.Length - 1] = inserted_Node_Id;

            node_1_min = new TRNode();
            node_2_min = new TRNode();

            node_1 = new TRNode();
            node_2 = new TRNode();

            axe = chooseSplitAxis(splited_Node_Id, inserted_Node_Id);

            area_min = double.MaxValue;

            for (i = 0; i <= 1; i++)
            {
                QuickSort(arr_node, 0, arr_node.Length - 1, axe, (TBound)i);

                for (k = MIN_M - 1; k <= MAX_M - MIN_M; k++)
                {
                    node_1.clearChildren();
                    node_2.clearChildren();

                    j = 0;

                    while (j <= k)
                    {
                        node_1.setChild(j, arr_node[j]);
                        j++;
                    }

                    for (j = k; j < arr_node.Length - 1; j++)
                        node_2.setChild(j - k, arr_node[j + 1]);

                    updateMBR(node_1);
                    updateMBR(node_2);

                    Area = node_1.Area() + node_2.Area();
                    if (Area < area_min)
                    {
                        node_1_min.copy(node_1);
                        node_2_min.copy(node_2);
                        area_min = Area;
                    }
                }
            }

            node_1_min.Level = FNodeArr[splited_Node_Id].Level;
            node_2_min.Level = FNodeArr[splited_Node_Id].Level;

            FNodeArr[splited_Node_Id].copy(node_1_min);
            FNodeArr[splited_Node_Id].Parent = parent_id;

            new_child_id = newNode();
            FNodeArr[new_child_id].copy(node_2_min);
            FNodeArr[new_child_id].Parent = parent_id;

            node_1 = null;
            node_2 = null;
            node_1_min = null;
            node_2_min = null;



            for (i = 0; i < FNodeArr[new_child_id].FChildren.Length; i++)
                FNodeArr[FNodeArr[new_child_id].getChild(i)].Parent = new_child_id;

            if (FNodeArr[parent_id].FChildren.Length < MAX_M)
            {
                FNodeArr[parent_id].setChild(FNodeArr[parent_id].FChildren.Length, new_child_id);
                FNodeArr[parent_id].FisLeaf = false;
                updateMBR(parent_id);
            }
            else
                splitNodeRStar(parent_id, new_child_id);
        }
        #endregion

        /// <summary>
        /// метод создания нового узла. Возвращает индекс только что созданого узла
        /// </summary>
        /// <returns></returns>
        private int newNode()
        {
            //Console.WriteLine("NewNode work");
            Array.Resize(ref FNodeArr, FNodeArr.Length + 1);
            FNodeArr[FNodeArr.Length - 1] = new TRNode();
            return FNodeArr.Length - 1;
        }

        /// <summary>
        ///  метод определяющий является ли корнем узел с индексом node_id
        /// </summary>
        /// <param name="node_id"></param>
        /// <returns></returns>
        private bool isRoot(int node_id)
        {
            if (node_id == FRoot) return true; else return false;
        }

        /// <summary>
        /// обновляет MBR узла
        /// </summary>
        /// <param name="node"></param>
        #region
        public void updateMBR(TRNode node)
        {
            //Console.WriteLine("updateMBR1 work");
            int i, idx;
            bool changed = false;

            node.fmbr.Left.X = double.MaxValue;
            node.fmbr.Left.Y = double.MinValue;
            node.fmbr.Right.X = double.MinValue;
            node.fmbr.Right.Y = double.MaxValue;

            if (node.getIsLeaf())
            {
                for (i = 0; i < node.FObject.Length; i++)
                {
                    if (node.FObject[i].x < node.fmbr.Left.X)
                    {
                        node.fmbr.Left.X = node.FObject[i].x;
                        changed = true;
                    }
                    if (node.FObject[i].y > node.fmbr.Left.Y)
                    {
                        node.fmbr.Left.Y = node.FObject[i].y;
                        changed = true;
                    }
                    if (node.FObject[i].x > node.fmbr.Right.X)
                    {
                        node.fmbr.Right.X = node.FObject[i].x;
                        changed = true;
                    }
                    if (node.FObject[i].y < node.fmbr.Right.Y)
                    {
                        node.fmbr.Right.Y = node.FObject[i].y;
                        changed = true;
                    }
                }
            }
            else
            {
                for (i = 0; i < node.FChildren.Length; i++)
                {
                    idx = node.FChildren[i];
                    if (FNodeArr[idx].fmbr.Left.X < node.fmbr.Left.X)
                    {
                        node.fmbr.Left.X = FNodeArr[idx].fmbr.Left.X;
                        changed = true;
                    }
                    if (FNodeArr[idx].fmbr.Left.Y > node.fmbr.Left.Y)
                    {
                        node.fmbr.Left.Y = FNodeArr[idx].fmbr.Left.Y;
                        changed = true;
                    }
                    if (FNodeArr[idx].fmbr.Right.X > node.fmbr.Right.X)
                    {
                        node.fmbr.Right.X = FNodeArr[idx].fmbr.Right.X;
                        changed = true;
                    }
                    if (FNodeArr[idx].fmbr.Right.Y < node.fmbr.Right.Y)
                    {
                        node.fmbr.Right.Y = FNodeArr[idx].fmbr.Right.Y;
                        changed = true;
                    }
                }
            }
            if (changed)
                if (node.Parent >= 0) updateMBR(node.Parent);
        }

        private void updateMBR(int node_id)
        {
            //Console.WriteLine("updateMBR2 work");
            int i, idx;
            bool changed = false;

            FNodeArr[node_id].fmbr.Left.X = double.MaxValue;
            FNodeArr[node_id].fmbr.Left.Y = double.MinValue;
            FNodeArr[node_id].fmbr.Right.X = double.MinValue;
            FNodeArr[node_id].fmbr.Right.Y = double.MaxValue;

            if (FNodeArr[node_id].getIsLeaf())
            {
                for (i = 0; i < FNodeArr[node_id].FObject.Length; i++)
                {
                    if (FNodeArr[node_id].getObject(i).x < FNodeArr[node_id].fmbr.Left.X)
                    {
                        FNodeArr[node_id].fmbr.Left.X = FNodeArr[node_id].getObject(i).x;
                        changed = true;
                    }
                    if (FNodeArr[node_id].getObject(i).y > FNodeArr[node_id].fmbr.Left.Y)
                    {
                        FNodeArr[node_id].fmbr.Left.Y = FNodeArr[node_id].getObject(i).y;
                        changed = true;
                    }
                    if (FNodeArr[node_id].getObject(i).x > FNodeArr[node_id].fmbr.Right.X)
                    {
                        FNodeArr[node_id].fmbr.Right.X = FNodeArr[node_id].getObject(i).x;
                        changed = true;
                    }
                    if (FNodeArr[node_id].getObject(i).y < FNodeArr[node_id].fmbr.Right.Y)
                    {
                        FNodeArr[node_id].fmbr.Right.Y = FNodeArr[node_id].getObject(i).y;
                        changed = true;
                    }
                }
            }
            else
            {
                for (i = 0; i < FNodeArr[node_id].FChildren.Length; i++)
                {
                    idx = FNodeArr[node_id].FChildren[i];

                    if (FNodeArr[idx].fmbr.Left.X < FNodeArr[node_id].fmbr.Left.X)
                    {
                        FNodeArr[node_id].fmbr.Left.X = FNodeArr[idx].fmbr.Left.X;
                        changed = true;
                    }
                    if (FNodeArr[idx].fmbr.Left.Y > FNodeArr[node_id].fmbr.Left.Y)
                    {
                        FNodeArr[node_id].fmbr.Left.Y = FNodeArr[idx].fmbr.Left.Y;
                        changed = true;
                    }
                    if (FNodeArr[idx].fmbr.Right.X > FNodeArr[node_id].fmbr.Right.X)
                    {
                        FNodeArr[node_id].fmbr.Right.X = FNodeArr[idx].fmbr.Right.X;
                        changed = true;
                    }
                    if (FNodeArr[idx].fmbr.Right.Y < FNodeArr[node_id].fmbr.Right.Y)
                    {
                        FNodeArr[node_id].fmbr.Right.Y = FNodeArr[idx].fmbr.Right.Y;
                        changed = true;
                    }
                }
            }
            if (changed)
                if (FNodeArr[node_id].Parent >= 0) updateMBR(FNodeArr[node_id].Parent);
        }
        #endregion

        /// <summary>
        /// TAXis
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="node_id"></param>
        /// <returns></returns>
        #region
        private TAxis chooseSplitAxis(MyLib.Point obj, int node_id)
        {
            //Console.WriteLine("chooseSplitAxis1 work");
            MyLib.Point[] arr_obj;
            int i, j, k, idx;
            TRNode node_1, node_2;
            double perimetr_min, perimetr;
            TAxis result = new TAxis();

            arr_obj = new MyLib.Point[MAX_M + 1];

            if (!FNodeArr[node_id].isLeaf) return 0;

            for (i = 0; i < FNodeArr[node_id].FObject.Length; i++)
                arr_obj[i] = FNodeArr[node_id].FObject[i];

            arr_obj[arr_obj.Length - 1] = obj;

            node_1 = new TRNode();
            node_2 = new TRNode();

            perimetr_min = double.MaxValue;

            for (i = 0; i <= 1; i++)
            {
                perimetr = 0;

                for (j = 0; j <= 1; j++)
                {
                    node_1.clearObjects();
                    node_2.clearObjects();

                    QuickSort(arr_obj, 0, arr_obj.Length - 1, (TAxis)i);

                    for (k = 1; k <= MAX_M - MIN_M * 2 + 2; k++) // высчитваем периметры
                    {
                        idx = 0;

                        while (idx < ((MIN_M - 1) + k))
                        {
                            node_1.setObject(idx, arr_obj[idx]);
                            idx++;
                        }

                        for (; idx < arr_obj.Length; idx++)
                            node_2.setObject(idx - ((MIN_M - 1) + k), arr_obj[idx]);

                        updateMBR(node_1);
                        updateMBR(node_2);

                        perimetr = perimetr + ((node_1.mbr.Right.X - node_1.mbr.Left.X) * 2 + (node_2.mbr.Left.Y - node_2.mbr.Right.Y) * 2);
                    }
                }

                if (perimetr <= perimetr_min)
                {
                    result = (TAxis)i;
                    perimetr_min = perimetr;
                }
            }
            return result;
        }

        private TAxis chooseSplitAxis(int nodeFather, int nodeChild)
        {
            //Console.WriteLine("chooseSplitAxis work");
            int[] arr_node;
            int i, j, k, idx;
            TRNode node_1, node_2;
            double perimetr, perimetr_min;
            TAxis result = new TAxis();

            arr_node = new int[MAX_M + 1];

            for (i = 0; i < FNodeArr[nodeFather].FChildren.Length; i++)
                arr_node[i] = FNodeArr[nodeFather].FChildren[i];

            arr_node[arr_node.Length - 1] = nodeChild;

            perimetr_min = double.MaxValue;

            node_1 = new TRNode();
            node_2 = new TRNode();

            for (i = 0; i <= 1; i++)
            {
                perimetr = 0;
                for (j = 0; j <= 1; j++)
                {
                    node_1.clearChildren();
                    node_2.clearChildren();

                    QuickSort(arr_node, 0, arr_node.Length - 1, (TAxis)i, (TBound)j);

                    for (k = 1; k <= MAX_M - MIN_M * 2 + 2; k++)
                    {
                        idx = 0;

                        while (idx < ((MIN_M - 1) + k))
                        {
                            node_1.setChild(idx, arr_node[idx]);
                            idx++;
                        }

                        for (; idx < arr_node.Length; idx++)
                            node_2.setChild(idx - ((MIN_M - 1) + k), arr_node[idx]);

                        updateMBR(node_1);
                        updateMBR(node_2);

                        perimetr = perimetr + node_1.margin() + node_2.margin();
                    }
                }

                if (perimetr <= perimetr_min)
                {
                    result = (TAxis)i;
                    perimetr_min = perimetr;
                }

                perimetr = 0;
            }

            node_1 = null;
            node_2 = null;
            arr_node = new int[0];
            return result;
        }
        #endregion




        private int chooseSubtree(MyLib.Point obj, int node_id)
        {
            //Console.WriteLine("chooseSubtree work");
            int i, id_child;
            int[] idChild_area = new int[0];
            int id_zero;
            double dspace;

            if (FNodeArr[node_id].isLeaf) return node_id;

            Array.Resize(ref idChild_area, FNodeArr[node_id].FChildren.Length);

            dspace = double.MaxValue;
            id_zero = 0;
            for (i = 0; i < FNodeArr[node_id].FChildren.Length; i++)
                idChild_area[i] = FNodeArr[node_id].FChildren[i];

            dspace = double.MaxValue;

            for (i = 0; i < idChild_area.Length; i++)
            {
                id_child = idChild_area[i];

                if (FNodeArr[id_child].Area() < dspace)
                {
                    id_zero = idChild_area[i];
                    dspace = FNodeArr[id_child].Area();
                }
            }
            node_id = id_zero;
            chooseSubtree(obj, node_id);
            return node_id;
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        public TRtree()
        {
            FNodeArr = new TRNode[1];
            FNodeArr[0] = new TRNode();
            FRoot = 0;
            FNodeArr[FRoot].FisLeaf = true;
        }

        static int ct=0;
        /// <summary>
        /// метод для вставки обьекта в дерево
        /// </summary>
        /// <param name="obj"></param>
        public void insertObject(MyLib.Point obj)
        {
            int node_id = FRoot;
            Console.WriteLine(ct++);
            node_id = chooseSubtree(obj, node_id);
            if (FNodeArr[node_id].FObject.Length < MAX_M) //если количество объектов в узле меньше максимального
            {
                FNodeArr[node_id].setObject(FNodeArr[node_id].FObject.Length, obj);
                updateMBR(node_id);
            }
            else//если количество объектов достигло максимально допустимого
            {
                splitNodeRStar(node_id, obj); //делим узел
            }
        }


        public List<MyLib.Point> findobjectinarea(TMBR mbr, int node_id, List<MyLib.Point> array, int idx)
        {

            if (FNodeArr[node_id].FChildren.Length > 0)
            {
                for (int i = 0; i < FNodeArr[node_id].FChildren.Length; i++)
                {
                    if (FNodeArr[FNodeArr[node_id].FChildren[i]].IsIntersected(mbr) && !FNodeArr[FNodeArr[node_id].FChildren[i]].IsVisited) { findobjectinarea(mbr, FNodeArr[node_id].FChildren[i], array, idx); }
                }
            }
            else
            {
                array = findObjectinarea(mbr, node_id, array, idx);
                FNodeArr[node_id].IsVisited = true;
            }
            //Console.WriteLine("count " + array.Count);
            return array;
        }

        public List<MyLib.Point> findObjectinarea(TMBR mbr, int node_id, List<MyLib.Point> array, int idx)
        {
            double eps = Dbscanwithrtree.eps;

            MyLib.Point f = new MyLib.Point(mbr.Left.X + eps, mbr.Left.Y - eps, -1);

            for (int i = 0; i < FNodeArr[node_id].FObject.Length; i++)
                if (FNodeArr[node_id].FObject[i].idx != idx && f.Distance(FNodeArr[node_id].FObject[i]) <= eps) array.Add(FNodeArr[node_id].FObject[i]);

            return array;
        }

        /// <summary>
        /// Свойство, возвращающее высоту дерева
        /// </summary>
        public int Height
        {
            get
            {
                return FHeight;
            }
        }
    }
}

