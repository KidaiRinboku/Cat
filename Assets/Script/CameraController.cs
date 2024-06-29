using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //カメラ追従フラグ
    public bool cameraFollowMoveMode = false;
    //カメラ追従対象
    public GameObject cameraFollowObject;
    //左右の壁
    public GameObject leftWall;
    public GameObject rightWall;
    //右の壁
    // Start is called before the first frame update
    void Start()
    {
        if (cameraFollowObject == null|| leftWall == null|| rightWall == null){
            Debug.Log("SET OBJECTS.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        //カメラ追従フラグがON かつ 追従対象が画面の半分以上右側にきたら、真ん中に捉えて追従
        //左右のWallオブジェクトが視界に入った時は追従しない
        if(cameraFollowMoveMode){
            CameraFollowMove();
        }
    }
    //カメラ追従を実行する
    private void CameraFollowMove(){
        //追従対象の座標を取得
        Vector3 toMovePositon = cameraFollowObject.transform.position;
        //
        transform.position = new Vector3(cameraFollowObject.transform.position.x,cameraFollowObject.transform.position.y,transform.position.z);

    }
}
