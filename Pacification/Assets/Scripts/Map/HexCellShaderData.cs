using UnityEngine;

public class HexCellShaderData : MonoBehaviour
{
    Texture2D cellTexture;
    Color32[] cellTextureData;

    public void Initialize(int sizeX, int sizeZ)
    {
        if(cellTexture)
            cellTexture.Resize(sizeX, sizeZ);
        else
        {
            cellTexture = new Texture2D(sizeX, sizeZ, TextureFormat.RGBA32, false, true);
            cellTexture.filterMode = FilterMode.Point;
            cellTexture.wrapMode = TextureWrapMode.Clamp;
            Shader.SetGlobalTexture("_HexCellData", cellTexture);
        }
        Shader.SetGlobalVector("_HexCellData_TexelSize", new Vector4(1f / sizeX, 1f / sizeZ, sizeX, sizeZ));

        if(cellTextureData == null || cellTextureData.Length != sizeX * sizeZ)
            cellTextureData = new Color32[sizeX * sizeZ];
        else
        {
            for(int i = 0; i < cellTextureData.Length; ++i)
                cellTextureData[i] = new Color32(0, 0, 0, 0);
        }

        enabled = true;
    }

    public void RefreshTerrain(HexCell cell)
    {
        cellTextureData[cell.Index].a = (byte)cell.TerrainBiomeIndex;
        enabled = true;
    }

    public void RefreshVisibility(HexCell cell)
    {
        int index = cell.Index;
        cellTextureData[index].r = cell.IsVisible ? (byte)255 : (byte)0;
        cellTextureData[index].g = cell.IsExplored ? (byte)255 : (byte)0;
        enabled = true;
    }

    void LateUpdate()
    {
        cellTexture.SetPixels32(cellTextureData);
        cellTexture.Apply();
        enabled = false;
    }
}