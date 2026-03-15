using UnityEngine;

[CreateAssetMenu(fileName = "GunPart", menuName = "Shop/Gun Part")]
public class GunPart : ScriptableObject
{
    public string partName;
    public GunPartType partType;
    public string countryOfOrigin;
    public int price;

    [TextArea]
    public string description;

    public Sprite image;
    public string platformTag; // např. "AR15" nebo "AKM"
}