﻿
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Google.OrTools.ConstraintSolver;



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
        //ici on peut pas obtenir la data viens la spark car la connection denied
        //donc on utilise une ligne pour tester la fonction
        var soduko_str = "100920000524010000000000070050008102000000000402700090060000000000030945000071006";
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
    //ici on defini la solution
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
        //ici on obtenir la donne viens la fonction get_data()
        int[,] initial_grid = get_data();

        //ou on peut juste specifique une  matrix
        // 0 Représente les inconnus du problème
        //int[,] initial_grid = {{0, 6, 0, 0, 5, 0, 0, 2, 0},
        //                   {0, 0, 0, 3, 0, 0, 0, 9, 0},
        //                   {7, 0, 0, 6, 0, 0, 0, 1, 0},
        //                   {0, 0, 6, 0, 3, 0, 4, 0, 0},
        //                   {0, 0, 4, 0, 7, 0, 1, 0, 0},
        //                   {0, 0, 5, 0, 9, 0, 8, 0, 0},
        //                   {0, 4, 0, 0, 0, 1, 0, 0, 6},
        //                   {0, 3, 0, 0, 0, 8, 0, 0, 0},
        //                   {0, 2, 0, 0, 4, 0, 0, 5, 0}};


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
