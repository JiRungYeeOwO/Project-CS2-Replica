using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CGunFire : MonoBehaviour
{
    #region 인스펙터
    [Header("총기 특징")]
    [SerializeField] private float _damage = 20f;
    [SerializeField] private float _range = 100f;
    [SerializeField] private float _fireRate = 0.1f; // 연사 속도

    [Header("총알 시각 효과")]
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _firePoint;
    #endregion

    #region 내부 변수
    private float _lastFireTime;
    #endregion

    public bool TryFire(Camera playerCam)
    {
        // 쿨타임 체크
        if (Time.time < _lastFireTime + _fireRate)
            return false;

        _lastFireTime = Time.time;

        // 실제 발사 로직
        ProcessRaycast(playerCam);
        return true;
    }

    private void ProcessRaycast(Camera camera)
    {
        RaycastHit hit;
        // 화면 정중앙으로 레이 발사
        if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, _range))
        {
            CPrint.Log($"명중: {hit.transform.name} (데미지 {_damage})");
            // 데미지 처리 로직 추가 가능
        }
    }
}
