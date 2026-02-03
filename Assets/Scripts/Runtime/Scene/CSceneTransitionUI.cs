using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CSceneTransitionUI : MonoBehaviour
{
    #region 인스펙터
    [Header("페이드")]
    [SerializeField] private CanvasGroup _fadeGroup;
    // 연결 슬롯 → 알파 한 값으로 패널 + 자식 UIO 전체 투명도 제어가 가능하다.
    //  ㄴ blocksRayCasts → 페이드 중 클릭을 막을지도 제어가 가능하다.
    [SerializeField] private float _defaultFadeDuration = 0.25f;
    [SerializeField] private bool _useUnscaledTime = true;
    // Time.timeScale이 0이어도 페이드가 진행되게 할지

    [Header("로딩 테스트")]
    [SerializeField] private TMP_Text _loadingTMP;

    [Header("옵션")]
    [SerializeField] private bool _hideTextWhenEmpty = true;
    // 로딩 문구가 빈 문자열이면 → 텍스트 UI 자체를 꺼버릴지 여부
    #endregion

    #region 내부 변수
    private Coroutine _fadeRoutine;
    #endregion

    public void Initialize()
    {
        if (_fadeGroup == null)
        {
            CPrint.Warn("FadeGroup이 비어 있다. / 인스펙터 확인");
            return;
        }

        // 화면 밝음 / 클릭 막지 않겠다.

        // FadeIn 완료 상태
        _fadeGroup.alpha = 0.0f;
        _fadeGroup.blocksRaycasts = false;
        // 버튼 같은 것들이 들어간다.
        _fadeGroup.interactable = false;

        SetLoadingText("");
        CPrint.Log("Initialize 완료");
    }

    // 로딩 문구를 바꾸는 함수
    public void SetLoadingText(string msg)
    {
        if (_loadingTMP != null)
        {
            _loadingTMP.text = msg;

            if (_hideTextWhenEmpty)
            {
                _loadingTMP.enabled = !string.IsNullOrEmpty(msg);
            }
        }
    }

    // 외부에서 호출하는 페이드 코루틴
    public IEnumerator Co_FadeTo(float targetAlpha, float duration = -1f, bool blockRayCastsWhileFading = true)
    {
        // targetAlpha : 최종 투명도 / duration : 페이드 시간 / blockRayCastsWhileFading : 페이드 진행 중 입력을 막을지 말지

        if (_fadeGroup == null)
        {
            CPrint.Warn("Co_FadeTo 실패 → _fadeGroup 확인");
            yield break;
        }

        if (duration < 0f)
        {
            duration = _defaultFadeDuration;
        }

        if (_fadeRoutine != null)
        {
            StopCoroutine(_fadeRoutine);
            _fadeRoutine = null;
        }

        _fadeRoutine = StartCoroutine(Co_Fade_Internal(targetAlpha, duration, blockRayCastsWhileFading));

        yield return _fadeRoutine;

        _fadeRoutine = null;
    }

    // alpha를 시간에 따라 변경하는 코루틴 (내부용)
    private IEnumerator Co_Fade_Internal(float targetAlpha, float duration, bool blockRayCastsWhileFading)
    {
        float startAlpha = _fadeGroup.alpha;

        _fadeGroup.blocksRaycasts = blockRayCastsWhileFading;

        _fadeGroup.interactable = false;

        // 0 → 즉시 전환 → 보간을 수행하지 않는다.
        if (duration <= 0f)
        {
            _fadeGroup.alpha = targetAlpha;

            // 완전 어두울때만 입력을 막겠다.
            _fadeGroup.blocksRaycasts = (targetAlpha >= 0.99f);

            yield break;
        }

        // 누적 시간
        float t = 0;

        while (t < duration)
        {
            // deltaTime : 타임 스케일 영향을 받는다.
            // unscaledDeltaTime : 타임 스케일 영향을 받지 안흔다. (무시)
            float dt = _useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

            // 경과 시간 누적
            t += dt;

            // Clamp01 : 주어진 값이 0과 1 사이에 있는지 확인 → 벗어나면 최소값인 0 또는 최대겂인 1로 반환
            float lerp = Mathf.Clamp01(t / duration);

            _fadeGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, lerp);

            // 다음 프레임까지 대기
            yield return null;
        }

        _fadeGroup.alpha = targetAlpha;

        // 0 (밝은 상태) / 1 (어두운 상태)
        _fadeGroup.blocksRaycasts = (targetAlpha >= 0.99f);
    }
}
