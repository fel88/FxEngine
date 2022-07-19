#version 330 core
uniform sampler2D texSDF;
out vec4 FragColor;
varying vec2 v_texSDFCoords;

void main()
{
	
		
	
	FragColor = texture2D(texSDF, v_texSDFCoords);
	
	
}