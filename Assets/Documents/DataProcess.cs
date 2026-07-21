using System.Collections.Generic;
using UnityEngine;

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
        List<SamplePoints> samplePoints = new List<SamplePoints>();
        for (int i = 0; i < data.timeStamp.Length; i += samplingRate)
        {
            SamplePoints point = new SamplePoints();
            
            point.timeStamp = data.timeStamp[i];

            switch (data.motionType)
            {
                case "jump rope":
                    point.pHMD = data.pHMD[i];
                    point.rHMD = data.rHMD[i];
                    point.pRC = data.pRC[i];
                    point.rRC = data.rRC[i];
                    point.pLC = data.pLC[i];
                    point.rLC = data.rLC[i];
                    break;
                    
                case "vertical jump":
                    point.pHMD = data.pHMD[i];
                    point.rHMD = data.rHMD[i];
                    point.pRC = data.pRC[i];
                    point.rRC = data.rRC[i];
                    point.pLC = data.pLC[i];
                    point.rLC = data.rLC[i];
                    break;
                case "long jump":
                    point.pHMD = data.pHMD[i];
                    point.rHMD = data.rHMD[i];
                    point.pRC = data.pRC[i];
                    point.rRC = data.rRC[i];
                    point.pLC = data.pLC[i];
                    point.rLC = data.rLC[i];
                    break;
                case "row machine":
                    point.pHMD = data.pHMD[i];
                    point.rHMD = data.rHMD[i];
                    point.pLC = data.pLC[i];
                    point.rLC = data.rLC[i];
                    break;
                case "bike":
                    point.pHMD = data.pHMD[i];
                    point.rHMD = data.rHMD[i];
                    point.pRC = data.pLC[i];
                    point.rRC = data.rLC[i];
                    point.pLC = data.pRC[i];
                    point.rLC = data.rRC[i];
                    break;
                case "dumbbell":
                    point.pHMD = data.pHMD[i];
                    point.rHMD = data.rHMD[i];
                    point.pLC = data.pLC[i];
                    point.rLC = data.rLC[i];
                    break;
                default:
                    point.pHMD = data.pHMD[i];
                    point.rHMD = data.rHMD[i];
                    point.pRC = data.pRC[i];
                    point.rRC = data.rRC[i];
                    point.pLC = data.pLC[i];
                    point.rLC = data.rLC[i];
                    break;
            }
            samplePoints.Add(point);
        }
        SampleData sampleData = new SampleData
        {
            userName = data.userName,
            motionType = data.motionType,
            samplePoints = samplePoints.ToArray()
        };
        return sampleData;
    }
    public static SampleData ReSampled(SampleData data, string[] keys)
    {
        if (data == null ) return null;
        List<SamplePoints> samplePoints = new List<SamplePoints>();
        for (int i = 0; i < data.samplePoints.Length; i ++)
        {
            SamplePoints point = new SamplePoints();
            point.timeStamp = data.samplePoints[i].timeStamp;
            foreach(string key in keys){
                string _key = key.Trim();
                switch (_key)
                {
                    case "pHMD":
                        point.pHMD = data.samplePoints[i].pHMD;
                        break;
                    case "rHMD":
                        point.rHMD = data.samplePoints[i].rHMD;
                        break;
                    case "pRC":
                        point.pRC = data.samplePoints[i].pRC;
                        break;
                    case "rRC":
                        point.rRC = data.samplePoints[i].rRC;
                        break;
                    case "pLC":
                        point.pLC = data.samplePoints[i].pLC;
                        break;
                    case "rLC":
                        point.rLC = data.samplePoints[i].rLC;
                        break;
                    default:
                        break;
                }
            }
            samplePoints.Add(point);
        }
        SampleData sampleData = new SampleData
        {
            userName = data.userName,
            motionType = data.motionType,
            samplePoints = samplePoints.ToArray()
        };
        return sampleData;
    }
}