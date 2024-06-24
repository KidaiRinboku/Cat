using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public string sceneName; //遷移先のシーン名を指定するための変数

    //ボタンがクリックされたときに呼び出されるメソッド
    public void SwitchScene()
    {
        //指定されたシーンに遷移
        SceneManager.LoadScene(sceneName);
    }
}
