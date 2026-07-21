using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class GeminiClient : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private APIKey _APIKey;
    [SerializeField] private string APIKey = "";
    private string _apikey => !string.IsNullOrEmpty(APIKey) ? APIKey : _APIKey.apikey;
    [SerializeField] private string model  = "gemini-1.5-flash";
    private string apiUrl => $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={_apikey}";
    public async Task<string> Generate(string content, string prompt)
    {
        GenerateContentRequest request = new  GenerateContentRequest(content, prompt);
        string jsonRequest = JsonUtility.ToJson(request);
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
            Debug.LogError($"Gemini API Error: {web.error}\nResponse: {responseText}");
            return null;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Gemini API request failed: {ex.Message}");
            return null;
        }
        finally
        {
            web.Dispose();
        }
    }
    
}
