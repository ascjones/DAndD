namespace DAndD

module Levels =

    open DAndD.Model

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
                | []         -> grid |> List.rev
                | '#'::[]    -> loop (Blocked::row) grid []
                | c::' '::cs -> loop ((c |> toCell)::row) grid cs
                | '#'::'\013'::'\010'::cs ->
                    let row' = Blocked::row |> List.rev
                    loop [] (row'::grid) cs
                | cs -> failwithf "Failed to parse grid: invalid chars combo %A" <| new System.String(cs |> List.toArray)
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

