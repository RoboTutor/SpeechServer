module MathX.Numerics

// Gradient Descent, Posted by Jon Harrop
// http://fsharpnews.blogspot.nl/2011/01/gradient-descent.html

let mutable precision = 1e-05

// we define a small number that we be used to calculate numerical approximations to derivatives:
let δ = 6.055454452e-06;;  // PowerPack : epsilon_float ** (1.0 / 3.0);;

// There is no built-in function that returns the third element of a triple, but you can easily write one as follows.
// http://msdn.microsoft.com/en-us/library/dd233200.aspx
let third (_, _, c) = c

// The following function repeatedly applies the given function 
//  to the given initial value until the result stops changing
let rec fixedPoint f x =
    let f_x = f x
    // End condition
    if f_x = x // original
    //if (third f_x) - (third x) < precision
    //if (third f_x) < precision
        then x 
        else fixedPoint f f_x;;

// The numerical approximation to the grad of a scalar field is built up 
//  from partial derivatives in each direction
let partialD f_xs f (xs : vector) i xi =
    xs.[i] <- xi + δ
    try 
        (f xs - f_xs) / δ 
    finally
        xs.[i] <- xi;;

// The following function performs a single iteration of gradient descent 
//  by scaling the step size λ by either α or β if the result increases 
//  or decreases the function being minimized, respectively
let descend α β f (f': _ -> vector) (λ, xs, f_xs) =
    let xs_2 = xs - λ * f' xs
    let f_xs_2 = f xs_2
    if f_xs_2 >= f_xs 
      then α * λ, xs, f_xs
      else β * λ, xs_2, f_xs_2;;

// Finally, the following function uses the gradient descent algorithm 
//  to minimize a given function and derivative
let gradientDescent f f' xs =
    let _, xs, _ = fixedPoint (descend 0.5 1.1 f f') (δ, xs, f xs)
    xs;;

// For example, the following computes a numerical approximation to the derivative of a function:
let grad f xs =
    Vector.mapi (partialD (f xs) f xs) xs;;


