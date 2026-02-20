using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CEnemyController : MonoBehaviour
{
    #region 인스펙터
    [Header("참조")]
    [SerializeField] private Animator _animator;
    [SerializeField] private CharacterController _controller;

    [Header("적 공격 옵션")]
    [SerializeField][Range(0f, 0.5f)] private float _accuracyError = 0.05f;
    [SerializeField] private float _detectionRange = 30f;
    [SerializeField] private float _attackRange = 25f;
    [SerializeField] private float _minDelay = 0.4f;
    [SerializeField] private float _maxDelay = 1.0f;

    [Header("무기")]
    [SerializeField] private CGun _enemyWeapon;

    [Header("애니메이터 파라미터")]
    [SerializeField] private string _paramFire = "tFire";

    [Header("시야 체크 옵션")]
    [SerializeField] private LayerMask _mapLayer;

    [Header("최적화 옵션")]
    [SerializeField] private float _sightCheckInterval = 0.15f;
    #endregion

    #region 내부 변수
    private int _hashFire;
    private bool _hasFireParam;

    private Transform _target;

    private float _checkTimer;
    private bool _isCheckedPlayer = false;

    private float _currentAimingTime = 0f;
    private float _reactionTiming = 0f;
    #endregion

    void Reset()
    {
        _controller = GetComponent<CharacterController>();
        _animator = GetComponentInChildren<Animator>();
    }

    void Awake()
    {
        if (_controller == null)
        {
            _controller = GetComponent<CharacterController>();
        }

        if (_animator == null)
        {
            _animator = GetComponentInChildren<Animator>();
        }

        _hasFireParam = !string.IsNullOrEmpty(_paramFire);
        if (_hasFireParam)
        {
            _hashFire = Animator.StringToHash(_paramFire);
        }

        if (_enemyWeapon == null)
        {
            _enemyWeapon = GetComponentInChildren<CGun>();
        }

        if (_enemyWeapon == null)
        {
            CPrint.Warn($"{gameObject.name} : 자식 오브젝트에 CGun없음");
        }
    }

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            _target = playerObj.transform;
        }

        ResetReactionTime();
    }

    void Update()
    {
        if (_target == null) return;

        _checkTimer += Time.deltaTime;

        if (_checkTimer >= _sightCheckInterval)
        {
            _checkTimer = 0;
            _isCheckedPlayer = HasLineOfSight();
        }

        float distance = Vector3.Distance(transform.position, _target.position);

        if (distance <= _attackRange)
        {
            if (_isCheckedPlayer)
            {
                LookPlayer(_target);

                _currentAimingTime += Time.deltaTime;

                if (_currentAimingTime > _reactionTiming)
                {
                    Attack(_target);
                }
            }
        }
        else if (distance <= _detectionRange)
        {
            if (_isCheckedPlayer)
            {
                LookPlayer(_target);
            }

            ResetReactionTime();
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
    }

    private void Attack(Transform target)
    {
        if (_enemyWeapon == null)
            return;

        Vector3 targetPos = target.position + Vector3.up * 1.5f;

        Vector3 error = Random.insideUnitSphere * _accuracyError;

        Vector3 finalTargetPos = targetPos + error;

        bool isFired = _enemyWeapon.Attack(finalTargetPos);

        if (isFired)
        {
            if (_hasFireParam)
            {
                _animator.SetTrigger(_hashFire);
            }
        }
    }

    private void LookPlayer(Transform target)
    {
        Vector3 dir = target.position - transform.position;
        dir.y = 0f;

        if (dir != Vector3.zero)
        {
            Quaternion lookRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 5f);
        }
    }

    private bool HasLineOfSight()
    {
        Vector3 origin = transform.position + Vector3.up * 1.5f;

        Vector3 targetPos = _target.position + Vector3.up * 1.5f;

        Vector3 dir = targetPos - origin;
        float distance = dir.magnitude; // 거리

        if (Physics.Raycast(origin, dir, out RaycastHit hit, distance, _mapLayer))
        {
            return false;
        }

        return true;
    } // HasLineOfSight()

    private void ResetReactionTime()
    {
        _currentAimingTime = 0f;

        _reactionTiming = Random.Range(_minDelay, _maxDelay);
    }
}
