// This file is a script that can be executed with the F# Interactive.  
// It can be used to explore and test the library project.
// Note that script files will not be part of the project build.

#r "FSharp.Powerpack.dll";;
#r "FSharp.Powerpack.Compatibility.dll";;

#nowarn "62";;

#load "Numerics.fs"
open Numerics

// And the following defines the famous Rosenbrock "banana" function 
//  that is notoriously difficult to minimize due to its curvature around the minimum
let rosenbrock (xs: vector) =
    let x, y = xs.[0], xs.[1]
    pown (1.0 - x) 2 + 100.0 * pown (y - pown x 2) 2;;

// The minimum at (1, 1) may be found quickly and easily using the functions defined above as follows:
let xs =
    vector[0.0; 0.0]
    |> gradientDescent rosenbrock (grad rosenbrock);;