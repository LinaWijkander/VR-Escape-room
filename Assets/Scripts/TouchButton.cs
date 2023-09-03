using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// todo should not register a second hand touching the button if already being touched

public class TouchButton : XRBaseInteractable
{
    [SerializeField] private Material hoverMaterial;
    private ChangeMaterial materialChanger;
    private NumberPad numberPad;
    private int numberOfInteractors;

    protected override void Awake()
    {
        base.Awake();
        
        materialChanger = GetComponent<ChangeMaterial>();
        numberPad = FindObjectOfType<NumberPad>();
    }

    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        base.OnHoverEntered(args); // innan eller efter return check

        if (numberOfInteractors == 0)
        {
            numberPad.OnNumpadKeyPressed(GetComponentInChildren<TMP_Text>().text);
            materialChanger.SetOtherMaterial();
        }

        numberOfInteractors++;
    }

    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        base.OnHoverExited(args);

        numberOfInteractors--;
        
        if(numberOfInteractors == 0)
            materialChanger.SetOriginalMaterial();
    }
    
    public override bool IsHoverableBy(IXRHoverInteractor interactor)
    {
        return base.IsHoverableBy(interactor) && (interactor is XRDirectInteractor);
    }
}
