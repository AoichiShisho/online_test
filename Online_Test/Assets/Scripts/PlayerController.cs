using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Transform viewPoint; // カメラの親オブジェクト
    public float mouseSensitivity = 1f; // 視点移動の速度
    private Vector2 mouseInput; // ユーザーのマウス入力格納
    private float verticalMouseInput; // y軸の回転移動
    private Camera cam; // カメラ

    private Vector3 moveDir; // 入力された値格納
    private Vector3 movement; // 進む方向格納
    private float activeMoveSpeed = 4f;// 実際の移動速度

    public Vector3 jumpForce = new Vector3(0, 6, 0); // ジャンプ力
    public Transform groundCheckPoint; //例を飛ばすオブジェクトの位置
    public LayerMask groundLayers; // 地面レイヤー
    private Rigidbody rb; // 剛体

    public float walkSpeed = 4f; // 歩きの速度
    public float runSpeed = 8f; // 走りの速度

    private bool cursorLock = true; // カーソルの表示判定


    // Start is called before the first frame update
    private void Start()
    {
        cam = Camera.main; // カメラの格納

        rb = GetComponent<Rigidbody>();

        // カーソルの表示判定関数
        UpdateCursorLock();
    }

    // Update is called once per frame
    private void Update()
    {
        PlayerRotate(); // 視点移動関数の呼び出し
        PlayerMove(); // 移動関数を呼び出す
        Run(); // 走り関数の呼び出し
        Jump(); // ジャンプ関数を呼び起こす

        // カーソルの表示判定関数
        UpdateCursorLock();
    }

    public void PlayerRotate()
    {
        // 変数にユーザーのマウスの動きを格納
        mouseInput = new Vector2(Input.GetAxisRaw("Mouse X") * mouseSensitivity, 
                                 Input.GetAxisRaw("Mouse Y") * mouseSensitivity);

        // マウスのx軸の動きを反映
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x,
                                              transform.eulerAngles.y + mouseInput.x, 
                                              transform.eulerAngles.z);

        // y軸の値に現在の値を足す
        verticalMouseInput += mouseInput.y;

        // 数値を丸める
        verticalMouseInput = Mathf.Clamp(verticalMouseInput, -60f, 60f);

        // viewpointに丸めた数値を反映
        viewPoint.rotation = Quaternion.Euler(-verticalMouseInput,
                                              viewPoint.transform.rotation.eulerAngles.y,
                                              viewPoint.transform.rotation.eulerAngles.z);
        

    }

    private void LateUpdate()
    {
        cam.transform.position = viewPoint.position; // カメラの位置調整   
        cam.transform.rotation = viewPoint.rotation; // 回転
    }

    public void PlayerMove()
    {
        // 移動用キーの入力検知して入力を格納する
        moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        // 進む方向を出して変数に格納
        movement = ((transform.forward * moveDir.z) + (transform.right * moveDir.x)).normalized;

        // 現在位置に反映していく
        transform.position += movement * activeMoveSpeed * Time.deltaTime;
    }

    public void Jump()
    {
        // 地面についていて、スペースキーが押された時
        if (isGround() && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(jumpForce, ForceMode.Impulse);
        }
    }

    // 地面についていればtrue
    public bool isGround()
    {
        // 判定してbool値を返す
        return Physics.Raycast(groundCheckPoint.position, Vector3.down, 0.25f, groundLayers);
    }

    public void Run()
    {
        // シフトが押されている時にスピーとを切り替える
        if (Input.GetKey(KeyCode.LeftShift)) {
            activeMoveSpeed = runSpeed;
        }
        else {
            activeMoveSpeed = walkSpeed;
        }
    }

    public void UpdateCursorLock()
    {
        // boolを切り替える
        if (Input.GetKeyDown(KeyCode.Escape)) {
            cursorLock = false; // 表示
        } 
        else if (Input.GetMouseButtonDown(0)) {
            cursorLock = true; // 非表示
        }

        if (cursorLock) {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
