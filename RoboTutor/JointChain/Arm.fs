// Learn more about F# at http://fsharp.net

module NaoJointChain.Arm

open MathX.GeometryLinearAlgebra
open MathX.Numerics

// kinematics REF: http://www.aldebaran-robotics.com/documentation/nao/hardware/kinematics/nao-links-33.html#nao-links-33
// unit: mm
let UpperArmLen = 105.0
let LowerArmLen = 55.95
let Hand_OffsetX = 57.75
let ElbowShoulder_OffsetY = 15.0   // 11
let TorsoShoulderY = 98.0
let TorsoShoulderZ = 100.0

let mutable RShoulderRoll = 0.0
let mutable RShoulderPitch = 0.0
let mutable RElbowRoll = 0.0
let mutable RElbowYaw = 0.0

let mutable TgX = 0.0
let mutable TgY = 0.0
let mutable TgZ = 0.0

let mutable CNT = 0;

let TranCoorRShoulder2Torso (v:matrix) = 
    v.[1, 0] <- v.[1, 0] - TorsoShoulderY
    v.[2, 0] <- v.[2, 0] + TorsoShoulderZ
    v;;

let LocateRElbow shoulderpitch shoulderroll =
    // NOTE: must first rotate roll, then the pitch axis does not change
    //       if rotate pitch first, then the roll axis will change!
    let vecElbowRolled = Rotate UpperArmLen -ElbowShoulder_OffsetY 0.0 
                                0.0 0.0 shoulderroll
    // then rotate pitch
    let vecElbowRollPitched = Rotate vecElbowRolled.[0,0] 
                                     vecElbowRolled.[1,0] 
                                     vecElbowRolled.[2,0] 
                                     0.0 shoulderpitch 0.0 
    // the vector rolled and pitched
    let vecElbow2Torso = TranCoorRShoulder2Torso vecElbowRollPitched
    vecElbow2Torso;;


let LocateRHand elbowyaw elbowroll shoulderpitch shoulderroll =
    // First rotate elbowRoll, so that other axes do not change 
    let vecHandRolled_ToElbow = Rotate (LowerArmLen+Hand_OffsetX) 0.0 0.0
                                       0.0 0.0 elbowroll
    let vecHandRollYawed_ToElbow = Rotate vecHandRolled_ToElbow.[0,0]
                                          vecHandRolled_ToElbow.[1,0]
                                          vecHandRolled_ToElbow.[2,0]
                                          elbowyaw 0.0 0.0
    let vecHand_ToShoulder = vecHandRollYawed_ToElbow + matrix[[UpperArmLen];[-ElbowShoulder_OffsetY];[0.0]]
    let vecHandRolled_ToShoulder = Rotate vecHand_ToShoulder.[0,0] 
                                          vecHand_ToShoulder.[1,0] 
                                          vecHand_ToShoulder.[2,0] 
                                          0.0 0.0 shoulderroll 
    let vecHand = Rotate vecHandRolled_ToShoulder.[0,0] 
                         vecHandRolled_ToShoulder.[1,0] 
                         vecHandRolled_ToShoulder.[2,0] 
                         0.0 shoulderpitch 0.0
    let vecHand2Torso = TranCoorRShoulder2Torso vecHand
    vecHand2Torso;;

// Pointing direction
let PointingDirection (elbow : matrix) (hand : matrix) = 
    let pd = hand - elbow
    pd;;

