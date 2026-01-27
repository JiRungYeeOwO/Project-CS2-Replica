using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CGunRecoil : MonoBehaviour
{
    #region 인스펙터
    [Header("반동 설정")]
    [SerializeField] private Transform _modelTr; // 움직일 총 모델 (껍데기)
    [SerializeField] private float _recoilDistance = -0.2f; // 뒤로 밀리는 거리
    [SerializeField] private float _recoverSpeed = 5.0f;    // 복구 속도
    #endregion

    #region 내부 변수
    private Vector3 _originPos;
    private Vector3 _targetPos;
    #endregion

    void Start()
    {
        if (_modelTr == null) _modelTr = transform; // 없으면 자기 자신
        _originPos = _modelTr.localPosition;
    }

    void Update()
    {
        _modelTr.localPosition = Vector3.Lerp(_modelTr.localPosition, _originPos, Time.deltaTime * _recoverSpeed);
    }

    public void PlayRecoil()
    {
        // 순간적으로 뒤로 확 밀어버림 (Z축 기준)
        Vector3 recoilPos = _originPos;
        recoilPos.z += _recoilDistance;

        _modelTr.localPosition = recoilPos;
    }
}
