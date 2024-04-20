using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SeparateIngredient : UI_Popup
{
    enum Images
    {
        Back,
    }

    enum InputFields
    {
        InputField,
    }

    enum Buttons
    {
        Increase,
        Decrease,
        Confirm,
        Deny,
    }

    private TMP_InputField _inputField;
    
    private UI_Inven_Item _originItem;
    
    private int _itemOriginAmount;      // �������� ���� ����
    
    public override void Init()
    {
        Bind<Image>(typeof(Images));
        Bind<TMP_InputField>(typeof(InputFields));
        Bind<Button>(typeof(Buttons));
        
        Get<Image>((int)Images.Back).gameObject.AddUIEvent(ClosePopupUI);
        Get<Button>((int)Buttons.Increase).onClick.AddListener(Increase);
        Get<Button>((int)Buttons.Decrease).onClick.AddListener(Decrease);
        Get<Button>((int)Buttons.Confirm).onClick.AddListener(Confirm);
        Get<Button>((int)Buttons.Deny).onClick.AddListener(Deny);

        _inputField = Get<TMP_InputField>((int)InputFields.InputField);
        _inputField.onValueChanged.AddListener(CheckValue);
    }
    
    public void InitItemReference(UI_Inven_Item originItem)
    {
        _originItem = originItem;
        _itemOriginAmount = _originItem.Amount;
    }

    /// <summary>
    /// ���� 0�̸� Confirm ��ư ��Ȱ��ȭ
    /// </summary>
    /// <param name="str"></param>
    void CheckValue(string str)
    {
        Get<Button>((int)Buttons.Confirm).interactable = int.Parse(str) != 0;
    }
    
    void Increase()
    {
        var current = int.Parse(_inputField.text);
        _inputField.text = $"{current + 1}";
    }
    
    void Decrease()
    {
        var current = int.Parse(_inputField.text);
        if (current == 0) return;
        _inputField.text = $"{current - 1}";
    }

    void Confirm()
    {
        // �и��� �������� ���콺 Ŀ���� ���ܾ� �Ѵ�.
        UI_Inven_Item item = Managers.UI.MakeSubItem<UI_Inven_Item>();
        item.IngredientInit(_originItem.Name, _originItem.Quality, int.Parse(_inputField.text));
        
        _originItem.Amount -= int.Parse(_inputField.text);
        _originItem.OnValueChange.Invoke();
        
        ClosePopupUI(null);
        item.transform.SetParent(Managers.UI.GetTopPopupUI().transform);
        item.Catched();
    }

    void Deny()
    {
        ClosePopupUI(null);
    }
}
