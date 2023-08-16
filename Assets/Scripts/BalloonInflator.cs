using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class BalloonInflator : XRGrabInteractable
{
    [Header("Balloon Data")]
    public Transform attachPoint;
    public Balloon balloonPrefab;
    private Balloon balloonInstance;
    private XRBaseController controller;
    //private bool inflating;
    private Rigidbody rb;
    private float releaseThreshold = 1.25f;
    
    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody>();
    }
    
    // Interactables special Update step
    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        base.ProcessInteractable(updatePhase);
        
        CheckVelocity();
        
        if (isSelected && controller != null /* && inflating*/) 
        {
            balloonInstance.transform.localScale = Vector3.one * Mathf.Lerp(1.0f, 4.0f,
                controller.activateInteractionState.value); 
            
            // Add haptics depending on how quick or slow inflation
        }
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
      
        if(!balloonInstance)
            balloonInstance = Instantiate(balloonPrefab, attachPoint); //enable disbale instead?
        
        var controllerInteractor = args.interactorObject as XRBaseControllerInteractor;
        controller = controllerInteractor.xrController;
        
        //controller.SendHapticImpulse(1, 0.5f);
        //Debug.Log(controller); 
    }
    
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        Destroy(balloonInstance.gameObject);
    }

    protected override void OnActivated(ActivateEventArgs args)
    {
        base.OnActivated(args);
        //inflating = true;
    }
    
    protected override void OnDeactivated(DeactivateEventArgs args)
    {
        base.OnDeactivated(args);
        //inflating = false;
    }
    
    void CheckVelocity()
    {
        float speed = rb.velocity.magnitude;
        if(speed > releaseThreshold)
            ReleaseBalloon();
    }

    private void ReleaseBalloon()
    {
        balloonInstance.GetComponent<Balloon>().Detach();
        //inflating = false;
    }
}
