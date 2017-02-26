<Query Kind="FSharpProgram" />

open System
open System.IO

let SplitAs (conversion:string -> 'T) (s:string) = 
    s.Split([|" "|], StringSplitOptions.RemoveEmptyEntries)
    |> Array.map conversion

let input = Console.ReadLine() |> SplitAs int

let total = input.[0]
let coinTypeCount = input.[1]
let coinTypes = Console.ReadLine() |> SplitAs int |> Array.sortBy (~-)

let cache = ref Map.empty
let Memoize f =
    fun x ->
        match (!cache).TryFind(x) with
        | Some res -> res
        | None ->
            let res = f x
            cache := (!cache).Add(x,res)
            res

let rec CountCombinations (coinTypes:int list, total:int) = 
    match coinTypes, total with
    | [], _ -> 0
    | _, total when total < 0 -> 0
    | _, total when total = 0 -> 1
    | largest::rest, total -> 
        (Memoize CountCombinations)(rest, total)
        + (Memoize CountCombinations)(coinTypes, total - largest)
        
CountCombinations (List.ofArray coinTypes, total)
|> Console.WriteLine
