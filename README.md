# Résolution de sudokus avec Google.OrTools-


***** Le code se trouve dans le Program.cs*****


## 1- Introduction

Nous sommes le groupe 7 chargé de résoudre le problème de sudoku avec l'outil Or-tools. Notre groupe est constitué de : chen zhihan, hu xiangxing,jiang bin et hedi khalifa.

Pour le projet on s’est inspiré sur https://github.com/abdelkimo/or-tools/blob/master/examples/csharp/sudoku.cs pour intégrer le code de résolution de sudoku.


On a passé beaucoup de temps pour essayer de faire le projet car on a jamais fait ça et Pendant le cours ,il y a beaucoup de problèmes avec nos ordinateurs donc c vraiment dur pour nous.

On a essayé plusieurs fois de faire avec spark mais malheureusement mais on n'a pas réussi. On a fait tourner le programme sur Visual Studio mais on en parvient pas à le faire tourner sous spark

## 2- Lecture des données string sous spark et la mise en place de ces données en matrice 9x9.

Dans le dossier MySparkApp:

Dans un premier temps nous avons tourné le programme avec un sudoku prédéfinis sous forme de string. On adapté le code qui nous est fournis pour que le programme puisse traiter des chaines de caractères car on veut que le programme puisse lire les données d’un fichier qui contiens les sudokus. Le ligne de code suivant permet de mettre sous forme de matrice 9x9 les données des sudokus.

Voici les commandes utilisés pour lire les données sous spark：
```
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
            Console.WriteLine($"chemin du fichier : {filePath}");
            DataFrame dataFrame = spark.Read().Text(filePath);
            DataFrame soduko = dataFrame.Limit(1);
            var soduko_str = soduko.ToString();
            soduko_str.Show();
            spark.Stop();

        }

    }
}

```

Pour récupérer les données en string du sudoku et pour le mettre sous forme matricielle on a écris une fonction qui le permet, voici le code:
```
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
            Console.WriteLine($"chemin du fichier : {filePath}");
            DataFrame dataFrame = spark.Read().Text(filePath);
            //ici on choisis la 1eme ligne pour tster;
            DataFrame soduko = dataFrame.Limit(1);
            var soduko_str = soduko.ToString();
            //soduko_str.Show();
            spark.Stop();
            var rowstr = soduko_str;
            var initial_grid = new int[9, 9];//matrice à 2 dimension;
            var colindex = 0;//Variable de boucle de colonne;
            var rowindex = 0;//Variable de boucle de ligne；
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
```

## 3- résolution de sudoku


Par ailleurs, nous avons procéder à une résolution de sudoku grâce au outils or-tools dans le répertoire groupe_7, Voici le résultat obtenu par le programme group_7	:
```
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
```
### quand on changer la data,le resulta comme ca:
```
1 7 6 9 2 3 5 8 4

5 2 4 8 1 7 6 3 9

8 9 3 6 5 4 2 7 1

9 5 7 3 4 8 1 6 2

6 3 8 1 9 2 4 5 7

4 1 2 7 6 5 3 9 8

2 6 5 4 8 9 7 1 3

7 8 1 2 3 6 9 4 5

3 4 9 5 7 1 8 2 6

Solutions: 1
WallTime: 93ms
Failures: 1
Branches: 2
```
### 3eme essaye:
```
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
WallTime: 95ms
Failures: 21
```
ca marche bien!

## 4- Réalisation du programme final et de son exécution，sur MacOS


Dans le répertoire ESGF.Sudoku.Spark.RecursiveSearch, nous avons mis tout d'abord les codes qui permettent de récupérer les données string et de les transformer sous forme de matrice, ensuite on a intégré les codes qui utilisait l'outil Or-tools pour résoudre le sudoku. Ci-dessous se trouver les différent étapes réaliser pour l'exécution, en prenant note qu'il faut adapter le chemin de l'emplacement des fichiers.

```
export SPARK_HOME=/Users/hu/Desktop/spark
export PATH="$SPARK_HOME/bin:$PATH"
spark-submit --version


export DOTNET_WORKER_DIR=/Users/hu/Desktop/Microsoft.Spark.Worker-1.0.0


spark-submit \
--class org.apache.spark.deploy.dotnet.DotnetRunner \
--master local \
/Users/hu/ESGF.Sudoku.Spark.RecursiveSearch/bin/Debug/net5.0/microsoft-spark-3-0_2.12-1.0.0.jar \
donet /Users/hu/ESGF.Sudoku.Spark.RecursiveSearch/bin/Debug/net5.0/ESGF.Sudoku.Spark.RecursiveSearch.dll /Users/hu/ESGF.Sudoku.Spark.RecursiveSearch/input.txt

```

## 5- problème rencontré



Les exécutions distincts des programmes sont réussi, comme en témoigne le fichier log. Cependant à l'exécution du programme final, nous avons rencontré des problèmes liées au connection du serveur Spark, Spark a refusé l'accès. Nous avons essayer de chercher les solutions sur Google mais cela ne marchais pas, ci-dessous se trouve les erreur que nous avons rencontré:

```
log4j:WARN No appenders could be found for logger (org.apache.spark.util.ShutdownHookManager).

log4j:WARN Please initialize the log4j system properly.

log4j:WARN See http://logging.apache.org/log4j/1.2/faq.html#noconfig for more info.

```



Nous espérons que vous pourriez prends en compte de la situation réelle de notre groupe. La raison pour laquelle nous avons réalisé plusieurs programme est pour vous montrer que l'exécution distinct des programmes ne pose pas de problème, en revanche l'exécution du programme final nous donne des erreur. C'est dans ce but que nous avons créer 3 repértoires. Nous pouvons récupérer les données par  Spark, de les transformer en matrice pour la résolution, et de les résoudres. Merci de votre compréhension.
