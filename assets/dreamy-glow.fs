/*{
  "DESCRIPTION": "A soft ethereal bloom that lifts shadows and blooms highlights \u2014 the dreamy, angelic glow that dominates beauty and lifestyle content.",
  "CATEGORIES": ["Guillotine", "Stylize"],
  "INPUTS": [
  {
    "NAME": "inputImage",
    "TYPE": "image"
  },
  {
    "NAME": "amount",
    "TYPE": "float",
    "DEFAULT": 0.6,
    "MIN": 0.0,
    "MAX": 1.0
  },
  {
    "NAME": "lift",
    "TYPE": "float",
    "DEFAULT": 0.12,
    "MIN": 0.0,
    "MAX": 0.5
  }
]
}*/
void main() {
  vec2 uv = isf_FragNormCoord;
  vec2 t = 1.0 / RENDERSIZE;
  vec4 c = IMG_THIS_PIXEL(inputImage);

  // Wide 8-tap blur ring approximates a soft Orton-style diffusion without a second pass.
  vec3 soft = c.rgb;
  soft += IMG_NORM_PIXEL(inputImage, uv + vec2( t.x*4.0, 0.0)).rgb;
  soft += IMG_NORM_PIXEL(inputImage, uv + vec2(-t.x*4.0, 0.0)).rgb;
  soft += IMG_NORM_PIXEL(inputImage, uv + vec2(0.0,  t.y*4.0)).rgb;
  soft += IMG_NORM_PIXEL(inputImage, uv + vec2(0.0, -t.y*4.0)).rgb;
  soft += IMG_NORM_PIXEL(inputImage, uv + vec2( t.x*3.0,  t.y*3.0)).rgb;
  soft += IMG_NORM_PIXEL(inputImage, uv + vec2(-t.x*3.0,  t.y*3.0)).rgb;
  soft += IMG_NORM_PIXEL(inputImage, uv + vec2( t.x*3.0, -t.y*3.0)).rgb;
  soft += IMG_NORM_PIXEL(inputImage, uv + vec2(-t.x*3.0, -t.y*3.0)).rgb;
  soft /= 9.0;

  // Screen-blend the blur back over the original: bright areas bloom, darks stay put.
  vec3 screened = 1.0 - (1.0 - c.rgb) * (1.0 - soft * amount);

  // Lifted, milky shadows finish the dreamy look.
  vec3 lifted = screened * (1.0 - lift) + vec3(lift);
  gl_FragColor = vec4(lifted, c.a);
}
