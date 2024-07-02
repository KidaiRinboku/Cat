using System.Collections.Generic; // ListやDictionaryなどの汎用コレクションクラスを使用するための名前空間

using UnityEngine; // Unityの基本的なクラスや機能を使用するための名前空間

[CreateAssetMenu(fileName = "DialogueData", menuName = "ScriptableObjects/DialogueData", order = 1)] // ScriptableObjectを作成するためのメニュー項目を追加
public class DialogueData : ScriptableObject // ScriptableObjectを継承することで、Unityエディタでデータコンテナとして使用できるようにする
{
    public List<DialogueEntry> dialogues; // 複数の会話エントリを格納するためのリスト
}

[System.Serializable] // クラスをシリアライズ可能にすることで、Unityエディタ内でデータを編集可能にする
public class DialogueEntry // 各会話エントリを表すクラス
{
    public string dialogueId; // 会話エントリを識別するための一意なID
    public List<DialogueLine> lines; // 会話エントリ内の複数のセリフ行を格納するリスト
}

[System.Serializable] // クラスをシリアライズ可能にすることで、Unityエディタ内でデータを編集可能にする
public class DialogueLine // 各セリフ行を表すクラス
{
    public string IconID; // 話者のアイコンを識別するためのID
    public string Name; // 話者の名前
    public List<string> Line; // セリフの内容を格納するリスト、複数の行で構成される
}