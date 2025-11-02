#version 330 core

out vec4 FragColor;        // saída final do fragment shader

uniform float iTime;       // tempo em segundos (animações)
uniform vec2 iResolution;  // resolução da tela (x, y)
uniform vec4 iMouse;       // posição e estado do mouse

#define EPS 0.01
#define PI 3.14159265359
#define SQRT_3 1.73205080757
#define COL_TRI vec3(0.9, 0.3, 0.2)   // cor do triângulo
#define COL_BG  vec3(0.1, 0.1, 0.15)  // cor de fundo

// distância do ponto p até a circunferência de centro c e raio r
float df_circ(in vec2 p, in vec2 c, in float r) {
    return abs(r - length(p - c));
}

bool in_disc(in vec2 p, in vec2 c, in float r, in float k) {
    return df_circ(p, c, r) < k*r;
}

float df_line(in vec2 p, in vec2 a, in vec2 b) {
    vec2 pa = p - a, ba = b - a;
    float h = clamp(dot(pa,ba) / dot(ba,ba), 0., 1.);
        return length(pa - ba * h);
}

// desenha uma linha visual entre dois pontos
bool line(vec2 p, vec2 a, vec2 b) {
    vec2 ab = normalize(b - a);
    vec2 ap = p - a;
    return length((a + ab * dot(ab, ap)) - p) < 0.0025;
}

// interseção do ponto p na linha ab
vec2 intersect (vec2 p, vec2 a, vec2 b) {
    vec2 ba = normalize(b - a);

    return a + ba * dot(ba, p - a);
}

float determinante_3x3(vec3 a, vec3 b, vec3 c) {
    return a.x * (b.y*c.z - b.z * c.y)
         - a.y * (b.x*c.z - b.z * c.x)
         + a.z * (b.x*c.y - b.y * c.x);
}

// calcula coordenadas baricêntricas
vec3 bary(in vec2 a, in vec2 b, in vec2 c, in vec2 p) {
    vec3 newA = vec3(a, 1);
    vec3 newB = vec3(b, 1);
    vec3 newC = vec3(c, 1);
    vec3 newP = vec3(p, 1);

    float detMain   = determinante_3x3(newA, newB, newC);
    float detAlpha1 = determinante_3x3(newP, newB, newC);
    float detAlpha2 = determinante_3x3(newA, newP, newC);
    float detAlpha3 = determinante_3x3(newA, newB, newP);

    float alpha1 = detAlpha1 / detMain;
    float alpha2 = detAlpha2 / detMain;
    float alpha3 = detAlpha3 / detMain;

    return vec3(alpha1, alpha2, alpha3);
}

// verifica se o ponto p está dentro do triângulo ABC
bool is_inside(in vec2 a, in vec2 b, in vec2 c, in vec2 p, inout vec3 barycoords) {
    barycoords = bary(a, b, c, p);

    return barycoords.x >= 0.0 && barycoords.y >= 0.0 && barycoords.z >= 0.0;
}

float ease(float t) {
    return 0.5 - 0.5 * cos(t * PI);
}

float easeInOutBack(float t) {
    float c1 = 1.70158;
    float c2 = c1 * 1.525;

    return t < 0.5
        ? (pow(2.0 * t, 2.0) * ((c2 + 1.0) * 2.0 * t - c2)) / 2.0
        : (pow(2.0 * t - 2.0, 2.0) * ((c2 + 1.0) * (t * 2.0 - 2.0) + c2) + 2.0) / 2.0;
}

float easeInOutCirc(float t) {
    return t < 0.5
        ? (1.0 - sqrt(1.0 - pow(2.0 * t, 2.0))) / 2.0
        : (sqrt(1.0 - pow(-2.0 * t + 2.0, 2.0)) + 1.0) / 2.0;
}

float easeOutBounce(float t) {
    float n1 = 7.5625;
    float d1 = 2.75;

    if (t < 1.0 / d1) {
        return n1 * t * t;
    } else if (t < 2.0 / d1) {
        t -= 1.5 / d1;
        return n1 * t * t + 0.75;
    } else if (t < 2.5 / d1) {
        t -= 2.25 / d1;
        return n1 * t * t + 0.9375;
    } else {
        t -= 2.625 / d1;
        return n1 * t * t + 0.984375;
    }
}

