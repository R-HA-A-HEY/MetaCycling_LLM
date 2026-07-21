using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

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
public class ReSamplePoints
{
    public float timeStamp;
    public Dictionary<string, SerializableVector3> p;
    public Dictionary<string, SerializableVector4> r;
}
[Serializable]
public class SampleData
{
    public string userName;
    public string motionType;
    public SamplePoints[] samplePoints;
}
[Serializable]
public class ReSampleData
{
    public string userName;
    public string motionType;
    public ReSamplePoints[] reSamplePoints;
}
[Serializable]
public struct SerializableVector3
{
    public float x;
    public float y;
    public float z;

    public SerializableVector3(Vector3 v)
    {
        x = v.x;
        y = v.y;
        z = v.z;
    }
}
[Serializable]
public struct SerializableVector4
{
    public float x;
    public float y;
    public float z;
    public float w;
    public SerializableVector4(Vector4 v)
    {
        x = v.x;
        y = v.y;
        z = v.z;
        w = v.w;
    }
}
