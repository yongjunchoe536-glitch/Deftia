using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    // 이동 관련 변수
    public float walkSpeed = 5.0f;
    public float runSpeed = 10.0f;
    public float jumpHeight = 2.0f;
    public float gravity = -9.81f;

    // 슬라이딩 관련 변수
    public float slideSpeed = 12.0f;
    public float slideDuration = 1.0f;
    private float slideTimer;
    private bool isSliding = false;

    // 컴포넌트 및 상태 변수
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool isGrounded;
    private float currentSpeed;

    // 카메라 및 플레이어 높이 관련
    private float originalHeight;
    private float crouchHeight;

    void Start()
    {
        // Character Controller 컴포넌트 가져오기
        controller = GetComponent<CharacterController>();
        // 초기 플레이어 높이 저장
        originalHeight = controller.height;
        crouchHeight = originalHeight * 0.5f; // 슬라이딩 시 높이를 절반으로 줄임
    }

    void Update()
    {
        // 플레이어가 땅에 있는지 확인
        isGrounded = controller.isGrounded;

        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f; // 중력 적용을 부드럽게 하기 위함
        }

        // 슬라이딩 중이 아닐 때만 이동 처리
        if (!isSliding)
        {
            HandleMovement();
            HandleJump();
        }
        
        HandleSlide();
        HandleGravity();
    }

    // 기본 이동 및 달리기 처리
    void HandleMovement()
    {
        // 키보드 입력 받기 (W, A, S, D)
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        move.Normalize();

        // 달리기 상태 확인 (Shift 키)
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        currentSpeed = isRunning ? runSpeed : walkSpeed;

        // 최종 이동 적용
        controller.Move(move * currentSpeed * Time.deltaTime);
    }

    // 점프 처리
    void HandleJump()
    {
        // 점프 (Space 키)
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    // 슬라이딩 처리
    void HandleSlide()
    {
        // 슬라이딩 시작 (C 키)
        if (Input.GetKeyDown(KeyCode.C) && isGrounded && !isSliding)
        {
            isSliding = true;
            slideTimer = slideDuration;

            // 슬라이딩 시 플레이어 컨트롤러 높이 줄이기
            controller.height = crouchHeight;
            controller.center = new Vector3(0, crouchHeight / 2, 0);
        }

        if (isSliding)
        {
            // 슬라이딩 방향으로 이동
            Vector3 slideDirection = transform.forward;
            controller.Move(slideDirection * slideSpeed * Time.deltaTime);

            // 슬라이딩 타이머 감소
            slideTimer -= Time.deltaTime;

            // 슬라이딩 종료
            if (slideTimer <= 0)
            {
                isSliding = false;
                // 원래 높이로 복원
                controller.height = originalHeight;
                controller.center = new Vector3(0, originalHeight / 2, 0);
            }
        }
    }

    // 중력 처리
    void HandleGravity()
    {
        // 중력 적용
        playerVelocity.y += gravity * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }
}