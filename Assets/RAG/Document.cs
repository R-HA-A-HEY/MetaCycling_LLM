using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class DocChunk
{
    public string content;          // chunk 文字
    public float[] embedding;       // 對應向量
    public string source;
    public int index;
}
[Serializable] public class EmbedRequest  { public string model; public string prompt; }
[Serializable] public class EmbedResponse { public float[] embedding; }

[Serializable] public class ChatMessage   { public string role; public string content; }
[Serializable] public class ChatRequest   { public string model; public List<ChatMessage> messages; public bool stream; }
[Serializable] public class ChatResponse  { public ChatMessage message; }
