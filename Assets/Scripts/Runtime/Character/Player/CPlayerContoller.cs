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
    [SerializeField] private string _paramGround = "bIsGrounded";

    [Header("애니메이터 튜닝")]
    [SerializeField] private float _speedDamp = 0.12f;

    [Header("UI 관련")]
    [SerializeField] private GameObject _menuCanvas;
    [SerializeField] private GameObject _infoPanel;

    [Header("소리 옵션")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _footstepClip;
    [SerializeField] private AudioClip _jumpClip;
    [SerializeField] private AudioClip _landingClip;
    [SerializeField, Range(0f, 1f)] private float _volume = 1.0f;
    #endregion

    #region 내부 변수
    private float _verticalVel;
    private bool _wasGrounded;
    private int _hashSpeed;
    private int _hashRun;
    private int _hashJump;
    private int _hashGround;
    private bool _hasRunParam;
    private bool _hasJumpParam;
    private bool _hasGroundParam;

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

        _hasGroundParam = !string.IsNullOrEmpty(_paramGround);
        if (_hasGroundParam)
        {
            _hashGround = Animator.StringToHash(_paramGround);
        }

        if (_audioSource == null)
        {
            _audioSource = GetComponent<AudioSource>();

            if (_audioSource == null)
            {
                _audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
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

        if (_infoPanel != null)
        {
            _infoPanel.SetActive(false);
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

        ViewGameInfo();
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

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 input = new Vector3(h, 0, v);

        input = Vector3.ClampMagnitude(input, 1.0f);

        bool isRunKey = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        bool jumpKeyDown = Input.GetKeyDown(KeyCode.Space);

        Vector3 moveDir = (input.sqrMagnitude > 0.0001f) ? BuildMoveDirection(input) : Vector3.zero;

        float speed = _walkSpeed * (isRunKey ? _runMultiplier : 1.0f);
        bool jumpedThisFrame = TickJumpAndGravity(jumpKeyDown);

        if (_hasJumpParam && jumpedThisFrame)
        {
            _animator.SetTrigger(_hashJump);
            PlayJumpSound();
        }

        _wasGrounded = _controller.isGrounded;

        Vector3 velocity = moveDir * speed;
        velocity.y = _verticalVel;

        _controller.Move(velocity * Time.deltaTime);

        if (!_wasGrounded && _controller.isGrounded)
        {
            PlayLandingSound();
        }

        float speed01 = moveDir.magnitude * (isRunKey ? 1.0f : 0.5f);

        _animator.SetFloat(_hashSpeed, speed01, _speedDamp, Time.deltaTime);

        if (_hasRunParam)
        {
            _animator.SetBool(_hashRun, isRunKey && moveDir.sqrMagnitude > 0.0001f);
        }

        if (_hasGroundParam)
        {
            _animator.SetBool(_hashGround, _controller.isGrounded);
        }
    }

    private Vector3 BuildMoveDirection(Vector3 input)
    {
        if (_cameraTr == null)
        {
            return input.normalized;
        }

        Vector3 camF = Vector3.ProjectOnPlane(_cameraTr.forward, Vector3.up).normalized;
        Vector3 camR = Vector3.ProjectOnPlane(_cameraTr.right, Vector3.up).normalized;

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

    public void PlayRunSound()
    {
        if (_footstepClip == null)
        {
            CPrint.Warn("FootstepClip 비어있다. / 인스펙터 확인");
            return;
        }

        if (_controller != null && !_controller.isGrounded) return;

        _audioSource.PlayOneShot(_footstepClip, _volume);
    }

    private void PlayJumpSound()
    {
        if (_jumpClip == null)
        {
            CPrint.Warn("JumpClip 비어있다. / 인스펙터 확인");
            return;
        }

        _audioSource.PlayOneShot(_jumpClip, _volume);
    }

    private void PlayLandingSound()
    {
        if (_landingClip == null)
        {
            CPrint.Warn("LandingClip 비어있다. / 인스펙터 확인");
            return;
        }

        _audioSource.PlayOneShot(_landingClip, _volume);
    }

    private bool TickJumpAndGravity(bool jumpKeyDown)
    {
        bool jumped = false;

        if (_controller.isGrounded)
        {
            if (_verticalVel < 0.0f)
            {
                _verticalVel = _groundStick;
            }

            if (jumpKeyDown)
            {
                _verticalVel = Mathf.Sqrt(_jumpHeight * -2.0f * _gravity);

                jumped = true;
            }
        }

        _verticalVel += _gravity * Time.deltaTime;

        return jumped;
    }

    private void ToggleMenu()
    {
        if (_menuCanvas == null)
        {
            CPrint.Warn("MenuCanvas 없음, 인스펙터 확인");
            return;
        }

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

    private void ViewGameInfo()
    {
        if (_infoPanel == null)
        {
            CPrint.Warn("InfoPanel 없음, 인스펙터 확인");
            return;
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            _infoPanel.SetActive(true);
        }
        else if (Input.GetKeyUp(KeyCode.Tab))
        {
            _infoPanel.SetActive(false);
        }
    } // ViewGameInfo()
}
