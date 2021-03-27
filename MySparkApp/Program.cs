using Microsoft.Spark.Sql;
using System;
using System.Diagnostics;
using static Microsoft.Spark.Sql.Functions;
/*
* CHEN ZHIHAN
* JIANG BIN
* HU XIANGXING
* BEN HEDI KHALIFA
*/
namespace MySparkApp
{
    class Program
    {
        static void Main(string[] args)
        {
            get_data();
        }



        private static void get_data()

        {

            SparkSession spark =
                SparkSession
                    .Builder()
                    .AppName("get_data")
                    .GetOrCreate();

            // Create initial DataFrame
            string filePath = @"/Users/hu/MySparkApp/input.txt";
            //string filePath = args[0];
            Console.WriteLine($"chemin du fichier : {filePath}"); //chemin du fichier : C:\Users\vadaz\Documents\BIG_DATA\05\Big_Data_C_Computing\TP_Spark\MySparkApp\input.txt
            DataFrame dataFrame = spark.Read().Text(filePath);
            DataFrame soduko = dataFrame.Limit(1);
            var soduko_str = soduko.ToString();
            soduko_str.Show();
            spark.Stop();

        }





    }
}
