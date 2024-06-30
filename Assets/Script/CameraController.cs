using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public bool cameraFollowMoveMode = false; //カメラの追従モードのフラグ
    public GameObject cameraFollowObject; //追従対象のオブジェクト
    public GameObject leftWall; //左側の壁オブジェクト
    public GameObject rightWall; //右側の壁オブジェクト
    public LayerMask groundLayerMask; //地面のレイヤーマスク
    public float rayLength = 10.0f; //レイキャストの長さ
    private Camera mainCamera; //メインカメラの参照
    private float initialBottomLimit; //初期の下限位置
    private bool groundFound = false; //初期の地面が見つかったかのフラグ

    void Start()
    {
        mainCamera = gameObject.GetComponent<Camera>(); //メインカメラのコンポーネントを取得
        if (mainCamera == null || cameraFollowObject == null || leftWall == null || rightWall == null)
        {
            Debug.LogError("SET OBJECTS."); //必要なオブジェクトがセットされていない場合のエラーメッセージ
        }
    }

    void Update()
    {
        if (cameraFollowMoveMode)
        {
            CameraFollowMove(); //追従モードが有効な場合にカメラを追従させる
        }
        if (!groundFound)
        {
            float groundHeight = GetGroundHeight(cameraFollowObject.transform.position);
            if (groundHeight != cameraFollowObject.transform.position.y - rayLength)
            {
                initialBottomLimit = groundHeight + mainCamera.orthographicSize; //初期の地面の下限位置を設定
                groundFound = true; //初期の地面が見つかったフラグを設定
            }
        }
    }

    private void CameraFollowMove()
    {
        Vector3 targetPosition = cameraFollowObject.transform.position; //追従対象の位置を取得
        float cameraHalfWidth = mainCamera.orthographicSize * mainCamera.aspect; //カメラの半分の幅を計算
        float cameraHalfHeight = mainCamera.orthographicSize; //カメラの半分の高さを計算

        BoxCollider2D leftWallCollider = leftWall.GetComponent<BoxCollider2D>(); //左壁のコライダーを取得
        BoxCollider2D rightWallCollider = rightWall.GetComponent<BoxCollider2D>(); //右壁のコライダーを取得

        if (leftWallCollider != null && rightWallCollider != null)
        {
            //左壁の右端と右壁の左端を取得して制限を設定
            float leftLimit = leftWallCollider.bounds.max.x + cameraHalfWidth;
            float rightLimit = rightWallCollider.bounds.min.x - cameraHalfWidth;

            float groundHeight = GetGroundHeight(targetPosition) + cameraHalfHeight;
            if (groundHeight > initialBottomLimit)
            {
                groundHeight = initialBottomLimit; //初期の下限位置を超えないように制限
            }

            float clampedX = Mathf.Clamp(targetPosition.x, leftLimit, rightLimit); //X方向の位置を制限
            float clampedY = Mathf.Clamp(targetPosition.y, groundHeight, float.MaxValue); //Y方向の位置を制限

            //カメラの位置を更新
            transform.position = new Vector3(clampedX, clampedY, transform.position.z);
        }
        else
        {
            Debug.LogError("Walls must have BoxCollider2D components."); //壁にBoxCollider2Dがない場合のエラーメッセージ
        }
    }

    private float GetGroundHeight(Vector3 position)
    {
        RaycastHit2D hit = Physics2D.Raycast(position, Vector2.down, rayLength, groundLayerMask); //下方向にレイキャストを実行して地面を検出
        if (hit.collider != null)
        {
            return hit.collider.bounds.min.y; //地面の下辺の位置を返す
        }
        return position.y - rayLength; //地面が見つからなかった場合のデフォルト値を返す
    }
}