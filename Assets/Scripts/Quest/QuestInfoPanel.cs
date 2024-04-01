using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using VInspector;

public class QuestInfoPanel : MonoBehaviour
{
    //����Ʈ �󼼼��� -> �̸�, ����, ��ġ,���õ� npc�̸�,�����Ȳ textmeshpro 5�� �ʿ�?
    [Tab("QuestInfo")]
    [SerializeField]private TextMeshPro questTitle;
    [SerializeField] private TextMeshPro questDescription;
    [SerializeField] private TextMeshPro questLoc;
    [SerializeField] private TextMeshPro npcName;
    [SerializeField] private TextMeshPro questState;
    private static QuestInfoPanel instance;
    private void Awake()
    {
        questTitle.text = "";
        questDescription.text = "";
        questLoc.text = "";
        npcName.text = "";
        questState.text = "";
    }

    public static QuestInfoPanel GetInstance()
    {
        return instance;
    }
    public void setQuestInfo(QuestData questData)
    {
        questTitle.text = questData.questName;
        questDescription.text = questData.getQuestInfo();
        questLoc.text = questData.loc;
        npcName.text = questData.npcname;
        questState.text = questData.qs.ToString();
    }
}
