using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPlayerContoller : MonoBehaviour
{
    #region 인스펙터
    [Header("참조")]
    [SerializeField] private Animator _animator;
    [SerializeField] private CharacterController _controller;

    [Header("무기 회전")]
    [SerializeField] private Transform _weaponPivot;

    [Header("카메라 기준 이동 (옵션)")]
    [SerializeField] private Transform _cameraTr;

    [Header("회전 감도")]
    [SerializeField] private float _lookSensitiveYaw = 1.0f;
    [SerializeField] private float _lookSensitivePitch = 1.5f;

    [Header("시야 범위")]
    [SerializeField] private float _lookPitchMin = -60f;
    [SerializeField] private float _lookPitchMax = 60f;

    [Header("이동")]
    [SerializeField] private float _walkSpeed = 5.0f;
    [SerializeField] private float _runMultiplier = 1.8f;

    [Header("점프")]
    [SerializeField] private float _jumpHeight = 1.2f;
    [SerializeField] private float _gravity = -9.81f;
    [SerializeField] private float _groundStick = -2.0f;

    [Header("애니메이터 파라미터")]
    [SerializeField] private string _paramSpeed = "aSpeed";
    [SerializeField] private string _paramRun = "bRun";
    [SerializeField] private string _paramJump = "tJump";

    [Header("애니메이터 튜닝")]
    [SerializeField] private float _speedDamp = 0.12f;

    [Header("메뉴 캔버스")]
    [SerializeField] private GameObject _menuCanvas;
    #endregion

    #region 내부 변수
    private float _verticalVel;
    private int _hashSpeed;
    private int _hashRun;
    private int _hashJump;
    private bool _hasRunParam;
    private bool _hasJumpParam;

    private float _lookYaw;
    private float _lookPitch;

    private IWeapon _currentWeapon;

    private bool _isMenuOpen = false;
    #endregion

    private void Reset()
    {
        _controller = GetComponent<CharacterController>();
        _animator = GetComponentInChildren<Animator>();
    }

    private void Awake()
    {
        if (_controller == null)
        {
            _controller = GetComponent<CharacterController>();
        }

        if (_animator == null)
        {
            _animator = GetComponentInChildren<Animator>();
        }

        if (_cameraTr == null && Camera.main != null)
        {
            _cameraTr = Camera.main.transform;
        }

        _hashSpeed = Animator.StringToHash(_paramSpeed);

        // 파라미터가 비어있으면 → 사용하지 않겠다.
        _hasRunParam = !string.IsNullOrEmpty(_paramRun);
        if (_hasRunParam)
        {
            _hashRun = Animator.StringToHash(_paramRun);
        }

        _hasJumpParam = !string.IsNullOrEmpty(_paramJump);
        if (_hasJumpParam)
        {
            _hashJump = Animator.StringToHash(_paramJump);
        }

        // 문자열 → 해시로 바꿔서 캐싱
    }

    void Start()
    {
        _currentWeapon = GetComponentInChildren<IWeapon>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (_menuCanvas != null)
        {
            _menuCanvas.SetActive(false);
        }

        _lookYaw = transform.eulerAngles.y;
    }

    void Update()
    {
        if (_controller == null)
            return;

        LookAtForwardByMouse();

        PlayerMove();

        TickWeaponInput();


        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _isMenuOpen = _isMenuOpen ? false : true;

            ToggleMenu();
        }
    }

    private void TickWeaponInput()
    {
        if (_currentWeapon == null) return;

        if (Input.GetMouseButton(0) && Cursor.visible == false)
        {
            if (_cameraTr != null)
            {
                Camera cam = _cameraTr.GetComponent<Camera>();
                if (cam == null) cam = Camera.main;

                _currentWeapon.Attack(cam);
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            _currentWeapon.Reload();
        }
    }

    private void PlayerMove()
    {
        if (Cursor.visible == true) return;

        // 입력
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 input = new Vector3(h, 0, v);

        // ClampMagnitude : 벡터 크기 제한
        // 대각선 이동이 더 빠르지 않게 0 ~ 1로 정규화
        input = Vector3.ClampMagnitude(input, 1.0f);

        bool isRunKey = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        bool jumpKeyDown = Input.GetKeyDown(KeyCode.Space);

        // 이동 방향
        // 입력이 거의 없으면 → zero처리
        Vector3 moveDir = (input.sqrMagnitude > 0.0001f) ? BuildMoveDirection(input) : Vector3.zero;

        // 속도 (달리기)
        float speed = _walkSpeed * (isRunKey ? _runMultiplier : 1.0f);

        // 점프 + 중력 (점프가 이번 프레임에 시작됐는지 반환)
        bool jumpedThisFrame = TickJumpAndGravity(jumpKeyDown);

        if (_hasJumpParam && jumpedThisFrame)
        {
            _animator.SetTrigger(_hashJump);
        }

        // 이동
        //  ㄴ 수평 이동 + 수직 속도를 합쳐서 → Move
        Vector3 velocity = moveDir * speed;

        velocity.y = _verticalVel;

        _controller.Move(velocity * Time.deltaTime);

        float speed01 = moveDir.magnitude * (isRunKey ? 1.0f : 0.5f);

        _animator.SetFloat(_hashSpeed, speed01, _speedDamp, Time.deltaTime);

        if (_hasRunParam)
        {
            _animator.SetBool(_hashRun, isRunKey && moveDir.sqrMagnitude > 0.0001f);
        }
    }

    private Vector3 BuildMoveDirection(Vector3 input)
    {
        if (_cameraTr == null)
        {
            return input.normalized;
        }

        // 카메라 → f / r → 바닥 평면 투영 → 바닥 평면 (X, Z)
        //   ㄴ 카메라가 위를 보고 있어도 (기울어져 있어도) 캐릭터는 땅 위로만 움직이게 하기 위해
        Vector3 camF = Vector3.ProjectOnPlane(_cameraTr.forward, Vector3.up).normalized;
        Vector3 camR = Vector3.ProjectOnPlane(_cameraTr.right, Vector3.up).normalized;

        // 카메라 기준으로 입력 방향 합성 → 최종 이동 방향 (dir)
        Vector3 dir = camF * input.z + camR * input.x;

        return dir.normalized;
    }

    private void LookAtForwardByMouse()
    {
        float playerYaw = Input.GetAxis("Mouse X");
        float playerPitch = Input.GetAxis("Mouse Y");

        if (Cursor.visible == false)
        {
            _lookYaw += playerYaw * _lookSensitiveYaw;
            _lookPitch -= playerPitch * _lookSensitivePitch;

            _lookPitch = Mathf.Clamp(_lookPitch, _lookPitchMin, _lookPitchMax);

            transform.rotation = Quaternion.Euler(0f, _lookYaw, 0f);

            _cameraTr.localRotation = Quaternion.Euler(_lookPitch, _lookYaw, 0f);

            _weaponPivot.localRotation = Quaternion.Euler(0f, 0f, -_lookPitch);
        }
    }

    private bool TickJumpAndGravity(bool jumpKeyDown)
    {
        bool jumped = false;

        // isGrounded : 컨트롤러가 바닥에 바닥에 닿아있다고 판단하는 상태
        //  ㄴ 바닥 경사 / 턱 / 틈(!)에서 t / f 가 흔들릴 수 있다
        if (_controller.isGrounded)
        {
            // 바닥에 붙어 있으면 → y속도가 음수면 너무 떨어지지 않게 고정
            if (_verticalVel < 0.0f)
            {
                _verticalVel = _groundStick;
            }

            if (jumpKeyDown)
            {
                // 점프 → 원하는 높이(h)에서 속도가 0이 되도록 → 시작 속도(v)를 역으로 계산한다.

                // v = Sqrt(h * -2g)
                // -9.81
                // _verticalVel += g * dt 중력기 적용된다.

                // - 등가속도 운동

                _verticalVel = Mathf.Sqrt(_jumpHeight * -2.0f * _gravity);

                jumped = true;
            }
        }

        _verticalVel += _gravity * Time.deltaTime;

        return jumped;
    }

    private void ToggleMenu()
    {
        if (_isMenuOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (_menuCanvas != null)
            {
                _menuCanvas.SetActive(true);
            }
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            if (_menuCanvas != null)
            {
                _menuCanvas.SetActive(false);
            }
        }
    } // ToggleMenu()
}
