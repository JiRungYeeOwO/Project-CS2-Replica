using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCharacterHealth : MonoBehaviour, IHit
{
    #region 인스펙터
    [Header("체력 설정")]
    [SerializeField] private float _maxHp = 100f;

    [Header("플레이어 확인")]
    [SerializeField] private bool _isPlayer = false;
    #endregion

    #region 내부 변수
    private float _currentHealth;
    #endregion

    void Start()
    {
        _currentHealth = _maxHp;
    }

    public void ApplyDamage(float damage)
    {
        _currentHealth -= damage;
        CPrint.Log($"{gameObject.name} 데미지 받음. 남은 체력 {_currentHealth}");
    }
}
