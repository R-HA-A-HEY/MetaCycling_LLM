using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class OllamaClient : MonoBehaviour
{
    private const string BaseUrl   = "http://localhost:11434";
    private const string EmbedModel = "nomic-embed-text";
    private const string ChatModel  = "llama3.1";

    public IEnumerator GetEmbedding(string text, Action<float[]> done)
    {
        var body = JsonUtility.ToJson(new EmbedRequest { model = EmbedModel, prompt = text });
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
        var body = JsonUtility.ToJson(new ChatRequest { model = ChatModel, messages = messages, stream = false });
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
}
