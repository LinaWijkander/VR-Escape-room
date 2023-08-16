using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// todo should not register a second hand touching the button if already being touched

public class TouchButton : XRBaseInteractable
{
    [SerializeField] private Material hoverMaterial;
    private Material originalMaterial;
    private MeshRenderer meshRenderer;
   private NumberPad numberPad;
   //private bool hoverActive;

    protected override void Awake()
    {
        base.Awake();
        meshRenderer = GetComponent<MeshRenderer>();
        originalMaterial = meshRenderer.material;
        numberPad = FindObjectOfType<NumberPad>();
    }

    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        base.OnHoverEntered(args); // innan eller efter return check

        if (isHovered)
            return;
        
        // detect button press - change color
        numberPad.OnNumpadKeyPressed(GetComponent<TMP_Text>().text);
        meshRenderer.material = hoverMaterial;
        
    }

    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        base.OnHoverExited(args);
        // change back color
        meshRenderer.material = originalMaterial;
        
    }
}
