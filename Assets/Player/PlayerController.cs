using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    //rigid body 力を加えたりする
    Rigidbody2D rbody;
    //移動入力
    float axisH = 0.0f;
    //歩く速さ
    public float speed = 5.0f;
    //恋しいにゃーゲージ（マックスになると帰宅）
    public int nowLove = 1;
    public int maxLove = 6;
    //恋しいにゃーゲージクラス呼び出し用
    private LoveGaugeManager loveGaugeManager;
    //元気ゲージ
    //食べ過ぎゲージ（元気を越えて食べすぎると眠ってしまいタイムロス）
    //重力　初期値1.5 ステージによって変える
    //テンション　MAXになるとアイテム取りすぎ、デバフ要素を一定時間受けなくなる
    //キャラクターのジャンプ力
    public float jumpPw = 5.0f;
    //ジャンプを実行してるか
    bool goJump = false;
    //地面レイヤーを指定する
    public LayerMask groundLayer;
    
    

    private SpriteRenderer spriteRenderer;
    
    // Start is called before the first frame update
    void Start()
    {
        //rigidbody2d取得
        rbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        loveGaugeManager = FindObjectOfType<LoveGaugeManager>();
        UpdateLoveGauge();
        
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("axisH:"+axisH);
        //地面判定
        bool onGround = onGroundCheck(gameObject);
        //歩く用の入力チェック
        axisH = Input.GetAxisRaw("Horizontal");
        if(axisH > 0){
                //transform.localScale = new Vector2(1,1);
                spriteRenderer.flipX = false;
        }
        else if(axisH < 0){
                //transform.localScale = new Vector2(-1,1);
                spriteRenderer.flipX = true;
        }
        //地面にいるときにジャンプ用ボタンが押されてたらジャンプメソッド
        if(onGround&&Input.GetButton("Jump")){
    // if(Input.GetButton("Jump")){
            Jump();
        }
    }
    void FixedUpdate() {
            //指定方向に歩く。　ジャンプ中に移動キーをなしても落ちないように条件付け。
            //地面にいるか、速度が０じゃない時歩く。
            bool onGround = onGroundCheck(gameObject);
            if(onGround || axisH!=0){
               rbody.velocity = new Vector2(speed * axisH,rbody.velocity.y);
            }
            //地ジャンプフラグがONの時ジャンプする
            if(goJump){
      //      if(onGround&&goJump){
                Vector2 jumpVec = new Vector2(0,jumpPw);
                rbody.velocity = new Vector2(rbody.velocity.x,0);
                rbody.AddForce(jumpVec,ForceMode2D.Impulse);
                goJump = false;
            }
    }
    private void OnTriggerEnter2D(Collider2D collision) {
        //恋しいにゃ〜ゲージに関与するアイテムに触った時の処理
        if(collision.gameObject.tag=="LoveItem"){
            IncreaseLove();
            Destroy(collision.gameObject);
        }    
    }
    //オブジェクトを渡して、地面にいるか判定する
    public bool onGroundCheck(GameObject obj){
        bool onGround = Physics2D.CircleCast(transform.position,0.2f,Vector2.down,0.0f,groundLayer);
        return onGround;
    }
    //ジャンプメソッド　ジャンプフラグをONにする
    public void Jump(){
        goJump = true;
    }
    //恋しいにゃーゲージの増加と帰宅に関する処理
    public void IncreaseLove()
    {
        if (nowLove < maxLove)
        {
            nowLove++;
            UpdateLoveGauge();
        }

        if (nowLove == maxLove)
        {
            //恋しいにゃ〜
            Debug.Log("BIG Love!!!!!!!!!");
        }
    }
    //恋しいにゃ〜ゲージ増加用
    public void IncreaseMaxLove()
    {
        if (maxLove < loveGaugeManager.hearts.Length)
        {
            maxLove++;
            UpdateLoveGauge();  
        }
    }

    //恋しいにゃ〜ゲージ見た目変更
    private void UpdateLoveGauge()
    {
        if (loveGaugeManager != null)
        {
            loveGaugeManager.UpdateHearts(nowLove, maxLove);
        }
    }
}
