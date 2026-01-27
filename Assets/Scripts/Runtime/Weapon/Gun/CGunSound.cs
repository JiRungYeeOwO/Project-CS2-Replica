using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CGunSound : MonoBehaviour
{
    #region 인스펙터
    [Header("오디오")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _fireClip;
    #endregion

    public void PlayFireSound()
    {
        if (_audioSource != null && _fireClip != null)
        {
            _audioSource.PlayOneShot(_fireClip);
        }
    }
}
