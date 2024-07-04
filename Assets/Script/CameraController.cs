using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public bool cameraFollowMoveMode = false; //カメラの追従モードのフラグ。trueの場合、カメラが追従する。
    public GameObject cameraFollowObject; //追従対象のオブジェクト。プレイヤーキャラクターなど。
    public GameObject leftWall; //左側の壁オブジェクト。カメラの左限界を設定するため。
    public GameObject rightWall; //右側の壁オブジェクト。カメラの右限界を設定するため。
    public LayerMask groundLayerMask; //地面のレイヤーマスク。レイキャストがどのオブジェクトを地面として認識するかを指定する。
    public float rayLength = 10.0f; //レイキャストの長さ。レイキャストが地面を検出する範囲。
    private Camera mainCamera; //メインカメラの参照。カメラのプロパティにアクセスするために使用。
    private float initialBottomLimit; //初期の下限位置。カメラのY方向の下限を設定するための変数。
    private bool groundFound = false; //初期の地面が見つかったかのフラグ。地面の初期位置を一度だけ設定するために使用。

    //Unityのライフサイクルメソッド。ゲーム開始時に一度だけ呼び出される
    void Start()
    {
        mainCamera = gameObject.GetComponent<Camera>(); //メインカメラのコンポーネントを取得。カメラのプロパティにアクセスするため。
        //もしmainCamera、cameraFollowObject、leftWall、rightWallのいずれかがnullならばエラーメッセージを表示
        if (mainCamera == null || cameraFollowObject == null || leftWall == null || rightWall == null)
        {
            Debug.LogError("SET OBJECTS."); //必要なオブジェクトがセットされていない場合のエラーメッセージを表示
        }
    }

    //Unityのライフサイクルメソッド。固定フレームレートで呼び出され、物理演算の更新に使用される
    void FixedUpdate()
    {
        //もしcameraFollowMoveModeがtrueならばカメラ追従の処理を実行する
        if (cameraFollowMoveMode)
        {
            CameraFollowMove(); //追従モードが有効な場合にカメラを追従させる
        }
        //groundFoundがfalseの場合のみ初期の地面の位置を設定する
        if (!groundFound)
        {
            //追従対象の位置を基準に地面の高さを取得
            float groundHeight = GetGroundHeight(cameraFollowObject.transform.position);
            //もし取得した地面の高さが、レイキャストの長さ分だけ下がった位置と異なるならば初期の下限位置を設定
            if (groundHeight != cameraFollowObject.transform.position.y - rayLength)
            {
                initialBottomLimit = groundHeight + mainCamera.orthographicSize; //初期の地面の下限位置を設定。カメラの高さの半分を加算
                groundFound = true; //初期の地面が見つかったフラグを設定。以後、この処理は実行されない
            }
        }
    }

    //カメラ追従の処理を実行するメソッド
    private void CameraFollowMove()
    {
        Vector3 targetPosition = cameraFollowObject.transform.position; //追従対象の現在位置を取得
        float cameraHalfWidth = mainCamera.orthographicSize * mainCamera.aspect; //カメラの半分の幅を計算。カメラの高さの半分とアスペクト比を掛け合わせる
        float cameraHalfHeight = mainCamera.orthographicSize; //カメラの半分の高さを計算。カメラの正射影サイズ

        BoxCollider2D leftWallCollider = leftWall.GetComponent<BoxCollider2D>(); //左壁のコライダーを取得。壁の位置を検出するため
        BoxCollider2D rightWallCollider = rightWall.GetComponent<BoxCollider2D>(); //右壁のコライダーを取得。壁の位置を検出するため

        //左壁と右壁のコライダーが存在する場合にカメラの位置を制限する処理を実行
        if (leftWallCollider != null && rightWallCollider != null)
        {
            //左壁の右端の位置にカメラの半分の幅を加算して左限を設定
            float leftLimit = leftWallCollider.bounds.max.x + cameraHalfWidth;
            //右壁の左端の位置からカメラの半分の幅を減算して右限を設定
            float rightLimit = rightWallCollider.bounds.min.x - cameraHalfWidth;

            //追従対象の位置に基づいて地面の高さを取得し、カメラの半分の高さを加算
            float groundHeight = GetGroundHeight(targetPosition) + cameraHalfHeight;
            //地面の高さが初期の下限位置を超えないように制限
            if (groundHeight > initialBottomLimit)
            {
                groundHeight = initialBottomLimit;
            }

            //追従対象のX位置を左限と右限の間に制限。プレイヤーを少し左にずらすために8.0fを加算  + 8.0f,
            float clampedX = Mathf.Clamp(targetPosition.x, leftLimit, rightLimit);
            //追従対象のY位置を地面の高さから無限大の間に制限
            float clampedY = Mathf.Clamp(targetPosition.y, groundHeight, float.MaxValue);

            //カメラの位置を更新。計算された制限値を使用してカメラの位置を設定
            transform.position = new Vector3(clampedX, clampedY, transform.position.z);
        }
        else
        {
            Debug.LogError("Walls must have BoxCollider2D components."); //壁にBoxCollider2Dがない場合のエラーメッセージを表示
        }
    }

    //追従対象の位置から下方向にレイキャストを実行して地面を検出するメソッド
    private float GetGroundHeight(Vector3 position)
    {
        RaycastHit2D hit = Physics2D.Raycast(position, Vector2.down, rayLength, groundLayerMask); //追従対象の位置から下方向にレイキャストを実行して地面を検出
        //もし地面のコライダーに当たった場合、その地面の下辺の位置を返す
        if (hit.collider != null)
        {
            return hit.collider.bounds.min.y;
        }
        return position.y - rayLength; //地面が見つからなかった場合のデフォルト値を返す。レイキャストの長さ分下を返す
    }
}