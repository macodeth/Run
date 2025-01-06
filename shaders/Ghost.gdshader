shader_type canvas_item;

uniform float r;
uniform float g;
uniform float b;
uniform float mix_color;

void fragment() {
	vec4 texture_color = texture(TEXTURE, UV);
	if (texture_color.a > 0.0)
		COLOR = vec4(mix(texture_color.rgb, vec3(r,g,b), mix_color), 0.2);
}
