using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CGunParticle : MonoBehaviour
{
    #region 인스펙터
    [Header("파티클")]
    [SerializeField] private GameObject _particlePrefab;
    [SerializeField] private float _destroyDelay = 2.0f;

    [Header("총구 위치")]
    [SerializeField] private Transform _firePoint;

    [Header("옵션")]
    [SerializeField] private bool _useMuzzleRotation = true;
    #endregion

    public void PlayMuzzleFlash()
    {
        if (_particlePrefab == null)
        {
            CPrint.Warn("파티클 프리팹이 비어있음. / 인스펙터 확인");
            return;
        }

        Quaternion rot = _useMuzzleRotation ? _firePoint.rotation : Quaternion.identity;

        GameObject fx = Instantiate(_particlePrefab, _firePoint.position, rot);

        fx.transform.SetParent(_firePoint);

        Destroy(fx, _destroyDelay);

        CPrint.Log($"파티클 플레이 : {name} / 프리팹 = {_particlePrefab.name}");
    }
}
