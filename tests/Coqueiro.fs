// CÓDIGO BASE PARA MONTAR O TRIANGULO

#version 330 core
out vec4 FragColor;

uniform float iTime;
uniform vec2 iResolution;
uniform vec4 iMouse;

#define EPS  .01
#define PI 3.14159265359

#define COL1 vec3(.9, .43, .34)
#define COL2 vec3(.96, .66, .13)
#define COL3 vec3(0.0)

float df_circ(in vec2 p, in vec2 c, in float r) {
    return abs(r - length(p - c));
}

// Find the intersection of "p" onto "ab".
vec2 intersect (vec2 p, vec2 a, vec2 b)
{
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
bool line (vec2 p, vec2 a, vec2 b)
{
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

float df_line(in vec2 p, in vec2 a, in vec2 b)
{
    vec2 pa = p - a, ba = b - a;
        float h = clamp(dot(pa,ba) / dot(ba,ba), 0., 1.);
        return length(pa - ba * h);
}

float sharpen(in float d, in float w)
{
    float e = 1. / min(iResolution.y , iResolution.x);
    return 1. - smoothstep(-e, e, d - w);
}

vec3 bary(in vec3 a, in vec3 b, in vec3 c, in vec3 p)
{
    return vec3(0.333);
}

bool test(in vec2 a, in vec2 b, in vec2 c, in vec2 p, inout vec3 barycoords)
{
    barycoords = bary(vec3(a.x, 0., a.y),
                  vec3(b.x, 0., b.y),
                  vec3(c.x, 0., c.y),
                  vec3(p.x, 0., p.y));

    return barycoords.x > 0. && barycoords.y > 0. && barycoords.z > 0.;
}


float df_bounds(in vec2 uv, in vec2 p, in vec2 a, in vec2 b, in vec2 c, in vec3 barycoords)
{
    float cp = 0.;


float c0 = sharpen(df_circ(uv, p,
                   (.03 + cos(15.*iTime) *.01))
                   , EPS * 1.);


    return cp;
}


vec3 globalColor (in vec2 uv, in vec2 a, in vec2 b, in vec2 c)
{
    vec3 r=vec3(1.0);

    return r;
}

float dist_01(vec2 p,float r) {
    float d = length(p);
    return smoothstep(r,r+0.01,d);
}

void main()
{
    vec2 p = gl_FragCoord.xy/iResolution.xy;
    vec3 col = mix(COL1,COL2,p.y); // degradê no fundo

    p.x *= iResolution.x/iResolution.y;
    float r = 0.12;

    vec2 q = p - vec2(0.7,0.7);
    r += 0.055*cos(atan(q.x,q.y)*13.0 - 40.0*q.x + 0.3*sin(iTime*3.1459));
    r += 0.02*sin(atan(q.x,q.y)*200.0);
    col *= dist_01(q,r);

    // vec2 t = p-vec2(1,0.2);
    // r = 0.2;
    // r += 0.06*cos(atan(t.x,t.y)*100.0);
    // col += (1-dist_01(t,r))*0.5;


    r = 0.02;
    r += 0.005*cos(q.y*100.0);
    r +=exp(-40.0*p.y);
    col *= (1.0-(1.0-smoothstep(r,r+0.01,abs(q.x -0.075*sin(q.y*4.5))))*smoothstep(0.1,0.0,q.y));
    // col *= vec3(0.2, 0.8, 0.4); // coqueiro verde

    FragColor = vec4(col, 1);
}