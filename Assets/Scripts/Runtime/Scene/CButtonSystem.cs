using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CButtonSystem : MonoBehaviour
{
    #region 인스펙터
    [Header("타이틀 버튼")]
    [SerializeField] private Button _titleButton;

    [Header("시작 버튼")]
    [SerializeField] private Button _startButton;

    [Header("결과 화면 버튼")]
    [SerializeField] private Button _resultButton;

    [Header("종료 버튼")]
    [SerializeField] private Button _quitButton;
    #endregion

    void Start()
    {
        if (_startButton != null)
        {
            _startButton.onClick.AddListener(OnClickStart);
        }

        if (_titleButton != null)
        {
            _titleButton.onClick.AddListener(OnClickTitle);
        }

        if (_resultButton != null)
        {
            _resultButton.onClick.AddListener(OnClickResult);
        }

        if (_quitButton != null)
        {
            _quitButton.onClick.AddListener(QuitGame);
        }
    }

    private void OnClickStart()
    {
        if (CSceneFlowManager.Instance != null)
        {
            CSceneFlowManager.Instance.LoadScene(ESceneId.Game);
        }
    }

    private void OnClickTitle()
    {
        if (CSceneFlowManager.Instance != null)
        {
            CSceneFlowManager.Instance.LoadScene(ESceneId.Title);
        }
    }

    private void OnClickResult()
    {
        if (CSceneFlowManager.Instance != null)
        {
            CSceneFlowManager.Instance.LoadScene(ESceneId.Result);
        }
    }

    private void QuitGame()
    {
#if UNITY_EDITOR

        UnityEditor.EditorApplication.isPlaying = false;

#else

        Application.Quit();

#endif
    }
}
