using UnityEngine;
using System;
using TMPro; 
using System.Collections.Generic;

public class OnStartSummary : MonoBehaviour
{
    // [SerializeField] private TMP_Dropdown motionTypeDropdown;
    [SerializeField] private MotionType motionType;
    [SerializeField] private OnStartQuestions[] onStartQuestions;
    private int motionTypeIndex;
    [Serializable]
    public enum MotionType
    {
        Jump_Rope,
        Vertical_Jump,
        Long_Jump,
        Row_Machine,
        Cycling,
        Dumbbell
    }
    [Serializable]
    public struct OnStartQuestions
    {
        public MotionType motionType;
        [TextArea(1, 5)]
        public string question;
    }
    void Start()
    {
        QuestionProcessor.Instance.Ask(GetQuestion(motionType));
        // InitializeDropdown();
    }
    private string GetQuestion(MotionType target)
    {
        if (onStartQuestions == null) return null;
        foreach (OnStartQuestions item in onStartQuestions)
        {
            if (item.motionType == target)
            {
                return item.question;
            }
        }
        return null; 
    }
    // public void OnMotionTypeChanged(int index)
    // {
    //     motionTypeIndex = index;
    //     Debug.LogWarning($"Current selected motion type: {motionTypeDropdown.options[motionTypeIndex].text}");
    // }
    // private void InitializeDropdown()
    // {
    //     motionTypeDropdown.ClearOptions();
    //     string[] _options = Enum.GetNames(typeof(MotionType));
    //     List<string> options = new List<string>(_options);
    //     motionTypeDropdown.AddOptions(options);
    // }

}
