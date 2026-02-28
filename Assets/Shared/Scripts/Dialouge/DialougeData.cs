using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "EmptyFrame/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    [TextArea(3, 10)]
    public List<string> Lines;
}