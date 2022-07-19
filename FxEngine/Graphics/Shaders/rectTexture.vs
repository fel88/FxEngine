
#version 330 core
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec2 in_textureCoords;
out vec2 pass_textureCoords;

uniform mat4 transform;
uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main()
{
    //gl_Position = projection*view*model*vec4(aPos.x, aPos.y, aPos.z, 1.0);    
	gl_Position = transform*vec4(aPos.x, aPos.y, aPos.z, 1.0);    
	pass_textureCoords = in_textureCoords;

}