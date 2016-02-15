
module NaoJointChain.Head

open MathX.GeometryLinearAlgebra
open MathX.Numerics

let mutable TgX = 0.0
let mutable TgY = 0.0
let mutable TgZ = 0.0

let mutable CNT = 0;

let LookDir headpitch headyaw = 
    let initLkDir = matrix[[1.0];[0.0];[0.0]]
    let LkDirPitched = Rotate initLkDir.[0,0] initLkDir.[1,0] initLkDir.[2,0]
                              0.0 headpitch 0.0
    let LkDir = Rotate LkDirPitched.[0,0] LkDirPitched.[1,0] LkDirPitched.[2,0]
                       0.0 0.0 headyaw
    LkDir;;

// XS: [headpitch headyaw]
let LookAtTarget (XS:vector) = 
    let headpitch = XS.[0]
    let headyaw   = XS.[1]
    let ld = LookDir headpitch headyaw
    let endpoint = matrix[[0.0];[0.0];[0.0]]
    let p = matrix[[TgX];[TgY];[TgZ]]
    let d = DistPntVec p endpoint ld
    CNT <- CNT+1
    //printfn "Distance: %A Iteration: %A" d cnt
    d;;

// optimaze
let LookAt tarX tarY tarZ = 
    TgX <- tarX
    TgY <- tarY
    TgZ <- tarZ
    // XS: [headpitch headyaw]
    let xs =
        vector[0.0; -0.5]
        |> gradientDescent LookAtTarget (grad LookAtTarget)
    let finalD = LookAtTarget xs
    printfn "Looking Final Distance: %A TotalIteration: %A" finalD CNT
    CNT <- 0
    xs;;

// XS: [headpitch headyaw]
let DistractFromTarget (XS:vector) = 
    let headpitch = XS.[0]
    let headyaw   = XS.[1]
    let ld = LookDir headpitch headyaw
    let endpoint = matrix[[0.0];[0.0];[0.0]]
    let p = matrix[[TgX];[TgY];[TgZ]]
    let d = DistPntVec p endpoint ld
    CNT <- CNT+1
    //printfn "Distance: %A Iteration: %A" d cnt
    1.0/d;;

// optimaze
let DistractFrom tarX tarY tarZ = 
    TgX <- tarX
    TgY <- tarY
    TgZ <- tarZ
    // XS: [headpitch headyaw]
    let xs =
        vector[0.0; 0.5]
        |> gradientDescent DistractFromTarget (grad DistractFromTarget)
    let finalD = LookAtTarget xs // want to know D
    printfn "Distract Final Distance: %A TotalIteration: %A" finalD CNT
    CNT <- 0
    xs;;