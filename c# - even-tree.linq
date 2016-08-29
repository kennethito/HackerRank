<Query Kind="Program" />

//https://www.hackerrank.com/challenges/even-tree

static void Main()
{
//    using (var file = File.Open(Util.CurrentQueryPath.Replace(".linq", ".txt"), FileMode.Open))
//    using (var reader = new StreamReader(file))
//    {
//        Console.SetIn(reader);
//        Execute();
//    }
    
    Execute();
}

public static void Execute()
{
    var counts = Console.ReadLine().Split(' ').Select(int.Parse).ToList();
    int verticesCount = counts[0];
    int edgesCount = counts[1];

    var nodes = GenerateNodes(verticesCount, edgesCount);
    int count = CountDecompositions(nodes[0]);

    Console.WriteLine(count);
}

public class Node
{
    public int Id { get; set; }
    public List<Node> Nodes { get; set; } = new List<Node>();
}

public static int CountDecompositions(Node node)
{
    if(node == null || node.Nodes.Count == 0)
        return 0;
        
    int decompositions = 0;
    
    for (int i = 0; i < node.Nodes.Count; i++)
    {
        var subNode = node.Nodes[i];
        var subCount = CountNodes(subNode);
        if (subCount % 2 == 0)
        {
            decompositions++;
        }
        decompositions += CountDecompositions(subNode);
    }
    
    return decompositions;
}

public static int CountNodes(Node node)
{
    if(node == null)
        return 0;
        
    return 1 + node.Nodes.Select(CountNodes).Sum();
}

public static List<Node> GenerateNodes(int nodeCount, int edgesCount)
{
    var idToNode =
        Enumerable.Range(1, nodeCount)
            .Select(id => new Node() { Id = id })
            .ToDictionary(node => node.Id, node => node);
            
    for (int i = 0; i < edgesCount; i++)
    {
        var edge = 
            Console.ReadLine()
                .Split(' ')
                .Select(int.Parse)
                .ToList();
        
        var child = idToNode[edge[0]];
        var parent = idToNode[edge[1]];
        
        parent.Nodes.Add(child);
    }
    
    return idToNode.Select(kvp => kvp.Value).ToList();
}

// Define other methods and classes here
