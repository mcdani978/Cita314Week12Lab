using System;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SimpleUIControl : MonoBehaviour
{
    //Serialized Fields
    [SerializeField]
    ProgressControl progressControl;


    [Header("String Handling")]

    [SerializeField]
    TMP_Text[] messageTexts;

    private void OnEnable()
    {
        if (progressControl != null)
        {
            progressControl.OnStartGame.AddListener(StartGame);
            progressControl.OnChallengeComplete.AddListener(ChallengeComplete);
        }
    }

    private void ChallengeComplete(string arg0)
    {
        SetText(arg0);
    }

    private void StartGame(string arg0)
    {
        SetText(arg0);
    }

    public void SetText(string message)
    {
        //Update all text elements that should change when button is pressed.
        for (int i = 0; i < messageTexts.Length; i++)
        {
            messageTexts[i].text = message;
        }
    }
}
