using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class NPCController : MonoBehaviour
{       
    //会話済みかどうかのフラグ
    bool isDialogueCompleted = false;
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player"))
        {
            LinesManager linesManager = GetComponent<LinesManager>();
            if (linesManager != null && !linesManager.GetIsDisplaying())
            {
                linesManager.InitializeDialogue();
            }
        }
    }
    public void SetisDialogueCompleted(bool flag){
        isDialogueCompleted = flag;
    }
    public bool GetisDialogueCompleted(){
        return isDialogueCompleted;
    }
}
