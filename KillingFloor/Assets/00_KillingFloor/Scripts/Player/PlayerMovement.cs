using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.Windows;

public class PlayerMovement : MonoBehaviour
{
    private PlayerInputs input; 
    private CharacterController controller; 
    public Animator tpsAnimator;    // 플레이어 TPS 모델 애니메이터
    public Animator fpsAnimator;    // 플레이어 FPS 모델 애니메이터

    [Header("Player")]
    [Tooltip("플레이어 이동속도 m/s")]
    public float moveSpeed;
    [Tooltip("플레이어 대시속도 m/s")]
    public float dashSpeed;
    [Tooltip("플레이어 회전속도")]
    public float rotationSpeed;
    [Tooltip("이동 가속도")]
    public float speedChangeRate;

    [Space(10)]
    [Tooltip("점프 높이")]
    public float jumpHeight;
    [Tooltip("플레이어 중력 값. 중력 기본값 : -9.81f")]
    public float gravity;

    [Space(10)]
    [Tooltip("점프 상태를 확인하는 값. 값이 0이면 바로 점프 가능")]
    public float jumpTimeout;
    [Tooltip("추락 상태를 확인하는 값")]
    public float fallTimeout;

    [Header("Player Grounded")]
    [Tooltip("바닥에 있는지 없는지 체크")]
    public bool isGrounded = true;
    [Tooltip("바닥의 오차")]
    public float groundedOffset;
    [Tooltip("바닥 체크 영역")]
    public float groundedRadius;


    [Header("Cinemachine")]
    [Tooltip("시네버신 버츄얼 카메라가 따라갈 타겟. FPS 플레이어의 머리 위치")]
    public GameObject cinemachineCameraTarget;
    public CinemachineVirtualCamera followCamera;
    [Tooltip("카메라 최대 각도")]
    public float topClamp;
    [Tooltip("카메라 최소 각도")]
    public float bottomClamp;

    // cinemachine
    private float _cinemachineTargetPitch;

    // player
    private float _speed;
    private float _rotationVelocity;
    private float _verticalVelocity;
    private float _terminalVelocity = 53.0f;

    // timeout deltatime
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;

    private const float _threshold = 0.01f;
    private bool IsCurrentDeviceMouse;

    void Start()
    {
        input = GetComponent<PlayerInputs>();
        controller = GetComponent<CharacterController>();
        _jumpTimeoutDelta = jumpTimeout;
        _fallTimeoutDelta = fallTimeout;
    }

    void Update()
    {
        GroundedCheck();    // 바닥 체크
        JumpAndGravity();   // 점프와 중력 관련 메서드
        Move();             // 이동 관련 메서드
        ActiveAnimation();  // 애니메이션 적용
    }
    private void LateUpdate()
    {
        CameraRotation();
    }

    // 바닥 체크
    private void GroundedCheck()
    {
        isGrounded = controller.isGrounded;
    }
    private void JumpAndGravity()
    {
        if (isGrounded)
        {
            // 추락 타임아웃 초기화
            _fallTimeoutDelta = fallTimeout;

            // 수직 힘이 0보다 떨어졌을 경우 -2로 제한시켜 속도가 무한하게 떨어지는 것 방지
            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }

            // 점프
            if (input.jump && _jumpTimeoutDelta <= 0.0f)
            {
                // H * -2 * G 의 제곱근 = 원하는 높이에 도달하는데 필요한 속도
                _verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            // 점프 타임아웃
            if (_jumpTimeoutDelta >= 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            // 바닥에 닿아있는 상태라면 점프 타임아웃 초기화
            _jumpTimeoutDelta = jumpTimeout;

            // 낙하 상태 타임아웃도 초기화
            if (_fallTimeoutDelta >= 0.0f)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            }

            // 바닥이 아니면 점프를 못하게 처리
            input.jump = false;
        }

        // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
        if (_verticalVelocity < _terminalVelocity)
        {
            _verticalVelocity += gravity * Time.deltaTime;
        }
    }
    private void CameraRotation()
    {
        // 마우스 입력이 있으면
        if (input.look.sqrMagnitude >= _threshold)
        {
            float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

            _cinemachineTargetPitch += input.look.y * rotationSpeed * deltaTimeMultiplier;
            _rotationVelocity = input.look.x * rotationSpeed * deltaTimeMultiplier;

            // 회전값 제한
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, bottomClamp, topClamp);

            // 시네머신 카메라 타겟 업데이트
            cinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);
            followCamera.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);   // 시네머신 또한 위아래 회전 가능하게 설정. (안하면 고정되어있음)

            // 플레이어 좌우로 회전시키기
            transform.Rotate(Vector3.up * _rotationVelocity);
        }
    }
    private void Move()
    {
        // 타겟 속도를 대시 상태에 따라 달라지게 설정
        float targetSpeed = input.dash ? dashSpeed : moveSpeed;

        // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

        // Vector2의 ==연산자는 근사치를 사용하므로, 부동소수점 오류가 발생하지 않는다.
        // 입력이 없으면 속도는 0으로 제한
        if (input.move == Vector2.zero) targetSpeed = 0.0f;

        // 플레이어의 수평 속도
        float currentHorizontalSpeed = new Vector3(controller.velocity.x, 0.0f, controller.velocity.z).magnitude;

        float speedOffset = 0.1f;
        float inputMagnitude = input.move.magnitude;

        // 목표 속도까지 가속 또는 감속을 한다
        if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            // 보다 유기적인 속도 변화를 위해 선형이 아닌 곡선 결과로 생성. 보간?
            // Lerp의 T가 고정되어있으므로 속도를 따로 고정할 필요가 없다고한다. T는 Time.deltaTime * speedChangeRate
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * speedChangeRate);

            // 속도를 소수점 이하 3자리로 반올림
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }

        // 입력 방향 정규화
        Vector3 inputDirection = new Vector3(input.move.x, 0.0f, input.move.y).normalized;

        // move 입력이 있는 경우 플레이어가 움직일 때 플레이어 회전
        if (input.move != Vector2.zero)
        {
            // 이동
            inputDirection = transform.right * input.move.x + transform.forward * input.move.y;
        }

        // 플레이어 위치 이동
        controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
    }
   
    // 카메라 각도 제한
    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    // 바닥과 닿았는지 확인할 수 있는 기즈모
    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (isGrounded) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        // 기즈모 그리기
        Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z), groundedRadius);
    }

    //// 애니메이션
    public void ActiveAnimation()
    {
        //// 걷기 애니메이션 셋팅
        if (input.move.x != 0 || input.move.y != 0)
        {
            tpsAnimator.SetBool("isWalk", true);
            fpsAnimator.SetBool("isWalk", true);
        }
        else
        {
            tpsAnimator.SetBool("isWalk", false);
            fpsAnimator.SetBool("isWalk", false);
        }
        tpsAnimator.SetBool("isRun", input.dash);
        fpsAnimator.SetBool("isRun", input.dash);

        tpsAnimator.SetBool("isJump", input.jump);


        if (isGrounded)
        {
            tpsAnimator.SetBool("isGrounded", isGrounded);
            fpsAnimator.SetBool("isGrounded", isGrounded);
        }
        else if (!isGrounded)
        {
            tpsAnimator.SetBool("isGrounded", isGrounded);
            fpsAnimator.SetBool("isGrounded", isGrounded);
        }
        tpsAnimator.SetFloat("xDir", input.move.x);
        tpsAnimator.SetFloat("yDir", input.move.y);
    }

}
