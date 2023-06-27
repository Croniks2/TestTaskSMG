using UnityEngine;

[CreateAssetMenu()]
public class TextureData : ScriptableObject
{

    public Color[] baseColours;
    [Range(0, 1)]
    public float[] baseStartHeights;

    public float savedMinHeight = 0f;
    public float savedMaxHeight = 50f;

    public void ApplyToMaterial(Material material)
    {

        material.SetInt("baseColorCount", baseColours.Length);
        material.SetColorArray("baseColors", baseColours);
        material.SetFloatArray("baseStartHeights", baseStartHeights);

        UpdateMeshHeights(material, savedMinHeight, savedMaxHeight);
    }

    public void UpdateMeshHeights(Material material, float minHeight, float maxHeight)
    {
        savedMinHeight = minHeight;
        savedMaxHeight = maxHeight;

        material.SetFloat("minHeight", minHeight);
        material.SetFloat("maxHeight", maxHeight);
    }

}