using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopTreeUI : MonoBehaviour
{
    [Header("Root")]
    public ShopNode rootNode;

    [Header("UI Refs")]
    public TMP_Text titleText;
    public TMP_Text descriptionText;
    public Transform optionsContainer;
    public Button optionButtonPrefab;
    public Button backButton;

    private ShopNode currentNode;
    private readonly Stack<ShopNode> history = new Stack<ShopNode>();
    private readonly List<Button> spawnedButtons = new List<Button>();

    void OnEnable()
    {
        OpenRoot();
    }

    public void OpenRoot()
    {
        history.Clear();
        ShowNode(rootNode, false);
    }

    void ShowNode(ShopNode node, bool pushCurrent)
    {
        if (node == null) return; // <- důležité: null node ignorovat

        if (pushCurrent && currentNode != null && currentNode != node)
            history.Push(currentNode);

        currentNode = node;

        if (titleText) titleText.text = node.title;
        if (descriptionText) descriptionText.text = node.description;

        ClearOptions();

        foreach (var opt in node.options)
        {
            var btn = Instantiate(optionButtonPrefab, optionsContainer);
            spawnedButtons.Add(btn);

            var txt = btn.GetComponentInChildren<TMP_Text>();
            if (txt) txt.text = opt.label;

            var next = opt.nextNode;
            btn.onClick.AddListener(() =>
            {
                if (next != null)
                    ShowNode(next, true);
                // když je next null, zatím nic (později sem dáme Buy akci)
            });
        }

        if (backButton != null)
        {
            backButton.gameObject.SetActive(history.Count > 0);
            backButton.onClick.RemoveAllListeners();
            backButton.onClick.AddListener(GoBack);
        }
    }

    public void GoBack()
    {
        if (history.Count == 0) return;
        var prev = history.Pop();
        ShowNode(prev, false);
    }

    void ClearOptions()
    {
        for (int i = 0; i < spawnedButtons.Count; i++)
        {
            if (spawnedButtons[i] != null)
                Destroy(spawnedButtons[i].gameObject);
        }
        spawnedButtons.Clear();
    }
}