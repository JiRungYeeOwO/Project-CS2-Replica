using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CGunParticle : MonoBehaviour
{
    #region 인스펙터
    [Header("이펙트")]
    [SerializeField] private ParticleSystem _muzzleFlash;
    #endregion

    public void PlayMuzzleFlash()
    {
        if (_muzzleFlash != null)
        {
            _muzzleFlash.Stop();
            _muzzleFlash.Play();
        }
    }
}
