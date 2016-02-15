
module MathX.GeometryLinearAlgebra

open MathProvider
open MathProvider.LinearAlgebra

// rotation whose yaw, pitch, and roll are α, β, and γ, respectively
let Rotate x y z alpha beta gamma =
    let p = matrix [ [x];
                     [y];
                     [z] ]
    let RotX = matrix [ [1.0;0.0;0.0];
                        [0.0;(cos alpha);-(sin alpha)];
                        [0.0;(sin alpha);(cos alpha)] ]
    let RotY = matrix [ [(cos beta);0.0;(sin beta)];
                        [0.0;1.0;0.0];
                        [-(sin beta);0.0;(cos beta)] ]
    let RotZ = matrix [ [(cos gamma);-(sin gamma);0.0];
                        [(sin gamma);(cos gamma);0.0];
                        [0.0;0.0;1.0] ]
    RotX*RotY*RotZ*p;;

// x-y-z convention
let RotateEuler x y z alpha beta gamma =
    let p = matrix [ [x];
                     [y];
                     [z] ]
    let RotX = matrix [ [1.0;0.0;0.0];
                        [0.0;(cos alpha);-(sin alpha)];
                        [0.0;(sin alpha);(cos alpha)] ]
    let RotY = matrix [ [(cos beta);0.0;(sin beta)];
                        [0.0;1.0;0.0];
                        [-(sin beta);0.0;(cos beta)] ]
    let RotZ = matrix [ [(cos gamma);-(sin gamma);0.0];
                        [(sin gamma);(cos gamma);0.0];
                        [0.0;0.0;1.0] ]
    RotZ*RotY*RotX*p;;

// Rotate at arbitray axis
// - "x y z" the point to be rotated
// - "ux uy uz" the unit direction of the axis
// - "theta" the rotation angle
// REF: rotgen_Chapter9 Rotation About an Arbitrary Axis
let RotateAxis x y z ux uy uz (theta:float) =
    let c = cos theta
    let s = sin theta
    let t = 1.0 - c
    let m11 = t*ux*ux + c
    let m12 = t*ux*uy - s*uz
    let m13 = t*ux*uz + s*uy
    let m21 = t*ux*uy + s*uz
    let m22 = t*uy*uy + c
    let m23 = t*uy*uz - s*ux
    let m31 = t*ux*uz - s*uy
    let m32 = t*uy*uz + s*ux
    let m33 = t*uz*uz + c
    let R = matrix[[m11;m12;m13];
                   [m21;m22;m23];
                   [m31;m32;m33]]
    let v = matrix[[x];
                   [y];
                   [z]]
    let v_ = R*v
    v_;;

// Normalize
let Normalize (v:matrix) = 
    let n = v.Norm
    let nm = v * (1.0 / n)
    nm;;

// Distance from a point to a vector
// http://en.wikipedia.org/wiki/Distance_from_a_point_to_a_line
let DistPntVec (p:matrix) (vpHead:matrix) (vpTail:matrix)=
    let v = vpHead - vpTail
    let n = Normalize v
    let a = vpTail
    let ap = a - p
    let apn = (ap.Transpose * n)
    let proj = n * apn
    let perp = ap - proj
    perp.Norm;;