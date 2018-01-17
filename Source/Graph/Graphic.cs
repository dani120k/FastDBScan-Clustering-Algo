using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ZedGraph;
using System.Diagnostics;

namespace Graph
{
    public partial class Graphic : Form
    {
        Stopwatch dbscantime = null;
        Stopwatch kmeantime = null;
        List<MyLib.Point> points;

        Color[] array = { Color.Blue, Color.Red, Color.Orange, Color.Violet, Color.Pink, Color.Purple, Color.Green, Color.Brown, Color.SeaGreen, Color.SkyBlue, Color.Tomato, Color.SteelBlue, Color.Aqua, Color.Bisque, Color.BlanchedAlmond, Color.BlueViolet, Color.BurlyWood, Color.CadetBlue, Color.Chocolate, Color.Crimson, Color.Fuchsia, Color.Indigo, Color.Khaki, Color.Lavender, Color.LimeGreen, Color.Silver, Color.YellowGreen , Color.Azure, Color.CornflowerBlue, Color.DarkOrange, Color.DarkViolet, Color.ForestGreen, Color.Tan, Color.Teal};

        public Graphic()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Конструктор для графика FastDBscan
        /// </summary>
        /// <param name="points">Лист точек</param>
        /// <param name="numClusters">Количество кластеров</param>
        public Graphic(List<MyLib.Point> points, int numClusters, Stopwatch kmeantime, Stopwatch dbscantime, string name)
        {
            InitializeComponent();
            zedGraph.ContextMenuBuilder += new ZedGraphControl.ContextMenuBuilderEventHandler(zedGraph_ContextMenuBuilder);
            this.kmeantime = kmeantime;
            this.dbscantime = dbscantime;
            this.points = points;
            DrawGraph(points, numClusters, name);
        }

        /// <summary>
        /// Конструктор для графика DBscan
        /// </summary>
        /// <param name="points">Лист точек</param>
        /// <param name="numClusters">Количество кластеров</param>
        public Graphic(List<MyLib.Point> points, int numClusters, Stopwatch dbscantime, string name)
        {
            InitializeComponent();
            zedGraph.ContextMenuBuilder += new ZedGraphControl.ContextMenuBuilderEventHandler(zedGraph_ContextMenuBuilder);
            this.dbscantime = dbscantime;
            this.points = points;
            DrawGraph(points, numClusters, name, " ");
        }
        



        /// <summary>
        /// Метод для отрисовки графика FastDBScan'a
        /// </summary>
        /// <param name="points">Лист точек</param>
        /// <param name="numClusters">Количество кластеров</param>
        void DrawGraph(List<MyLib.Point> points, int numClusters, string name)
        {
            GraphPane pane = zedGraph.GraphPane;
            pane.Title.Text = name;
            pane.CurveList.Clear();
            PointPairList list = new PointPairList();

            for (int j = -1; j < numClusters; j++)
            {
                int count = 0;
                for (int i = 0; i < points.Count; i++)
                    if (points[i].cluster == j) { list.Add(points[i].x, points[i].y); list[count].Tag = j.ToString(); count++; }

                LineItem myCurve;
                if (j==-1) myCurve = pane.AddCurve("", list, Color.Black, SymbolType.Circle); else myCurve = pane.AddCurve("", list, array[j % array.Length], SymbolType.Circle);
                myCurve.Line.IsVisible = false;
                myCurve.Symbol.Fill.Type = FillType.Solid;
                myCurve.Symbol.Size = 3;
                list = new PointPairList();
            }


            pane.XAxis.Cross = 0.0;
            pane.YAxis.Cross = 0.0;

            pane.XAxis.Scale.Min = MyLib.WorkWithFile.xmin;
            pane.XAxis.Scale.Max = MyLib.WorkWithFile.xmax;

            pane.YAxis.Scale.Min = MyLib.WorkWithFile.ymin;
            pane.YAxis.Scale.Max = MyLib.WorkWithFile.ymax;

            zedGraph.AxisChange();

            zedGraph.Invalidate();

            pane.XAxis.Title.IsVisible = false;
            pane.YAxis.Title.IsVisible = false;
        }

        /// <summary>
        /// Метод для отрисовки графика DBScan'a
        /// </summary>
        /// <param name="points">Лист точек</param>
        /// <param name="numClusters">Количество кластеров</param>
        /// <param name="name"></param>
        void DrawGraph(List<MyLib.Point> points, int numClusters, string name, string test)
        {
            GraphPane pane = zedGraph.GraphPane;
            pane.Title.Text = name;
            pane.CurveList.Clear();
            PointPairList list = new PointPairList();

            for (int j = -1; j < numClusters; j++)
            {
                int count = 0;
                for (int i = 0; i < points.Count; i++)
                    if (points[i].db == j) { list.Add(points[i].x, points[i].y); list[count].Tag = j.ToString(); count++; }

                LineItem myCurve;
                if (j == -1) myCurve = pane.AddCurve("", list, Color.Black, SymbolType.Circle); else myCurve = pane.AddCurve("", list, array[j%array.Length], SymbolType.Circle);
                myCurve.Line.IsVisible = false;
                myCurve.Symbol.Fill.Type = FillType.Solid;
                myCurve.Symbol.Size = 3;
                list = new PointPairList();
            }


            pane.XAxis.Cross = 0.0;
            pane.YAxis.Cross = 0.0;

            pane.XAxis.Scale.Min = MyLib.WorkWithFile.xmin;
            pane.XAxis.Scale.Max = MyLib.WorkWithFile.xmax;

            pane.YAxis.Scale.Min = MyLib.WorkWithFile.ymin;
            pane.YAxis.Scale.Max = MyLib.WorkWithFile.ymax;

            zedGraph.AxisChange();

            zedGraph.Invalidate();

            pane.XAxis.Title.IsVisible = false;
            pane.YAxis.Title.IsVisible = false;
        }


