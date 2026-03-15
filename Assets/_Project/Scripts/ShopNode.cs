using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShopNodeOption
{
    public string label;      // text tlačítka
    public ShopNode nextNode; // kam přejde po kliknutí
}

[CreateAssetMenu(fileName = "ShopNode", menuName = "Shop/Node")]
public class ShopNode : ScriptableObject
{
    [TextArea] public string title;
    [TextArea] public string description;

    public List<ShopNodeOption> options = new List<ShopNodeOption>();
}