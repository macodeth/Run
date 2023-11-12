shader_type canvas_item;

void fragment() {
	// Place fragment code here.
	vec2 uv = UV;
	uv.x = uv.x + TIME;
	COLOR = texture(TEXTURE, uv);
}