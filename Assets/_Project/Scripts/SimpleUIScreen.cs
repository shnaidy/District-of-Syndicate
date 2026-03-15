using UnityEngine;

public class SimpleUIScreen : MonoBehaviour
{
    [Header("Panel Root")]
    public GameObject panelRoot;

    [Header("Optional Player Objects To Disable")]
    public GameObject playerObjectToDisable;   // optional
    public MonoBehaviour lookScriptToDisable;  // optional (např. PlayerFollowCamera / FirstPersonController)

    private bool isOpen = false;

    void Start()
    {
        if (panelRoot != null) panelRoot.SetActive(false);
        LockCursor(true);
    }

    void Update()
    {
        if (isOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            Close();
        }
    }

    public void Toggle()
    {
        if (isOpen) Close();
        else Open();
    }

    public void Open()
    {
        isOpen = true;

        if (panelRoot != null) panelRoot.SetActive(true);

        if (lookScriptToDisable != null) lookScriptToDisable.enabled = false;
        if (playerObjectToDisable != null) playerObjectToDisable.SetActive(false);

        LockCursor(false);
    }

    public void Close()
    {
        isOpen = false;

        if (panelRoot != null) panelRoot.SetActive(false);

        if (lookScriptToDisable != null) lookScriptToDisable.enabled = true;
        if (playerObjectToDisable != null) playerObjectToDisable.SetActive(true);

        LockCursor(true);
    }

    void LockCursor(bool lockIt)
    {
        Cursor.lockState = lockIt ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !lockIt;
    }
}