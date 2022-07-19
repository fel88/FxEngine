#version 330 core
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec2 texSDFCoords;

uniform mat4 projection; 
uniform mat4 transformation; 
varying vec2 v_texSDFCoords;

out vec3 ourColor;

void main()
{
	v_texSDFCoords = texSDFCoords;
	gl_Position =  ( projection*transformation)*vec4(aPos,1) ;	
    
    
		
}