//// Palm direction: origin @ shoulder
//let PalmDirection wristyaw elbowyaw elbowroll shoulderpitch shoulderroll = 
//    // The end point of initial direction vector
//    let initPalmDirEnd = matrix[[Hand_OffsetX];[0.0];[-100.0]]
//    // rotate around wrist
//    let pdeW = Rotate initPalmDirEnd.[0,0] initPalmDirEnd.[1,0] initPalmDirEnd.[2,0] wristyaw 0.0 0.0
//    let pdeW0 = pdeW + matrix[[LowerArmLen];[0.0];[0.0]]
//    // rotate around elbow
//    let pdeWER = Rotate pdeW0.[0,0] pdeW0.[1,0] pdeW0.[2,0] 0.0 0.0 elbowroll
//    let pdeWEREY = Rotate pdeWER.[0,0] pdeWER.[1,0] pdeWER.[2,0] elbowyaw 0.0 0.0
//    // elbow -> shoulder
//    let pdeWEREY0 = pdeWEREY + matrix[[UpperArmLen];[-ElbowShoulder_OffsetY];[0.0]]
//    // rotate around shoulder
//    let pdeWEREYSR = Rotate pdeWEREY0.[0,0] pdeWEREY0.[1,0] pdeWEREY0.[2,0] 0.0 0.0 shoulderroll 
//    let pdeWEREYSRSP = Rotate pdeWEREYSR.[0,0] pdeWEREYSR.[1,0] pdeWEREYSR.[2,0] 0.0 shoulderpitch 0.0
//    //
//    let h = LocateRHand elbowyaw elbowroll shoulderpitch shoulderroll
//    //
//    let palmdir = pdeWEREYSRSP - h
//    //printfn "Palm Direction: %A" palmdir 
//    palmdir;;

// Palm direction: origin @ shoulder
let PalmDirection wristyaw elbowyaw elbowroll shoulderpitch shoulderroll = 
    // The end point of initial direction vector
    let initPalmDirEnd = matrix[[0.0];[0.0];[-1.0]]
    // rotate around wrist
    let pdeW = Rotate initPalmDirEnd.[0,0] initPalmDirEnd.[1,0] initPalmDirEnd.[2,0] wristyaw 0.0 0.0
    // rotate around elbow
    let pdeWER = Rotate pdeW.[0,0] pdeW.[1,0] pdeW.[2,0] 0.0 0.0 elbowroll
    let pdeWEREY = Rotate pdeWER.[0,0] pdeWER.[1,0] pdeWER.[2,0] elbowyaw 0.0 0.0
    // rotate around shoulder
    let pdeWEREYSR = Rotate pdeWEREY.[0,0] pdeWEREY.[1,0] pdeWEREY.[2,0] 0.0 0.0 shoulderroll 
    let pdeWEREYSRSP = Rotate pdeWEREYSR.[0,0] pdeWEREYSR.[1,0] pdeWEREYSR.[2,0] 0.0 shoulderpitch 0.0
    //printfn "Palm Direction: %A" palmdir 
    pdeWEREYSRSP;;

//////////////////////////////////////////////////////////////////////////
// 1. Compute the palm direction with X axis, return theta.
// for ElbowRoll and WristYaw
let mutable pd_ElbowYaw = 0.0
let mutable pd_ShoulderPitch = 0.0
let mutable pd_ShoulderRoll = 0.0
let mutable pd_CNT = 0
let PalmDirComp (XS:vector) = 
    let wristyaw = XS.[0]
    let elbowroll = XS.[1]
    let palmdir = PalmDirection wristyaw pd_ElbowYaw elbowroll pd_ShoulderPitch pd_ShoulderRoll
    pd_CNT <- pd_CNT+1
    // Calculate the angle with X axis
    let vec_product = matrix[[1.0; 0.0; 0.0]] * palmdir
    let cos_theta = vec_product.[0,0] / palmdir.Norm
    let theta = acos cos_theta
    //printfn "Palm Direction Angle@X: %A TotalIteration: %A   CurVal: %A   PalmDir: %A" theta pd_CNT XS palmdir
    //printfn "Palm Direction: %A" palmdir
    theta;;
    ////////////////////////////////////////////////////////////////////
    // return y
    //let y = abs palmdir.[1,0]
    //let z = abs palmdir.[2,0]
    //z+y;;

