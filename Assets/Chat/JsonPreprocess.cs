using System.Collections.Generic;
using UnityEngine;

public class JsonProcess
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public RawData LoadFilteredJson(string raw)
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
    public SampleData SampledData(RawData data, int samplingRate)
    {
        if (data == null ) return null;
        List<SamplePoints> samplePoints = new List<SamplePoints>();
        List<float> _timeStamp = new List<float>();
        List<Vector3> _pHMD = new List<Vector3>();
        List<Vector4> _rHMD = new List<Vector4>();
        List<Vector3> _pRC = new List<Vector3>();
        List<Vector4> _rRC = new List<Vector4>();
        List<Vector3> _pLC = new List<Vector3>();
        List<Vector4> _rLC = new List<Vector4>();
        for (int i = 0; i < data.timeStamp.Length; i += samplingRate)
        {
            samplePoints.Add(new SamplePoints
            {
                timeStamp = data.timeStamp[i],
                pHMD = data.pHMD[i],
                rHMD = data.rHMD[i],
                pRC = data.pRC[i],
                rRC = data.rRC[i],
                pLC = data.pLC[i],
                rLC = data.rLC[i]
            });
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