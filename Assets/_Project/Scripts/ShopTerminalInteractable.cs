using UnityEngine;

public class ShopTerminalInteractable : MonoBehaviour, IInteractable
{
    public SimpleUIScreen shopScreen;

    [TextArea]
    public string interactionText = "E - Open Shop";

    public string GetInteractionText()
    {
        return interactionText;
    }

    public void Interact()
    {
        if (shopScreen != null)
            shopScreen.Toggle();
    }
}