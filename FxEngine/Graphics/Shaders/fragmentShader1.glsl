#version 330 core
const vec2 lightBias = vec2(0.7, 0.6);//just indicates the balance between diffuse and ambient lighting

in vec2 pass_textureCoords;
in vec3 pass_normal;

out vec4 FragColor;

uniform sampler2D diffuseMap;
uniform bool useColors;
uniform vec4 color;

void main()
{
 vec3 lightDirection=vec3(0,0,-1);
vec4 diffuseColour = texture(diffuseMap, pass_textureCoords);		
    //FragColor = vec4(1.0f, 0.5f, 0.2f, 1.0f);
	vec3 unitNormal = normalize(pass_normal);
	float diffuseLight = max(dot(-lightDirection, unitNormal), 0.0) * lightBias.x + lightBias.y;
	//FragColor = diffuseColour * diffuseLight;
	if(useColors){
	FragColor = diffuseColour *diffuseLight;}else{
	FragColor = color*diffuseLight; 
	}
} 