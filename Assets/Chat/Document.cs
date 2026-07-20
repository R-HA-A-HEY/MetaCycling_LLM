using System;
using System.Collections.Generic;
using UnityEngine;
//----------------- Content Classes -----------------
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
//----------------- Config Classes -----------------
[Serializable]
public class GenerationConfig
{
    public float temperature = 1f;
    // public int topK = 1;
    // public int topP = 1;
    // public int maxOutputTokens = 2048;
    public List<string> stopSequences = new List<string>();
}
//----------------- Response Classes -----------------
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
//----------------- Requests Classes -----------------
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
        // if (config == null)
        // {
        //     generationConfig = new GenerationConfig();
        // }
        // else
        // {
        //     generationConfig = config;
        // }
    }
}
//----------------- Json Classes -----------------
[Serializable]
public class RawData
{
    public string userName;
    public string motionType;
    public float[] timeStamp;
    public Vector3[] pHMD;
    public Vector4[] rHMD;
    public Vector3[] pRC;
    public Vector4[] rRC;
    public Vector3[] pLC;
    public Vector4[] rLC;
    public bool[] ptRC;
    public bool[] rtRC;
    public bool[] ptLC;
    public bool[] rtLC;
}
[Serializable]
public class SamplePoints
{
    public float timeStamp;
    public Vector3 pHMD;
    public Vector4 rHMD;
    public Vector3 pRC;
    public Vector4 rRC;
    public Vector3 pLC;
    public Vector4 rLC;
}
[Serializable]
public class SampleData
{
    public string userName;
    public string motionType;
    public SamplePoints[] samplePoints;
}
