using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.XRContent.Interaction;
using UnityEngine;

public class NumberPad : MonoBehaviour
{
    [SerializeField] private TMP_Text screenText;
    private int codeLength = 4;
    private string inputCode;
    private string correctCode = "1443";
    [SerializeField] private KeyCard card;
    //[SerializeField] private GameObject cardPrefab;
    //[SerializeField] private GameObject ejectPosition;
    private Material unlockedMaterial;
    
    private PlaySoundsFromList keyPressPlayer;
    private PushButton[] pushButtons;
    private int digitsEntered;
    private bool numPadDisabled;
    
    private void Awake()
    {
        keyPressPlayer = GetComponent<PlaySoundsFromList>();
       
    }

    // for push button test
    private void Start()
    {
        //pushButtons = FindObjectsOfType<PushButton>();
        
        /*foreach (PushButton button in pushButtons)
        {
            button.OnPress += OnNumpadKeyPressed;
        }*/
    }

    public void OnNumpadKeyPressed(String keyText)
    {
        if (numPadDisabled)
            return;
        
        digitsEntered++;
        keyPressPlayer.PlayAtIndex(0);
        screenText.color = Color.black; // toggle color thing
        inputCode += keyText;
        screenText.text = inputCode;
        
        // Validate that the last digit in the sequence has completed the code
        if (digitsEntered == codeLength/*screenText.text.Length == codeLength - 1*/)
        {
            if (screenText.text == correctCode)
            {
                screenText.color = Color.green;
                screenText.text = "Code is valid. Card is unlocked";
                card.ActivateCard();
                numPadDisabled = true;

                // disable terminal
                // Eject card from dispenser
                //screenText.text = "Code is valid. Please receive card";
                //Instantiate(cardPrefab, ejectPosition.transform.position, transform.rotation);
            }
            else
            {
                screenText.color = Color.red;
                screenText.text = "Invalid Code";
                inputCode = "";
                digitsEntered = 0;
            }
        }
    }

    public void SetValidCode(string newCode)
    {
        if (newCode.Length != codeLength)
        {
            Debug.LogWarning($"The code you are trying to change to does not correspond to the set codeLength ({codeLength})");
            return;
        }
        
        correctCode = newCode;
    }
}