        /// <summary>
        /// Меняет надписи по правой кнопке мыши на русский язык и убирает лишнее
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="menuStrip"></param>
        /// <param name="mousePt"></param>
        /// <param name="objState"></param>
        void zedGraph_ContextMenuBuilder(ZedGraphControl sender, ContextMenuStrip menuStrip, System.Drawing.Point mousePt, ZedGraphControl.ContextMenuObjectState objState)
        {
            menuStrip.Items[0].Text = "Копировать";
            menuStrip.Items[1].Text = "Сохранить как картинку…";
            menuStrip.Items[2].Text = "Параметры страницы…";
            menuStrip.Items[3].Text = "Печать…";
            menuStrip.Items[4].Text = "Показывать значения в точках…";

            menuStrip.Items.RemoveAt(4);
            menuStrip.Items.RemoveAt(4);
            menuStrip.Items.RemoveAt(4);
            menuStrip.Items.RemoveAt(4);




            ToolStripItem clusteritem = new ToolStripMenuItem("Показывать номер кластера…");
            menuStrip.Items.Add(clusteritem);
            clusteritem.Click += new EventHandler(newMenuItem_Click);

            ToolStripItem pointitem = new ToolStripMenuItem("Показывать координаты…");
            menuStrip.Items.Add(pointitem);
            pointitem.Click += new EventHandler(pointitem_Click);

        }

        void pointitem_Click(object sender, EventArgs e)
        {
            zedGraph.IsShowPointValues = true;
            zedGraph.PointValueEvent += new ZedGraphControl.PointValueHandler(zedGraph_Point_show);
        }

        string zedGraph_Point_show(ZedGraphControl sender,
        GraphPane pane,
        CurveItem curve,
        int iPt)
        {
            // Получим точку, около которой находимся
            PointPair point = curve[iPt];

            // Сформируем строку
            string result = $"X: {point.X:F3} Y: {point.Y}";

            return result;
        }


        void newMenuItem_Click(object sender, EventArgs e)
        {
            zedGraph.IsShowPointValues = true;
            zedGraph.PointValueEvent += new ZedGraphControl.PointValueHandler(zedGraph_Cluster_show);
        }

        string zedGraph_Cluster_show(ZedGraphControl sender,
        GraphPane pane,
        CurveItem curve,
        int iPt)
        {
            // Получим точку, около которой находимся
            PointPair point = curve[iPt];
            // Сформируем строку
            string result = point.Tag.ToString();

            return result;
        }

        private void toolStripLabel1_Click(object sender, EventArgs e)
        {
            if (kmeantime != null)
                if (points != null) MessageBox.Show("Количество точек: " + points.Count + "\n" + "Время выполнения K-means: " + kmeantime.Elapsed.Seconds + " сек." + "\n" + "Время выполнения DBScan: " + dbscantime.Elapsed.Seconds + " сек." + "\n" + "Общее время выполнения: " + (kmeantime.Elapsed.Seconds + dbscantime.Elapsed.Seconds) + " сек." + "\n" + "Скорость выполнения: " + $"{((kmeantime.Elapsed.Seconds + dbscantime.Elapsed.Seconds > 0) ? (points.Count / (kmeantime.Elapsed.Seconds + dbscantime.Elapsed.Seconds)) : points.Count):F2} точки в сек.", "Время выполнения", MessageBoxButtons.OK, MessageBoxIcon.Information); else MessageBox.Show("Количество точек: " + points.Count + "\n" + "Время выполнения K-means: " + kmeantime.Elapsed.Seconds + " сек." + "\n" + "Время выполнения DBScan с R*-деревом: " + dbscantime.Elapsed.Seconds + " сек." + "\n" + "Общее время выполнения: " + (kmeantime.Elapsed.Seconds + dbscantime.Elapsed.Seconds) + " сек." + "\n" + "Скорость выполнения: " + $"{((kmeantime.Elapsed.Seconds + dbscantime.Elapsed.Seconds > 0) ? (points.Count / (kmeantime.Elapsed.Seconds + dbscantime.Elapsed.Seconds)) : points.Count):F2} точки в сек.", "Время выполнения", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                if (points != null) MessageBox.Show("Количество точек: " + points.Count + "\n" + "Время выполнения DBScan: " + dbscantime.Elapsed.Seconds + " сек." + "\n" + "Общее время выполнения: " + (dbscantime.Elapsed.Seconds) + " сек." + "\n" + "Скорость выполнения: " + $"{((dbscantime.Elapsed.Seconds > 0) ? (points.Count / (dbscantime.Elapsed.Seconds)) : points.Count):F2} точки в сек.", "Время выполнения", MessageBoxButtons.OK, MessageBoxIcon.Information); else MessageBox.Show("Количество точек: " + points.Count + "\n" + "Время выполнения DBScan с R*-деревом: " + dbscantime.Elapsed.Seconds + " сек." + "\n" + "Общее время выполнения: " + (dbscantime.Elapsed.Seconds) + " сек." + "\n" + "Скорость выполнения: " + $"{((dbscantime.Elapsed.Seconds > 0) ? (points.Count / dbscantime.Elapsed.Seconds) : points.Count):F2} точки в сек.", "Время выполнения", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

    }
}
