using System;
using UnityEngine;

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
