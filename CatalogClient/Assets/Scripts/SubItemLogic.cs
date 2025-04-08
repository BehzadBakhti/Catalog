using UnityEngine.UIElements;

public class SubItemLogic
{
    public SubItemLogic(VisualElement root, string tokenName, int amount)
    {
        var tokenNameLabel = root.Q<Label>("tokenName");
        var amountLabel = root.Q<Label>("amount");

        tokenNameLabel.text = tokenName;
        amountLabel.text = amount.ToString();

    }
}
