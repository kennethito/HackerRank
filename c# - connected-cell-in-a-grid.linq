<Query Kind="Program" />

//https://www.hackerrank.com/challenges/connected-cell-in-a-grid
/*
5
4
0 0 1 1
0 0 1 0
0 1 1 0
0 1 0 0
1 1 0 0
outputs 8
*/

static void Main()
{
    var rows = int.Parse(Console.ReadLine());
    var columns = int.Parse(Console.ReadLine());
    
    int[,] matrix = new int[rows, columns];
    
    for (int i = 0; i < rows; i++)
    {
        int[] row = Console.ReadLine().Split(' ').Select(int.Parse).ToArray();
        for (int j = 0; j < columns; j++)
        {
            matrix[i, j] = row[j];
        }
    }
    
    int count = FindMaxRegionCount(matrix);
    Console.WriteLine(count);
}

static int FindMaxRegionCount(int[,] matrix)
{
    var regions = new List<HashSet<Tuple<int, int>>>();
    for (int row = 0; row < matrix.GetLength(0); row++)
    {
        for (int column = 0; column < matrix.GetLength(1); column++)
        {
            var acc = new HashSet<Tuple<int, int>>();
            FindRegion(matrix, row, column, new int[matrix.GetLength(0), matrix.GetLength(1)], acc);
            if (acc.Count > 0)
                regions.Add(acc);
        }
    }
    
    return regions.Max(r => r.Count);
}

static void FindRegion(int[,] matrix, int x, int y, int[,] visited, HashSet<Tuple<int, int>> acc)
{
    if(matrix[x, y] == 0)
        return;
    else
        acc.Add(Tuple.Create(x, y));
    
    visited[x, y] = 1;
    
    foreach (var dir in AllDirections(matrix, x, y))
    {
        if(visited[dir.Item1, dir.Item2] == 0 && matrix[dir.Item1, dir.Item2] == 1)
            FindRegion(matrix, dir.Item1, dir.Item2, visited, acc);
    }
}

static IEnumerable<Tuple<int, int>> AllDirections(int[,] matrix, int x, int y)
{
    int xMax = matrix.GetLength(0) - 1;
    int yMax = matrix.GetLength(1) - 1;
    
    if(x >= 1) yield return Tuple.Create(x - 1, y);
    if(x >= 1 && y >= 1) yield return Tuple.Create(x - 1, y - 1);
    if(y >= 1) yield return Tuple.Create(x, y - 1);
    if(x < xMax && y >= 1) yield return Tuple.Create(x + 1, y - 1);
    if(x < xMax) yield return Tuple.Create(x + 1, y);
    if(x < xMax && y < yMax) yield return Tuple.Create(x + 1, y + 1);
    if(y < yMax) yield return Tuple.Create(x, y + 1);
    if(x >= 1 && y < yMax) yield return Tuple.Create(x - 1, y + 1);
}
