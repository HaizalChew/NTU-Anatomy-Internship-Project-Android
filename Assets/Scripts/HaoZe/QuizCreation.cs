using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using static UnityEngine.UIElements.UxmlAttributeDescription;
using System.Xml.Linq;

public class QuestionData
{
    public string question;
    public string optionOne;
    public string optionTwo;
    public string optionThree;
    public string optionFour;
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
            correctOption = correctOptionInput.text,
        };

        string filePath = "C:/Users/hao yu/Documents/GitHub/NTU-Anatomy-Internship-Project-Android/Assets/Json/question" + questionNo + ".json";
        string jsonString = JsonUtility.ToJson(newData);
        File.WriteAllText(filePath, jsonString);
        Debug.Log("Created");
        Debug.Log(jsonString);
        AssetDatabase.Refresh();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
