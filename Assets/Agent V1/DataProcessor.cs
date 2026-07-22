using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

public class DataProcessor : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // [SerializeField] private OllamaClient ollamaClient;
    [SerializeField] private QuestionProcessor questionProcessor;
    [SerializeField] private TextAsset jsonFile;
    [SerializeField] private int sampledRate = 5;
    // [TextArea(3, 10)]
    // [SerializeField] private string prompt = "";
    public bool isProcessing { get; private set; }
    public event Action<ReSampleData, string> OnGetData;
    public static DataProcessor Instance;
    void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        isProcessing = false;
    }
    void OnEnable()
    {
        questionProcessor.OnGetAnalysis += (r, d, q) => Run(r, d, q);
    }
    void OnDisable()
    {
        questionProcessor.OnGetAnalysis -= (r, d, q) => Run(r, d, q);
    }


    public SampleData DataLoad()
    {
        if(jsonFile == null)
        {
            Debug.LogError("Null data.");
            return null;
        }
        var filtered = DataProcess.Filtered(jsonFile.text);
        var sampled = DataProcess.Sampled(filtered, Mathf.Max(1, sampledRate));
        return sampled;
    }


    private void Run(string analysis, SampleData data, string question)
    {
        Process(analysis, data, question);
    }
    private void Process(string analysis, SampleData data, string question)
    {
        if (isProcessing) return;
        isProcessing = true;
        Debug.LogWarning("Processing...");
        // try
        // {
        //     // string systemInstruction = string.Format(prompt, analysis);
        //     // Debug.Log(systemInstruction);
        //     string content = $"以下為 JSON 資料內容：{JsonUtility.ToJson(data)}\n以下為使用者要求的資料標籤：{analysis}";
        //     Debug.Log($"以下為使用者要求：{analysis}");
        //     Debug.Log(content);
        //     string result = await ollamaClient.Generate(content, prompt);
        //     Debug.Log(result);
        //     if (!string.IsNullOrEmpty(result))
        //     {
        //         OnGetData?.Invoke(result);
        //     }
        // }
        // finally
        // {
        //     isProcessing = false;
        // }
        string[] keys = analysis.Split(
            new char[] { ',', ' ', '\n', '\r' }, 
            StringSplitOptions.RemoveEmptyEntries
        );
        ReSampleData result = DataProcess.ReSampled(data, keys);
        Debug.Log(JsonConvert.SerializeObject(result));
        isProcessing = false;
        if (result != null)
        {
            OnGetData?.Invoke(result, question);
            isProcessing = false;
        }
    }
    
}
