using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class AgentManager : MonoBehaviour
{
    [SerializeField] private OllamaManager ollamaManager;
    [Header("Gemini API Configuration")]
    [SerializeField] private string apiKey = "YOUR_GEMINI_API_KEY";
    [SerializeField] private string model = "gemini-1.5-flash-latest";
    private string apiUrl => $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={apiKey}";

    [Header("Generation Parameters")]
    [SerializeField] private float temperature = 0.3f;
    [Header("Content Parameters")]
    [SerializeField] private TextAsset jsonFile;
    [SerializeField] private int sampledRate;
    [TextArea(3, 10)]
    [SerializeField] private string prompt = "";
    [TextArea(3, 10)]
    [SerializeField] private string content = "";
    // [SerializeField] private TextMeshProUGUI message;
    [Header("Others Settings")]
    [SerializeField] private int maxRetries = 4;
    [SerializeField] private int baseRetryDelay = 2;
    [SerializeField] bool isGenerating;
    
    public void Upload()
    {
        if(!isGenerating) 
        {
            _ = Generate();
        }
        else
        {
            Debug.LogWarning("A request is in progress.");
        }
    }
    private async Task<string> GenerateContent(string content, string prompt)
    {
        if (string.IsNullOrEmpty(apiKey))
        {
            Debug.LogError("Gemini API Key is not set!");
            return null;
        }
        if (string.IsNullOrEmpty(content))
        {
            Debug.LogWarning("Content is empty.");
            return null;
        }
        if (string.IsNullOrEmpty(prompt))
        {
            Debug.LogWarning("Prompt is empty.");
            return null;
        }
        GenerationConfig config = new GenerationConfig
        {
            temperature = Mathf.Clamp(this.temperature, 0f, 1f),
            // maxOutputTokens = Math.Clamp(this.maxOutputTokens, 1024, 16384)
        };
        GenerateContentRequest request = new GenerateContentRequest(content, prompt);
        string jsonRequest = JsonUtility.ToJson(request);
        
        for (int times = 1; times <= maxRetries; times++)
        {
            UnityWebRequest web = new UnityWebRequest(apiUrl, "POST");
            byte[] raw = Encoding.UTF8.GetBytes(jsonRequest);
            web.uploadHandler = new UploadHandlerRaw(raw);
            web.downloadHandler = new DownloadHandlerBuffer();
            web.SetRequestHeader("Content-Type", "application/json");

            try
            {
                await web.SendWebRequest();

                if (web.result == UnityWebRequest.Result.Success)
                {
                    string jsonResponse = web.downloadHandler.text;
                    GenerateContentResponse response = JsonUtility.FromJson<GenerateContentResponse>(jsonResponse);

                    if (response != null && response.candidates != null && response.candidates.Count > 0)
                    {
                        if (response.candidates[0].content != null && response.candidates[0].content.parts != null && response.candidates[0].content.parts.Count > 0)
                        {
                            return response.candidates[0].content.parts[0].text;
                        }
                    }
                    Debug.LogError($"Gemini API didn't response.");
                    return null;
                }
                string responseText = web.downloadHandler != null ? web.downloadHandler.text : "";
                if (IsQuotaExceeded(web.responseCode, web.GetResponseHeader("Retry-After")))
                {
                    float delay = GetRetryDelaySeconds(responseText, times);
                    if (times < maxRetries)
                    {
                        Debug.LogWarning($"Gemini API rate reached the limit.Retrying in {delay:F1}s (Retries {times}/{maxRetries}).");
                        await Task.Delay(TimeSpan.FromSeconds(delay));
                        continue;
                    }
                }
                Debug.LogError($"Gemini API Error: {web.error}\nResponse: {responseText}");
                return null;
            }
            catch (Exception ex)
            {
                if(times < maxRetries)
                {
                    float delay = GetRetryDelaySeconds("", times);
                    Debug.LogWarning($"Gemini request exception: {ex.Message}. Retrying in {delay:F1}s.");
                    await Task.Delay(TimeSpan.FromSeconds(delay));
                    continue;
                }
                Debug.LogError($"Gemini API request failed: {ex.Message}");
                return null;
            }
            finally
            {
                web.Dispose();
            }
        }
        return null;
    }
    private bool IsQuotaExceeded(long responseCode, string retryAfterHeader)
    {
        if (responseCode == 429) return true;
        if (!string.IsNullOrEmpty(retryAfterHeader)) return true;
        return false;
    }
    private float GetRetryDelaySeconds(string response, int times)
    {
        Match retryDelayMatch = Regex.Match(response, "\"retryDelay\"\\s*:\\s*\"(?<delay>\\d+)s\"");
        if (retryDelayMatch.Success && float.TryParse(retryDelayMatch.Groups["delay"].Value, out float delay))
        {
            return Mathf.Max(1f, delay);
        }
        return Mathf.Min(baseRetryDelay * Mathf.Pow(2f, times - 1), 20f);
    }
    public async Task Generate()
    {
        if (isGenerating) return;
        isGenerating = true;
        try
        {
            var preprocess = new JsonProcess();
            var data = preprocess.LoadFilteredJson(jsonFile.text);
            if (data == null)
            {
                Debug.LogError("Null data.");
                return;
            }

            int _sampledRate = Mathf.Max(1, sampledRate);
            var sampled = preprocess.SampledData(data, _sampledRate);
            string context = string.Format(content, JsonUtility.ToJson(sampled));
            string answer = await GenerateContent(context, prompt);
            Debug.Log(answer);
            if (!string.IsNullOrEmpty(answer))
            {
                ollamaManager.data = answer;
                ollamaManager.LoadData();
            }
        }
        finally
        {
            isGenerating = false;
        }
    }
}
