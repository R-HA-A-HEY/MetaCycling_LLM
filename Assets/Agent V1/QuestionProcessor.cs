using System;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Collections.Generic;

public class QuestionProcessor : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private GeminiClient geminiClient;
    [SerializeField] private DataProcessor dataProcessor;
    [SerializeField] private TextMeshProUGUI chat;
    [SerializeField] private TMP_InputField inputField;
    public string message { get; set; }
    [TextArea(3, 10)]
    [SerializeField] private string analyzePrompt = "";
    [Space(5)]
    [TextArea(3, 10)]
    [SerializeField] private string responsePrompt = "";
    public static QuestionProcessor Instance;
    public bool isAnalyzing { get; private set; }
    public bool isResponsing { get; private set; }
    // [SerializeField] private TMP_Dropdown languageDropdown;
    // [SerializeField] private string[] language;
    [SerializeField] private Language language;
    public enum Language
    {
        English,
        Chinese
    }
    private int languageIndex;
    public event Action<string, SampleData, string> OnGetAnalysis;
    void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        isAnalyzing = false;
        isResponsing = false;
    }
    void OnEnable()
    {
        dataProcessor.OnGetData += (d, q) => RunResponse(d, q);
    }
    void OnDisable()
    {
        dataProcessor.OnGetData -= (d, q) => RunResponse(d, q);
    }


    // void Start()
    // {
    //     InitializeDropdown();
    // }
    // public void OnLanguageChanged(int index)
    // {
    //     Debug.LogWarning($"Current selected language: {languageDropdown.options[languageDropdown.value].text}");
    // }
    // private void InitializeDropdown()
    // {
    //     languageDropdown.ClearOptions();
    //     List<string> options = new List<string>(language);
    //     languageDropdown.AddOptions(options);
    // }


    public void OnInputEnd(string inputText)
    {
        message = inputText;
        Debug.Log("資料已更新為: " +  message);
    }
    public void Send()
    {
        Ask(message);
    }
    public void Ask(string question)
    {
        if(!isAnalyzing && !isResponsing && !DataProcessor.Instance.isProcessing)
        {
            inputField.interactable = false;
            _ = Analyze(question);
        }
        else
        {
            Debug.LogWarning("Agent is responsing...");
        }
    }
    private async Task Analyze(string question)
    {
        if(isAnalyzing) return;
        isAnalyzing = true;
        Debug.LogWarning("Analyzing...");
        SampleData data = DataProcessor.Instance.DataLoad();
        string motionType = data.motionType;
        try
        {
            string prompt = string.Format(analyzePrompt, motionType);
            string result = await geminiClient.Generate(question, prompt);
            Debug.Log(result);
            if (!string.IsNullOrEmpty(result))
            {
                OnGetAnalysis?.Invoke(result, data, question);
            }
        }
        finally
        {
            isAnalyzing = false;
        }
    }


    private void RunResponse(ReSampleData data, string question)
    {
        _ = Response(data, question);
    }
    private async Task Response(ReSampleData data, string question)
    {
        if(isResponsing) return;
        Debug.LogWarning("Responsing...");
        isResponsing = true;
        string motionType = data.motionType;
        try
        {
            string prompt = string.Format(responsePrompt, motionType, JsonConvert.SerializeObject(data), language.ToString());
            string content = $"以下為「使用者」問題：{question}";
            string response = await geminiClient.Generate(content, prompt);
            Debug.Log(response);
            if (!string.IsNullOrEmpty(response))
            {
                chat.text += "\n\n\n-------------------------------------------------------------------\n\n\n" + response;
            }
        }
        finally
        {
            isResponsing = false;
            inputField.interactable = true;
        }
    }
}
