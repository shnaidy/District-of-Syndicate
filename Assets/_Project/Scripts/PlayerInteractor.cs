using UnityEngine;
using TMPro;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Raycast")]
    public Camera playerCamera;
    public float interactDistance = 3f;
    public LayerMask interactLayer = ~0;

    [Header("UI Prompt")]
    public TextMeshProUGUI promptText;

    private IInteractable currentInteractable;

    void Start()
    {
        if (playerCamera == null) playerCamera = Camera.main;
        HidePrompt();
    }

    void Update()
    {
        CheckInteractable();

        if (currentInteractable != null && Input.GetKeyDown(KeyCode.E))
        {
            currentInteractable.Interact();
        }
    }

    void CheckInteractable()
    {
        currentInteractable = null;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactLayer))
        {
            var interactable = hit.collider.GetComponentInParent<IInteractable>();
            if (interactable != null)
            {
                currentInteractable = interactable;
                ShowPrompt(interactable.GetInteractionText());
                return;
            }
        }

        HidePrompt();
    }

    void ShowPrompt(string txt)
    {
        if (promptText == null) return;
        promptText.gameObject.SetActive(true);
        promptText.text = txt;
    }

    void HidePrompt()
    {
        if (promptText == null) return;
        promptText.gameObject.SetActive(false);
    }
}