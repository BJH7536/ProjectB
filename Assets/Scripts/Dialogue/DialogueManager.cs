using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;


public class DialogueManager : MonoBehaviour
{
    private const int npcId = 10000;
    private const int dialogueId = 10;
    
    Dictionary<int, string[]> talkData;
    Dictionary<int,int> portraitData;
    Dictionary<int, string[]> ChoiceData;
    private NpcData npcdata;

    private const string SPEAKER_TAG = "speaker";                   //�ױװ��� �ױװ� : ����
    private const string PORTRAIT_TAG = "portrait";
    private const string LAYOUT_TAG = "layout";
    private UI_DialoguePopup popup;

    //����Ʈ �����Ȳ�� ����Ʈ �޴������� ����


    private void Awake()
    {

        talkData = new Dictionary<int, string[]>();
        portraitData = new Dictionary<int, int>();
        GenerateData();
    }

    void GenerateData()     //����Ʈ��ȣ+NPCId => ����Ʈ�� ��ȭ ������ Id
    {
        var Textfile = @"c:/";
        if (File.Exists(Textfile))
        {
            using(var reader = new StreamReader(Textfile)) 
            { 
                while(!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    string[] sentence = line.Split(",");
                }
            }
        }
        //Basic Talk Data
        talkData.Add(1000, new string[] { "�ȳ�?:0", " �� ������ ��¾���̾�?:1" });

        talkData.Add(2000, new string[] { "�ȳ�?:0", " �ʴ� ������?:1" });

        //Quest Talk Data
        talkData.Add(1000+10, new string[] { "���:0", "���� npc2�� ������÷�?:1:��:�ƴϿ�" });       //����Ʈ ��ȭ
        talkData.Add(1000 + 11, new string[] { "��:0", "npc�� ������ �Ծ�?:1" });                      //����Ʈ ���� �� ��ȭ
        talkData.Add(1000 + 12, new string[] { "���� npc2�� ������ �Ա���:1", "���߾�!:1" });                      //����Ʈ ���� �� ��ȭ
        talkData.Add(2000+11, new string[] { "�ȳ�?:0", "������ �Ϸ� ������� �Ծ�?:1" });
        portraitData.Add(1000 + 0, 0);
        portraitData.Add(1000 + 1, 1);
        portraitData.Add (1000 + 2, 2);

        portraitData.Add(2000 + 0, 0);
        portraitData.Add(2000 + 1, 1);
        portraitData.Add(2000 + 2, 2);
    }

    public string GetTalk(int id,int talkIndex)
    {
        if (!talkData.ContainsKey(id))       //����Ʈ �������� ����ִ��� üũ
        {
            //����Ʈ ���� �߿��� ����Ʈ ���� ������
            if (!talkData.ContainsKey(id - id % 10))
            {
                return GetTalk(id - id % 100, talkIndex);
            }
            else    //����Ʈ ��� ��������
            {
                return GetTalk(id - id % 10, talkIndex);

            }
        }
        //�⺻ ��� �������� �ڵ�
        if (talkIndex == talkData[id].Length) return null;
        else return talkData[id][talkIndex];
    }

    //public void GetTalk2(int id, int talkIndex)
    //{
    //    currentStory = new Story();
    //}


    public int GetPortraitIndex(int id,int portraitIndex)
    {
        return portraitData[id + portraitIndex];
    }

   

    private IEnumerator SelectFirstChoice()
    {
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(popup.choices[0].gameObject);
    }

    //public void MakeChoice(int choiceIndex, string npcId)
    //{
    //    currentStory.ChooseChoiceIndex(choiceIndex);
    //    Debug.Log(npcId);
    //    if (choiceIndex == 0 && npcId.Contains("Quest") && start)
    //    {
    //        //NPC id �� Quest ID�� ��ġ���Ѿ���
    //        Managers.Questevent.StartQuest(npcId);
    //    }
    //    else if (choiceIndex == 0 && npcId.Contains("Quest") && end)
    //    {
    //        Managers.Questevent.FinishQuest(npcId);
    //    }
    //}

}
