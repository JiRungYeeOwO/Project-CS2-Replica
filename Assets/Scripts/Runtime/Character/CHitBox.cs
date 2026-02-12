using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CHitBox : MonoBehaviour, IHit
{
    #region 인스펙터
    [Header("히트박스 설정")]
    [SerializeField] private float _damageMultiplier = 1.0f;
    #endregion

    #region 내부 변수
    private IHit _rootCharacter;
    #endregion

    void Start()
    {
        if (transform.parent != null)
        {
            _rootCharacter = transform.root.GetComponent<IHit>();
        }

        if (_rootCharacter == (IHit)this)
        {
            _rootCharacter = null;
        }
    }

    public void ApplyDamage(float damage)
    {
        if (_rootCharacter != null)
        {
            float finalDamage = damage * _damageMultiplier;
            _rootCharacter.ApplyDamage(finalDamage);
        }
    }
}
