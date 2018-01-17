using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLib
{
    public class kmeans
    {

        public static int[] Cluster(List<Point> rawData, int numClusters, int seed)
        {
            //массив для итоговых значений кластеров
            int[] clustering = new int[rawData.Count];
            //копируем точки в новый лист
            List<Point> data = new List<Point>(0);
            for (int i = 0; i < rawData.Count; i++)
                data.Add(new Point(rawData[i].x, rawData[i].y));

            Normalized(data);

            bool changed = true;
            bool success = true;

            List<Point> means = InitMeans(numClusters, data, seed);
            Console.WriteLine("count" + means.Count);
            int maxCount = rawData.Count * 10;
            int ct = 0;

            while (changed == true && success == true && ct < maxCount) 
            {
                changed = UpdateClustering(data, means); 
                success = UpdateMeans(data, means); 
                ++ct;
            }
            for (int i = 0; i < data.Count; i++)
                clustering[i] = data[i].cluster;
            return clustering;

        }

        /// <summary>
        /// Инициализация центроидов для кластеризации
        /// </summary>
        /// <param name="numClusters">Количество кластеров</param>
        /// <param name="data">Точки</param>
        /// <param name="seed"></param>
        /// <returns>Вовзвращает лист центров</returns>
        static List<Point> InitMeans(int numClusters, List<Point> data, int seed)
        {
            List<Point> means = new List<Point>(numClusters);
            for (int i = 0; i < numClusters; i++)
                means.Add(new Point(0, 0));

            List<int> used = new List<int>(); 

            Random rnd = new Random(seed);
            int idx = rnd.Next(0, data.Count); 
            means.Add(data[idx]);

            used.Add(idx); 

            for (int k = 1; k < numClusters; k++) 
            {
                double[] dSquared = new double[data.Count]; 
                int newMean = -1; 
                for (int i = 0; i < data.Count; i++)
                {
                    if (used.Contains(i) == true) continue; 

                    double[] distances = new double[k]; 
                    for (int j = 0; j < k; ++j)
                        distances[j] = data[i].Distance(means[k]);

                    int m = MinIndex(distances);

                    dSquared[i] = distances[m] * distances[m];
                }


                double p = rnd.NextDouble();
                double sum = 0.0; 
                for (int i = 0; i < dSquared.Length; i++)
                    sum += dSquared[i];
                double cumulative = 0.0;

                int ii = 0; 
                int sanity = 0; 
                while (sanity < data.Count * 2) // 'stochastic acceptance'
                {
                    cumulative += dSquared[ii] / sum;
                    if (cumulative >= p && used.Contains(ii) == false)
                    {
                        newMean = ii; // the chosen index
                        used.Add(newMean); // don't pick again
                        break;
                    }
                    ++ii; // next candidate
                    if (ii >= dSquared.Length) ii = 0; // back to first item
                    ++sanity;
                }
                // check if newMean is still -1 . . . 

                // save the data of the chosen index
                means[k] = data[newMean];
            } 

            return means;

        } // InitMeans

        /// <summary>
        /// Нормализация
        /// </summary>
        /// <param name="rawData">Лист с точками</param>
        static void Normalized(List<Point> rawData)
        {
            for (int j = 0; j < 2; j++) 
            {
                double colSum = 0.0;//сумма всех точек по одной из координат (j==0)?x:y; 
                for (int i = 0; i < rawData.Count; i++)
                    if (j == 0) colSum += rawData[i].x; else colSum += rawData[i].y;
                double mean = colSum / rawData.Count;//среднее всех точек по одной из координат
                double sum = 0.0;
                for (int i = 0; i < rawData.Count; i++)
                    if (j==0) sum += (rawData[i].x - mean) * (rawData[i].x - mean); else sum += (rawData[i].y - mean) * (rawData[i].y - mean);

                double sd = sum / rawData.Count;
                for (int i = 0; i < rawData.Count; i++)
                    if (j == 1) rawData[i].xnorm = (rawData[i].x - mean) / sd; else rawData[i].ynorm = (rawData[i].y - mean) / sd;
            }
        }


        private static bool UpdateMeans(List<Point> data, List<Point> means)
        {
            int numClusters = means.Count;
            int[] clusterCounts = new int[numClusters];
            for (int i = 0; i < data.Count; i++)
            {
                int cluster = data[i].cluster;
                ++clusterCounts[cluster];
            }

            for (int k = 0; k < numClusters; k++)
                if (clusterCounts[k] == 0)
                    return false; // bad clustering. no change to means[][]

            // update, zero-out means so it can be used as scratch matrix 
            for (int k = 0; k < means.Count; k++)
                for (int j = 0; j < 2; j++)
                    if (j == 0) means[k].xnorm = 0.0; else means[k].ynorm = 0.0;

            for (int i = 0; i < data.Count; i++)
            {
                int cluster = data[i].cluster;
                for (int j = 0; j < 2; j++)
                    if (j == 0) means[cluster].xnorm += data[i].xnorm; else means[cluster].ynorm += data[i].ynorm;
            }

            for (int k = 0; k < means.Count; k++)
                for (int j = 0; j < 2; j++)
                    if (j==0) means[k].xnorm /= clusterCounts[k]; else means[k].ynorm /= clusterCounts[k];
            return true;
        }

        private static bool UpdateClustering(List<Point> data, List<Point> means)
        {
            int numClusters = means.Count;
            bool changed = false;

            int[] newClustering = new int[data.Count]; // proposed result
            for (int i = 0; i < data.Count; i++)
                newClustering[i] = data[i].cluster;

            double[] distances = new double[numClusters]; // from curr tuple to each mean

            for (int i = 0; i < data.Count; i++) // walk thru each tuple
            {
                for (int k = 0; k < numClusters; k++)
                    distances[k] = data[i].Distance(means[k]); // usually Euclidean

                int newClusterID = MinIndex(distances); // find closest mean ID
                                                        //Console.WriteLine("new cluster Id = " + newClusterID);
                                                        //Console.ReadLine();
                if (newClusterID != newClustering[i])
                {
                    changed = true;
                    newClustering[i] = newClusterID; // update
                }
            }

            if (changed == false)
                return false; // no change so bail and don't update clustering[][]

            // check proposed clustering[] cluster counts
            int[] clusterCounts = new int[numClusters];
            for (int i = 0; i < data.Count; i++)
            {
                int cluster = newClustering[i];
                ++clusterCounts[cluster];
            }

            for (int k = 0; k < numClusters; k++)
                if (clusterCounts[k] == 0)
                    return false; // bad clustering. no change to clustering[][]

            for (int i = 0; i < data.Count; i++)
                data[i].cluster = newClustering[i];

            return true;
        }

        private static int MinIndex(double[] distances)
        {
            // index of smallest value in array
            // helper for UpdateClustering()
            int indexOfMin = 0;
            double smallDist = distances[0];
            for (int k = 0; k < distances.Length; k++)
            {
                if (distances[k] < smallDist)
                {
                    smallDist = distances[k];
                    indexOfMin = k;
                }
            }
            return indexOfMin;
        }
    }
}
