Bonjour,je suis chen zhihan avec hu xiangxing,jiang bin et hedi khalifa dans la groupe 7.

On a passé beaucoup de temps pour essayer de faire le projet car on a jamais fait ça et Pendant la cour ,il y a beaucoup de problèmes avec nos ordinateurs donc c vraiment dur pour nous.

On a essayé plusieur fois de faire avec spark mais malheureusement mais on n'a pas réussi. On a fait tourner le programme sur Visual Studio mais on en parvient pas à le faire tourner sous spark

Dans le dossier Groupe-7, nous avons utilisé l'outil or-tools pour résoudre le problème de sudoku, nous avons testé le programme avec un sudoku prédéfinis  en matrice 9X9 et nous avons réussi.

Par la suite dans le dossier MySparkApp, nous essayons de se connecter au serveur de Spark pour lire les données de sudoku dans le fichier soduko_easy50.txt en sauvegardant chaque ligne  dans un Dataframe.et nous avons choissi la première ligne pour le test.

nous avons réussi a faire tourner les 2 progammes séparement, mais quand nous les réunissions nous avons des erreurs, et nous ne savons pas quelle est la source de ce problème.

Voici ce qu'on obtient:

log4j:WARN No appenders could be found for logger (org.apache.spark.util.ShutdownHookManager).

log4j:WARN Please initialize the log4j system properly.

log4j:WARN See http://logging.apache.org/log4j/1.2/faq.html#noconfig for more info.





Voici le résultat obtenu par le programme group_7	:

8 6 1 4 5 9 7 2 3

4 5 2 3 1 7 6 9 8

7 9 3 6 8 2 5 1 4

2 1 6 8 3 5 4 7 9

9 8 4 2 7 6 1 3 5

3 7 5 1 9 4 8 6 2

5 4 7 9 2 1 3 8 6

1 3 9 5 6 8 2 4 7

6 2 8 7 4 3 9 5 1


Solutions: 1
WallTime: 72ms
Failures: 0
Branches: 0

quand on changer la data,le resulta comme ca:

1 7 6 9 2 3 5 8 4
5 2 4 8 1 7 6 3 9
8 9 3 6 5 4 2 7 1
9 5 7 3 4 8 1 6 2
6 3 8 1 9 2 4 5 7
4 1 2 7 6 5 3 9 8
2 6 5 4 8 9 7 1 3
7 8 1 2 3 6 9 4 5
3 4 9 5 7 1 8 2 6

3eme essaye:

6 3 9 2 1 8 4 5 7
4 7 1 5 3 9 2 6 8
8 2 5 6 7 4 1 3 9
5 6 4 8 2 3 7 9 1
7 9 3 4 5 1 8 2 6
2 1 8 7 9 6 3 4 5
3 5 2 9 8 7 6 1 4
1 8 6 3 4 5 9 7 2
9 4 7 1 6 2 5 8 3


Solutions: 1
WallTime: 93ms
Failures: 1
Branches: 2
Solutions: 1
WallTime: 95ms
Failures: 21

ca marche bien!

Pour obtenir les données par spark on a utilisé les commande suivant dans la MySparkApp:



export SPARK_HOME=/Users/hu/Desktop/spark
export PATH="$SPARK_HOME/bin:$PATH"
spark-submit --version

export DOTNET_WORKER_DIR=/Users/hu/Desktop/Microsoft.Spark.Worker-1.0.0


dotnet new console -o ESGF.Sudoku.Spark.RecursiveSearch

dotnet add package Microsoft.Spark

dotnet add package Google.OrTools


cd ESGF.Sudoku.Spark.RecursiveSearch


APRES RENTRER DANS LE FICHIER /Users/hu/ESGF.Sudoku.Spark.RecursiveSearch/Program.cs

REMPLACER LE CONTENU PAR :


using Microsoft.Spark.Sql;
using System;
using System.Diagnostics;
using static Microsoft.Spark.Sql.Functions;

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



ENREGISTRER

QUITTER



EXECUTER :

