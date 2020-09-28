using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InputGate_Game : MonoBehaviour
{

    [HideInInspector] public bool isReadingInput = true;

    private void Awake()
    {
        Cursor.visible = false;
    }
    private void Update()
    {
        if (!isReadingInput) return;

        if(Input.GetKeyDown(KeyCode.Escape))
        {


#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        if(Input.GetKeyDown(KeyCode.Backspace))
        {
            SceneManager.LoadScene("Test_Scene");
        }
    }
}
