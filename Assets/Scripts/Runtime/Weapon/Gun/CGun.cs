using UnityEngine;

public class CGun : MonoBehaviour,IWeapon
{
    #region 인스펙터
    [Header("내부 로직")]
    [SerializeField] private CGunFire _gunFire;
    [SerializeField] private CGunRecoil _recoil;
    [SerializeField] private CGunSound _sound;
    [SerializeField] private CGunParticle _particle;
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
        if (_gunFire != null && _gunFire.TryFire(playerCam))
        {
            // 발사 성공 시, 다른 부품들에게도 일하라고 명령 (이벤트 전파)
            OnFireSuccess();
        }
    }

    public void Reload()
    {
        CPrint.Log("재장전");
    }

    private void OnFireSuccess()
    {
        if (_recoil != null) _recoil.PlayRecoil(); // 반동 실행
        if (_sound != null) _sound.PlayFireSound(); // 소리 재생
        if (_particle != null) _particle.PlayMuzzleFlash();   // 이펙트 재생
    }
}
