<Query Kind="Program" />

//https://www.hackerrank.com/challenges/dijkstrashortreach

static void Main()
{
    using (var file = File.Open(Util.CurrentQueryPath.Replace(".linq", ".txt"), FileMode.Open))
    using (var reader = new StreamReader(file))
    {
        Console.SetIn(reader);
		
		var stopwatch = Stopwatch.StartNew();
        Execute();
		stopwatch.Stop();
		stopwatch.Elapsed.Dump();
    }

    //Execute();
}

static void Execute()
{
    int testCount = int.Parse(Console.ReadLine());

    for (int i = 0; i < testCount; i++)
    {
        var edges = ParseGraph().ToList();
        int startingNode = int.Parse(Console.ReadLine()) - 1; //input is one based, we're zero based

        var dji = new Djikstra(edges, Djikstra.GraphEdgeType.Undirected);
        var idsToVertices = dji.Execute(startingNode);
        
        string line = string.Join(" ", idsToVertices.Values.Where(v => v.Weight != 0).Select(v => v.Weight == int.MaxValue ? -1 : v.Weight));
        Console.WriteLine(line);
    }
}

static IEnumerable<Djikstra.Edge> ParseGraph()
{
    var counts = Console.ReadLine().Split(' ').Select(int.Parse).ToList();
    int nodeCount = counts[0];
    int edgeCount = counts[1];
    
    for (int i = 0; i < edgeCount; i++)
    {
        var edge = Console.ReadLine().Split(' ').Select(int.Parse).ToArray();
        int node1 = edge[0] - 1; //input is one based, we're zero based
        int node2 = edge[1] - 1; //input is one based, we're zero based
        int weight = edge[2];

        yield return new Djikstra.Edge(node1, node2, weight);
    }
}

public class Djikstra
{
    private readonly IEnumerable<Edge> edges;

    public Djikstra(IEnumerable<Edge> edges, GraphEdgeType graphEdgeType)
    {
        if (graphEdgeType == GraphEdgeType.Directed)
        {
            this.edges = edges;
        }
        else
        {
            this.edges = edges.Concat(edges.Select(e => new Edge(e.To, e.From, e.Weight)));
        }
    }

    public IDictionary<int, Vertex> Execute(int start)
    {
        var vertices = new SortedDictionary<int, Vertex>();
        foreach (var edge in this.edges)
        {
            if (!vertices.ContainsKey(edge.From))
            {
                vertices.Add(edge.From, new Vertex(edge.From));
            }

            if (!vertices.ContainsKey(edge.To))
            {
                vertices.Add(edge.To, new Vertex(edge.To));
            }

            vertices[edge.From].Edges.Add(edge);
        }

        vertices[start].Weight = 0;

        bool[] visited = new bool[vertices.Count];
        
        var queue = new PriorityQueue<Vertex>(new VertexComparer());
        foreach (var kvp in vertices)
        {
            queue.Enqueue(kvp.Value);
        }

        while (!queue.IsEmpty)
        {
            var vertex = queue.Dequeue();
            visited[vertex.Id] = true;

            if (vertex.Weight != int.MaxValue)
            {
                foreach (Edge edge in vertex.Edges)
                {
                    if (visited[edge.To])
                        continue;

                    var neighbor = vertices[edge.To];
                    int alternateWeight = vertex.Weight + edge.Weight;

                    if (alternateWeight < neighbor.Weight)
                    {
                        queue.Remove(neighbor);
                        neighbor.Weight = alternateWeight;
                        neighbor.Previous = vertex;
                        queue.Enqueue(neighbor);
                    }
                }
            }
        }

        return vertices;
    }

    public class Edge
    {
        private readonly int from;
        private readonly int to;
        private readonly int weight;

        public Edge(int from, int to, int weight)
        {
            this.from = from;
            this.to = to;
            this.weight = weight;
        }

        public int From => this.from;
        public int To => this.to;
        public int Weight => this.weight;
    }

    public enum GraphEdgeType
    {
        Undirected,
        Directed
    }

    private class VertexComparer : IComparer<Vertex>
    {
        public int Compare(Vertex left, Vertex right)
        {
            int compare = left.Weight.CompareTo(right.Weight);
            if (compare == 0)
            {
                return left.Id.CompareTo(right.Id);
            }

            return compare;
        }
    }

    public class Vertex
    {
        private readonly int id;

        public Vertex(int id)
        {
            this.id = id;
        }

        public int Id => this.id;
        public int Weight { get; set; } = int.MaxValue;
        public List<Edge> Edges { get; } = new List<Edge>();
        public Vertex Previous { get; set; }

        public IEnumerable<int> GetPath()
        {
            Stack<int> ids = new Stack<int>();

            var vertex = this;
            while (vertex != null)
            {
                ids.Push(vertex.id);

                vertex = vertex.Previous;
            }

            return ids.ToList();
        }
    }

    private class PriorityQueue<T>
    {
        //Binary tree complexity instead of heap
        //Requires the comparer to avoid duplicates
        private readonly SortedSet<T> set;

        public PriorityQueue(IComparer<T> comparer = null)
        {
            this.set = new SortedSet<T>(comparer);
        }

        public void Enqueue(T item)
        {
            if (!this.set.Add(item))
            {
                throw new InvalidOperationException("Duplicates not currently allowed (use comparer)");
            }
        }

        public T Dequeue()
        {
            var item = this.set.FirstOrDefault();
            this.Remove(item);

            return item;
        }

        public void Remove(T item)
        {
            this.set.Remove(item);
        }

        public bool IsEmpty => this.set.Count == 0;
    }
}