vec3 rgb_triangle(in vec3 barycoords, float speedFactor) {
    float speed = iTime * speedFactor;
    float speedFrac = fract(speed);
    float time = mod(speed, 3.0);

    float a = (barycoords.x + barycoords.y);
    float b = (barycoords.y + barycoords.z);
    float c = (barycoords.z + barycoords.x);

    float eased = easeInOutBack(speedFrac);

    // time 0.0: R: a, G: b, B: c
    // time 1.0: R: c, G: a, B: b
    // time 2.0: R: b, G: c, B: a
    if (time < 1.0)
        return vec3(mix(a, c, eased), mix(b, a, eased), mix(c, b, eased));
    else if (time < 2.0)
        return vec3(mix(c, b, eased), mix(a, c, eased), mix(b, a, eased));
    else 
        return vec3(mix(b, a, eased), mix(c, b, eased), mix(a, c, eased));
}

mat2 rotateZ(float angle) {
    return mat2(cos(angle), -sin(angle),
                sin(angle),  cos(angle));
}

float findMinDist(in vec2 a, in vec2 b, in vec2 c, in vec2 p, inout vec2 i) {
    float ab = df_line(p, a, b);
    float bc = df_line(p, b, c);
    float ca = df_line(p, c, a);

    float minDist = min(min(ab, bc), ca);

    if (minDist == ab) i = intersect(p, a, b);
    else if (minDist == bc) i = intersect(p, b, c);
    else i = intersect(p, c, a);

    return minDist;
}

void main() {
    // ajusta o aspecto da tela (aspect ratio)
    float ar = iResolution.x / iResolution.y;

    // posição do mouse e do "pixel" normalizados
    vec2 mc = (iMouse.xy / iResolution.xy * 2.0 - 1.0) * vec2(ar, 1.0) * vec2(1.0, -1.0);
    vec2 uv = (gl_FragCoord.xy / iResolution.xy * 2.0 - 1.0) * vec2(ar, 1.0);

    float side = 1.2;
    // float scale = ease(iTime * 2.0)*0.5 + 0.5;
    float scale = 1.0;
    float angle = iTime * 2.0;

    // coords iniciais
    vec2 iA = vec2(-side/2, -side * SQRT_3/6.0);
    vec2 iB = vec2(0.0    ,  side * SQRT_3/3.0);
    vec2 iC = vec2(side/2 , -side * SQRT_3/6.0);

    vec2 center = vec2(0.0, 0.0);

    // define os vértices do triângulo de fato (com rotação)
    vec2 a = (iA - center) * scale * rotateZ(angle) + center;
    vec2 b = (iB - center) * scale * rotateZ(angle) + center;
    vec2 c = (iC - center) * scale * rotateZ(angle) + center;

    // guarda as coordenadas baricêntricas
    vec3 barycoords;

    bool mouseInside = is_inside(a, b, c, mc, barycoords);
    bool pixelInside = is_inside(a, b, c, uv, barycoords);

    // cor default
    vec3 color = COL_BG;
    bool debugLine = false;

    if (debugLine) {
        if (line(uv, a, b)) color = vec3(1.0, 1.0, 0.0);
        if (line(uv, b, c)) color = vec3(1.0, 0.0, 1.0);
        if (line(uv, c, a)) color = vec3(0.0, 1.0, 1.0);

        if (in_disc(uv, a, EPS, 0.5)) color = vec3(0.0, 1.0, 0.0);
        if (in_disc(uv, b, EPS, 0.5)) color = vec3(1.0, 0.0, 0.0);
        if (in_disc(uv, c, EPS, 0.5)) color = vec3(0.0, 0.0, 1.0);
    }

    vec2 intersectPoint;
    float minDistance = findMinDist(a, b, c, uv, intersectPoint);
    float maxDistance = 0.2;

    if (pixelInside) {
        if (mouseInside) {
            color = rgb_triangle(barycoords, 2.0);
        } else
            color = vec3(1.0);
    } else if (minDistance < maxDistance && mouseInside) {
        float glow = (1.0 - ease(minDistance/maxDistance)) * 0.5;
        color += rgb_triangle(bary(a, b, c, intersectPoint), 2.0) * glow;
    }

    // disco na posição do mouse
    if (in_disc(uv, mc, EPS, .5)) color = vec3(1.0, 1.0, 1.0);

    FragColor = vec4(color, 1.0);
}