namespace DAndD

module Levels =
    
    open DAndD.Contract

    let buildGrid (gridAscii : string) = 
        let toCell ch = 
            match ch with
            | '#' -> Blocked
            | ' ' -> Empty
            | 'b' -> ContainsItem(Bone)
            | 'g' -> ContainsItem(GoldCoin)
            | 's' -> ContainsItem(Scroll)
            | x -> failwithf "Unsupported character %A" x

        let gridChars = gridAscii.Trim().ToCharArray() |> Array.toList

        let cells = 
            let rec loop row grid chars =
                match chars with
                | []         -> grid |> List.rev |> List.toArray
                | '#'::[]    -> loop (Blocked::row) grid []
                | c::' '::cs -> loop ((c |> toCell)::row) grid cs
                | '#'::'\013'::'\010'::cs ->
                    let row' = Blocked::row |> List.rev |> List.toArray
                    loop [] (row'::grid) cs
                | cs -> failwithf "Failed to parse grid: invalid chars combo %A" <| new System.String(cs |> List.toArray)
            loop [] [] gridChars

        // todo: [AJ] could refactor this to build list in rec loop above
        seq {
            for y in [0..cells.Length - 1] do
                let row = cells.[y]
                for x in [0..row.Length - 1] do
                    yield { X = x; Y = y },cells.[y].[x]
        }

    let Level1 = 
        buildGrid @"
# # # # # # # #
# b     s     #
# # # # # b   #
#             #
#   g   # g   #
# # # # #     #
# s         b #
# # # # # # # #
"

