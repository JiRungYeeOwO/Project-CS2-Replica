using UnityEngine;

public class CGun : MonoBehaviour,IWeapon
{
    #region 인스펙터
    [Header("내부 로직")]
    [SerializeField] private CGunFire _gunFire;
    [SerializeField] private CGunRecoil _recoil;
    [SerializeField] private CGunSound _sound;
    [SerializeField] private CGunParticle _particle;

    [Header("적 설정")]
    [SerializeField] private bool _isInfiniteAmmo = false;
    #endregion

    void Awake()
    {
        if (_gunFire == null) _gunFire = GetComponent<CGunFire>();
        if (_recoil == null) _recoil = GetComponent<CGunRecoil>();
        if (_sound == null) _sound = GetComponent<CGunSound>();
        if (_particle == null) _particle = GetComponent<CGunParticle>();
    }

    public void Attack(Camera playerCam)
    {
        if (_gunFire != null && _gunFire.TryFire(playerCam, _isInfiniteAmmo))
        {
            OnFireSuccess();
        }
    }
    
    public bool Attack(Vector3 targetPosition)
    {
        if (_gunFire == null) return false;

        Vector3 origin = _gunFire.transform.position;

        Vector3 direction = (targetPosition - origin).normalized;

        Debug.DrawRay(origin, direction * 100f, Color.red, 1.0f);

        if (_gunFire.TryFire(origin, direction, _isInfiniteAmmo))
        {
            OnFireSuccess();
            return true;
        }

        return false;
    }

    public void Reload()
    {
        if (_gunFire != null)
        {
            _gunFire.Reload();
            if (_sound != null) _sound.PlayReloadSound();
        }
    }

    private void OnFireSuccess()
    {
        if (_recoil != null) _recoil.PlayRecoil(); // 반동 실행
        if (_sound != null) _sound.PlayFireSound(); // 소리 재생
        if (_particle != null) _particle.PlayMuzzleFlash(); // 이펙트 재생
    }
}
