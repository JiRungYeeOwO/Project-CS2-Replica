using UnityEngine;
using TMPro;

public class CResultUI : MonoBehaviour
{
    #region 인스펙터
    [Header("Text 연결")]
    [SerializeField] private TMP_Text _enemyInfo;
    [SerializeField] private TMP_Text _killCount;
    [SerializeField] private TMP_Text _fireCount;
    [SerializeField] private TMP_Text _resultDetail;
    #endregion

    void Start()
    {
        if (_enemyInfo != null)
        {
            _enemyInfo.text = $"남은 적 수 : {CGameData.RemainEnemyCount}";
            CPrint.Log("적 수 출력 완료");
        }

        if (_killCount != null)
        {
            _killCount.text = $"킬 수 : {CGameData.KillCount}";
        }

        if (_fireCount != null)
        {
            _fireCount.text = $"발사 횟수 : {CGameData.FireBulletCount}";
        }

        if (_resultDetail != null)
        {
            _resultDetail.text = $"{CGameData.GameResultString}";
        }
    }
}
