using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorkbenchUI : MonoBehaviour
{
    [Header("Refs")]
    public WorkbenchCraftingV2 crafting;
    public GameObject panelRoot;

    [Header("Player Input Lock")]
    public MonoBehaviour playerLookScript; // sem dej script co otáčí kamerou
    public bool pauseTimeWhenOpen = false;

    [Header("UI")]
    public TMP_Text recipeNameText;
    public TMP_Text recipeDescText;
    public TMP_Text ingredientsText;
    public TMP_Text statusText;
    public Button craftButton;

    [Header("Recipe Selection")]
    public WeaponRecipeSO selectedRecipe;

    private bool isOpen;

    private void Start()
    {
        if (crafting == null) crafting = FindObjectOfType<WorkbenchCraftingV2>();

        if (panelRoot != null) panelRoot.SetActive(false);
        isOpen = false;

        if (craftButton != null)
        {
            craftButton.onClick.RemoveAllListeners();
            craftButton.onClick.AddListener(OnClickCraft);
        }

        ApplyCursorAndInputState(false);
        RefreshUI();
    }

    private void Update()
    {
        if (!isOpen) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseWorkbench();
        }
    }

    public void ToggleWorkbench()
    {
        if (isOpen) CloseWorkbench();
        else OpenWorkbench();
    }

    public void OpenWorkbench()
    {
        isOpen = true;
        if (panelRoot != null) panelRoot.SetActive(true);

        ApplyCursorAndInputState(true);
        RefreshUI();
    }

    public void CloseWorkbench()
    {
        isOpen = false;
        if (panelRoot != null) panelRoot.SetActive(false);

        ApplyCursorAndInputState(false);
    }

    public void SetSelectedRecipe(WeaponRecipeSO recipe)
    {
        selectedRecipe = recipe;
        RefreshUI();
    }

    private void ApplyCursorAndInputState(bool open)
    {
        if (open)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (playerLookScript != null)
                playerLookScript.enabled = false;

            if (pauseTimeWhenOpen)
                Time.timeScale = 0f;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            if (playerLookScript != null)
                playerLookScript.enabled = true;

            if (pauseTimeWhenOpen)
                Time.timeScale = 1f;
        }
    }

    public void RefreshUI()
    {
        if (selectedRecipe == null)
        {
            if (recipeNameText) recipeNameText.text = "No recipe selected";
            if (recipeDescText) recipeDescText.text = "";
            if (ingredientsText) ingredientsText.text = "Select recipe.";
            if (statusText) statusText.text = "";
            if (craftButton) craftButton.interactable = false;
            return;
        }

        if (recipeNameText) recipeNameText.text = selectedRecipe.displayName;
        if (recipeDescText) recipeDescText.text = selectedRecipe.description;

        if (crafting != null)
        {
            if (ingredientsText) ingredientsText.text = BuildIngredientsText(selectedRecipe);
            bool canCraft = crafting.CanCraft(selectedRecipe);
            if (craftButton) craftButton.interactable = canCraft;
        }
        else
        {
            if (ingredientsText) ingredientsText.text = "Missing WorkbenchCraftingV2 ref.";
            if (craftButton) craftButton.interactable = false;
        }

        if (statusText) statusText.text = "";
    }

    private void OnClickCraft()
    {
        if (crafting == null || selectedRecipe == null) return;

        bool ok = crafting.TryCraft(selectedRecipe);

        if (statusText)
        {
            statusText.text = ok
                ? $"Crafted: {selectedRecipe.displayName}"
                : crafting.GetMissingIngredientsText(selectedRecipe);
        }

        RefreshUI();
    }

    private string BuildIngredientsText(WeaponRecipeSO recipe)
    {
        if (crafting == null || crafting.inventoryV2 == null || recipe == null) return "Missing refs.";

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.AppendLine("Required parts:");

        foreach (var ing in recipe.ingredients)
        {
            if (ing == null || ing.item == null) continue;

            int need = Mathf.Max(1, ing.amount);
            int have = crafting.inventoryV2.GetTotalCountByData(ing.item);

            string ok = have >= need ? "OK" : "MISSING";
            sb.AppendLine($"- {ing.item.displayName}: {have}/{need} [{ok}]");
        }

        return sb.ToString();
    }
}