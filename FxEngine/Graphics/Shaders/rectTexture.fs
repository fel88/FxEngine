#version 330 core
in vec2 pass_textureCoords;

out vec4 FragColor;
uniform sampler2D diffuseMap;

void main()
{
    vec4 diffuseColour = texture(diffuseMap, pass_textureCoords);		

    FragColor=diffuseColour;
    //FragColor = vec4(1.0f, 0.5f, 0.2f, 1.0f);
    //FragColor = vec4(diffuseColour.x, 0.5f, 0.2f, 1.0f);
    //FragColor = vec4(color, 1.0f);
} 