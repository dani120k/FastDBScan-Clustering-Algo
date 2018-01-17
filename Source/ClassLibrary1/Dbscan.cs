using System.Collections.Generic;


namespace MyLib
{
    public class dbscan
    {
         static double eps;
         static int minPTS;

        public static int Clustering(List<Point> points, double e, int mpts)
        {
            eps = e;
            minPTS = mpts;
            int cluster_number = 0;
            CheckAllPoint(points);
            for(int i=0; i < points.Count; i++)
            {
                //points[i].dbscan_result = true;
                if (points[i].IsVisited == false && points[i].minPTScheck)
                {
                    points[i].IsVisited = true;
                    points[i].db = cluster_number;
                    StartClusterization(points, points[i],cluster_number);
                    cluster_number++;
                }
            }
            return cluster_number;
        }

        static void CheckAllPoint(List<Point> points)
        {
            for(int i=0;i<points.Count;i++)
            {
                int count = -1;
                for (int j = 0; j < points.Count; j++)
                    if (points[i].Distance(points[j]) <= eps) count++;
                if (count >= minPTS) points[i].minPTScheck = true;
            }
        }


        static void StartClusterization(List<Point> points, Point p, int cluster)
        {
            for(int i=0;i<points.Count;i++)
            {
                if (points[i].IsVisited == false && p.Distance(points[i]) <= eps && points[i].minPTScheck) { points[i].db = cluster; points[i].IsVisited = true; StartClusterization(points, points[i],cluster); }
            }
        }
    }
}
