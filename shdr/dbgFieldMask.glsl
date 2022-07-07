#version 330

// Input fragment attributes (from fragment shader)
in vec2 fragTexCoord;
in vec4 fragColor;


// Input uniform values
uniform sampler2D texture0;


// Output fragment color
out vec4 finalColor;

void main()
{

if (fragTexCoord.y <= 0.2 )
{
finalColor = vec4(1,0,0,1);
}


 
}