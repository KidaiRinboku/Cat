using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LinesManager : MonoBehaviour
{
    public Text lineText; //LineTextフィールドをInspectorで設定
    public float charDisplayInterval = 0.05f; //1文字ごとの表示間隔
    public float lineEndDelay = 0.5f; //, の後のディレイ時間
    public string dialogueId; //会話IDをInspectorで設定
    public DialogueData dialogueData; //ScriptableObjectをInspectorで設定
    public bool fadeOutAfterDialogue = false; //会話終了後にフェードアウトするかのフラグ

    private List<DialogueLine> dialogueLines;
    private int currentLineIndex = 0;
    private bool isDisplaying = false;
    private GameManager gameManager;

    void LoadDialogue(string dialogueId)
    {
        DialogueEntry entry = dialogueData.dialogues.Find(d => d.dialogueId == dialogueId);
        if (entry != null)
        {
            dialogueLines = entry.lines;
            if (dialogueLines == null || dialogueLines.Count == 0)
            {
                Debug.LogError("Dialogue lines are null or empty");
            }
        }
        else
        {
            Debug.LogError($"Dialogue ID {dialogueId} not found in DialogueData.");
        }
    }

    public void InitializeDialogue()
    {
        gameManager = FindObjectOfType<GameManager>();
        gameManager.SetGamePaused(true); //会話開始時にゲームをポーズ
        LoadDialogue(dialogueId);
        StartCoroutine(DisplayNextLine());
    }

    public IEnumerator DisplayNextLine()
    {
        if (dialogueLines == null || dialogueLines.Count == 0)
        {
            Debug.LogError("Dialogue lines are null or empty");
            yield break;
        }
        if (currentLineIndex < dialogueLines.Count)
        {
            DialogueLine line = dialogueLines[currentLineIndex];
            lineText.text = $"{line.Name}\n";
            isDisplaying = true;

            foreach (string sentence in line.Line)
            {
                foreach (char letter in sentence.ToCharArray())
                {
                    lineText.text += letter;
                    yield return new WaitForSeconds(charDisplayInterval);
                }
                lineText.text += "\n";
                yield return new WaitForSeconds(lineEndDelay);
            }

            isDisplaying = false;
            currentLineIndex++;
            if (currentLineIndex >= dialogueLines.Count)
            {
                gameManager.SetGamePaused(false); //会話終了時にゲームを再開
                if (fadeOutAfterDialogue)
                {
                    StartCoroutine(FadeOutObject());
                }
                StartCoroutine(FadeOutText());
            }
        }
    }

    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D) || Input.GetMouseButtonDown(0)) && !isDisplaying && dialogueLines != null)
        {
            StartCoroutine(DisplayNextLine());
        }
    }

    private IEnumerator FadeOutObject()
    {
        yield return new WaitForSeconds(3.0f);
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer component is missing on this object.");
            yield break;
        }

        float fadeDuration = 1.5f;
        float elapsedTime = 0f;
        Color originalColor = spriteRenderer.color;

        while (elapsedTime < fadeDuration)
        {
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Lerp(1, 0, elapsedTime / fadeDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0);
        gameObject.SetActive(false);
    }

    private IEnumerator FadeOutText()
    {
        yield return new WaitForSeconds(1.5f);
        float fadeDuration = 1.5f;
        float elapsedTime = 0f;

        Color originalColor = lineText.color;
        while (elapsedTime < fadeDuration)
        {
            lineText.color = new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Lerp(1, 0, elapsedTime / fadeDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        lineText.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0);
    }

    public bool GetIsDisplaying()
    {
        return isDisplaying;
    }
}