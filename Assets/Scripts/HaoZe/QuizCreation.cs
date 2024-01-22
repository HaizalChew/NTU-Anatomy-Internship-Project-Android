using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;


public class QuestionData
{
    public string question;
    public string optionOne;
    public string optionTwo;
    public string optionThree;
    public string optionFour;
    public string optionFive;
    public string correctOption;

}

public class QuizCreation : MonoBehaviour
{
    public TMP_InputField questionNoInput;
    public TMP_InputField questionInput;
    public TMP_InputField optionInput;
    public TMP_InputField optionTwoInput;
    public TMP_InputField optionThreeInput;
    public TMP_InputField optionFourInput;
    public TMP_InputField optionFiveInput;
    public TMP_InputField correctOptionInput;
    public Button createBtn;

    public void CreateJsonQuestion()
    {
        string questionNo = questionNoInput.text;
        QuestionData newData = new QuestionData
        {
            question = questionInput.text,
            optionOne = optionInput.text,
            optionTwo = optionTwoInput.text,
            optionThree = optionThreeInput.text,
            optionFour = optionFourInput.text,
            optionFive = optionFiveInput.text,
            correctOption = correctOptionInput.text,
        };

        string filePath = "Assets/Json/question" + questionNo + ".json";
        string jsonString = JsonUtility.ToJson(newData);
        File.WriteAllText(filePath, jsonString);
        Debug.Log("Created");
        Debug.Log(jsonString);
        ///AssetDatabase.Refresh();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
