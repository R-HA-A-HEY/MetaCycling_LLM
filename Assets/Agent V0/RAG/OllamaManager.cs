using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class OllamaManager : MonoBehaviour
{
    [SerializeField] private OllamaClient ollama;
    public string data;
    [SerializeField] private TextMeshProUGUI message;
    [SerializeField] private TextMeshProUGUI response;
    [SerializeField] private bool isAsking;
    private readonly VectorStore _store = new VectorStore();

    private const string SystemPrompt =
        "你是一位專業的虛擬實境（VR）動作分析專家。";

    public void LoadData()
    {
        StartCoroutine(IngestJson());
    }
    public IEnumerator IngestJson()
    {
        Debug.Log("Try to load data......");
        List<string> chunks = SplitJson(data, chunkSize: 1000, overlap: 100);

        foreach (var (text, i) in chunks.Select((t, i) => (t, i)))
        {
            yield return ollama.GetEmbedding(text, vec =>
            {
                if (vec != null)
                    _store.Add(new DocChunk { content = text, embedding = vec, source = data, index = i });
            });
        }
        Debug.Log($"已索引 {_store.Count} 個 chunk");
        Debug.Log("Data load success.");
    }
    public void Ask()
    {
        if(isAsking) return;
        StartCoroutine(AskCoroutine(message.text));
    }
    private IEnumerator AskCoroutine(string question)
    {
        isAsking = true;
        float[] queryVec = null;
        yield return ollama.GetEmbedding(question, v => queryVec = v);
        if (queryVec == null) yield break;

        var top = _store.Search(queryVec, k: 5);
        string context = string.Join("\n---\n", top.Select(c => c.content));

        var messages = new List<ChatMessage>
        {
            new ChatMessage { role = "system", content = SystemPrompt },
            new ChatMessage { role = "user",   content = $"以下是檢索到的資料:\n{context}\n\n使用者問題:{question}" }
        };

        yield return ollama.Chat(messages, answer =>
        {
            response.text = answer;
            Debug.Log($"回答: {answer}");
            Debug.Log($"引用來源: {string.Join(", ", top.Select(c => $"{c.source}#{c.index}"))}");
        });
        isAsking = false;
    }

    private List<string> SplitJson(string raw, int chunkSize, int overlap)
    {
        var result = new List<string>();
        for (int i = 0; i < raw.Length; i += chunkSize - overlap)
        {
            int length = Mathf.Min(chunkSize, raw.Length - i);
            result.Add(raw.Substring(i, length));
            if (i + length >= raw.Length) break;
        }
        return result;
    }
    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
