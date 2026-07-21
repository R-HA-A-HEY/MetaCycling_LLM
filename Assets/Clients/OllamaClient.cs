using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class OllamaClient : MonoBehaviour
{
    private string BaseUrl = "http://localhost:11434";
    [SerializeField] private string model  = "llama3.1";
    [SerializeField] private string embedModel = "nomic-embed-text";

    public IEnumerator GetEmbedding(string text, Action<float[]> done)
    {
        var body = JsonUtility.ToJson(new EmbedRequest { model = embedModel, prompt = text });
        using var request = new UnityWebRequest($"{BaseUrl}/api/embeddings", "POST");
        request.uploadHandler   = new UploadHandlerRaw(Encoding.UTF8.GetBytes(body));
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Embedding failed: {request.error}");
            done(null);
            yield break;
        }
        var response = JsonUtility.FromJson<EmbedResponse>(request.downloadHandler.text);
        done(response.embedding);
    }

    public IEnumerator Chat(List<ChatMessage> messages, System.Action<string> onDone)
    {
        var body = JsonUtility.ToJson(new ChatRequest { model = model, messages = messages, stream = false });
        using var req = new UnityWebRequest($"{BaseUrl}/api/chat", "POST");
        req.uploadHandler   = new UploadHandlerRaw(Encoding.UTF8.GetBytes(body));
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Chat failed: {req.error}");
            onDone(null);
            yield break;
        }
        var res = JsonUtility.FromJson<ChatResponse>(req.downloadHandler.text);
        onDone(res.message.content);
    }
    public async Task<string> Generate(string content, string prompt)
    {
        OllamaRequest request = new OllamaRequest
        {
            model = model,
            messages = new List<Message>(),
            stream = false
        };
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
        request.messages.Add(new Message {
                role = "system",
                content = prompt
        });
        request.messages.Add(new Message {
                role = "user",
                content = content
        });
        string jsonRequest = JsonUtility.ToJson(request);
        UnityWebRequest web = new UnityWebRequest($"{BaseUrl}/api/chat", "POST");
        byte[] raw = Encoding.UTF8.GetBytes(jsonRequest);
        web.uploadHandler = new UploadHandlerRaw(raw);
        web.downloadHandler = new DownloadHandlerBuffer();
        web.SetRequestHeader("Content-Type", "application/json");
        try
        {
            await web.SendWebRequest();

            if(web.result == UnityWebRequest.Result.Success)
            {
                string json = web.downloadHandler.text;
                OllamaResponse response = JsonUtility.FromJson<OllamaResponse>(json);
                Debug.Log(response.message.content);
                if(response != null && response.message != null && response.message.content != null)
                {
                    return response.message.content;
                }
                Debug.LogError($"Ollama didn't response.");
                return null;
            }
            string responseText = web.downloadHandler != null ? web.downloadHandler.text : "";
            Debug.LogError($"Ollama Error: {web.error}\nResponse: {responseText}");
            return null;
        }
        catch(Exception ex)
        {
            Debug.LogError($"Ollama request failed: {ex.Message}");
            return null;
        }
        finally
        {
            web.Dispose();
        }
    }
}
