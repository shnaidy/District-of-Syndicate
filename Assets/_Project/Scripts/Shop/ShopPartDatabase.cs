using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopPartDatabase", menuName = "Shop/Part Database")]
public class ShopPartDatabase : ScriptableObject
{
    public List<GunPart> parts = new List<GunPart>();
}