using System;
using System.Windows.Forms;
using MyLib;
using System.Collections.Generic;
using System.Diagnostics;
using Lib4rtree;

namespace Start_page
{
    public partial class Form1 : Form
    {
        bool OnlyDBscan = false;//использовать ли только DBscan
        bool UseRTree = false;//Использовать ли дерево в алгоритмах

        /// <summary>
        /// Конструктор создания стартового окна
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            comboBox1.Items.Add("FastDBScan");
            comboBox1.Items.Add("Только DBscan");
            comboBox1.SelectedItem = comboBox1.Items[0];
            //всплывающие надписи над лэйблами
            ToolTip t = new ToolTip();
            t.SetToolTip(label1, "Нажмите кнопку справа и выберите файл в проводнике(.txt)");
            t.SetToolTip(label2, "Количество кластеров для K-means(целое положительное >1)");
            t.SetToolTip(label3, "Минимальное расстояние для DBScan(вещественное положительное)");
            t.SetToolTip(label4, "Минимальное количество точек в кластере для DBScan(целое положительное)");
            t.SetToolTip(label5, "Количество точек каждого кластера после K-means которые берутся для dbscan*в процентах(0-100)");
            t.SetToolTip(label6, "Максимальное количество точек в узле R*-дерева(>4)");
            textBox6.ReadOnly = true;
            progressBar1.Maximum = 100;
            progressBar1.Minimum = 0;
            progressBar1.Value = 0;
        }




