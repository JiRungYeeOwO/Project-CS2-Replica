using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCharacterHealth : MonoBehaviour, IHit
{
    public float CurrentHealth;
    public bool IsPlayerDead => _isPlayerDead;

    #region 인스펙터
    [Header("체력 설정")]
    [SerializeField] private float _maxHp = 100f;

    [Header("플레이어 확인")]
    [SerializeField] private bool _isPlayer = false;

    [Header("애니메이터 파라미터")]
    [SerializeField] private string _paramDie = "tDie";
    #endregion

    #region 내부 변수
    private float _currentHealth;
    private bool _isDead = false;
    private bool _isPlayerDead = false;
    private Animator _animator;
    private CharacterController _controller;

    private int _hashDie;
    private bool _hasDieParam;

    WaitForSeconds _disableDelay;
    #endregion

    void Reset()
    {
        if (!_isPlayer)
        {
            _controller = GetComponent<CharacterController>();
            _animator = GetComponentInChildren<Animator>();
        }
    }

    void Awake()
    {
        _hasDieParam = !string.IsNullOrEmpty(_paramDie);
        if (_hasDieParam)
        {
            _hashDie = Animator.StringToHash(_paramDie);
        }

        _disableDelay = new WaitForSeconds(5.0f);
    }

    void Start()
    {
        _currentHealth = _maxHp;
        CurrentHealth = _maxHp;

        if (!_isPlayer)
        {
            if (_controller == null)
            {
                _controller = GetComponent<CharacterController>();
            }

            if (_animator == null)
            {
                _animator = GetComponentInChildren<Animator>();
            }
        }
    }

    public void ApplyDamage(float damage)
    {
        if (_isDead)
            return;

        _currentHealth -= damage;
        CPrint.Log($"{gameObject.name} 데미지 받음. 남은 체력 {_currentHealth}");

        CurrentHealth = _currentHealth;

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        _isDead = true;

        if (_isPlayer)
        {
            _isPlayerDead = true;
            CPrint.Log("플레이어 사망, 결과 화면으로 이동");
        }
        else
        {
            if (_hasDieParam)
            {
                _animator.SetTrigger(_hashDie);
            }

            CPrint.Log("적 사망, 5초 후 비활성화");

            CEnemyController enemyController = GetComponent<CEnemyController>();

            if (enemyController != null)
            {
                enemyController.enabled = false;
            }

            CGameData.KillCount++;
            CGameData.RemainEnemyCount--;


            if (_controller != null)
            {
                _controller.enabled = false;
            }

            StartCoroutine(Co_DisableObjectAfterDelay());
        }
    } // Die()

    private IEnumerator Co_DisableObjectAfterDelay()
    {
        yield return _disableDelay;

        gameObject.SetActive(false);
    }
}
