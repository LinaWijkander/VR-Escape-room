using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

//todo build out the excape room. Tips here 
// https://learn.unity.com/tutorial/door-4maz?uv=2021.3&pathwayId=627c12d8edbc2a75333b9185&missionId=627bbcd5edbc2a6233664a72&projectId=627bbb0bedbc2a65b10d39ca#62a241a3edbc2a002157a58b
public class DoorHandle : XRBaseInteractable
{
    // kolla tips om inte funkar 
    [SerializeField] private Rigidbody doorRb;
    private bool pullingDoor;
    private GameObject hand;
    [SerializeField] private GameObject padlockPivot;
    [SerializeField] private GameObject bar;
    [SerializeField] private float forceRange = 0.80f;
    private int forceDirection;
    
    // Only be able to be moved after the lock has been opened or removed (listen to event?)
    // Door should feel heavy. Speed up the further away from it you pull. (Lot of drag)
    // Only move if pulling in appropriate direction (dot product) Nothing if pulling perpendicular
    // freeze position
    private Vector3 pullDistance;
    private Vector3 pullStartPos;
    private AudioSource audioSource;
    
    // return dot product, abs.nära 1 = no effect. Därefter gradvis starkare. Multiplicationfactor

    protected override void Awake()
    {
        base.Awake();
        audioSource = GetComponent<AudioSource>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        FindObjectOfType<CardReader>().unlockEvent += UnlockDoor;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        FindObjectOfType<CardReader>().unlockEvent -= UnlockDoor;
    }

    protected override void OnActivated(ActivateEventArgs args)
    {
        base.OnActivated(args);
        hand = args.interactorObject.transform.gameObject;
        pullStartPos = args.interactorObject.transform.position;
    }


    protected override void OnDeactivated(DeactivateEventArgs args)
    {
        base.OnDeactivated(args);
        hand = null;
        audioSource.Stop();
    }
    

    // Kolla om valid direction nära abs(1). 
    // apply forces i direction. -1 är i draghållet. 1 är motsatt håll. Om nära 0 nothing
    
    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        base.ProcessInteractable(updatePhase);
        
        // pulling same way
        if (isSelected && hand && ValidForceDir(-transform.right, GetPullDir()))
        {
            Debug.Log("ValidForcedDir: " + ValidForceDir(transform.up, GetPullDir()));
            
            // add force in the valid pull direction & scale med distance
            Vector3 force = new Vector3(forceDirection, 0, 0); 
            doorRb.AddForce(force * GetPullDistance(), ForceMode.Force); // force mode... door weight increase
            audioSource.Play();
            // todo dynamic haptic feedback. Vibrate when open then more or less when door pulled speed
        }
    }

    private Vector3 GetPullDir()
    {
        Vector3 direction = doorRb.transform.position - hand.transform.position;
        return direction.normalized;
    }

    // om rb inte funkar
    private float GetPullSpeed()
    {
        /*dotproduct genom Time.DeltaTime */ // Divide speed by a weight variable to slow down door movement
        return 1.0f; 
    }
    
    private float GetPullDistance()
    {
        return GetPullDir().magnitude;
    }
   
    private bool ValidForceDir(Vector3 dir1, Vector3 dir2)
    {
        float dotProduct = Vector3.Dot(dir1, dir2);
        forceDirection = dotProduct > 0 ? 1 : -1; // should be somewhere else, also there is a func for this
        return Mathf.Abs(dotProduct) > forceRange;
    }
    
    // call on when notified of event
    private void UnlockDoor()
    {
        // moveable
        ClearLock();
    }

    private IEnumerator ClearLock()
    {
        padlockPivot.transform.rotation = new Quaternion(0, -32, 0, 0);
        yield return new WaitForSeconds(1.0f);
        bar.SetActive(false);
        // rotate bar
        // fade away bar
    }
}
