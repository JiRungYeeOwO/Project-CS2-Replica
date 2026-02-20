using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CPlayerUI : MonoBehaviour
{
    #region 인스펙터
    [Header("게임 정보 Text 연결")]
    [SerializeField] private TMP_Text _magazine;
    [SerializeField] private TMP_Text _playerHP;
    [SerializeField] private TMP_Text _remainTime;

    [Header("Info 패널")]
    [SerializeField] private TMP_Text _enemyInfo;
    [SerializeField] private TMP_Text _killCount;
    [SerializeField] private TMP_Text _fireCount;
    #endregion

    #region 내부 변수
    private CGunFire _targetGun;
    private CCharacterHealth _hpInfo;
    private CGameFlowManager _flowManager;

    #endregion

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            _hpInfo = player.GetComponentInChildren<CCharacterHealth>();
            _targetGun = player.GetComponentInChildren<CGunFire>();
        }
        else
        {
            CPrint.Error("CPlayerUI : Player 태그 없음");
        }

        _flowManager = CGameFlowManager.Instance;
    }

    void Update()
    {
        if (_flowManager == null)
        {
            _flowManager = CGameFlowManager.Instance;
            return;
        }

        if (_hpInfo != null && _playerHP != null)
        {
            if (_hpInfo.CurrentHealth > 0)
            {
            _playerHP.text = $"HP : {_hpInfo.CurrentHealth}";
            _playerHP.fontStyle = FontStyles.Bold;
            }
            else
            {
                _playerHP.text = $"HP : 0";
                _playerHP.color = Color.red;
                _playerHP.fontStyle = FontStyles.Bold;
            }
        }

        if (_targetGun != null && _magazine != null)
        {
            if (_targetGun.IsReloading)
            {
                _magazine.text = "Reloading..";
                _magazine.color = Color.yellow;
            }
            else
            {
                _magazine.color = Color.white;
                _magazine.text = $"{_targetGun.CurrentAmmo} / {_targetGun.MaxAmmo}";
            }
        }

        if (_enemyInfo != null)
        {
            _enemyInfo.text = $"남은 적 :{CGameData.RemainEnemyCount} / {CGameData.SpawnEnemyCount}";
        }

        if (_killCount != null)
        {
            _killCount.text = $"킬 : {CGameData.KillCount}";
        }

        if (_fireCount != null)
        {
            _fireCount.text = $"발사 횟수 : {CGameData.FireBulletCount}";
        }

        if (_remainTime != null)
        {
            float time = _flowManager.CurrentTime;
            _remainTime.text = $"{time:F2}";
        }
    }
}
