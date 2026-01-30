using UnityEngine;

public class CBullet : MonoBehaviour
{
    #region 인스펙터
    [Header("총알 속도")]
    [SerializeField] private float _speed = 100f;
    #endregion

    void Update()
    {
        transform.Translate(Vector3.forward * _speed * Time.deltaTime);
    }
}
