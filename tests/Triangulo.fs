// ESSE AQUI É O TRIANGULO BONITINHO

#version 330 core
out vec4 FragColor;

uniform float iTime;
uniform vec2 iResolution;
uniform vec4 iMouse;

#define COL_TRI vec3(0.9, 0.3, 0.2)   // cor do triângulo
#define COL_BG  vec3(0.1, 0.1, 0.15)  // cor de fundo

// Distância de ponto para linha
float df_line(in vec2 p, in vec2 a, in vec2 b)
{
    vec2 pa = p - a, ba = b - a;
    float h = clamp(dot(pa, ba) / dot(ba, ba), 0., 1.);
    return length(pa - ba * h);
}

// Barycentric coords aproximadas (stub simples)
vec3 bary(in vec3 a, in vec3 b, in vec3 c, in vec3 p)
{
    vec3 v0 = b - a, v1 = c - a, v2 = p - a;
    float d00 = dot(v0, v0);
    float d01 = dot(v0, v1);
    float d11 = dot(v1, v1);
    float d20 = dot(v2, v0);
    float d21 = dot(v2, v1);
    float denom = d00 * d11 - d01 * d01;
    float v = (d11 * d20 - d01 * d21) / denom;
    float w = (d00 * d21 - d01 * d20) / denom;
    float u = 1.0 - v - w;
    return vec3(u, v, w);
}

// Testa se ponto p está dentro do triângulo
bool test(in vec2 a, in vec2 b, in vec2 c, in vec2 p)
{
    vec3 barycoords = bary(vec3(a,0.), vec3(b,0.), vec3(c,0.), vec3(p,0.));
    return barycoords.x >= 0.0 && barycoords.y >= 0.0 && barycoords.z >= 0.0;
}

void main()
{
    // Coordenadas normalizadas [-1,1]
    float ar = iResolution.x / iResolution.y;
    vec2 uv = (gl_FragCoord.xy / iResolution.xy * 2. - 1.) * vec2(ar, 1.);

    // Define triângulo fixo
    vec2 a = vec2(-0.6, -0.5);
    vec2 b = vec2(0.6, -0.5);
    vec2 c = vec2(0.0, 0.6);

    // Cor de fundo
    vec3 col = COL_BG;

    // Se o ponto está dentro do triângulo, muda a cor
    if (test(a, b, c, uv)) {
        col = COL_TRI;
    }

    // Suaviza as bordas (anti-aliasing simples)
    // float edge = min(min(df_line(uv, a, b), df_line(uv, b, c)), df_line(uv, c, a));
    // col = mix(col, vec3(1.0), smoothstep(0.01, 0.0, edge));

    FragColor = vec4(col, 1.0);
}