using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CSceneFlowManager : MonoBehaviour
{
    #region 인스펙터
    [Header("카탈로그")]
    [SerializeField] private CSceneCatalog _catalog;

    [Header("UI 전환")]
    [SerializeField] private CSceneTransitionUI _transitionUI;


    [Header("옵션 - 유지")]
    [SerializeField] private bool _dontDestroyOnLoad = true;
    [Header("옵션 - 전환")]
    [SerializeField] private float _fadeDuration = 1.5f;

    [Header("화면 전환 버튼")]
    [SerializeField] private Button _startButton;
    #endregion

    #region 내부 변수
    private static CSceneFlowManager _instance;
    private int _cursorIndex = 0;
    private bool _isLoading = false;

    public static CSceneFlowManager Instance => _instance;
    #endregion

    void Awake()
    {

        if (_instance != null && _instance != this)
        {
            CPrint.Warn("중복 씬 시스템 감지 → 기존 인스턴스가 있으므로 현재 오브젝트 제거");
            Destroy(gameObject);
            return;
        }

        _instance = this;

        if (_dontDestroyOnLoad)
        {
            DontDestroyOnLoad(this.gameObject);
        }

        if (_catalog == null)
        {
            CPrint.Warn("카탈로그가 비어 있다. / 인스펙터 확인");
            Destroy(gameObject);
            return;
        }

        _catalog.BuildMaps();

        SyncCursorToCurrentScene();
    }

    void Start()
    {
        if (_transitionUI != null)
        {
            _transitionUI.Initialize();
        }

        LoadSceneOnStart();
    }

    private void SyncCursorToCurrentScene()
    {
        List<SceneEntry> entries = _catalog.GetEntries();

        if (entries == null || entries.Count == 0)
        {
            return;
        }

        // Title / Game / Result
        string currentName = SceneManager.GetActiveScene().name;

        for (int i = 0; i < entries.Count; i++)
        {
            if (entries[i].SceneName == currentName)
            {
                _cursorIndex = i;
                CPrint.Log($"커서 싱크 → [{_cursorIndex}] / {currentName}");

                return;
            }
        }

        _cursorIndex = 0;

        if (currentName != "SceneSystem")
        {
            CPrint.Warn("커서 싱크 실패 : 현재 씬이 카탈로그 엔트리에 없다.");
        }
    }

    public void LoadSceneOnStart()
    {
        LoadScene(ESceneId.Title);
    }

    public void LoadScene(ESceneId id)
    {
        if (_catalog.TryGetSceneName(id, out string sceneName) == false)
        {
            CPrint.Warn($"LoadScene 실패 → 카탈로그에 없는 ID = {id}");
            return;
        }

        if (string.IsNullOrEmpty(sceneName))
        {
            CPrint.Warn($"LoadScene 실패 → sceneName이 비어 있다. = {id}");
            return;
        }

        StartCoroutine(Co_LoadSceneWithTransition(id, sceneName));
    }

    private IEnumerator Co_LoadSceneWithTransition(ESceneId id, string sceneName)
    {
        if (_isLoading)
        {
            CPrint.Warn("LoadScene 무시 → 이미 로딩중..");
            yield break;
        }

        _isLoading = true;

        if (_transitionUI != null)
        {
            _transitionUI.SetLoadingText("로딩중...");

            yield return _transitionUI.Co_FadeTo(1f, _fadeDuration);
        }

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);

        op.allowSceneActivation = false;

        while (op.progress < 0.9f)
        {
            yield return null;
        }

        op.allowSceneActivation = true;

        yield return null;

        if (_transitionUI != null)
        {
            yield return _transitionUI.Co_FadeTo(0f, _fadeDuration);
            _transitionUI.SetLoadingText("");
        }

        SyncCursorToCurrentScene();

        CPrint.Success($"로드 성공 → {sceneName}");

        _isLoading = false;
    } // Co_LoadSceneWithTransition()

    private void ReloadCurrent()
    {
        string current = SceneManager.GetActiveScene().name;

        if (_catalog.TryGetSceneId(current, out ESceneId id) == false)
        {
            CPrint.Warn("리로드 실패 → current가 카탈로그에 없다.");
            return;
        }

        CPrint.Log($"리로드 : {current}");

        LoadScene(id);
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }
}
