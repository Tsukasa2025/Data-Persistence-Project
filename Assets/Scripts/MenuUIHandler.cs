using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MenuUIHandler : MonoBehaviour
{
    TMP_InputField _inputField;
    [SerializeField] TMP_Text HighScoreText;

    // Awake()
    private void Awake()
    {
        _inputField = GameObject.Find("Name InputField").GetComponent<TMP_InputField>();

        // ハイスコアをロード
        MainManager.SaveData data = MainManager.LoadScore();

        if (data != null)
        {
            HighScoreText.text = MainManager.GetHighScoreString(data.Name, data.Score);
        }
    }

    // StartNew()
    public void StartNew()
    {
        SceneManager.LoadScene(1);
    }

    // Exit()
    public void Exit()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

    // InputUserName()
    // for InputFiled Handler
    public void InputUserName()
    {
        MainManager.UserName = _inputField.text;
    }
}
