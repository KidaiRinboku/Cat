using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
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
}
