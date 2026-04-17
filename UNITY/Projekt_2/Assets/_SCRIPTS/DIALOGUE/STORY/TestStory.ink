EXTERNAL ChangeSphereColour(colour)

VAR Sphere_Colour = "Grey"
VAR New_Sphere_Colour = ""


===SphereTest===
{Sphere_Colour:
    -"Grey": ->Grey
    -"Blue": -> Blue
    -"Green": -> Green
}

=Grey
The sphere's colour is <incr><bounce>grey!</incr></bounce>

<br>

*[Make it blue!]
    ~New_Sphere_Colour = "Blue"
    ~ChangeSphereColour(New_Sphere_Colour)
    -> END
*[Make it green!]
    ~New_Sphere_Colour = "Green"
    ~ChangeSphereColour(New_Sphere_Colour)
    ->END
 
 =Blue
 The sphere's colour is <incr><bounce>blue!</incr></bounce>

<br>

*[Make it grey!]
    ~New_Sphere_Colour = "Grey"
    ~ChangeSphereColour(New_Sphere_Colour)
    -> END
*[Make it green!]
    ~New_Sphere_Colour = "Green"
    ~ChangeSphereColour(New_Sphere_Colour)
    -> END

 
 =Green
 The sphere's colour is <incr><bounce>green!</incr></bounce>

<br>

*[Make it blue!]
    ~New_Sphere_Colour = "Blue"
    ~ChangeSphereColour(New_Sphere_Colour)
    -> END
*[Make it grey!]
    ~New_Sphere_Colour = "Grey"
    ~ChangeSphereColour(New_Sphere_Colour)
    -> END