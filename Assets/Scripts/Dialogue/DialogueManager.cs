using Ink.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.EventSystems;


public class DialogueManager : MonoBehaviour
{


    private NpcData npcdata;
    private Story currentStory;                                     //Ink �� ������ �ؽ�Ʈ�� �޾ƿ� Class����

    private const string SPEAKER_TAG = "speaker";                   //�ױװ��� �ױװ� : ����
    private const string PORTRAIT_TAG = "portrait";
    private const string PLAYER_TAG = "player";
    private const string LAYOUT_TAG = "layout";
    public UI_DialoguePopup popup;
    public int choicelen;
    public int curchoice;
    
    public bool dialogueIsPlaying { get; private set; }             //���� ��ȭâ�� �����ߴ��� Ȯ���� ����
    //����Ʈ �����Ȳ�� ����Ʈ �޴������� ����

    public static DialogueManager instance;

    private void Awake()
    {
        instance = this;

    }

    public static DialogueManager GetInstance()
    {
        return instance;
    }

    private void Update()
    {
        if (!dialogueIsPlaying)
        {
            return;
        }
        if (PlayerController.GetInstance().GetInteractPressed())
        {
            ContinueStory();
        }
    }

    public void GetTalk2(NpcData npc)
    {
        npcdata = npc;
        currentStory = new Story(npc.dialogue[QuestManager.GetInstance().questActionIndex].text);
        dialogueIsPlaying = true;
        popup.dialoguePanel.SetActive(true);

        //�±� �ʱ�ȭ
        popup.displayNameText.text = "???";
        ContinueStory();
    }



    private void ExitDialogueMode()
    {
        dialogueIsPlaying = false;
        popup.dialoguePanel.SetActive(false);
        popup.dialogueText.text = "";
    }

    private void ContinueStory()
    {
        if (currentStory.canContinue)                   //�� ������ �̾߱Ⱑ �ִٸ�
        {
            popup.dialogueText.text = currentStory.Continue();            //���� ���
            DisplayChoices();                                       //������ ������ �������
            //�±װ���
            HandleTags(currentStory.currentTags);
        }
        else
        {
            ExitDialogueMode();
        }
    }

    private void HandleTags(List<string> currentTags)
    {
        foreach (string tag in currentTags)
        {
            string[] splitTag = tag.Split(':');
            if (splitTag.Length != 2)
            {
                Debug.LogError("Tag parsed error : " + tag);
            }
            string tagkey = splitTag[0].Trim();
            string tagvalue = splitTag[1].Trim();

            switch (tagkey)
            {
                case SPEAKER_TAG:
                    popup.displayNameText.text = tagvalue;
                    break;
                case PORTRAIT_TAG:
                    popup.portraitImage.sprite = npcdata.npcPortrait[int.Parse(tagvalue)];
                    break;
                case PLAYER_TAG:
                    popup.portraitImage.sprite = PlayerController.GetInstance().getplayerPortrait(int.Parse(tagvalue));
                    break;
                default:
                    Debug.LogWarning("Tag exists but not handled");
                    break;
            }

        }
    }

    private void DisplayChoices()
    {
        List<Choice> currentChoices = currentStory.currentChoices;
        choicelen = currentChoices.Count;
        if (currentChoices.Count > popup.choices.Length)           //���� �������� ������ ��ư�� �������� ������ ���� 
        {
            Debug.LogError("More choices than ever");
        }

        int index = 0;
        foreach (Choice choice in currentChoices)
        {
            popup.choices[index].gameObject.SetActive(true);
            popup.choicesText[index].text = choice.text;
            index++;
        }

        for (int i = index; i < popup.choices.Length; i++)
        {
            popup.choices[i].gameObject.SetActive(false);
        }
        popup.choicep.SetActive(true);

        StartCoroutine(SelectFirstChoice());
    }

    private IEnumerator SelectFirstChoice()
    {
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(popup.choices[0].gameObject);
    }

    public void makeChoice(int choice)
    {
        currentStory.ChooseChoiceIndex(choice);
        if (choice == 0)
        {
            if (npcdata.questId.Length > 0)
            {
                QuestState qs = QuestManager.GetInstance().CheckState(npcdata.questId[npcdata.questIndex]);
                if (qs == QuestState.CAN_START)
                {
                    QuestManager.GetInstance().AdvanceQuest(npcdata.questId[npcdata.questIndex], npcdata);
                }
            }
        }
        DebugEx.Log(choice);
    }


}
