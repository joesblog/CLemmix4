// empty shader file
#version 330

// Input vertex attributes (from vertex shader)
in vec2 fragTexCoord;
in vec4 fragColor;

// Input uniform values
uniform sampler2D texture0;
uniform vec4 colDiffuse;
uniform sampler2D PassPrev4Texture;

// Output fragment color
out vec4 finalColor;


vec2 InputSize = vec2(1024,768);
vec2 OutputSize = vec2(1024,768);
#define SourceSize vec4(InputSize, 1.0 / InputSize) //either TextureSize or InputSize
#define outsize vec4(OutputSize, 1.0 / OutputSize)
int FrameCount = 1;


#if __VERSION__ >= 130
#define COMPAT_VARYING in
#define COMPAT_TEXTURE texture
//out vec4 FragColor;
#else
#define COMPAT_VARYING varying
#define FragColor gl_FragColor
#define COMPAT_TEXTURE texture2D
#endif


#define GAMMA_OUTPUT 1.2
#define SHARPNESS_H 0.5
#define SHARPNESS_V 0.6
#define MASK_TYPE 1.0
#define MASK_STRENGTH_MIN 0.1
#define MASK_STRENGTH_MAX 0.2
#define MASK_SIZE 1.0
#define SCANLINE_STRENGTH_MIN 0.2
#define SCANLINE_STRENGTH_MAX 0.6
#define SCANLINE_BEAM_MIN 1.0
#define SCANLINE_BEAM_MAX 1.0
#define GEOM_CURVATURE 0.5
#define GEOM_WARP 0.0
#define GEOM_CORNER_SIZE 0.0
#define GEOM_CORNER_SMOOTH 150
#define INTERLACING_TOGGLE 0.0
#define HALATION 0.3
#define DIFFUSION 0.0
#define BRIGHTNESS 1.0

#define FIX(c) max(abs(c), 1e-5)
#define PI 3.141592653589
#define TEX2D(c) COMPAT_TEXTURE(tex, c)




float curve_distance(float x, float sharp)
{
    float x_step = step(0.5, x);
    float curve = 0.5 - sqrt(0.25 - (x - x_step) * (x - x_step)) * sign(0.5 - x);

    return mix(x, curve, sharp);
}

mat4 get_color_matrix(sampler2D tex, vec2 co, vec2 dx)
{
    return mat4(TEX2D(co - dx), TEX2D(co), TEX2D(co + dx), TEX2D(co + 2.0 * dx));
}

vec4 filter_lanczos(vec4 coeffs, mat4 color_matrix)
{
    vec4 col = color_matrix * coeffs;
    vec4 sample_min = min(color_matrix[1], color_matrix[2]);
    vec4 sample_max = max(color_matrix[1], color_matrix[2]);

    col = clamp(col, sample_min, sample_max);

    return col;
}

vec3 get_scanline_weight(float pos, float beam, float strength)
{
    float weight = 1.0 - pow(cos(pos * 2.0 * PI) * 0.5 + 0.5, beam);
    
    weight = weight * strength * 2.0 + (1.0 - strength);
    
    return vec3(weight);
}

vec2 curve_coordinate(vec2 co, float curvature)
{
    vec2 curve = vec2(curvature, curvature * 0.75);
    vec2 co2 = co + co * curve - curve / 2.0;
    vec2 co_weight = vec2(co.y, co.x) * 2.0 - 1.0;

    co = mix(co, co2, co_weight * co_weight);

    return co;
}

 float get_corner_weight(vec2 co, vec2 corner, float smoothfunc)
{
    float corner_weight;
    
    co = min(co, vec2(1.0) - co) * vec2(1.0, 0.75);
    co = (corner - min(co, corner));
    corner_weight = clamp((corner.x - sqrt(dot(co, co))) * smoothfunc, 0.0, 1.0);
    corner_weight = mix(1.0, corner_weight, ceil(corner.x));
    
    return corner_weight;
}

