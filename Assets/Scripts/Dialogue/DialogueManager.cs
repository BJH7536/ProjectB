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
    private Story currentStory;                                     //Ink 로 생성된 텍스트를 받아올 Class변수

    private const string SPEAKER_TAG = "speaker";                   //테그값들 테그값 : 변수
    private const string PORTRAIT_TAG = "portrait";
    private const string PLAYER_TAG = "player";
    private const string LAYOUT_TAG = "layout";
    public UI_DialoguePopup popup;
    public QuestLayer qpanel;
    public PlayerController playerController;
    
    public bool dialogueIsPlaying { get; private set; }             //현재 대화창에 진입했는지 확인할 변수
    //퀘스트 진행상황은 퀘스트 메니저에서 관리

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
        if (playerController.GetInteractPressed())
        {
            ContinueStory();
        }
    }

    public void GetTalk2(NpcData npc)
    {
        npcdata = npc;
        if (QuestManager.GetInstance().CheckState(npc.questId[npc.questIndex]) == QuestState.FINISHED)
        {
            currentStory = new Story(npc.dialogue[npc.dialogue.Length-1].text);
        }
        else
        {
            if (npc.questId.Length > 1)
            {   
                //questIndex : 0부터 시작
                //questActionIndex : 0 시작 1; 중간 2: 끝낼수있을때 3: 끝 
                currentStory = new Story(npc.dialogue[(npc.questIndex*4 + QuestManager.GetInstance().questActionIndex)].text);         //0:퀘스트시작 2: 퀘스트중간 3: 퀘스트 끝낼수 있을때 4: 퀘스트끝나고 퀘스트없을때 
            }
            else
            {
                currentStory = new Story(npc.dialogue[(npc.questIndex + QuestManager.GetInstance().questActionIndex)].text);         //0:퀘스트시작 2: 퀘스트중간 3: 퀘스트 끝낼수 있을때 4: 퀘스트끝나고 퀘스트없을때
            }
            
        }
        dialogueIsPlaying = true;
        popup.dialoguePanel.SetActive(true);

        //태그 초기화
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
        if (currentStory.canContinue)                   //더 보여줄 이야기가 있다면
        {
            popup.dialogueText.text = currentStory.Continue();            //한줄 출력
            DisplayChoices();                                       //선택이 있으면 선택출력
            //태그관리
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
                    popup.portraitImage.sprite = playerController.getplayerPortrait(int.Parse(tagvalue));
                    break;
                case LAYOUT_TAG:
                    popup.layoutAnimator.Play(tagvalue);
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
        if (currentChoices.Count > popup.choices.Length)           //현재 선택지의 개수가 버튼의 개수보다 많으면 오류 
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
                    //qpanel.questlist.AddQuest(QuestManager.GetInstance().GetQuestData(npcdata.questId[npcdata.questIndex]));
                }else if (qs == QuestState.CAN_FINISH)
                {
                    QuestManager.GetInstance().AdvanceQuest(npcdata.questId[npcdata.questIndex], npcdata);
                    QuestManager.GetInstance().AdvanceNpcIndex(npcdata);
                }
            }
        }
        DebugEx.Log(choice);
    }


}
