using System.Collections; //コルーチン機能を使用するための名前空間をインポートします。
using System.Collections.Generic; //ListやDictionaryなどの汎用コレクションクラスを使用するための名前空間をインポートします。
using UnityEngine; //Unityの基本的な機能を使用するための名前空間をインポートします。
using UnityEngine.UI; //UI要素を操作するための名前空間をインポートします。

//MonoBehaviourを継承したLinesManagerクラスを定義します。このクラスは会話システムの管理を行います。
public class LinesManager : MonoBehaviour
{
    //Inspectorで設定するためのpublicフィールドです。会話テキストを表示するUIのTextコンポーネントへの参照を保持します。
    public Text lineText;
    //1文字ごとの表示間隔を設定します。0.05秒ごとに1文字が表示されます。
    public float charDisplayInterval = 0.05f;
    //セリフの各行が終わった後に少し待機するための時間を設定します。0.5秒間の待機時間です。
    public float lineEndDelay = 0.5f;
    //会話IDをInspectorで設定します。特定の会話データを識別するために使用します。
    public string dialogueId;
    //ScriptableObjectをInspectorで設定します。このオブジェクトは会話データを格納しています。
    public DialogueData dialogueData;
    //会話終了後にオブジェクトをフェードアウトするかどうかを決定するフラグです。
    public bool fadeOutAfterDialogue = false;
    //会話が完了したかどうかを示すフラグです。
    private bool isDialogueCompleted = false;

    //会話の各行を格納するリストです。現在の会話エントリのセリフ行が含まれます。
    private List<DialogueLine> dialogueLines;
    //現在表示している行のインデックスを保持します。
    private int currentLineIndex = 0;
    //現在会話が表示されているかどうかを示すフラグです。
    private bool isDisplaying = false;
    //ゲーム全体の管理を行うGameManagerクラスのインスタンスを保持します。
    private GameManager gameManager;

    //指定された会話IDに基づいて会話データをロードするメソッドです。
    void LoadDialogue(string dialogueId)
    {
        //指定された会話IDを持つエントリを検索します。
        DialogueEntry entry = dialogueData.dialogues.Find(d => d.dialogueId == dialogueId);
        //会話エントリが見つかった場合
        if (entry != null)
        {
            //会話の各行を取得します。
            dialogueLines = entry.lines;
            //会話の行がnullまたは空の場合
            if (dialogueLines == null || dialogueLines.Count == 0)
            {
                //エラーメッセージを表示します。
                Debug.LogError("Dialogue lines are null or empty");
            }
        }
        else
        {
            //会話エントリが見つからなかった場合、エラーメッセージを表示します。
            Debug.LogError($"Dialogue ID {dialogueId} not found in DialogueData.");
        }
    }

