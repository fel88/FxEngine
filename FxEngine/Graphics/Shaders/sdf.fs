#version 330 core
uniform sampler2D texSDF;
out vec4 FragColor;
varying vec2 v_texSDFCoords;
uniform float gammaSDF;
uniform vec3 color;

void main()
{		
	FragColor=vec4(color,1);	
	float sdf_dist = texture2D(texSDF, v_texSDFCoords).a;
	float sdf_alpha = smoothstep(0.47-gammaSDF, 0.47+gammaSDF, sdf_dist);		
	FragColor.a = FragColor.a * sdf_alpha;	
	
}