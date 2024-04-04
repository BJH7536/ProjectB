using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public int[] questId;               //��ü ���̵� �����ϴ°� ������?
    public int questActionIndex;
    public int questIndex;

    public Dictionary<int, QuestData> questList;
    private Dictionary<int, NpcData> questNpc;

    private static QuestManager instance;

    private void Awake()
    {
        instance = this;
        questList = new Dictionary<int, QuestData>();               //����Ʈ ���̵�: ����Ʈ ����
        questNpc = new Dictionary<int, NpcData>();
        questIndex = 1;
        GenerateData();
        CheckRequirement();
    }

    public static QuestManager GetInstance()
    {
        return instance;
    }

    void GenerateData()
    {
        questList.Add(1, new MeetPeopleQuest("��ȭ�ϱ�", 1000,"�Ҿƹ���",1,QuestState.REQUIREMENTS_NOT_MET,10,"Ʃ�丮�� ����"));

        questList.Add(2, new CoincollectQuest("���� ������", 1000, "�Ҿƹ���", 2, QuestState.CAN_START, 20, "Ʃ�丮�� ����"));

    }

    public void AdvanceQuest(int id,NpcData npc)            //����Ʈ �����Ȳ ������Ʈ
    {

        questActionIndex++;
        questNpc.Add(id, npc);
        questList[questIndex].qs++;
        Debug.Log(questList[id].qs);
        if (questList[questIndex].qs == QuestState.FINISHED)
        {
            AdvanceIndex(id);
            return;
        }  

    }


    public void CheckRequirement()             //���� ������ �¾����ٸ� ���۰����� ����Ʈ üũ
    {
        for (int i = 1; i <= questList.Count; i++){
            if (questIndex >= questList[i].Indexrequirment && questList[i].qs==QuestState.REQUIREMENTS_NOT_MET)
            {
                questList[i].qs = QuestState.CAN_START;
                Debug.Log(questList[i].qs);
            }
        }
    }

    public QuestState CheckState(int id)          //����Ʈ ���� ���� ��ȯ
    {
        return questList[id].qs;
    }
    
    public void updateState(int id)
    {
        questList[id].updateQuest();
    }

    public int getnpcId(int id)
    {
        return questList[id].npcId;
    }
    public int GetQuestTalkIndex(int id)            //NPC Id�� ����
    {
        return questIndex + questActionIndex;
    }

    public void AdvanceIndex(int qid)      //���丮 ���࿡ ���� ���� ����Ʈ�� ����� �� �ְ� �ε��� �� ����
    {
        questIndex ++;
        if (questNpc[qid].questId.Length>1)
        {
            questNpc[qid].questIndex++;
        }
        questActionIndex = 0;
    }

    public NpcData GetNpcId(int questId)
    {
        return questNpc[questId];
    }



    public QuestData GetQuestData(int questId)
    {
        return questList[questId].getQuestData();
    }

    //public int GetcurrentActionIndex(int questId)
    //{
    //    return questActionIndex;
    //}
}
