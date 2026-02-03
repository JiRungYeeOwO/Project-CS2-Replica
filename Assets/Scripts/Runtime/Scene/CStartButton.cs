using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CStartButton : MonoBehaviour
{
    #region 인스펙터
    [Header("시작 버튼")]
    [SerializeField] private Button _startButton;
    #endregion

    void Start()
    {
        if (_startButton != null)
        {
            _startButton.onClick.AddListener(OnClickStart);
        }
    }

    private void OnClickStart()
    {
        if (CSceneFlowManager.Instance != null)
        {
            CSceneFlowManager.Instance.LoadScene(ESceneId.Game);
        }
    }
}
