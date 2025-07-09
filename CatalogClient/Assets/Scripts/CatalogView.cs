using UnityEngine;
using CatalogApi;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;

public class CatalogView : MonoBehaviour
{
    private Catalog _catalog;

    private VisualTreeAsset _itemTemplate;
    private ScrollView _scrollView;

    // Filter
    private MinMaxSlider _priceRange;
    private RadioButtonGroup _filterPref;
    private GroupBox _tokenFilterList;
    private HashSet<string> _selectedFilterTokens = new HashSet<string>();
    private Toggle _selectOnlyBundles;

    // Sort
    private ListView _tokenSortList;
    private List<string> _orderedTokenList = new List<string>();
    private int selectedIndex = -1;
    public Button _moveUpButton;
    public Button _moveDownButton;
    private RadioButtonGroup _sortPref;
    private RadioButtonGroup _sortBy;
    private Button _applyButton;

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        _itemTemplate = Resources.Load<VisualTreeAsset>("CatalogItem");
        _scrollView = root.Q<ScrollView>("itemsParent");
        _filterPref = root.Q<RadioButtonGroup>("filterPref");
        _tokenSortList = root.Q<ListView>("tokenSortList");
        _tokenFilterList = root.Q<GroupBox>("tokenList");
        _priceRange = root.Q<MinMaxSlider>("priceRange");
        _selectOnlyBundles = root.Q<Toggle>("onlyBundles");

        _sortPref = root.Q<RadioButtonGroup>("sortPref");
        _sortBy = root.Q<RadioButtonGroup>("sortBy");
        _moveUpButton = root.Q<Button>("moveUpButton");
        _moveDownButton = root.Q<Button>("moveDownButton");

        _applyButton = root.Q<Button>("apply");
        _applyButton.clicked += OnApplyClicked;

        if (_scrollView == null)
        {
            Debug.LogError("ScrollView not found in the UI Document.");
            return;
        }

        _catalog = new Catalog(new LocalDataProvider());
        var result = _catalog.Initialize();

        if (result.IsSuccess)
        {
            var items = _catalog.GetAllItems();
            _orderedTokenList = _catalog.GetAllTokenTypes();

            UpdateItemsScrollView(items);
        }
        else
        {
            Debug.LogError($"Catalog Initialization Failed, {result.ErrorMessage}");
        }

        InitializeFilterMenu();
        InitializeSortMenu();
    }

    private void InitializeFilterMenu()
    {
        foreach (var token in _orderedTokenList)
        {
            Toggle checkBox = new Toggle();
            checkBox.label = token;
            checkBox.value = false;
            _tokenFilterList.Add(checkBox);
            checkBox.RegisterValueChangedCallback(OnTokenFilterChange);
        }

        (_priceRange.lowLimit, _priceRange.highLimit) = _catalog.GetMinMaxPrice();
        _priceRange.minValue = _priceRange.lowLimit;
        _priceRange.maxValue = _priceRange.highLimit;
    }

    private void InitializeSortMenu()
    {
        _tokenSortList.itemsSource = _orderedTokenList;
        _tokenSortList.fixedItemHeight = 30;

        _tokenSortList.makeItem = () => new Label();
        _tokenSortList.bindItem = (element, index) => (element as Label).text = _orderedTokenList[index];

        _tokenSortList.selectionChanged += (IEnumerable<object> selectedItems) =>
        {
            if (selectedItems.Any())
            {
                selectedIndex = _orderedTokenList.IndexOf(selectedItems.First() as string);
                UpdateMoveButtonsState();
            }
            else
            {
                selectedIndex = -1;
                UpdateMoveButtonsState();
            }
        };

        _moveUpButton.clicked += MoveSelectedItemUp;
        _moveDownButton.clicked += MoveSelectedItemDown;

        UpdateMoveButtonsState(); // Initialize button state
        _tokenSortList.Rebuild();
    }

    private void OnTokenFilterChange(ChangeEvent<bool> evt)
    {
        if (evt.target is Toggle changedToggle)
        {
            if (evt.newValue)
                _selectedFilterTokens.Add(changedToggle.label);
            else
                _selectedFilterTokens.Remove(changedToggle.label);
        }
    }

    private void OnApplyClicked()
    {
        FilterObject filter = new FilterObject
        {
            IsOr = _filterPref.value == 1,
            MinPrice = _priceRange.minValue,
            MaxPrice = _priceRange.maxValue,
            SelectedTokens = _selectedFilterTokens.ToList(),
            OnlyBundles =_selectOnlyBundles.value           
        };

        SortObject sortObject = new SortObject
        {
            Descending = _sortPref.value == 0,
            SortCriteria = (SortBy)_sortBy.value,
            SelectedTokens = _orderedTokenList
        };

        var newItems = _catalog.FilterAndSort(filter, sortObject);
        UpdateItemsScrollView(newItems);
    }

    void UpdateMoveButtonsState()
    {
        _moveUpButton.SetEnabled(selectedIndex > 0);
        _moveDownButton.SetEnabled(selectedIndex < _orderedTokenList.Count - 1 && selectedIndex != -1);
    }

    void MoveSelectedItemUp()
    {
        if (selectedIndex > 0)
        {
            (_orderedTokenList[selectedIndex], _orderedTokenList[selectedIndex - 1]) = (_orderedTokenList[selectedIndex - 1], _orderedTokenList[selectedIndex]);
            selectedIndex--;
            _tokenSortList.RefreshItems(); 
            _tokenSortList.ScrollToItem(selectedIndex); 
            _tokenSortList.selectedIndex = selectedIndex;
            UpdateMoveButtonsState();
        }
    }

    void MoveSelectedItemDown()
    {
        if (selectedIndex < _orderedTokenList.Count - 1 && selectedIndex != -1)
        {
            (_orderedTokenList[selectedIndex], _orderedTokenList[selectedIndex + 1]) = (_orderedTokenList[selectedIndex + 1], _orderedTokenList[selectedIndex]);
            selectedIndex++;
            _tokenSortList.RefreshItems();
            _tokenSortList.ScrollToItem(selectedIndex);
            _tokenSortList.selectedIndex = selectedIndex;
            UpdateMoveButtonsState();
        }
    }

    public void UpdateItemsScrollView(List<Product> items)
    {
        _scrollView.Clear();
        foreach (var item in items)
        {
            // Instantiate the UXML template
            VisualElement newItem = _itemTemplate.Instantiate();

            var itemLogic = new CatalogItemLogic(newItem, item, OnItemClicked);
            newItem.userData = itemLogic;

            _scrollView.contentContainer.Add(newItem);
        }
    }

    // For demonestration only
    private void OnItemClicked(string itemName)
    {
        Debug.Log($"Item clicked: {itemName}");
    }
}

