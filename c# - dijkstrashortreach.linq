<Query Kind="Program" />

//https://www.hackerrank.com/challenges/dijkstrashortreach

static void Main()
{
    using (var file = File.Open(Util.CurrentQueryPath.Replace(".linq", ".txt"), FileMode.Open))
    using (var reader = new StreamReader(file))
    {
        Console.SetIn(reader);
        Execute();
    }

    //Execute();
}

static void Execute()
{
    int testCount = int.Parse(Console.ReadLine());

    for (int i = 0; i < testCount; i++)
    {
        var graph = ParseGraph();
        int startingNode = int.Parse(Console.ReadLine()) - 1; //input is one based, we're zero based

        var distances = CalculateDistances(graph, startingNode); 
        
        string line = string.Join(" ", distances.Where(d => d != 0).Select(d => d == int.MaxValue ? -1 : d));
        Console.WriteLine(line);
    }
}

static int[] CalculateDistances(int[,] graph, int startingNode)
{
    var distances = new int[graph.GetLength(0)];
    var visited = new bool[graph.GetLength(0)];
    
    for (int i = 0; i < distances.Length; i++)
    {
        distances[i] = int.MaxValue;
    }
    
    distances[startingNode] = 0;
    
    for (int node = 0; node < graph.GetLength(0); node++)
    {
        int next = FindMinimalNextNode(distances, visited);
        visited[next] = true;
        
        for (int edge = 0; edge < graph.GetLength(1); edge++)
        {
            if(visited[edge])
                continue;
            
            if (graph[next, edge] > 0 //connected
                    && distances[next] != int.MaxValue                              //don't overflow
                    && distances[next] + graph[next, edge] < distances[edge]) //lower cost route
            {
                distances[edge] = distances[next] + graph[next, edge];
            }
        }
    }
    
    return distances;
}

static int FindMinimalNextNode(int[] distances, bool[] visited)
{
    return
        distances
            .Select((d, i) => new { Index = i, Distance = d})
            .Where(x => !visited[x.Index])
            .OrderBy(x => x.Distance)
            .Select(x => x.Index)
            .First();
}

static int[,] ParseGraph()
{
    var counts = Console.ReadLine().Split(' ').Select(int.Parse).ToList();
    int nodeCount = counts[0];
    int edgeCount = counts[1];
    
    int[,] graph = new int[nodeCount, nodeCount];
    
    for (int i = 0; i < edgeCount; i++)
    {
        var edge = Console.ReadLine().Split(' ').Select(int.Parse).ToArray();
        int node1 = edge[0] - 1; //input is one based, we're zero based
        int node2 = edge[1] - 1; //input is one based, we're zero based
        int weight = edge[2];

        //multiple edges between the same nodes of differing weights, keep smallest
        if (graph[node1, node2] == 0 || graph[node1, node2] > weight)
        {
            graph[node1, node2] = weight;
            graph[node2, node1] = weight;
        }
    }
    
    return graph;
}



// Define other methods and classes here