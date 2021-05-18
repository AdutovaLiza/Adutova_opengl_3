#version 330 core
out vec4 FragColor;

in VS_OUT {
    vec3 FragPos;
    vec3 Normal;
    vec2 TexCoords;
} fs_in;

struct Light {
    vec3 Position;
    vec3 Color;
};

uniform Light lights[4];
uniform sampler2D texture1;
uniform vec3 viewPos;

void main()
{           
    vec3 color = texture(texture1, fs_in.TexCoords).rgb;
    vec3 normal = normalize(fs_in.Normal);
	
    // Фоновая составляющая
    vec3 ambient = 0.05 * color;
	
    // Освещение
    vec3 lighting = vec3(0.0);
    vec3 viewDir = normalize(viewPos - fs_in.FragPos);
    
    for(int i = 0; i < 4; i++)
    {
        // Диффузная составляющая
        vec3 lightDir = normalize(lights[i].Position - fs_in.FragPos);
        float diff = max(dot(lightDir, normal), 0.0);
        vec3 result = lights[i].Color * diff * color;  
		
        // Отраженная составляющая
        vec3 reflectDir = reflect(-lightDir, normal);
        vec3 halfwayDir = normalize(lightDir + viewDir);  
        float spec = pow(max(dot(normal, halfwayDir), 0.0), 64.0);
        vec3 specular = lights[i].Color * vec3(0.05) * spec;

        // Затухание (используется квадратичное, так как у нас есть гамма-коррекция)
        float distance = length(fs_in.FragPos - lights[i].Position);
        result *= 1.0 / (distance * distance);
        lighting = lighting + result + specular;
        
     }
     vec3 result = ambient + lighting;
     FragColor = vec4(result, 1.0);
}