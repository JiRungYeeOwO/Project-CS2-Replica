using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CQuitButton : MonoBehaviour
{
    #region 인스펙터
    [Header("종료 버튼")]
    [SerializeField] private Button _quitButton;
    #endregion

    void Start()
    {
        if (_quitButton != null)
        {
            _quitButton.onClick.AddListener(QuitGame);
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
