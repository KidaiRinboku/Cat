using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueData", menuName = "ScriptableObjects/DialogueData", order = 1)]
public class DialogueData : ScriptableObject
{
    public List<DialogueEntry> dialogues;
}

[System.Serializable]
public class DialogueEntry
{
    public string dialogueId;
    public List<DialogueLine> lines;
}

[System.Serializable]
public class DialogueLine
{
    public string IconID;
    public string Name;
    public List<string> Line;
}