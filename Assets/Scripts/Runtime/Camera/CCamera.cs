using UnityEngine;

public class CCamera : MonoBehaviour
{
    #region 인스펙터
    [Header("필수 연결")]
    [SerializeField] private Transform _target;
    [SerializeField] private Camera _camera;

    [Header("1인칭")]
    [SerializeField] private Vector3 _firstOffset = new Vector3(0f, 1.6f, 0.1f);
    [SerializeField] private float _firstSharpness = 20f;
    #endregion

    #region 내부 변수
    private Transform _camTr;
    #endregion

    void Start()
    {
        if (_camera == null)
        {
            GameObject mainCamGO = GameObject.FindGameObjectWithTag("MainCamera");

            if (mainCamGO != null)
            {
                _camera = mainCamGO.GetComponent<Camera>();
            }
        }

        if (_target == null || _camera == null)
        {
            CPrint.Warn("필수 참조 확인");
            enabled = false;
            return;
        }

        _camTr = _camera.transform;

        InitCamera();
    }

    void LateUpdate()
    {
        if (_target == null || _camTr == null)
            return;

        Tick();
    }

    private float GetSmoothT(float sharpness)
    {
        return 1f - Mathf.Exp(-sharpness * Time.deltaTime);
    }

    private void ApplyPose(Vector3 desiredPos, float sharpness, bool snap)
    {
        if (snap)
        {
            _camTr.position = desiredPos;
            //_camTr.rotation = desiredRot;
            return;
        }

        float t = GetSmoothT(sharpness);

        _camTr.position = Vector3.Lerp(_camTr.position, desiredPos, t);

        //_camTr.rotation = Quaternion.Slerp(_camTr.rotation, desiredRot, t);
    }

    private void InitCamera()
    {
        Vector3 desiredPos;
        //Quaternion desiredRot;

        BuildPose(out desiredPos);

        // 스냅 / 스무딩
        ApplyPose(desiredPos, _firstSharpness, true);
    }

    private void Tick()
    {
        Vector3 desiredPos;
        //Quaternion desiredRot;

        BuildPose(out desiredPos);

        // Lerp / Slerp + GetSmoothT
        ApplyPose(desiredPos, _firstSharpness, false);
    }

    private void BuildPose(out Vector3 desiredPos)
    {
        desiredPos = _target.position + (_target.rotation * _firstOffset);

        //desiredRot = _target.rotation;
    }
}