// Optimize wristyaw and elbowroll
let OptPalmDirComp elbowyaw shoulderpitch shoulderroll =
    pd_ElbowYaw <- elbowyaw
    pd_ShoulderPitch <- shoulderpitch
    pd_ShoulderRoll <- shoulderroll
    // XS: [wristyaw elbowroll]
    let xs =
        vector[-1.0; 1.5]
        |> gradientDescent PalmDirComp (grad PalmDirComp)
    let final_theta = PalmDirComp xs
    printfn "Palm Direction error @[1,0,0]: %A   TotalIteration: %A" final_theta pd_CNT
    pd_CNT <- 0
    xs;;

// 2. Optimize Only wristYaw
//let mutable pdwy_ElbowYaw = 0.0
let mutable pdwy_ElbowRoll = 0.0
let mutable pdwy_ShoulderPitch = 0.0
let mutable pdwy_ShoulderRoll = 0.0
let mutable pdwy_CNT = 0
let PalmDirCompWY (XS:vector) = 
    let wristyaw = XS.[0]
    let elbowyaw = XS.[1]
    let palmdir = PalmDirection wristyaw elbowyaw pdwy_ElbowRoll pdwy_ShoulderPitch pdwy_ShoulderRoll
    pdwy_CNT <- pdwy_CNT+1
    // Calculate the angle with X axis
    let vec_product = matrix[[1.0; 0.0; 0.0]] * palmdir
    let cos_theta = vec_product.[0,0] / palmdir.Norm
    let theta = acos cos_theta
    //printfn "Palm Direction Angle@X: %A TotalIteration: %A   CurVal: %A   PalmDir: %A" theta pd_CNT XS palmdir
    //printfn "Palm Direction: %A" palmdir
    theta;;
    ////////////////////////////////////////////////////////////////////
    // return y
    //let y = abs palmdir.[1,0]
    //let z = abs palmdir.[2,0]
    //z+y;;

// Optimize Only wristYaw
let OptPalmDirCompWY elbowroll shoulderpitch shoulderroll = 
    //pdwy_ElbowYaw <- elbowyaw
    pdwy_ElbowRoll <- elbowroll
    pdwy_ShoulderPitch <- shoulderpitch
    pdwy_ShoulderRoll <- shoulderroll
    // XS: [wristyaw]
    let xs =
        vector[-0.5; 0.5]
        |> gradientDescent PalmDirCompWY (grad PalmDirCompWY)
    let final_theta = PalmDirCompWY xs
    printfn "Palm Direction error @[1,0,0]: %A   TotalIteration: %A" final_theta pdwy_CNT
    pdwy_CNT <- 0
    xs;; 

// 3. Optimize Only ElbowYaw
let mutable pdey_WristYaw = 0.0
let mutable pdey_ElbowRoll = 0.0
let mutable pdey_ShoulderPitch = 0.0
let mutable pdey_ShoulderRoll = 0.0
let mutable pdey_CNT = 0
let PalmDirCompEY (XS:vector) = 
    let elbowyaw = XS.[0]
    let palmdir = PalmDirection pdey_WristYaw elbowyaw pdwy_ElbowRoll pdwy_ShoulderPitch pdwy_ShoulderRoll
    pdey_CNT <- pdey_CNT+1
    // Calculate the angle with X axis
    let vec_product = matrix[[1.0; 0.0; 0.0]] * palmdir
    let cos_theta = vec_product.[0,0] / palmdir.Norm
    let theta = acos cos_theta
    //printfn "Palm Direction Angle@X: %A TotalIteration: %A   CurVal: %A   PalmDir: %A" theta pd_CNT XS palmdir
    //printfn "Palm Direction: %A" palmdir
    theta;;
    ////////////////////////////////////////////////////////////////////
    // return y
    //let y = abs palmdir.[1,0]
    //let z = abs palmdir.[2,0]
    //z+y;;