        /// <summary>
        /// По нажатию на кнопку "Открыть файл"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog a = new OpenFileDialog();
            a.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*"; //только txt файлы
            a.ShowDialog();
            textBox1.Text = a.FileName;
        }

        /// <summary>
        /// По нажатию на кнопку "Запустить кластеризацию"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            bool check = true;//успешно ли заполнены все поля

            int k, minPTS;
            double eps;
            int t;
            int maxm;
            //проверка поля с адресом файла
            if (textBox1.Text.Length < 1 && check) { MessageBox.Show("Не выбран файл!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); check = false; }
            //проверка поля с К
            if (textBox2.Text == "" && check && OnlyDBscan == false) { textBox2.Focus(); check = false; }
            if ((!int.TryParse(textBox2.Text, out k) || k <= 1) && check && OnlyDBscan==false) { MessageBox.Show("Введено неверное значение k(k >1)", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); textBox2.Clear(); textBox2.Focus(); check = false; }
            //проверка поля с eps
            if (textBox3.Text == "" && check) { textBox3.Focus(); check = false; }
            if ((!double.TryParse(textBox3.Text.Replace('.', ','), out eps) || eps < 0.0) && check) { MessageBox.Show("Введено неверное значение eps(eps>0)", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); textBox3.Clear(); textBox3.Focus(); check = false; }
            //проверка поля с minPTS
            if (textBox4.Text == "" && check) { textBox4.Focus(); check = false; }
            if ((!int.TryParse(textBox4.Text, out minPTS) || minPTS <= 0) && check) { MessageBox.Show("Введено неверное значение minPTS(minPTS>0)", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); textBox4.Clear(); textBox4.Focus(); check = false; }
            //проверка поля с процентом
            if (textBox5.Text =="" && check && OnlyDBscan==false) { textBox5.Focus(); check = false; }
            if ((!int.TryParse(textBox5.Text,out t) || t<0 || t>100) && check && OnlyDBscan==false) { MessageBox.Show("Введено неверное значение t(0 <= t <= 100", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); textBox5.Clear(); textBox5.Focus(); check = false; }
            if (textBox6.Text == "" && check && UseRTree == true) { textBox6.Focus(); check = false; }
            if ((!int.TryParse(textBox6.Text, out maxm) || maxm <= 4) && check && UseRTree == true) { MessageBox.Show("Введено неверное значение MAXM (MAXM > 4)", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); textBox6.Clear(); textBox6.Focus(); check = false; }
            //если все поля заданы верно, то запускаем форму с графиком
            if (check) { ForRstartree.MAX_M = maxm; ForRstartree.MIN_M = (int)(maxm * 0.4); DrawGraph(textBox1.Text, k, eps, minPTS, t); }
        }

        /// <summary>
        /// Закрытие формы по нажатию на кнопку
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Метод для выбора варианта кластеризации 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="k"></param>
        /// <param name="eps"></param>
        /// <param name="minPTS"></param>
        /// <param name="t"></param>
        void DrawGraph(string path, int k, double eps, int minPTS, int t)
        {
            try
            {
                if (UseRTree == true)
                {

                    if (OnlyDBscan == true)
                    {
                        UseOnlyDBScanwithRtree(path, eps, minPTS);
                    }
                    else
                    {
                        UseFastDBscanwithRtree(path, k, eps, minPTS, t);
                    }
                }
                else
                {
                    if (OnlyDBscan == true)
                    {
                        UseOnlyDBScan(path, eps, minPTS);
                    }
                    else
                    {
                        UseOnlyFastDBScan(path, k, eps, minPTS, t);
                    }
                }
            }
            catch
            {
                MessageBox.Show("Произошла непредвиденная ошибка. Попробуйте еще раз!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Метод кластеризации Dbscan с использованием R*-дерева
        /// </summary>
        /// <param name="path">Путь до файла с точками</param>
        /// <param name="eps"></param>
        /// <param name="minPTS"></param>
        void UseOnlyDBScanwithRtree(string path, double eps, int minPTS)
        {
            
            bool check = true;
            List<Point> points = new List<Point>(0);
            try
            {
                points = WorkWithFile.GetPointFromFile(path);
                progressBar1.Value = 30;
            }
            catch (ArgumentException ex)
            {
                check = false;
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                progressBar1.Value = 0;
            }

            if (check)
            {
                Stopwatch dbscantime = new Stopwatch();

                    int count_of_db;
                    //if (ForRstartree.MAX_M )
                    dbscantime.Start();

                    Dbscanwithrtree.Clustering(points, eps, minPTS, out count_of_db);

                dbscantime.Stop();
                    progressBar1.Value = 70;
                    MessageBox.Show("Количество точек: " + points.Count + "\n" + "Время выполнения DBScan с R*-деревом: " + dbscantime.Elapsed.Seconds + " сек." + "\n" + "Скорость выполнения: " + $"{ ((dbscantime.Elapsed.Seconds > 0) ? (points.Count / dbscantime.Elapsed.Seconds) : points.Count):F2} точки в сек.", "Время выполнения", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Graph.Graphic f2 = new Graph.Graphic(points, count_of_db,dbscantime,"Алгоритм кластеризации DBscan с использованием R*-дерева");
                    progressBar1.Value = 100;
                    f2.Show();


            }
            progressBar1.Value = 100;

        }

        /// <summary>
        /// Кластеризация FastDBSCan с R*-деревом
        /// </summary>
        /// <param name="path"></param>
        /// <param name="k"></param>
        /// <param name="eps"></param>
        /// <param name="minPTS"></param>
        /// <param name="t"></param>
        void UseFastDBscanwithRtree(string path, int k, double eps, int minPTS,int t)
        {
            bool check = true;
            List<Point> points = new List<Point>(0);

            try
            {
                points = WorkWithFile.GetPointFromFile(path);
                progressBar1.Value = 30;
            }

            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                check = false;
                progressBar1.Value = 0;
            }

            if (k>points.Count) { MessageBox.Show("Введенный параметр K больше чем количество точек в выбранном файле. Уменьшите К или выберите другой файл!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); check = false; progressBar1.Value = 0; }

            if (check)
            {
                Stopwatch kmeantime = new Stopwatch();
                Stopwatch dbscantime = new Stopwatch();
                kmeantime.Start();
                int[] clustering = kmeans.Cluster(points, k-1, 0);
                progressBar1.Value = 50;

                for (int i = 0; i < points.Count; i++)
                    points[i].cluster = clustering[i];

                progressBar1.Value = 30;
                kmeantime.Stop();

                List<Point> fordbscan = new List<Point>(0);
                int ct = 0;
                for (int i = 0; i < k; i++)
                {
                    int count = 0;
                    for (int j = 0; j < points.Count; j++)
                        if (points[j].cluster == i) count++;
                    int finalcount = (int)Math.Ceiling((double)count * t / 100);
                    int l = 0;
                    for (int m = 0; m < points.Count; m++)
                        if (points[m].cluster == i && l < finalcount)
                        {
                            fordbscan.Add(points[m]);
                            ct++;
                            l++;
                        }
                }

                progressBar1.Value = 70;
                int count_db;
                dbscantime.Start();
                try
                {
                    Lib4rtree.Dbscanwithrtree.Clustering(fordbscan, eps, minPTS, out count_db);
                }
                catch
                {
                    count_db = 0;
                    MessageBox.Show("Превышено максимально допустимое количество итераций для Dbscan. Попробуйте другой значение MAXM.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                GetFullPicture(fordbscan, k, count_db);
                progressBar1.Value = 90;
                dbscantime.Stop();


                MessageBox.Show("Количество точек: " + points.Count + "\n" + "Время выполнения K-means: " + kmeantime.Elapsed.Seconds + " сек." + "\n" + "Время выполнения DBScan с R*-деревом: " + dbscantime.Elapsed.Seconds + " сек." + "\n" + "Общее время выполнения: " + (kmeantime.Elapsed.Seconds + dbscantime.Elapsed.Seconds) + " сек." + "\n" + "Скорость выполнения: " + $"{((kmeantime.Elapsed.Seconds + dbscantime.Elapsed.Seconds > 0) ? (points.Count / (kmeantime.Elapsed.Seconds + dbscantime.Elapsed.Seconds)) : points.Count):F2} точки в сек.", "Время выполнения", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Graph.Graphic f1 = new Graph.Graphic(points, k, kmeantime,dbscantime,"Алгоритм кластеризации FastDBscan с использованием R-дерева");
                progressBar1.Value = 100;
                f1.Show();
            }
        }

        /// <summary>
        /// Кластеризаця DBSCan
        /// </summary>
        /// <param name="path"></param>
        /// <param name="eps"></param>
        /// <param name="minPTS"></param>
        void UseOnlyDBScan(string path, double eps,int minPTS)
        {
            bool check = true;
            List<Point> points = new List<Point>(0);

            try
            {
                points = MyLib.WorkWithFile.GetPointFromFile(path);
                progressBar1.Value = 30;
            }

            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                check = false;
                progressBar1.Value = 0;
            }

            if (check)
            {
                Stopwatch dbscantime = new Stopwatch();
                dbscantime.Start();
                int count_of_db = dbscan.Clustering(points, eps, minPTS);
                progressBar1.Value = 80;
                dbscantime.Stop();
                MessageBox.Show("Количество точек: " + points.Count + "\n" + "Время выполнения DBScan: " + dbscantime.Elapsed.Seconds + " сек." + "\n" + "Время выполнения DBScan: " + dbscantime.Elapsed.Seconds + "\n" + "Скорость выполнения: " + $"{ ((dbscantime.Elapsed.Seconds > 0) ? (points.Count / dbscantime.Elapsed.Seconds) : points.Count):F2} точки в сек.", "Время выполнения", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Graph.Graphic f2 = new Graph.Graphic(points, count_of_db, dbscantime, "Алгоритм кластеризации DBscan без использования R*-дерева");
                f2.Show();
                progressBar1.Value = 100;
            }
        }

        /// <summary>
        /// Кластеризация FastDBScan
        /// </summary>
        /// <param name="path"></param>
        /// <param name="k"></param>
        /// <param name="eps"></param>
        /// <param name="minPTS"></param>
        /// <param name="t"></param>
        void UseOnlyFastDBScan(string path, int k, double eps, int minPTS, int t)
        {
            bool check = true;
            Stopwatch dbscantime = new Stopwatch();
            Stopwatch kmeantime = new Stopwatch();
            List<Point> points = new List<Point>(0);

            try
            {
                points = WorkWithFile.GetPointFromFile(path);
                progressBar1.Value = 30;
            }

            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                check = false;
                progressBar1.Value = 0;
            }
            if (k > points.Count) { MessageBox.Show("Введенный параметр K больше чем количество точек в выбранном файле. Уменьшите К или выберите другой файл!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); check = false; progressBar1.Value = 0; }
            if (check)
            {
                kmeantime.Start();
                int[] clustering = kmeans.Cluster(points, k-1, 0);
                progressBar1.Value = 50;

                for (int i = 0; i < points.Count; i++)
                    points[i].cluster = clustering[i];

                progressBar1.Value = 30;
                kmeantime.Stop();


                List<MyLib.Point> fordbscan = new List<MyLib.Point>(0);

                for (int i = 0; i < k; i++)
                {
                    int count = 0;
                    for (int j = 0; j < points.Count; j++)
                        if (points[j].cluster == i) count++;
                    int finalcount = (int)Math.Ceiling((double)count * t / 100);
                    int l = 0;
                    for (int m = 0; m < points.Count; m++)
                        if (points[m].cluster == i && l < finalcount) { fordbscan.Add(points[m]); l++; }
                }
                progressBar1.Value = 70;

                dbscantime.Start();
                int count_db = dbscan.Clustering(fordbscan, eps, minPTS);
                GetFullPicture(fordbscan, k, count_db);

                progressBar1.Value = 80;

                dbscantime.Stop();

                MessageBox.Show("Количество точек: " + points.Count + "\n" + "Время выполнения K-means: " + kmeantime.Elapsed.Seconds + " сек." + "\n" + "Время выполнения DBScan: " + dbscantime.Elapsed.Seconds + " сек." + "\n" + "Общее время выполнения: " + (kmeantime.Elapsed.Seconds + dbscantime.Elapsed.Seconds) + " сек." + "\n" + "Скорость выполнения: " + $"{((kmeantime.Elapsed.Seconds + dbscantime.Elapsed.Seconds > 0) ? (points.Count / (kmeantime.Elapsed.Seconds + dbscantime.Elapsed.Seconds)) : points.Count):F2} точки в сек.", "Время выполнения", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Graph.Graphic f1 = new Graph.Graphic(points, k,kmeantime,dbscantime, "Алгоритм кластеризации FastDbscan без ипользования R*-дерева");
                progressBar1.Value = 100;
                f1.Show();
            }
        }
    
        /// <summary>
        /// Обрабатываем каждый кластер K-means
        /// </summary>
        /// <param name="points">лист точек</param>
        /// <param name="k">количество кластеров k-means</param>
        /// <param name="count_db">количество кластеров в dbscan</param>
        void GetFullPicture(List<Point> points, int k, int count_db)
        {
            List<Point> cluster;
            for (int i = 0; i < k; i++)
            {
                cluster = new List<Point>(0);
                for (int j = 0; j < points.Count; j++)
                    if (points[j].cluster == i) cluster.Add(points[j]);
                WorkWithDb(cluster, i, count_db);
            }
            progressBar1.Value = 50;
        }

        /// <summary>
        /// В листе точек, принадлежащих одному кластеру K-means, ищем самый большой кластер dbscan - его принимаем за истинный, остальные как шум
        /// </summary>
        /// <param name="points">Лист точек, принадлежащих одному кластеру в K-means</param>
        /// <param name="clusterkmean">Количество кластеров K-means</param>
        /// <param name="count_db">Количество кластеров в dbscan</param>
        void WorkWithDb(List<Point> points, int clusterkmean, int count_db)
        {
            int count = 0;
            List<int> maxdb = new List<int>(0);
            for(int i=0;i<count_db;i++)
            {
                int counter = 0;
                for (int j = 0; j < points.Count; j++)
                    if (points[j].db == i) counter++;
                if (counter > count) { count = counter; maxdb.Clear(); maxdb.Add(i); }
                else if (counter == count) maxdb.Add(i);
            }
            
            for (int i = 0; i < points.Count; i++)
                if (!maxdb.Contains(points[i].db)) points[i].cluster = -1;
            progressBar1.Value = 70;
        }

        /// <summary>
        /// Открываем файл readme.txt блокнотом
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void readmetxtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string tmp = @"readme.txt";
            Process.Start("C:\\Windows\\System32\\notepad.exe", tmp.Trim());
        }

        
        
        /// <summary>
        /// Меняем булевскую переменную в зависимости от выбранного элемента из комбобокса
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == comboBox1.Items[0])
            {
                comboBox1.SelectedItem = comboBox1.Items[0];
                OnlyDBscan = false;
                textBox2.ReadOnly = false;
                textBox5.ReadOnly = false;
            }
            else
            {
                comboBox1.SelectedItem = comboBox1.Items[1];
                OnlyDBscan = true;
                textBox2.ReadOnly = true;
                textBox5.ReadOnly = true;
            }
        }

        /// <summary>
        /// Флажок для "Использовать R*-tree"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox1_Click(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                checkBox1.CheckState = CheckState.Checked;
                textBox6.ReadOnly = false;
                UseRTree = true;
            }
            else
            {
                checkBox1.CheckState = CheckState.Unchecked;
                textBox6.ReadOnly = true;
                UseRTree = false;
            }
        }
    }
}
