using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CPlayerUI : MonoBehaviour
{
    #region 인스펙터
    [Header("탄창 Text 연결")]
    [SerializeField] private TMP_Text _magazine;
    #endregion

    #region 내부 변수
    private CGunFire _targetGun;
    #endregion

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            _targetGun = player.GetComponentInChildren<CGunFire>();
        }
        else
        {
            CPrint.Error("CPlayerUI : Player 태그 없음");
        }
    }

    void Update()
    {
        if (_targetGun == null || _magazine == null)
            return;

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
}
