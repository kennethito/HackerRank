<Query Kind="FSharpProgram" />

//https://www.hackerrank.com/challenges/bfsshortreach

open System.Collections.Generic
open System
open System.IO
open System.Linq


type Node = {
    Id:int
    mutable Nodes:Node list
    mutable Distance: int
}

type Edge = {
    Start:int
    End:int
}

type Test = {
    NodeCount:int
    Edges: Edge list
    StartNodeId:int
}

let ReadEdge() = 
    let line = Console.ReadLine()
    let segments = line.Split([|" "|], StringSplitOptions.RemoveEmptyEntries)
    {
        Start = segments.[0] |> Int32.Parse
        End = segments.[1] |> Int32.Parse
    }
    
let rec ReadEdges(count:int) = 
    if count <= 0 then 
        []
    else
        ReadEdge() :: ReadEdges(count - 1)

let ParseTest() = 
    let line = Console.ReadLine()
    let segments = line.Split([|" "|], StringSplitOptions.RemoveEmptyEntries)
    let nodeCount = segments.[0] |> Int32.Parse
    let edgeCount = segments.[1] |> Int32.Parse
    let edges = ReadEdges edgeCount
    let startNode = Console.ReadLine() |> Int32.Parse
    
    {
        NodeCount = nodeCount
        Edges = edges
        StartNodeId = startNode
    }

    
let rec ParseTests(count:int) = 
    if count <= 0 then 
        []
    else 
        ParseTest() :: ParseTests(count - 1)
        
let GenerateGraph(test:Test) =
    let edges = test.Edges
    let reversedEdges = test.Edges |> List.map (fun edge -> { Start = edge.End; End = edge.Start })

    let edgeMap = 
        List.concat [edges; reversedEdges]
        |> Seq.groupBy (fun edge -> edge.Start)
        |> Seq.map (fun (id, edges) -> (id, edges |> Seq.map (fun edge -> edge.End)))
        |> Map.ofSeq
        
    let nodes = 
        Enumerable.Range(1, test.NodeCount)
        |> List.ofSeq
        |> List.map (fun id -> { Id = id; Nodes = []; Distance = -1 })
    
    let nodeMap = 
        nodes
        |> List.map (fun node -> (node.Id, node))
        |> Map.ofList
        
    let getNodes(nodeId:int) = 
        if edgeMap |> Map.containsKey nodeId then
            edgeMap.[nodeId]
            |> Seq.map (fun id -> nodeMap.[id])
            |> List.ofSeq
        else
            []
        
    nodes
    |> List.iter (fun node -> node.Nodes <- (getNodes node.Id))
        
    let startNode =
        nodes 
        |> List.find (fun node -> node.Id = test.StartNodeId)
        
    (startNode, nodes) 
    
let SetDistances(node:Node) =
    let queue = Queue<Node>()
    
    node.Distance <- 0
    queue.Enqueue node
    
    while queue.Count > 0 do
        let current = queue.Dequeue()
        for n in current.Nodes do
            if n.Distance = -1 then
                queue.Enqueue n
                n.Distance <- current.Distance + 6
                
let RunTest(test:Test) =
    let start, graph = GenerateGraph test
    SetDistances start
    for node in graph do
        if node.Id <> start.Id then
            Console.Write (node.Distance.ToString() + " ")
    Console.WriteLine()

let testCount = Console.ReadLine() |> Int32.Parse
let tests = ParseTests testCount
tests |> List.iter RunTest


