// Optimize Only ElbowYaw
let OptPalmDirCompEY wristyaw elbowroll shoulderpitch shoulderroll = 
    pdey_WristYaw <- wristyaw
    pdey_ElbowRoll <- elbowroll
    pdey_ShoulderPitch <- shoulderpitch
    pdey_ShoulderRoll <- shoulderroll
    // XS: [Elbowyaw]
    let xs =
        vector[1.5]
        |> gradientDescent PalmDirCompEY (grad PalmDirCompEY)
    let final_theta = PalmDirCompEY xs
    printfn "Palm Direction error @[1,0,0]: %A   TotalIteration: %A" final_theta pdey_CNT
    pdey_CNT <- 0
    xs;; 

// 4. Compute the palm direction with X axis, return theta.
// for ElbowYaw and WristYaw
let mutable pdEYWY_ElbowRoll = 0.0
let mutable pdEYWY_ShoulderPitch = 0.0
let mutable pdEYWY_ShoulderRoll = 0.0
let mutable pdEYWY_CNT = 0
let PalmDirCompEYWY (XS:vector) = 
    let wristyaw = XS.[0]
    let elbowyaw = XS.[1]
    let palmdir = PalmDirection wristyaw elbowyaw pdEYWY_ElbowRoll pdEYWY_ShoulderPitch pdEYWY_ShoulderRoll
    pdEYWY_CNT <- pdEYWY_CNT+1
    // Calculate the angle with X axis
    let vec_product = matrix[[1.0; 0.0; 0.0]] * palmdir
    let cos_theta = vec_product.[0,0] / palmdir.Norm
    let theta = acos cos_theta
    //printfn "Palm Direction Angle@X: %A TotalIteration: %A   CurVal: %A   PalmDir: %A" theta pd_CNT XS palmdir
    //printfn "Palm Direction: %A" palmdir
    theta;;
    ////////////////////////////////////////////////////////////////////
    // return y
    //let y = abs palmdir.[1,0]
    //let z = abs palmdir.[2,0]
    //z+y;;

// Optimize ElbowYaw and WristYaw
let OptPalmDirCompEYWY elbowroll shoulderpitch shoulderroll =
    pdEYWY_ElbowRoll <- elbowroll
    pdEYWY_ShoulderPitch <- shoulderpitch
    pdEYWY_ShoulderRoll <- shoulderroll
    // XS: [wristyaw elbowyaw]
    let xs =
        vector[-1.0; 1.5]
        |> gradientDescent PalmDirCompEYWY (grad PalmDirCompEYWY)
    let final_theta = PalmDirCompEYWY xs
    printfn "Palm Direction error @[1,0,0]: %A   TotalIteration: %A" final_theta pdEYWY_CNT
    pdEYWY_CNT <- 0
    xs;;

// 5. Compute the palm direction with X axis, return theta.
// for ElbowYaw, ElbowRoll and WristYaw
let mutable pdEYERWY_ShoulderPitch = 0.0
let mutable pdEYERWY_ShoulderRoll = 0.0
let mutable pdEYERWY_CNT = 0
let mutable pdEYERWY_ExpDir = [1.0;0.0;0.0;];
let PalmDirCompEYERWY (XS:vector) = 
    let wristyaw = XS.[0]
    let elbowyaw = XS.[1]
    let elbowroll = XS.[2]
    let palmdir = PalmDirection wristyaw elbowyaw elbowroll pdEYERWY_ShoulderPitch pdEYERWY_ShoulderRoll
    pdEYERWY_CNT <- pdEYERWY_CNT+1
    // Calculate the angle with X axis
    //let vec_product = matrix[[1.0; 0.0; 0.0]] * palmdir
    let vec_product = matrix[pdEYERWY_ExpDir] * palmdir
    let cos_theta = vec_product.[0,0] / palmdir.Norm
    let theta = acos cos_theta
    //printfn "Palm Direction Angle@X: %A TotalIteration: %A   CurVal: %A   PalmDir: %A" theta pd_CNT XS palmdir
    //printfn "Palm Direction: %A" palmdir
    theta;;
    ////////////////////////////////////////////////////////////////////
    // return y
    //let y = abs palmdir.[1,0]
    //let z = abs palmdir.[2,0]
    //z+y;;

