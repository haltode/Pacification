sampler2D _HexCellData;
float4 _HexCellData_TexelSize;

float4 GetCellData(appdata_full v, int index)
{
	float2 uv;
	uv.x = (v.texcoord2[index] + 0.5) * _HexCellData_TexelSize.x;
	float row = floor(uv.x);
	uv.x -= row;
	uv.y = (row + 0.5) * _HexCellData_TexelSize.y;
	float4 data = tex2Dlod(_HexCellData, float4(uv, 0, 0));
	data.w *= 255;
	return data;
}