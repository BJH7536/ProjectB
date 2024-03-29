using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_DialoguePopup : UI_Popup
{
    [SerializeField] private GameObject DialoguePopup;
    [SerializeField] public GameObject dialoguePanel;
    [SerializeField] public TextMeshProUGUI dialogueText;
    [SerializeField] public TextMeshProUGUI displayNameText;
    [SerializeField] public Image portraitImage;

    [SerializeField] public TextMeshProUGUI[] choicesText;
    [SerializeField] public GameObject[] choices;
    [SerializeField] public GameObject choicep;
    [SerializeField] public Button[] choiceButton;
    private bool isAction;

    public static UI_DialoguePopup instance;

    enum UIs
    {
        DialoguePanel,
        DialogueChoices
    }

    enum Buttons
    {
        Choice0, 
        Choice1, 
        Choice2
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        DialoguePopup = this.gameObject;
        dialoguePanel = DialoguePopup.transform.GetChild(0).gameObject;
        dialogueText=dialoguePanel.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        displayNameText=dialoguePanel.transform.GetChild(4).GetChild(0).GetComponent<TextMeshProUGUI>();
        portraitImage = dialoguePanel.transform.GetChild(3).transform.GetChild(0).GetComponent<Image>();


        choicep = dialoguePanel.transform.GetChild(2).gameObject;
        choices = new GameObject[2] { dialoguePanel.transform.GetChild(2).GetChild(0).gameObject, dialoguePanel.transform.GetChild(2).GetChild(1).gameObject };
        choicesText= new TextMeshProUGUI[2] { choices[0].GetComponent<TextMeshProUGUI>(), choices[1].GetComponent<TextMeshProUGUI>()};
        choiceButton = new Button[2] { choices[0].GetComponent<Button>(), choices[1].GetComponent<Button>() };
        return true;
    }

    private void Awake()
    {
        instance = this;
       

    }

    private void Start()
    {
        for (int i = 0; i < choices.Length; i++)
        {
            int id = i;
            choiceButton[i].onClick.AddListener(() => makeChoice(id));
        }
    }

    public static UI_DialoguePopup GetInstance()
    {
        return instance;
    }


    public void makeChoice(int choice)
    {
        Player.GetInstance().select = choice;
        DebugEx.Log(choice);
    }

   
}
