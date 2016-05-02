<Query Kind="FSharpProgram" />

//https://www.hackerrank.com/challenges/fp-list-replication

open System

let rec readData(): int list = 
    let (success, data) = Console.ReadLine() |> Int32.TryParse
    if not success then
        []
    else
        data :: readData()
        
let rec writeXTimes x (value:int) = 
    if x > 0 then
        Console.WriteLine value
        writeXTimes (x - 1) value

let rec writeData repeated data = 
    match data with
    | x::xs -> 
        writeXTimes repeated x
        writeData repeated xs
    | [] -> ()
    
let repeat = Console.ReadLine() |> Int32.Parse
let data = readData() 
writeData repeat data