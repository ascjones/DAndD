namespace DAndD

module Levels =

    open Model

    let buildGrid (gridAscii : string) = 
        let toCell ch = 
            match ch with
            | '#' -> Blocked
            | ' ' -> Empty
            | 'b' -> ContainsItem(Bone)
            | 'g' -> ContainsItem(GoldCoin)
            | 's' -> ContainsItem(Scroll)
            | x -> failwithf "Unsupported character %A" x

        let gridChars = gridAscii.ToCharArray() |> Array.toList

        let cells = 
            let rec loop row grid chars =
                match chars with
                | [] -> grid |> List.rev
                | c::' '::cs -> 
                    let cell = c |> toCell
                    loop (cell::row) grid cs
                | '#'::'\010'::cs ->
                    let row' = row |> List.rev
                    loop [] (row'::grid) cs
                | cs -> failwithf "Failed to parse grid: invalid chars combo %A" cs
            loop [] [] gridChars

        let dim1 = cells |> Seq.map (fun c -> c |> Seq.length) |> Seq.max
        let dim2 = cells |> Seq.length

        Array2D.init dim1 dim2 (fun x y -> cells |> Seq.nth y |> Seq.nth x)

    let Level1 = 
        buildGrid @"
# # # # # # # #
#             #    
# # # # #     #
#             #
#       #     #
# # # # #     #
#             #
# # # # # # # #
" 

