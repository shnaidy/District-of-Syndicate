using UnityEngine;

public class WorkbenchInteractable : MonoBehaviour, IInteractable
{
    [TextArea]
    public string interactionText = "E - Open Workbench";

    public WorkbenchUI workbenchUI;

    public string GetInteractionText()
    {
        return interactionText;
    }

    public void Interact()
    {
        if (workbenchUI == null)
            workbenchUI = FindObjectOfType<WorkbenchUI>(true);

        if (workbenchUI == null)
        {
            Debug.LogError("[WorkbenchInteractable] Missing WorkbenchUI reference.");
            return;
        }

        workbenchUI.ToggleWorkbench();
    }
}