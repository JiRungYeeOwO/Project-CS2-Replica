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
    // SceneId <-> SceneName 변환을 담당한다.

    [Header("UI 전환")]
    [SerializeField] private CSceneTransitionUI _transitionUI;
    // 연출 담당 UI
    //  ㄴ 없어도 씬 전환은 가능하다.

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
        // 가드
        if (_instance != null && _instance != this)
        {
            CPrint.Warn("중복 씬 시스템 감지 → 기존 인스턴스가 있으므로 현재 오브젝트 제거");
            Destroy(gameObject);
            return;
        }

        _instance = this;

        // 스크립트가 붙어 있다는 전제
        if (_dontDestroyOnLoad)
        {
            DontDestroyOnLoad(this.gameObject);
        }

        if (_catalog == null)
        {
            CPrint.Warn("카탈로그가 비어 있다. / 인스펙터 확인");
            //enabled = false;
            Destroy(gameObject);
            return;
        }

        _catalog.BuildMaps();

        // 커서 동기화
        SyncCursorToCurrentScene();
    }

    void Start()
    {
        if (_transitionUI != null)
        {
            _transitionUI.Initialize();
        }

        LoadScene(ESceneId.Title);
    }

    void Update()
    {
        
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

        // 못 찾았을 때 (안전 처리)
        _cursorIndex = 0;
        CPrint.Warn("커서 싱크 실패 : 현재 씬이 카탈로그 엔트리에 없다.");
    }

    public void LoadScene(ESceneId id)
    {
        // 01. 카탈로그 조회
        // 매니저 → 씬 이름을 직접 모른다.
        //  ㄴ 카탈로그 → 이 id의 실제 씬 이름이 뭔지? 확인

        // 실패하면 false
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

        //SceneManager.LoadScene() → 즉시 전환
        // ㄴ 코루틴 → 페이트 아웃 / Load / 페이드 인

        // 연출 / 비동기 로드로 전환
        StartCoroutine(Co_LoadSceneWithTransition(id, sceneName));
    }

    private IEnumerator Co_LoadSceneWithTransition(ESceneId id, string sceneName)
    {
        // 시작하면 락 검사 → 이미 로딩 중이면 무시
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

        // 바로 씬을 바꾸면 어색하다.
        // 비동기 씬 로드
        // LoadSceneAsync : 바로 씬을 바꾸는게 아닌 로딩을 백그라운드로 진행
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);

        // 바로 전환을 막겠다.
        op.allowSceneActivation = false;

        // op.progress → 0.9f / 0.0 ~ 0.9까지만 올라간다.
        //  ㄴ 0.9 → 거의 다 로드 됨 (준비 완료) → op.allowSceneActivation = true → 0.1이 채워지면서 씬이 전환되는 구조
        while (op.progress < 0.9f)
        {
            yield return null;
        }

        op.allowSceneActivation = true;

        // 씬 활성화 프레임 확보
        yield return null;

        // 페이드 인 단계
        if (_transitionUI != null)
        {
            yield return _transitionUI.Co_FadeTo(0f, _fadeDuration);
            _transitionUI.SetLoadingText("");
        }

        // 씬이 바뀐 뒤 → 현재 씬 기준으로 커서 다시 맞춘다.
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
