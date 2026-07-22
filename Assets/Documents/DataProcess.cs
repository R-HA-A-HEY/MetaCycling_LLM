using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using System.Linq;
public static class DataProcess
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static RawData Filtered(string raw)
    {
        var data = JsonUtility.FromJson<RawData>(raw);
        List<float> _timeStamp = new List<float>();
        List<Vector3> _pHMD = new List<Vector3>();
        List<Vector4> _rHMD = new List<Vector4>();
        List<Vector3> _pRC = new List<Vector3>();
        List<Vector4> _rRC = new List<Vector4>();
        List<Vector3> _pLC = new List<Vector3>();
        List<Vector4> _rLC = new List<Vector4>();
        List<bool> _ptRC = new List<bool>();
        List<bool> _rtRC = new List<bool>();
        List<bool> _ptLC = new List<bool>();
        List<bool> _rtLC = new List<bool>();
        for(int i = 0; i < data.timeStamp.Length; i++)
        {
            if(data.ptRC[i] && data.ptLC[i] && data.rtRC[i] && data.rtLC[i])
            {
                _timeStamp.Add(data.timeStamp[i]);
                _pHMD.Add(data.pHMD[i]);
                _rHMD.Add(data.rHMD[i]);
                _pRC.Add(data.pRC[i]);
                _rRC.Add(data.rRC[i]);
                _pLC.Add(data.pLC[i]);
                _rLC.Add(data.rLC[i]);
                _ptRC.Add(data.ptRC[i]);
                _rtRC.Add(data.rtRC[i]);
                _ptLC.Add(data.ptLC[i]);
                _rtLC.Add(data.rtLC[i]);
            }
        }
        RawData result = new RawData
        {
            userName = data.userName,
            motionType = data.motionType,
            timeStamp = _timeStamp.ToArray(),
            pHMD = _pHMD.ToArray(),
            rHMD = _rHMD.ToArray(),
            pRC = _pRC.ToArray(),
            rRC = _rRC.ToArray(),
            pLC = _pLC.ToArray(),
            rLC = _rLC.ToArray(),
            ptRC = _ptRC.ToArray(),
            rtRC = _rtRC.ToArray(),
            ptLC = _ptLC.ToArray(),
            rtLC = _rtLC.ToArray()
        };
        return result;
    }
    public static SampleData Sampled(RawData data, int samplingRate)
    {
        if (data == null ) return null;
        SamplePoints points = new SamplePoints();
        
        points.timeStamp = data.timeStamp;

        switch (data.motionType)
        {
            case "jump rope":
                points.pHMD = data.pHMD;
                points.rHMD = data.rHMD;
                points.pRC = data.pRC;
                points.rRC = data.rRC;
                points.pLC = data.pLC;
                points.rLC = data.rLC;
                break;
                
            case "vertical jump":
                points.pHMD = data.pHMD;
                points.rHMD = data.rHMD;
                points.pRC = data.pRC;
                points.rRC = data.rRC;
                points.pLC = data.pLC;
                points.rLC = data.rLC;
                break;
            case "long jump":
                points.pHMD = data.pHMD;
                points.rHMD = data.rHMD;
                points.pRC = data.pRC;
                points.rRC = data.rRC;
                points.pLC = data.pLC;
                points.rLC = data.rLC;
                break;
            case "row machine":
                points.pHMD = data.pHMD;
                points.rHMD = data.rHMD;
                points.pLC = data.pLC;
                points.rLC = data.rLC;
                break;
            case "cycling":
                points.pHMD = data.pHMD;
                points.rHMD = data.rHMD;
                points.pRC = data.pLC;
                points.rRC = data.rLC;
                points.pLC = data.pRC;
                points.rLC = data.rRC;
                break;
            case "dumbbell":
                points.pHMD = data.pHMD;
                points.rHMD = data.rHMD;
                points.pRC = data.pRC;
                points.rRC = data.rRC;
                points.pLC = data.pLC;
                points.rLC = data.rLC;
                break;
            default:
                points.pHMD = data.pHMD;
                points.rHMD = data.rHMD;
                points.pRC = data.pRC;
                points.rRC = data.rRC;
                points.pLC = data.pLC;
                points.rLC = data.rLC;
                break;
        }
        SampleData sampleData = new SampleData
        {
            userName = data.userName,
            motionType = data.motionType,
            points = points
        };
        return sampleData;
    }
    public static ReSampleData ReSampled(SampleData data, string[] keys)
    {
        if (data == null ) return null;
        ReSamplePoints points = new ReSamplePoints();

        points.timeStamp = data.points.timeStamp;
        points.position = new Dictionary<string, SerializableVector3[]>();
        points.rotation = new Dictionary<string, SerializableVector4[]>();
        foreach(string key in keys){
            switch (key)
            {
                case "pHMD":
                    points.position["pHMD"] = data.points.pHMD.Select(p => new SerializableVector3(p)).ToArray();
                    break;

                case "rHMD":
                    points.rotation["rHMD"] = data.points.rHMD.Select(r => new SerializableVector4(r)).ToArray();
                    break;

                case "pRC":
                    points.position["pRC"] = data.points.pRC.Select(p => new SerializableVector3(p)).ToArray();
                    break;

                case "rRC":
                    points.rotation["rRC"] = data.points.rRC.Select(r => new SerializableVector4(r)).ToArray();
                    break;

                case "pLC":
                    points.position["pLC"] = data.points.pLC.Select(p => new SerializableVector3(p)).ToArray();
                    break;

                case "rLC":
                    points.rotation["rLC"] = data.points.rLC.Select(r => new SerializableVector4(r)).ToArray();
                    break;
                default:
                    break;
            }
        }
        ReSampleData reSampleData = new ReSampleData
        {
            userName = data.userName,
            motionType = data.motionType,
            points = points
        };
        return reSampleData;
    }
}