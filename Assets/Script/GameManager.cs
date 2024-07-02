using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    PlayerController playerController;
    public GameObject player;
    private static GameManager instance;
    private bool isGamePaused;
    SceneController playerSceneController;

    public bool IsGamePaused()
    {
        return isGamePaused;
    }

    public void SetGamePaused(bool paused)
    {
        isGamePaused = paused;
    }

    void Start()
    {
        if(player != null){
            playerSceneController = player.GetComponent<SceneController>();
        }else{
            Debug.Log("player is null");
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //猫が帰る処理
    public void GoHome(){
        if(playerSceneController != null){
            playerSceneController.SwitchScene();
        }else{
            Debug.Log("playerSceneController is null");
        }
        
    }
}