// Optimize ElbowYaw, ElbowRoll and WristYaw
let OptPalmDirCompEYERWY shoulderpitch shoulderroll palmexpdir init_vec =
    pdEYERWY_ShoulderPitch <- shoulderpitch
    pdEYERWY_ShoulderRoll <- shoulderroll
    pdEYERWY_ExpDir <- palmexpdir
    // XS: [wristyaw elbowyaw elbowroll]
    let xs =
        //vector[-1.0; 1.5; 1.5]
        //vector[-1.5; 0.0; 0.8]
        init_vec
        |> gradientDescent PalmDirCompEYERWY (grad PalmDirCompEYERWY)
    let final_theta = PalmDirCompEYERWY xs
    printfn "Palm Direction error @[1,0,0]: %A   TotalIteration: %A" final_theta pdEYERWY_CNT
    pdEYERWY_CNT <- 0
    xs;;

//////////////////////////////////////////////////////////////////////////







//////////////////////////////////////////////////////////////////////////
// Pointing
//////////////////////////////////////////////////////////////////////////
// Here the question is Knowing "elbowRoll" (don't care wristYaw ) and "target" position,
// how to change "shoulderYaw shoulderPitch elbowYaw" to put the hand pointing at the target?
//  - "shoulderYaw Roll" determine the position of elbow
//  - then "elbowYaw" determine the hand direction
//////////////////////////////////////////////////////////////////////////
// Solution 1
// XS: [shoulderpitch shoulderroll elbowyaw]
let PointingTo3J (XS:vector) = 
    let shoulderpitch = XS.[0]
    let shoulderroll = XS.[1]
    let elbowyaw = XS.[2]
    let hand = LocateRHand elbowyaw RElbowRoll shoulderpitch shoulderroll
    let elbow = LocateRElbow shoulderpitch shoulderroll
    //let v = PointingDirection elbow hand
    let p = matrix[[TgX];[TgY];[TgZ]]
    let d = DistPntVec p hand elbow
    CNT <- CNT+1
    //printfn "Distance: %A Iteration: %A" d cnt
    d;;

// optimaze
let OptPntTo3J elbowroll tarX tarY tarZ = 
    RElbowRoll <- elbowroll
    TgX <- tarX
    TgY <- tarY
    TgZ <- tarZ
    // XS: [shoulderpitch shoulderroll elbowyaw]
    let xs =
        vector[1.0; -1.0; 0.0]
        |> gradientDescent PointingTo3J (grad PointingTo3J)
    let finalD = PointingTo3J xs
    printfn "Pointing Final Distance: %A TotalIteration: %A" finalD CNT
    CNT <- 0
    xs;;

// optimaze
let OptPntTo3JSeed elbowroll tarX tarY tarZ seedShldPtch seedShldRoll seedElbYaw= 
    RElbowRoll <- elbowroll
    TgX <- tarX
    TgY <- tarY
    TgZ <- tarZ
    // XS: [shoulderpitch shoulderroll elbowyaw]
    let xs =
        vector[seedShldPtch; seedShldRoll; seedElbYaw]
        |> gradientDescent PointingTo3J (grad PointingTo3J)
    let finalD = PointingTo3J xs
    printfn "Pointing Final Distance: %A TotalIteration: %A" finalD CNT
    CNT <- 0
    xs;;
//////////////////////////////////////////////////////////////////////////

//////////////////////////////////////////////////////////////////////////    
// Solution 2 fixed elbowYaw = 80 degree
// elbow: Roll <-> Yaw   REF: PalmDirection.docx
 

