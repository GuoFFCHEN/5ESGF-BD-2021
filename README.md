Bonjour,je suis chen zhihan avec hu xiangxing,jiang bin et hedi khalifa dans la groupe 7.

on a fait beaucoup de temps pour essayer de faire ça .car on a jamais fait ça et Pendant la cour ,il'y a beaucoup de problèmes avec nos ordinateurs .donc c vraiment dur pour nous.

J'ai essayé plusieur fois de faire avec spark mais malheureusement je n'ai pas réussi ,je sais comment faire avec la function sans spark pour résoudre la soduko problem avec ortools,et qui est march bien,mais on sais pas comment utiliser la spark pour nous de faire ca.

Dans le dossier Groupe-7, nous avons utilisé l'outil or-tools pour résoudre le problème de sudoku, nous avons testé le programme avec un sudoku prédéfinis  en matrice 9X9 et nous avons réussi. Par la suite dans le dossier myspark_app, nous essayons de se connecter au serveur de Spark pour lire les données de sudoku dans le fichier soduko_easy50.txt en sauvegardant chaque ligne  dans un Dataframe.

la résulta est comme ca:

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



ICI c'est comment on fait sur la terminal



export SPARK_HOME=/Users/chenzhihan/spark

export PATH="$SPARK_HOME/bin:$PATH"

export DOTNET_WORKER_DIR=/Users/chenzhihan/sparkworker/Microsoft.Spark.Worker-1.0.0 


dotnet new console -o ESGF

dotnet add package Microsoft.Spark

dotnet add package Google.OrTools


cd ESGF


APRES RENTRER DANS LE FICHIER /Users/chenzhihan/ESGF/Program.cs

REMPLACER LE CONTENU PAR :


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

        // 0 Représente les inconnus du problème
        int[,] initial_grid = {{0, 6, 0, 0, 5, 0, 0, 2, 0},
                           {0, 0, 0, 3, 0, 0, 0, 9, 0},
                           {7, 0, 0, 6, 0, 0, 0, 1, 0},
                           {0, 0, 6, 0, 3, 0, 4, 0, 0},
                           {0, 0, 4, 0, 7, 0, 1, 0, 0},
                           {0, 0, 5, 0, 9, 0, 8, 0, 0},
                           {0, 4, 0, 0, 0, 1, 0, 0, 6},
                           {0, 3, 0, 0, 0, 8, 0, 0, 0},
                           {0, 2, 0, 0, 4, 0, 0, 5, 0}};


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


ENREGISTRER

QUITTER



EXECUTER :

dotnet build



EXECUTER :







spark-submit \
--class org.apache.spark.deploy.dotnet.DotnetRunner \
--master local \
/Users/chenzhihan/ESGF/bin/Debug/net5.0/microsoft-spark-3-0_2.12-1.0.0.jar \
dotnet /Users/chenzhihan/ESGF/bin/Debug/net5.0/ESGF.dll
