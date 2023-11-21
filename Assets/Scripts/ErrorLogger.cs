using UnityEngine;
using UnityEngine.UI;

public class ErrorLogger : MonoBehaviour
{
    public Text PopUp;
    string error;

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {

        if (type == LogType.Error)
        {
            error = error + "\n" + logString;
            PopUp.gameObject.SetActive(true);
            PopUp.text = error;
        }
    }

    public void Dismiss()
    {
        PopUp.gameObject.SetActive(false);
    }

}