dotnet build



EXECUTER :



spark-submit \
--class org.apache.spark.deploy.dotnet.DotnetRunner \
--master local \
/Users/hu/ESGF.Sudoku.Spark.RecursiveSearch/bin/Debug/net5.0/microsoft-spark-3-0_2.12-1.0.0.jar \
donet /Users/hu/ESGF.Sudoku.Spark.RecursiveSearch/bin/Debug/net5.0/ESGF.Sudoku.Spark.RecursiveSearch.dll /Users/hu/ESGF.Sudoku.Spark.RecursiveSearch/input.txt





Nous avons procédez par un autre manière, voici le code:

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Google.OrTools.ConstraintSolver;

using Microsoft.Spark.Sql;
using Microsoft.Spark.Sql.Streaming;
using static Microsoft.Spark.Sql.Functions;

/*
* Groupe 7
* ZHIHAN CHEN
* BIN JIANG
* XIANGXIN HU
* HEDI KHALIFA
*/


// Solution
public class Sudoku
{

    /**
     *
     * Resoudre le probleme de SUDUKO
     *
     */
     private static int[,] get_data()

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
            //soduko_str.Show();
            spark.Stop();
            var rowstr = soduko_str;
            var initial_grid = new int[9, 9];//二维矩阵
            var colindex = 0;//列循环变量;
            var rowindex = 0;//行循环变量；
            foreach (var c in rowstr)
            {
                if (colindex >= 9)
                {
                    colindex = 0;
                    rowindex++;
                }
                if (rowindex >= 9)
                {
                    rowindex = 0;
                }
                initial_grid[rowindex, colindex] = int.Parse(c.ToString());
                colindex++;
            }
            return initial_grid;

        }






    private static void Solve()
    {
        Solver solver = new Solver("Sudoku");

        //
        // preparation des donnes
        //
        int cell_size = 3;
        IEnumerable<int> CELL = Enumerable.Range(0, cell_size);
        int n = cell_size * cell_size;
        IEnumerable<int> RANGE = Enumerable.Range(0, n);

        int[,] initial_grid = get_data();

        //
        // Decision variables
        //
        IntVar[,] grid = solver.MakeIntVarMatrix(n, n, 1, 9, "grid");
        IntVar[] grid_flat = grid.Flatten();

        //
        // Constraints
        //

        // Initialisation des lignes et colones
        foreach (int i in RANGE)
        {
            foreach (int j in RANGE)
            {
                if (initial_grid[i, j] > 0)
                {
                    solver.Add(grid[i, j] == initial_grid[i, j]);
                }
            }
        }


        foreach (int i in RANGE)
        {

            // Lignes
            solver.Add((from j in RANGE
                        select grid[i, j]).ToArray().AllDifferent());

            // colone
            solver.Add((from j in RANGE
                        select grid[j, i]).ToArray().AllDifferent());

        }

        // chaques cellules doivent etre differentes
        foreach (int i in CELL)
        {
            foreach (int j in CELL)
            {
                solver.Add((from di in CELL
                            from dj in CELL
                            select grid[i * cell_size + di, j * cell_size + dj]
                             ).ToArray().AllDifferent());
            }
        }


        //
        // Résolution à partir de l’outil de OrTool
        //
        DecisionBuilder db = solver.MakePhase(grid_flat,
                                              Solver.INT_VAR_SIMPLE,
                                              Solver.INT_VALUE_SIMPLE);

        solver.NewSearch(db);

        while (solver.NextSolution())
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    Console.Write("{0} ", grid[i, j].Value());
                }
                Console.WriteLine();
            }

            Console.WriteLine();
        }

        Console.WriteLine("\nSolutions: {0}", solver.Solutions());
        Console.WriteLine("WallTime: {0}ms", solver.WallTime());
        Console.WriteLine("Failures: {0}", solver.Failures());
        Console.WriteLine("Branches: {0} ", solver.Branches());

        solver.EndSearch();

    }


    public static void Main(String[] args)
    {

        Solve();
    }
}
