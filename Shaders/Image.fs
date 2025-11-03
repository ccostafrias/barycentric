#version 330 core
out vec4 FragColor;

uniform float iTime;
uniform vec2 iResolution;
uniform vec4 iMouse;

#define EPS 0.01
#define PI 3.14159265359

float df_circ(in vec2 p, in vec2 c, in float r) {
    return abs(r - length(p - c));
}

// acha intersecção de "p" na reta "ab"
vec2 intersect (vec2 p, vec2 a, vec2 b) {
    vec2 ba = normalize(b - a);

    return a + ba * dot(ba, p - a);
}

// verifica se um ponto "p" está suficientemente perto da reta "ab"
bool line (vec2 p, vec2 a, vec2 b) {
    vec2 ab = normalize(b - a);
    vec2 ap = p - a;

    return length((a + ab * dot(ab, ap)) - p) < 0.0025;
}

// distância de um ponto "p" até a reta "ab"
float df_line(in vec2 p, in vec2 a, in vec2 b) {
    vec2 pa = p - a, ba = b - a;
    float h = clamp(dot(pa,ba) / dot(ba,ba), 0., 1.);

    return length(pa - ba * h);
}

// calcula as coordenadas baricêntricas
vec3 bary(in vec3 a, in vec3 b, in vec3 c, in vec3 p) {
    return vec3(0.333);
}

// testa se um ponto está dentro do triangulo ABC
bool test(in vec2 a, in vec2 b, in vec2 c, in vec2 p, inout vec3 barycoords) {
    barycoords = bary(vec3(a.x, 0.0, a.y),
                      vec3(b.x, 0.0, b.y),
                      vec3(c.x, 0.0, c.y),
                      vec3(p.x, 0.0, p.y));

    return barycoords.x > 0. && barycoords.y > 0. && barycoords.z > 0.;
}

void main() {
    float ar = iResolution.x / iResolution.y;
    vec2 mouse = (iMouse.xy / iResolution.xy * 2.0 - 1.0) * vec2(ar, 1.0) * vec2(1.0, -1.0); // retire esse último vetor caso esteja espelhado
    vec2 pixel = (gl_FragCoord.xy / iResolution.xy * 2.0 - 1.0) * vec2(ar, 1.0);

    vec2 a = vec2( 0.73,  0.75);
    vec2 b = vec2(-0.85,  0.15);
    vec2 c = vec2( 0.25, -0.75);

    vec3 barycoords;
    vec3 finalColor = vec3(0.0);

    bool isDebug = true;
    
    if (isDebug) {
        // linhas para delimitar o triângulo
        if (line(pixel, a, b)) finalColor = vec3(1.0, 1.0, 0.0);
        if (line(pixel, b, c)) finalColor = vec3(1.0, 0.0, 1.0);
        if (line(pixel, c, a)) finalColor = vec3(0.0, 1.0, 1.0);

        // discos nos vértices
        if (df_circ(pixel, a, EPS) < 0.5*EPS) finalColor = vec3(0.0, 1.0, 0.0);
        if (df_circ(pixel, b, EPS) < 0.5*EPS) finalColor = vec3(1.0, 0.0, 0.0);
        if (df_circ(pixel, c, EPS) < 0.5*EPS) finalColor = vec3(0.0, 0.0, 1.0);
    }

    // disco na posição do cursor
    if (df_circ(mouse, pixel, EPS)< 0.5*EPS) finalColor = vec3(1.0, 1.0, 1.0);

    FragColor = vec4(finalColor, 1);
}