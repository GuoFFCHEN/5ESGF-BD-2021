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
         //ici on peut pas obtenir la data viens la spark car la connection denied
         //donc on utilise une ligne pour tester la fonction
         SparkSession spark =
             SparkSession
                 .Builder()
                 .AppName("get_data")
                 .GetOrCreate();

         // Create initial DataFrame
         string filePath = @"/Users/hu/MySparkApp/input.txt";
         //string filePath = args[0];
         Console.WriteLine($"chemin du fichier : {filePath}");
         DataFrame dataFrame = spark.Read().Text(filePath);
         //on choisir la 1eme ligne pour tster;
         DataFrame soduko = dataFrame.Limit(1);
         var soduko_str = soduko.ToString();
         var rowstr = soduko_str;
         var initial_grid = new int[9, 9];//on creer une matrix
         var colindex = 0;//boucle pour la colonne
         var rowindex = 0;//boucle pour la liogne
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
