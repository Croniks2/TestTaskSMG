using UnityEngine;

public class TerrainPainter : MonoBehaviour
{
    [SerializeField] private Material _terrainMaterial;
    [SerializeField] private TextureData _terrainTextureData;

    private void Start()
    {
        _terrainTextureData.ApplyToMaterial(_terrainMaterial);
    }
}