    //会話を初期化するメソッドです。このメソッドは、プレイヤーがNPCに接触したときに呼び出されます。
    public void InitializeDialogue()
    {
        //既に会話が完了している場合は、何もしないで終了します。
        if (isDialogueCompleted)
        {
            return;
        }
        Color originalColor = lineText.color;
        lineText.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1.0f);
        //ゲームマネージャーのインスタンスを取得します。
        gameManager = FindObjectOfType<GameManager>();
        //会話開始時にゲームをポーズします。
        gameManager.SetGamePaused(true);
        //会話データをロードします。
        LoadDialogue(dialogueId);
        //次の行を表示するコルーチンを開始します。
        StartCoroutine(DisplayNextLine());
    }

    //次の行を表示するコルーチンです。
    public IEnumerator DisplayNextLine()
    {
        //会話の行がnullまたは空の場合
        if (dialogueLines == null || dialogueLines.Count == 0)
        {
            //エラーメッセージを表示してコルーチンを終了します。
            Debug.LogError("Dialogue lines are null or empty");
            yield break; //コルーチンを終了します。
        }
        //まだ表示していない行がある場合
        if (currentLineIndex < dialogueLines.Count)
        {
            //現在の行を取得します。
            DialogueLine line = dialogueLines[currentLineIndex];
            //名前を表示します。
            lineText.text = $"{line.Name}\n";
            //表示中フラグを設定します。
            isDisplaying = true;

            //各文を順番に表示します。
            foreach (string sentence in line.Line)
            {
                //各文字を順番に表示します。
                foreach (char letter in sentence.ToCharArray())
                {
                    //文字を追加します。
                    lineText.text += letter;
                    //指定された間隔で待機します。
                    yield return new WaitForSeconds(charDisplayInterval);
                }
                //文の終わりに改行を追加します。
                lineText.text += "\n";
                //行の終わりに指定された間隔で待機します。
                yield return new WaitForSeconds(lineEndDelay);
            }

            //表示中フラグを解除します。
            isDisplaying = false;
            //次の行に進みます。
            currentLineIndex++;
            //全ての行を表示し終えた場合
            if (currentLineIndex >= dialogueLines.Count)
            {
                //会話が完了したことを記録します。
                isDialogueCompleted = true;
                //ゲームを再開します。
                gameManager.SetGamePaused(false);
                //フェードアウトフラグが有効な場合
                if (fadeOutAfterDialogue)
                {
                    //オブジェクトをフェードアウトするコルーチンを開始します。
                    StartCoroutine(FadeOutObject());
                }
                //テキストをフェードアウトするコルーチンを開始します。
                StartCoroutine(FadeOutText());
            }
        }
    }

    //毎フレーム実行されるメソッドです。
    void Update()
    {
        //スペースキー、Aキー、Dキー、またはマウスボタンが押され、かつ表示中でなく、会話の行がnullでない場合
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D) || Input.GetMouseButtonDown(0)) && !isDisplaying && dialogueLines != null)
        {
            //次の行を表示するコルーチンを開始します。
            StartCoroutine(DisplayNextLine());
        }
    }

    //オブジェクトをフェードアウトするコルーチンです。
    private IEnumerator FadeOutObject()
    {
        //3秒待機します。
        yield return new WaitForSeconds(3.0f);
        //SpriteRendererコンポーネントを取得します。
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        //SpriteRendererコンポーネントが見つからない場合
        if (spriteRenderer == null)
        {
            //エラーメッセージを表示してコルーチンを終了します。
            Debug.LogError("SpriteRenderer component is missing on this object.");
            yield break; //コルーチンを終了します。
        }

        //フェードアウトの時間を設定します。
        float fadeDuration = 1.5f;
        //経過時間を初期化します。
        float elapsedTime = 0f;
        //元の色を取得します。
        Color originalColor = spriteRenderer.color;

        //フェードアウトの時間が経過するまでループします。
        while (elapsedTime < fadeDuration)
        {
            //徐々に透明にします。
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Lerp(1, 0, elapsedTime / fadeDuration));
            //経過時間を更新します。
            elapsedTime += Time.deltaTime;
            //次のフレームまで待機します。
            yield return null;
        }

        //完全に透明にします。
        spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0);
        //オブジェクトを非アクティブにします。
        gameObject.SetActive(false);
    }

    //テキストをフェードアウトするコルーチンです。
    private IEnumerator FadeOutText()
    {
        //1.5秒待機します。
        yield return new WaitForSeconds(1.5f);
        //フェードアウトの時間を設定します。
        float fadeDuration = 1.5f;
        //経過時間を初期化します。
        float elapsedTime = 0f;

        //元の色を取得します。
        Color originalColor = lineText.color;
        //フェードアウトの時間が経過するまでループします。
        while (elapsedTime < fadeDuration)
        {
            //徐々に透明にします。
            lineText.color = new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Lerp(1, 0, elapsedTime / fadeDuration));
            //経過時間を更新します。
            elapsedTime += Time.deltaTime;
            //次のフレームまで待機します。
            yield return null;
        }

        //完全に透明にします。
        lineText.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0);
    }

    //現在会話が表示中かどうかを返すメソッドです。
    public bool GetIsDisplaying()
    {
        //表示中フラグを返します。
        return isDisplaying;
    }
}