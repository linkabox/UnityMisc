// Writen by Martin Nerurkar ( www.playful.systems). MIT license (see license.txt)
// Based on Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)
// Inspired by HSV Shader for Unity from Gregg Tavares (https://github.com/greggman/hsva-unity). MIT License (see license.txt)

// Shifts from rgb to hsv color space
half3 rgb2hsv(half3 c) {
	half4 K = half4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
	float4 p = c.g < c.b ? float4(c.bg, K.wz) : float4(c.gb, K.xy);
	float4 q = c.r < p.x ? float4(p.xyw, c.r) : float4(c.r, p.yzx);

	half d = q.x - min(q.w, q.y);
	half e = 1.0e-10;
	return half3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
}

// Shifts from hsv back to rgb color space
half3 hsv2rgb(half3 c) {
	c = half3(c.x, clamp(c.yz, 0.0, 1.0));
	half4 K = half4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
	half3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
	return c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

// Used for straightforward Hue Shift
half3 shiftHSV(half3 rgb, half3 hsv) {
	return hsv2rgb(rgb2hsv(rgb) + hsv); // this is because the RGB values range from 0 to 1 in the color but we expect -1 to +1 here.
}