// XS: [shoulderpitch shoulderroll]
let PointingTo2J (XS:vector)  = 
    let shoulderpitch = XS.[0]
    let shoulderroll = XS.[1]
    let hand = LocateRHand RElbowYaw RElbowRoll shoulderpitch shoulderroll
    let elbow = LocateRElbow shoulderpitch shoulderroll
    //let v = PointingDirection elbow hand
    let p = matrix[[TgX];[TgY];[TgZ]]
    let d = DistPntVec p hand elbow
    CNT <- CNT+1
    //printfn "Distance: %A Iteration: %A" d cnt
    d;;

// optimaze
let OptPntTo2J elbowroll elbowyaw tarX tarY tarZ = 
    RElbowRoll <- elbowroll
    RElbowYaw <- elbowyaw
    TgX <- tarX
    TgY <- tarY
    TgZ <- tarZ
    // XS: [shoulderpitch shoulderroll]
    let xs =
        // Different initial value could give different results
        //  in terms of vector direction
        // vector[1.0; -1.0] // target on the floor
        vector[-0.5; -0.5] //exp target in the air
        //vector[1.5; -1.0]    // screen
        |> gradientDescent PointingTo2J (grad PointingTo2J)
    let finalD = PointingTo2J xs
    printfn "Pointing Final Distance: %A TotalIteration: %A" finalD CNT
    CNT <- 0
    xs;;

// optimaze with initial seeds
let OptPntTo2JSeed elbowroll elbowyaw tarX tarY tarZ seedShldPtch seedShldRoll = 
    RElbowRoll <- elbowroll
    RElbowYaw <- elbowyaw
    TgX <- tarX
    TgY <- tarY
    TgZ <- tarZ
    // XS: [shoulderpitch shoulderroll]
    let xs =
        // Different initial value could give different results
        //  in terms of vector direction
        vector[seedShldPtch; seedShldRoll]
        //vector[1.0; -1.0]  // target on the floor
        //vector[-0.5; -0.5] // exp target in the air
        //vector[1.5; -1.0]  // screen
        |> gradientDescent PointingTo2J (grad PointingTo2J)
    let finalD = PointingTo2J xs
    printfn "Pointing Final Distance: %A TotalIteration: %A" finalD CNT
    CNT <- 0
    xs;;
//////////////////////////////////////////////////////////////////////////

//////////////////////////////////////////////////////////////////////////
// Solution 3 unfinished! should NOT be used!
// elbow: Roll <-> Yaw   REF: PalmDirection.docx

// XS: [shoulderpitch shoulderroll]
let PointingTo2JPalm (XS:vector)  = 
    let shoulderpitch = XS.[0]
    let shoulderroll = XS.[1]
    // need PalmDir->y = 0
    let elbowYaw  = 0.0 // unfinished!
    let elbowRoll = 0.0 // unfinished!
    let hand = LocateRHand elbowYaw elbowRoll shoulderpitch shoulderroll
    let elbow = LocateRElbow shoulderpitch shoulderroll
    //let v = PointingDirection elbow hand
    let p = matrix[[TgX];[TgY];[TgZ]]
    let d = DistPntVec p hand elbow
    CNT <- CNT+1
    //printfn "Distance: %A Iteration: %A" d cnt
    d;;

// optimaze
let OptPntTo2JPalm tarX tarY tarZ = 
    TgX <- tarX
    TgY <- tarY
    TgZ <- tarZ
    // XS: [shoulderpitch shoulderroll]
    let xs =
        vector[1.0; -1.0]
        |> gradientDescent PointingTo2JPalm (grad PointingTo2JPalm)
    let finalD = PointingTo2JPalm xs
    printfn "Pointing Final Distance: %A TotalIteration: %A" finalD CNT
    CNT <- 0
    xs;;
//////////////////////////////////////////////////////////////////////////
