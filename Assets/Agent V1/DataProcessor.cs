using System;
using System.Threading.Tasks;
using UnityEngine;

public class DataProcessor : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private OllamaClient ollamaClient;
    [SerializeField] private QuestionProcessor questionProcessor;
    [SerializeField] private TextAsset jsonFile;
    [SerializeField] private int sampledRate = 5;
    [TextArea(3, 10)]
    [SerializeField] private string prompt = "";
    public bool isProcessing { get; private set; }
    public event Action<string> OnGetData;
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
       questionProcessor.OnGetAnalysis += (r, d) => Run(r, d);
    }
    void OnDisable()
    {
        questionProcessor.OnGetAnalysis -= (r, d) => Run(r, d);
    }
    private void Run(string analysis, SampleData data)
    {
        _ = Process(analysis, data);
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
    private async Task Process(string analysis, SampleData data)
    {
        if (isProcessing) return;
        Debug.LogWarning("Processing...");
        isProcessing = true;
        try
        {
            string systemInstruction = string.Format(prompt, analysis);
            string result = await ollamaClient.Generate(JsonUtility.ToJson(data), systemInstruction);
            if (!string.IsNullOrEmpty(result))
            {
                OnGetData?.Invoke(result);
            }
        }
        finally
        {
            isProcessing = false;
        }
    }
}
