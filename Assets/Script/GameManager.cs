using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    PlayerController playerController;
    GameObject player;

    SceneController playerSceneController;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerSceneController = player.GetComponent<SceneController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //猫が帰る処理
    public void GoHome(){
        playerSceneController.SwitchScene();
    }
}
