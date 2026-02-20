using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CGameFlowManager : MonoBehaviour
{

    #region 인스펙터
    [Header("게임 설정")]
    [SerializeField] private float _limitTime = 90f;

    [Header("사운드 설정")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _winClip;
    [SerializeField] private AudioClip _loseClip;
    [SerializeField, Range(0f, 1f)] private float _volume = 1.0f;
    #endregion

    #region 내부 변수
    private float _currentTime;
    private bool _isGameOver = false;
    private CCharacterHealth _characterHealth;

    public static CGameFlowManager Instance;
    public float CurrentTime => _currentTime;
    #endregion

    void Awake()
    {
        if (Instance == null) Instance = this;

        if (_audioSource == null)
        {
            _audioSource = GetComponent<AudioSource>();

            if (_audioSource == null)
            {
                _audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
    }

    void Start()
    {
        _currentTime = _limitTime;
        _isGameOver = false;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            _characterHealth = player.GetComponentInChildren<CCharacterHealth>();
        }
    }

    void Update()
    {
        if (_isGameOver)
            return;

        _currentTime -= Time.deltaTime;

        if (_currentTime <= 0f || _characterHealth.IsPlayerDead)
        {
            EndGame(false);
            return;
        }

        if (CGameData.SpawnEnemyCount > 0 && CGameData.RemainEnemyCount <= 0)
        {
            EndGame(true);
            return;
        }
    }

    private void EndGame(bool isClear)
    {
        _isGameOver = true;

        if (_audioSource != null)
        {
            // 결과에 따라 승리 또는 패배 음악 재생
            AudioClip targetClip = isClear ? _winClip : _loseClip;

            if (targetClip != null)
            {
                _audioSource.PlayOneShot(targetClip, _volume);
            }
        }

        if (isClear)
        {
            CGameData.GameResultString = "모든 적 사살";
        }
        else
        {
            CGameData.GameResultString = _characterHealth.IsPlayerDead ? "플레이어 사망" : "시간 초과";
        }

        StartCoroutine(Co_DelayedSceneLoad());
    }

    private IEnumerator Co_DelayedSceneLoad()
    {
        yield return new WaitForSeconds(6.0f);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        CSceneFlowManager.Instance.LoadScene(ESceneId.Result);
    }
}
