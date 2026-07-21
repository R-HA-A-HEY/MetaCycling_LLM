using System;
using System.Collections.Generic;

[Serializable]
public class Message
{
    public string role;
    public string content;
}

[Serializable]
public class OllamaRequest
{
    public string model;
    public List<Message> messages;
    public bool stream = false;
}
[System.Serializable]
public class OllamaResponse
{
    public Message message;
    public bool done;
}
