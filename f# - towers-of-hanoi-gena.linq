<Query Kind="FSharpProgram" />

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

open System
open System.Linq
open System.IO

let Fingerprint(poles: int list list):string =
    String.Join("|", poles.Select(fun pole -> String.Join(",", pole.Select(fun value -> value.ToString()))));
    
let IsVisited(visited:Set<string>)(poles: int list list):bool =
    visited |> Set.contains(Fingerprint poles)

let MarkVisited(visited:Set<string>)(poles: int list list):Set<string> =
    visited |> Set.add(Fingerprint poles)

let IsLegalMove(from:int list, dest:int list):bool =
    from <> dest && from.Length > 0 && (dest.Length = 0 || dest.Head > from.Head);
    
let IsBadMove(disks:int, poles:int list list, from:int list, dest:int list) =
    let rec isIdeal(pole:int list):bool = 
        match pole with
        | head :: next :: rest ->
            if next - head = 1 then (isIdeal (next::rest))
            else false
        | _ -> true

    if from = poles.Head then
        from.Last() = disks && isIdeal from
    else false
    
let MoveHead(poles: int list list, fromIndex:int, destIndex:int) =
    let value = 
        poles 
        |> List.mapi(fun index pole -> (index, pole)) 
        |> List.filter(fun (index, pole) -> index = fromIndex)
        |> List.map snd
        |> List.head
        |> List.head
        
    let updated = 
        seq {
            let mutable index = 0
            
            for pole in poles do
                if index = fromIndex then
                    yield pole.Tail
                elif index = destIndex then
                    yield value::pole
                else
                    yield pole
                    
                index <- index + 1
        }
        |> List.ofSeq
    updated

let rec Solve(disks:int, bestSolution:int, movesAcc:int, poles:int list list, visited:Set<string>, history:string list) =
    if movesAcc > bestSolution then 
        -1
    elif poles.Head.Length = disks then 
        movesAcc
    else
        let mutable bestSoFar = bestSolution
        let mutable fromIndex = 0
        for from in poles do
            let mutable destIndex = 0
            for dest in poles do
                if IsLegalMove(from, dest) && not <| IsBadMove(disks, poles, from, dest) then
                    let moved = MoveHead(poles, fromIndex , destIndex)
                    let fingerprint = Fingerprint moved
                    
                    if not <| IsVisited visited moved then
                        let solution = Solve(disks, bestSoFar, movesAcc + 1, moved, MarkVisited visited poles, fingerprint::history);
                        bestSoFar <- if solution < 0 then bestSoFar else Math.Min(bestSoFar, solution);
                destIndex <- destIndex + 1
            fromIndex <- fromIndex + 1
        bestSoFar


let disks = Console.ReadLine() |> Int32.Parse
let input = Console.ReadLine().Split(' ') |> Seq.map(fun value -> Int32.Parse(value));

let polesToDiskSizes = 
    input
    |> Seq.mapi (fun index pole -> (pole, index + 1))
    |> Seq.groupBy (fun (pole, size) -> pole)
    |> Seq.map (fun (pole, groups) -> (pole, groups |> Seq.map snd |> Seq.sortBy (fun i -> i)))
    |> Map.ofSeq

let poles = 
    [1..4]
    |> List.map (fun poleIndex -> 
        let option = polesToDiskSizes.TryFind poleIndex
        if option.IsSome then option.Value |> List.ofSeqi
        else [])
       

let steps = Solve(disks, 15, 0, poles, Set.empty<string>, []);
Console.WriteLine(steps);