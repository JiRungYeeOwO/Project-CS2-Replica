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


        _fadeGroup.alpha = 0.0f;
        _fadeGroup.blocksRaycasts = false;
        _fadeGroup.interactable = false;

        SetLoadingText("");
        CPrint.Log("Initialize 완료");
    }

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

    public IEnumerator Co_FadeTo(float targetAlpha, float duration = -1f, bool blockRayCastsWhileFading = true)
    {

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

    private IEnumerator Co_Fade_Internal(float targetAlpha, float duration, bool blockRayCastsWhileFading)
    {
        float startAlpha = _fadeGroup.alpha;

        _fadeGroup.blocksRaycasts = blockRayCastsWhileFading;

        _fadeGroup.interactable = false;

        if (duration <= 0f)
        {
            _fadeGroup.alpha = targetAlpha;

            _fadeGroup.blocksRaycasts = (targetAlpha >= 0.99f);

            yield break;
        }

        float t = 0;

        while (t < duration)
        {
            float dt = _useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

            t += dt;

            float lerp = Mathf.Clamp01(t / duration);

            _fadeGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, lerp);

            yield return null;
        }

        _fadeGroup.alpha = targetAlpha;

        _fadeGroup.blocksRaycasts = (targetAlpha >= 0.99f);
    }
}
