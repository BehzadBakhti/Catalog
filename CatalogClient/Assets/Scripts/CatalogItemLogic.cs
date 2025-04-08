using UnityEngine.UIElements;
using System;
using CatalogApi;
using UnityEngine;

public class CatalogItemLogic
{
    private VisualTreeAsset _subItemTemplate;

    private readonly Button _button;
    private readonly Label _itemName;
    private readonly Label _description;
    private readonly Label _price;
    private readonly Action<string> _onClick;

    public CatalogItemLogic(VisualElement root, Product itemData, Action<string> onclick)
    {
        _subItemTemplate = Resources.Load<VisualTreeAsset>("SubItem");

        _itemName = root.Q<Label>("itemName");
        _description = root.Q<Label>("description");
        _price = root.Q<Label>("price");
        _button = root.Q<Button>("subItemsParent");

        _itemName.text = itemData.Name;
        _description.text = itemData.Description;
        _price.text = $"{itemData.Price.ToString()} $";
        _button.clicked += OnItemClicked;

        _onClick = onclick;

        foreach (var product in itemData.Tokens)
        {
            AddSubItemToItem(product.Key, product.Value);
        }
    }

    public void AddSubItemToItem(string name, int amount)
    {
        // Instantiate the UXML template
        VisualElement newItem = _subItemTemplate.Instantiate();

        var itemLogic = new SubItemLogic(newItem, name, amount);
        newItem.userData = itemLogic;

        _button.Add(newItem);
    }

    private void OnItemClicked()
    {
        _onClick?.Invoke(_itemName.text);
    }
}
