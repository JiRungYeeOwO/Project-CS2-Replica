using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum ESceneId
{
    Title = 0,
    Game = 1,
    Result = 2
}

[Serializable]
public class SceneEntry
{
    public ESceneId Id;
    public string SceneName;
}

public class CSceneCatalog : MonoBehaviour
{
    #region 인스펙터
    [Header("씬 카탈로그")]
    [SerializeField] private List<SceneEntry> _scenes = new List<SceneEntry>();

    [Header("옵션")]
    [SerializeField] private bool _buildOnAwake = true;
    #endregion

    #region 내부 변수
    private readonly Dictionary<ESceneId, string> _idToName = new Dictionary<ESceneId, string>();
    private readonly Dictionary<string, ESceneId> _nameToId = new Dictionary<string, ESceneId>();
    #endregion

    void Awake()
    {
        if (_buildOnAwake)
        {
            BuildMaps();
        }
    }

    [ContextMenu("BuildMaps (Rebuild Catalog)")]
    public void BuildMaps()
    {
        _idToName.Clear();
        _nameToId.Clear();

        for (int i = 0; i < _scenes.Count; i++)
        {
            SceneEntry e = _scenes[i];

            if (e == null)
            {
                continue;
            }

            if (string.IsNullOrEmpty(e.SceneName))
            {
                CPrint.Warn($"SceneEntry 비어 있음 / Id = {e.Id}");
                continue;
            }

            // id 중복 체크
            //  ㄴ 같은 씬 ID가 이미 등록되어 있다면
            if (_idToName.ContainsKey(e.Id))
            {
                CPrint.Warn($"Id 중복 : {e.Id} / 기존 : {_idToName[e.Id]} / 신규 : {e.SceneName}");
                continue;
            }

            // name 중복 체크
            if (_nameToId.ContainsKey(e.SceneName))
            {
                CPrint.Warn($"SceneName 중복 : {e.SceneName} / 기존 : {_nameToId[e.SceneName]} / 신규 : {e.Id}");
                continue;
            }

            // ID → Name
            _idToName.Add(e.Id, e.SceneName);
            // Name → ID
            _nameToId.Add(e.SceneName, e.Id);
        }
    } // BuildMaps()

    // 카탈로그 → 조회 API 파트
    //  ㄴ 외부 (씬 매니저 / 로더 / 플로우)
    public bool TryGetSceneName(ESceneId id, out string sceneName)
    {
        // Dictionary → Try 패턴
        // key(id)가 있으면 sceneName에 값을 넣고 true
        // key가 없으면 sceneName은 기본으로 남기고 false (null / empty)
        return _idToName.TryGetValue(id, out sceneName);
    }

    // 그냥 문자열로 받고 싶은 함수
    public string GetSceneName(ESceneId id)
    {
        if (_idToName.TryGetValue(id, out string name))
        {
            return name;
        }

        // 빈 문자열로 돌려주면서 안전
        // 예외 던지기 → throw
        return string.Empty;
    }

    // 현재 씬 이름만 있을 때 → 다시 enum ID로 역변환하는 용도
    //  ㄴ 현재 씬 로드 시 히스토리에 기록할 때 유용
    public bool TryGetSceneId(string sceneName, out ESceneId id)
    {
        // 특정 상황에서 → 현재 씬 이름만 얻어올 수 있다.

        return _nameToId.TryGetValue(sceneName, out id);
    }

    // 디버깅 / 출력용 → 부분 은닉화 / 문법
    public List<SceneEntry> GetEntries()
    {
        return _scenes;
    }
}
