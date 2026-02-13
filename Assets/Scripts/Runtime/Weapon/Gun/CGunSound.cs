using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CGunSound : MonoBehaviour
{
    #region 인스펙터
    [Header("오디오")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _fireClip;
    [SerializeField] private AudioClip _reloadClip;

    [Header("옵션 (필수)")]
    [SerializeField, Range(0f, 1f)] private float _volume = 1.0f;
    [SerializeField] private bool _randomPitch = true;
    [SerializeField] private Vector2 _pitchRange = new Vector2(0.95f, 1.05f);
    #endregion

    void Awake()
    {
        if (_audioSource == null)
        {
            _audioSource = GetComponent<AudioSource>();

            if (_audioSource == null)
            {
                _audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
    }

    public void PlayFireSound()
    {
        if (_fireClip == null)
        {
            CPrint.Warn("FireClip 비어있다. / 인스펙터 확인");
            return;
        }

        if (_randomPitch)
        {
            _audioSource.pitch = Random.Range(_pitchRange.x, _pitchRange.y);
        }
        else
        {
            _audioSource.pitch = 1.0f;
        }

        _audioSource.PlayOneShot(_fireClip, _volume);
    }

    public void PlayReloadSound()
    {
        if (_reloadClip == null)
        {
            CPrint.Warn("ReloadClip 비어있다. / 인스펙터 확인");
            return;
        }

        if (_randomPitch)
        {
            _audioSource.pitch = Random.Range(_pitchRange.x, _pitchRange.y);
        }
        else
        {
            _audioSource.pitch = 1.0f;
        }

        _audioSource.PlayOneShot(_reloadClip, _volume * 3.0f);
    }
}
