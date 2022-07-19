
#version 330 core
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec2 in_textureCoords;
layout (location = 2) in vec3 in_normal;
out vec2 pass_textureCoords;
out vec3 pass_normal;

uniform mat4 transform;


void main()
{
    gl_Position = transform*vec4(aPos.x, aPos.y, aPos.z, 1.0);
	pass_normal = in_normal.xyz;
		pass_textureCoords = in_textureCoords;

}