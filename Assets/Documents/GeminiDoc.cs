using System;
using System.Collections.Generic;

[Serializable]
public class Part
{
    public string text;

    public Part(string message)
    {
        text = message;
    }
}
[Serializable]
public class Content
{
    public List<Part> parts;

    public Content(string message)
    {
        parts = new List<Part> { new Part(message) };
    }
}
[Serializable]
public class SystemInstruction
{
    public List<Part> parts; 

    public SystemInstruction(string instructionText)
    {
        parts = new List<Part> { new Part(instructionText) };
    }
}
[Serializable]
public class GenerationConfig
{
    public float temperature = 1f;
    public List<string> stopSequences = new List<string>();
}
[Serializable]
public class GenerateContentRequest
{
    public SystemInstruction system_instruction;
    public Content contents;
    public GenerationConfig generationConfig;

    public GenerateContentRequest(string contentText, string systemInstructionText = null/*, GenerationConfig config = null*/)
    {
        if (!string.IsNullOrEmpty(contentText))
        {
            contents = new Content(contentText);
        }
        if (!string.IsNullOrEmpty(systemInstructionText))
        {
            system_instruction = new SystemInstruction(systemInstructionText);
        }
    }
}
[Serializable]
public class SafetyRating
{
    public string category;
    public string probability;
}
[Serializable]
public class Candidate
{
    public Content content;
    public string finishReason;
    public int index;
    public List<SafetyRating> safetyRatings;
}

[Serializable]
public class GenerateContentResponse
{
    public List<Candidate> candidates;
}
