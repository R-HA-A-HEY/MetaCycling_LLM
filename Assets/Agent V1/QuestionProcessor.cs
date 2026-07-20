using System;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class QuestionProcessor : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private GeminiClient geminiClient;
    [SerializeField] private DataProcessor dataProcessor;
    [SerializeField] private TextMeshProUGUI chat;
    public string message { get; set; }
    [TextArea(3, 10)]
    [SerializeField] private string analyzePrompt = "";
    [Space(5)]
    [TextArea(3, 10)]
    [SerializeField] private string responsePrompt = "";
    public static QuestionProcessor Instance;
    public bool isAnalyzing { get; private set; }
    public bool isResponsing { get; private set; }
    public event Action<string, SampleData> OnGetAnalysis;
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
        dataProcessor.OnGetData += (d) => Run(d);
    }
    void OnDisable()
    {
        dataProcessor.OnGetData -= (d) => Run(d);
    }
    public void OnInputEnd(string inputText)
    {
        message = inputText;
        Debug.Log("資料已更新為: " +  message);
    }
    public void Run(string data)
    {
        _ = Response(data);
    }
    public void Ask()
    {
        if(!isAnalyzing && !isResponsing && !DataProcessor.Instance.isProcessing)
        {
            _ = Analyze();
        }
        else
        {
            Debug.LogWarning("Agent is responsing...");
        }
    }
    private async Task Analyze()
    {
        if(isAnalyzing) return;
        isAnalyzing = true;
        Debug.LogWarning("Analyzing...");
        SampleData data = DataProcessor.Instance.DataLoad();
        string motionType = data.motionType;
        try
        {
            string prompt = string.Format(analyzePrompt, motionType);
            Debug.Log(prompt);
            Debug.Log(message);
            string result = await geminiClient.Generate(message, analyzePrompt);
            if (!string.IsNullOrEmpty(result))
            {
                OnGetAnalysis?.Invoke(result, data);
            }
        }
        finally
        {
            isAnalyzing = false;
        }
    }
    private async Task Response(string data)
    {
        if(isResponsing) return;
        Debug.LogWarning("Responsing...");
        isResponsing = true;
        try
        {
            string prompt = string.Format(responsePrompt, data);
            string content = $"以下為「使用者」問題：{message}";
            string response = await geminiClient.Generate(content, prompt);
            if (!string.IsNullOrEmpty(response))
            {
                chat.text = response;
            }
        }
        finally
        {
            isResponsing = false;
        }
    }
}
