using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class QuizManager : MonoBehaviour
{
    public GameObject[] quizText;
    public TMP_Text questionNumberText;

    private int questionNumber;
    private bool isPressedCorrect = false;

    private delegate void correctAnswer();
    private correctAnswer pressed;

    List<QuestionData> questionList = new List<QuestionData>();


    public void Awake()
    {
        //quizPanelButton.GetComponent<Button>().onClick.AddListener(() => ToggleQuizPanel());
        //quizPanelButton.GetComponent<Button>().onClick.AddListener(() => StartQuiz());
        StartQuiz();
    }

    public void ToggleQuizPanel()
    {
        //isPanelActive = !isPanelActive;
        //quizPanel.SetActive(isPanelActive);
    }

    public QuestionData ReadJsonQuestion(int questionNum)
    {
        string filePath = "Assets/Json/question" + questionNum + ".json";
        string stringFileContent = File.ReadAllText(filePath);
        QuestionData data = new QuestionData();
        data = JsonUtility.FromJson<QuestionData>(stringFileContent);
        return data;
    }

    public void StartQuiz()
    {
        //Get Questions From Json
        int totalQuestion = 3;
        for(int i = 1; i <= totalQuestion; i++)
        {
            QuestionData question = ReadJsonQuestion(i);
            questionList.Add(question);
        }

        //Randomise Questions
        List<QuestionData> randomQuestionList = new List<QuestionData>();
        for (int i = 1; i <= totalQuestion; i++)
        {
            int randomNumber = Random.Range(0, questionList.Count);
            randomQuestionList.Add(questionList[randomNumber]);
            questionList.RemoveAt(randomNumber);
        }

        //
        questionNumber = 1;

        foreach(QuestionData question in randomQuestionList)
        {
            Debug.Log(question.question);
        }

        StartCoroutine(ShowQuestions(randomQuestionList));

    }

    public IEnumerator ShowQuestions(List<QuestionData>randomQuestionList)
    {
        foreach(QuestionData question in randomQuestionList)
        {
            quizText[0].GetComponent<TextMeshProUGUI>().text = question.question;
            quizText[1].GetComponentInChildren<TextMeshProUGUI>().text = question.optionOne;
            quizText[2].GetComponentInChildren<TextMeshProUGUI>().text = question.optionTwo;
            quizText[3].GetComponentInChildren<TextMeshProUGUI>().text = question.optionThree;
            quizText[4].GetComponentInChildren<TextMeshProUGUI>().text = question.optionFour;
            quizText[5].GetComponentInChildren<TextMeshProUGUI>().text = question.optionFive;

            questionNumberText.text = "Question " + questionNumber + " of " + randomQuestionList.Count;
            questionNumber++;

            int correctOption = System.Convert.ToInt32(question.correctOption);
            quizText[correctOption].GetComponent<Button>().onClick.AddListener(() => PressedCorrect());
            yield return new WaitUntil(nextQuestion);
        }
        Debug.Log("QuizCompleted");
    }

    public bool nextQuestion()
    {
        if (isPressedCorrect)
        {
            isPressedCorrect = false;
            return true;
        }
        else
        {
            return false;
        }
    }
    public void PressedWrong()
    {
        Debug.Log("Wrong");
    }
    public void PressedCorrect()
    {
        Debug.Log("Correct");
        isPressedCorrect = true;
    }

 
}
