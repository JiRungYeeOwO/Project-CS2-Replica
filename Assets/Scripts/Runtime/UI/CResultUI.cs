using UnityEngine;
using TMPro;

public class CResultUI : MonoBehaviour
{
    #region 인스펙터
    [Header("Text 연결")]
    [SerializeField] private TMP_Text _fireCount;
    #endregion

    void Start()
    {
        if (_fireCount != null)
        {
            _fireCount.text = $"Fire Count : {CGameData.FireBulletCount}";
        }
    }
}