void main()
{
    vec2 tex_size = SourceSize.xy;
    vec2 midpoint = vec2(0.5, 0.5);
    float scan_offset = 0.0;
    float timer = vec2(FrameCount, FrameCount).x;

    if (INTERLACING_TOGGLE > 0.5 && InputSize.y >= 400.)
    {
        tex_size.y *= 0.5;

        if (mod(timer, 2.0) > 0.0)
        {
            midpoint.y = 0.75;
            scan_offset = 0.5;
        }        
        else midpoint.y = 0.25;
    }

    vec2 co = fragTexCoord * tex_size * (1.0 / InputSize.xy);
    vec2 xy = curve_coordinate(co, GEOM_WARP);
    float corner_weight = get_corner_weight(curve_coordinate(co, GEOM_CURVATURE), vec2(GEOM_CORNER_SIZE), GEOM_CORNER_SMOOTH);

    xy *= InputSize.xy / tex_size;

    vec2 dx = vec2(1.0 / tex_size.x, 0.0);
    vec2 dy = vec2(0.0, 1.0 / tex_size.y);
    vec2 pix_co = xy * tex_size - midpoint;
    vec2 tex_co = (floor(pix_co) + midpoint) / tex_size;
    vec2 dist = fract(pix_co);
    float curve_x, curve_y;
    vec3 col, col2, diff;

    curve_x = curve_distance(dist.x, SHARPNESS_H * SHARPNESS_H);
    curve_y = curve_distance(dist.y, SHARPNESS_V * SHARPNESS_V);

    vec4 coeffs_x = PI * vec4(1.0 + curve_x, curve_x, 1.0 - curve_x, 2.0 - curve_x);
    vec4 coeffs_y = PI * vec4(1.0 + curve_y, curve_y, 1.0 - curve_y, 2.0 - curve_y);

    coeffs_x = FIX(coeffs_x);
    coeffs_x = 2.0 * sin(coeffs_x) * sin(coeffs_x / 2.0) / (coeffs_x * coeffs_x);
    coeffs_x /= dot(coeffs_x, vec4(1.0));

    coeffs_y = FIX(coeffs_y);
    coeffs_y = 2.0 * sin(coeffs_y) * sin(coeffs_y / 2.0) / (coeffs_y * coeffs_y);
    coeffs_y /= dot(coeffs_y, vec4(1.0));

    mat4 color_matrix;


    color_matrix[0] = filter_lanczos(coeffs_x, get_color_matrix(PassPrev4Texture, tex_co - dy, dx));
    color_matrix[1] = filter_lanczos(coeffs_x, get_color_matrix(PassPrev4Texture, tex_co, dx));
    color_matrix[2] = filter_lanczos(coeffs_x, get_color_matrix(PassPrev4Texture, tex_co + dy, dx));
    color_matrix[3] = filter_lanczos(coeffs_x, get_color_matrix(PassPrev4Texture, tex_co + 2.0 * dy, dx));

    col = filter_lanczos(coeffs_y, color_matrix).rgb;
    diff = COMPAT_TEXTURE(texture0, xy).rgb;

    float rgb_max = max(col.r, max(col.g, col.b));
    float sample_offset = (InputSize.y * outsize.w) * 0.5;
    float scan_pos = xy.y * tex_size.y + scan_offset;
    float scan_strength = mix(SCANLINE_STRENGTH_MAX, SCANLINE_STRENGTH_MIN, rgb_max);
    float scan_beam = clamp(rgb_max * SCANLINE_BEAM_MAX, SCANLINE_BEAM_MIN, SCANLINE_BEAM_MAX);
    vec3 scan_weight = vec3(0.0);

    float mask_colors;
    float mask_dot_width;
    float mask_dot_height;
    float mask_stagger;
    float mask_dither;
    vec4 mask_config;

    if      (MASK_TYPE == 1.) mask_config = vec4(2.0, 1.0, 1.0, 0.0);
    else if (MASK_TYPE == 2.) mask_config = vec4(3.0, 1.0, 1.0, 0.0);
    else if (MASK_TYPE == 3.) mask_config = vec4(2.1, 1.0, 1.0, 0.0);
    else if (MASK_TYPE == 4.) mask_config = vec4(3.1, 1.0, 1.0, 0.0);
    else if (MASK_TYPE == 5.) mask_config = vec4(2.0, 1.0, 1.0, 1.0);
    else if (MASK_TYPE == 6.) mask_config = vec4(3.0, 2.0, 1.0, 3.0);
    else if (MASK_TYPE == 7.) mask_config = vec4(3.0, 2.0, 2.0, 3.0);

    mask_colors = floor(mask_config.x);
    mask_dot_width = mask_config.y;
    mask_dot_height = mask_config.z;
    mask_stagger = mask_config.w;
    mask_dither = fract(mask_config.x) * 10.0;

    vec2 mod_fac = floor(fragTexCoord * outsize.xy * SourceSize.xy / (InputSize.xy * vec2(MASK_SIZE, mask_dot_height * MASK_SIZE))) * 1.0001;
    int dot_no = int(mod((mod_fac.x + mod(mod_fac.y, 2.0) * mask_stagger) / mask_dot_width, mask_colors));
    float dither = mod(mod_fac.y + mod(floor(mod_fac.x / mask_colors), 2.0), 2.0);

    float mask_strength = mix(MASK_STRENGTH_MAX, MASK_STRENGTH_MIN, rgb_max);
    float mask_dark, mask_bright, mask_mul;
    vec3 mask_weight;

    mask_dark = 1.0 - mask_strength;
    mask_bright = 1.0 + mask_strength * 2.0;

    if (dot_no == 0) mask_weight = mix(vec3(mask_bright, mask_bright, mask_bright), vec3(mask_bright, mask_dark, mask_dark), mask_colors - 2.0);
    else if (dot_no == 1) mask_weight = mix(vec3(mask_dark, mask_dark, mask_dark), vec3(mask_dark, mask_bright, mask_dark), mask_colors - 2.0);
    else mask_weight = vec3(mask_dark, mask_dark, mask_bright);

    if (dither > 0.9) mask_mul = mask_dark;
    else mask_mul = mask_bright;

    mask_weight *= mix(1.0, mask_mul, mask_dither);
    mask_weight = mix(vec3(1.0), mask_weight, clamp(MASK_TYPE, 0.0, 1.0));

    col2 = (col * mask_weight);
    col2 *= BRIGHTNESS;

    scan_weight = get_scanline_weight(scan_pos - sample_offset, scan_beam, scan_strength);
    col = clamp(col2 * scan_weight, 0.0, 1.0);
    scan_weight = get_scanline_weight(scan_pos, scan_beam, scan_strength);
    col += clamp(col2 * scan_weight, 0.0, 1.0);
    scan_weight = get_scanline_weight(scan_pos + sample_offset, scan_beam, scan_strength);
    col += clamp(col2 * scan_weight, 0.0, 1.0);
    col /= 3.0;

    col *= vec3(corner_weight);
    col += diff * mask_weight * HALATION * vec3(corner_weight);
    col += diff * DIFFUSION * vec3(corner_weight);
    col = pow(col, vec3(1.0 / GAMMA_OUTPUT));

   finalColor = vec4(col, 1.0);
} 