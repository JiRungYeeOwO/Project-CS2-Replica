using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CGunFire : MonoBehaviour
{
    public int CurrentAmmo => _currentMagazine;
    public int MaxAmmo => _magazineSize;
    public bool IsReloading => _isReloading;

    #region 인스펙터
    [Header("총기 특징")]
    [SerializeField] private float _damage = 20f;
    [SerializeField] private float _range = 100f;
    [SerializeField] private float _fireRate = 0.1f; // 연사 속도
    [SerializeField] private int _magazineSize = 30;

    [Header("총알 시각 효과")]
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _firePoint;

    [Header("총알 풀링 설정")]
    [SerializeField] private int _poolSize = 30;
    [SerializeField] private float _bulletLifeTime = 2.0f;

    [Header("재장전 설정")]
    [SerializeField] private float _reloadTime = 1.5f;
    #endregion

    #region 내부 변수
    private float _lastFireTime;

    private Queue<GameObject> _pool = new Queue<GameObject>();
    private List<GameObject> _aliveBullets = new List<GameObject>();
    private Dictionary<GameObject, float> _lifeMap = new Dictionary<GameObject, float>();

    private int _currentMagazine;
    private bool _isReloading = false;
    private WaitForSeconds _reloadingTime;
    #endregion

    void Awake()
    {
        if (_bulletPrefab == null)
        {
            CPrint.Warn("총알 프리팹 없음");
        }
    }

    void Start()
    {
        for (int i = 0; i < _poolSize; i++)
        {
            GameObject bullet = Instantiate(_bulletPrefab);
            bullet.SetActive(false);
            bullet.transform.SetParent(null);
            _pool.Enqueue(bullet);
        }

        CGameData.ResetData();

        _currentMagazine = _magazineSize;

        _reloadingTime = new WaitForSeconds(_reloadTime);
    }

    void Update()
    {
        for (int i = _aliveBullets.Count - 1; i >= 0; i--)
        {
            GameObject b = _aliveBullets[i];

            if (!b.activeSelf)
            {
                ReturnToPool(b);
                _aliveBullets.RemoveAt(i);
                if (_lifeMap.ContainsKey(b))
                {
                    _lifeMap.Remove(b);
                }
                continue;
            }

            if (_lifeMap.ContainsKey(b))
            {
                _lifeMap[b] -= Time.deltaTime;

                if (_lifeMap[b] <= 0f)
                {
                    ReturnToPool(b);
                    _aliveBullets.RemoveAt(i);
                    _lifeMap.Remove(b);
                }
            }
        }
    }

    public void Reload()
    {
        if (_isReloading || _currentMagazine >= _magazineSize)
            return;

        StartCoroutine(Co_ReloadRoutine());
    }

    private IEnumerator Co_ReloadRoutine()
    {
        _isReloading = true;
        CPrint.Log("재장전 시작");
        yield return _reloadingTime;
        _currentMagazine = _magazineSize;
        CPrint.Log("재장전 완료");
        _isReloading = false;
    }

    public bool TryFire(Camera playerCam)
    {
        // 쿨타임 체크
        if (Time.time < _lastFireTime + _fireRate)
            return false;

        if (_currentMagazine <= 0)
            return false;

        _lastFireTime = Time.time;

        _currentMagazine--;

        CGameData.FireBulletCount++;

        // 실제 발사 로직
        ProcessRaycast(playerCam);
        return true;
    }

    private void ProcessRaycast(Camera camera)
    {
        RaycastHit hit;
        Vector3 targetPoint;

        // 화면 정중앙으로 레이 발사
        if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, _range))
        {
            targetPoint = hit.point;

            CPrint.Log($"명중: {hit.transform.name} (데미지 {_damage})");
            // 데미지 처리 로직 추가 가능
        }
        else
        {
            targetPoint = camera.transform.position + (camera.transform.forward * _range);
        }

        SpawnBullet(targetPoint);
    }

    private void SpawnBullet(Vector3 targetPos)
    {
        GameObject bullet = GetBulletFromPool();

        bullet.transform.position = _firePoint.position;
        bullet.transform.rotation = Quaternion.identity;
        bullet.transform.LookAt(targetPos);
    }

    private GameObject GetBulletFromPool()
    {
        GameObject bullet = null;

        if (_pool.Count > 0)
        {
            bullet = _pool.Dequeue();
        }
        else
        {
            bullet = Instantiate(_bulletPrefab);
            bullet.transform.SetParent(null);
        }

        bullet.SetActive(true);

        _aliveBullets.Add(bullet);

        if (_lifeMap.ContainsKey(bullet))
        {
            _lifeMap[bullet] = _bulletLifeTime;
        }
        else
        {
            _lifeMap.Add(bullet, _bulletLifeTime);
        }

        return bullet;
    }

    private void ReturnToPool(GameObject bullet)
    {
        bullet.SetActive(false);
        _pool.Enqueue(bullet);
    }
}
