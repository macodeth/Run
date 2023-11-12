shader_type canvas_item;

void fragment() {
	// Place fragment code here.
	vec2 uv = UV;
	uv.y = uv.y + TIME;
	COLOR = texture(TEXTURE, uv);
}