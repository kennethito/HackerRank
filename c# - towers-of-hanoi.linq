<Query Kind="Program" />

//https://www.hackerrank.com/challenges/gena

//3
//1 4 1
//outputs 3

//3
//1 3 3
//outputs 5

//5
//1 1 3 1 4
//outputs 11


public static void Main()
{
	var first = new LinkedList<int>();
	var second = new LinkedList<int>();
	var third = new LinkedList<int>();
	var fourth = new LinkedList<int>();

	var stacks = new[] { first, second, third, fourth };
	
	var disks = int.Parse(Console.ReadLine());
	var input = Console.ReadLine().Split(' ').Select(value => int.Parse(value));
	
	var poles = 
		input
			.Select((pole, index) => new { Pole = pole, Size = index + 1})
			.GroupBy(x => x.Pole)
			.Select(group => new { Pole = group.Key, Sizes = group.Select(x => x.Size).OrderByDescending(x => x).ToList()})
			.ToDictionary(x => x.Pole, x => x.Sizes);

	
	int poleIndex = 1;
	foreach (var stack in stacks)
	{
		List<int> sizes;
		if (poles.TryGetValue(poleIndex, out sizes))
		{
			foreach (int size in sizes)
			{
				stack.AddFirst(size);
			}
		}
		poleIndex++;
	}
	
	int steps = Solve(disks, int.MaxValue, 0, stacks, new HashSet<string>(), new List<string>());
	Console.WriteLine(steps);
}

public static int Solve(int disks, int bestSolution, int moves, IEnumerable<LinkedList<int>> poles, HashSet<string> visited, List<string> path)
{
	MarkVisited(visited, poles);
	
	if(moves > bestSolution)
		return -1;
	
	if(poles.First().Count == disks)
		return moves;
			
	foreach (var from in poles)
		foreach (var to in poles)
		{
			if (IsLegalMove(from, to) && !IsBadMove(disks, poles, from, to))
			{
				var state = Move(poles, from, to).ToList();

				if (!IsVisited(visited, state))
				{
					var newPath = new List<string>(path);
					newPath.Add(Fingerprint(state));
					
					int solution = Solve(disks, bestSolution, moves + 1, state, visited, newPath);
					bestSolution = solution < 0 ? bestSolution : Math.Min(bestSolution, solution);
				}
			}
		}
		
	return bestSolution;
}

public static bool IsVisited(HashSet<string> visited, IEnumerable<LinkedList<int>> poles)
{
	return visited.Contains(Fingerprint(poles));
}

public static void MarkVisited(HashSet<string> visited, IEnumerable<LinkedList<int>> poles)
{
	visited.Add(Fingerprint(poles));
}

public static bool IsLegalMove(LinkedList<int> from, LinkedList<int> to)
{
	return from != to && from.Count > 0 && (to.Count == 0 || to.First.Value > from.First.Value);
}

public static bool IsBadMove(int disks, IEnumerable<LinkedList<int>> poles, LinkedList<int> from, LinkedList<int> to)
{
	if (from == poles.First())
	{
		var values = from.Select(value => value).ToList();
		
		bool isBad = values.Last() == disks && values.SequenceEqual(Enumerable.Range(values.First(), values.Count));
		return isBad;
	}
	
	return false;
}

public static IEnumerable<LinkedList<int>> Move(IEnumerable<LinkedList<int>> poles, LinkedList<int> from, LinkedList<int> to)
{
	var value = from.First.Value;
	
	foreach (var pole in poles)
	{
		if (pole == from)
		{
			var newList = new LinkedList<int>(from);
			newList.RemoveFirst();
			yield return newList;
		}
		else if(pole == to)
		{
			var newList = new LinkedList<int>(to);
			newList.AddFirst(value);
			yield return newList;
		}
		else
			yield return pole;
		
	}
}

public static string Fingerprint(IEnumerable<LinkedList<int>> poles)
{
	string fingerprint = string.Join("|", poles.Select(pole => string.Join(",", pole.Select(val => val.ToString()))));
	
	return fingerprint;
}