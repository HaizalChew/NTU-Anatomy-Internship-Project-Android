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

}

public class QuizCreation : MonoBehaviour
{
    public TMP_InputField questionInput;
    public TMP_InputField optionInput;
    public TMP_InputField optionTwoInput;
    public TMP_InputField optionThreeInput;
    public TMP_InputField optionFourInput;
    public Button createBtn;

    public void CreateJsonQuestion()
    {
        QuestionData newData = new QuestionData
        {
            question = questionInput.text,
            optionOne = optionInput.text,
            optionTwo = optionTwoInput.text,
            optionThree = optionThreeInput.text,
            optionFour = optionFourInput.text,
        };

        string filePath = "C:/Users/hao yu/Documents/GitHub/NTU-Anatomy-Internship-Project-Android/Assets/Json/question2.json";
        string jsonString = JsonUtility.ToJson(newData);
        File.WriteAllText(filePath, jsonString);
        Debug.Log("Created");
        Debug.Log(jsonString);
        AssetDatabase.Refresh();
    }

    public void ReadJsonQuesiton()
    {
        string filePath = "C:/Users/hao yu/Documents/GitHub/NTU-Anatomy-Internship-Project-Android/Assets/Json/question2.json";
        string stringFileContent = File.ReadAllText(filePath);
        QuestionData data = new QuestionData();
        data = JsonUtility.FromJson<QuestionData>(stringFileContent);
        Debug.Log(data.optionFour);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
