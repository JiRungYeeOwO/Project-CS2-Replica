using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CEnemySpawner : MonoBehaviour
{
    #region 인스펙터
    [Header("필수 연결")]
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private Transform[] _spawnPoints;
    #endregion

    #region 내부 변수
    private int _spawnCount;
    #endregion

    void Start()
    {
        _spawnCount = Random.Range(4, 8);

        if (_spawnPoints == null || _spawnPoints.Length == 0)
        {
            CPrint.Error("스폰 위치 연결 안 됨");
        }

        SpawnEnemies();
    }

    private void SpawnEnemies()
    {
        List<Transform> usablePoints = new List<Transform>(_spawnPoints);

        int count = Mathf.Min(_spawnCount, usablePoints.Count);

        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(0, usablePoints.Count);
            Transform usedPoint = usablePoints[randomIndex];

            Instantiate(_enemyPrefab, usedPoint.position, usedPoint.rotation);

            usablePoints.RemoveAt(randomIndex);
        }

    }
}
