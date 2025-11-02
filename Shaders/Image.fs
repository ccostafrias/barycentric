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

// Find the intersection of "p" onto "ab".
vec2 intersect (vec2 p, vec2 a, vec2 b) {
    // Calculate the unit vector from "a" to "b".
    vec2 ba = normalize(b - a);

    // Calculate the intersection of p onto "ab" by
    // calculating the dot product between the unit vector
    // "ba" and the direction vector from "a" to "p", then
    // this value is multiplied by the unit vector "ab"
    // fired from the point "a".
    return a + ba * dot(ba, p - a);
}

// Visual line for debugging purposes.
bool line (vec2 p, vec2 a, vec2 b) {
    // Direction from a to b.
    vec2 ab = normalize(b - a);

    // Direction from a to the pixel.
    vec2 ap = p - a;

    // Find the intersection of the pixel on to vector
    // from a to b, calculate the distance between the
    // pixel and the intersection point, then compare
    // that distance to the line width.
    return length((a + ab * dot(ab, ap)) - p) < 0.0025;
}

float df_line(in vec2 p, in vec2 a, in vec2 b) {
    vec2 pa = p - a, ba = b - a;
    float h = clamp(dot(pa,ba) / dot(ba,ba), 0., 1.);

    return length(pa - ba * h);
}

float sharpen(in float d, in float w) {
    float e = 1.0 / min(iResolution.y , iResolution.x);
    return 1.0 - smoothstep(-e, e, d - w);
}

float df_bounds(in vec2 uv, in vec2 p, in vec2 a, in vec2 b, in vec2 c, in vec3 barycoords) {
    float cp = 0.0;
    float c0 = sharpen(df_circ(uv, p, (0.03 + cos(15.*iTime) *.01)), EPS * 1.);

    return cp;
}

float dist_01(vec2 p, float r) {
    float d = length(p);
    return smoothstep(r, r + 0.01, d);
}

vec3 bary(in vec3 a, in vec3 b, in vec3 c, in vec3 p) {
    return vec3(0.333);
}

bool test(in vec2 a, in vec2 b, in vec2 c, in vec2 p, inout vec3 barycoords) {
    barycoords = bary(vec3(a.x, 0.0, a.y),
                      vec3(b.x, 0.0, b.y),
                      vec3(c.x, 0.0, c.y),
                      vec3(p.x, 0.0, p.y));

    return barycoords.x > 0. && barycoords.y > 0. && barycoords.z > 0.;
}

void main() {
    float ar = iResolution.x / iResolution.y;
    vec2 mouse = (iMouse.xy / iResolution.xy * 2.0 - 1.0) * vec2(ar, 1.0) * vec2(1.0, -1.0);
    vec2 pixel = (gl_FragCoord.xy / iResolution.xy * 2.0 - 1.0) * vec2(ar, 1.0);

    vec2 a = vec2( 0.73,  0.75);
    vec2 b = vec2(-0.85,  0.15);
    vec2 c = vec2( 0.25, -0.75);

    vec3 barycoords;

    bool mouseInside = test(a, b, c, mouse, barycoords);
    bool pixelInside = test(a, b, c, pixel, barycoords);

    vec3 color = vec3(0.0);
    bool isDebug = true;
    
    if (isDebug) {
        // linhas para delimitar o triangulo
        if (line(pixel, a, b)) color = vec3(1.0, 1.0, 0.0);
        if (line(pixel, b, c)) color = vec3(1.0, 0.0, 1.0);
        if (line(pixel, c, a)) color = vec3(0.0, 1.0, 1.0);

        if (df_circ(pixel, a, EPS) < 0.5*EPS) color = vec3(0.0, 1.0, 0.0);
        if (df_circ(pixel, b, EPS) < 0.5*EPS) color = vec3(1.0, 0.0, 0.0);
        if (df_circ(pixel, c, EPS) < 0.5*EPS) color = vec3(0.0, 0.0, 1.0);
    }

    if (pixelInside) {
        if (mouseInside) {
            // o que será, com os pixels do triangulo, quando o mouse estiver dentro
        } else {
            // o que será feito dentro do triângulo
        }
    }

    // disco na posição do cursor
    if (df_circ(mouse, pixel, EPS)< 0.5*EPS) color = vec3(1.0, 1.0, 1.0);

    FragColor = vec4(color, 1);
}