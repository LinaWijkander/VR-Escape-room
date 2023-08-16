using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

public class Scanner : XRGrabInteractable
{
    
    [Header("Scanner Data")]
    [SerializeField]private Animator animator;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private LineRenderer laserRenderer;
    [SerializeField] private TextMeshProUGUI targetName;
    [SerializeField] private TextMeshProUGUI targetPosition;
    [SerializeField] private TextMeshProUGUI targetDistance;
    [SerializeField] private TextMeshProUGUI idleText;
    [SerializeField] private Material newMaterial;
    private MeshRenderer scannedRenderer;
    private Material oldMaterial;
    private GameObject currentTarget;

    //private GameObject previousTarget;

    protected override void Awake()
    {
        base.Awake(); 
        ScannerActivated(false);
    }
    
    // Update step for interactables. It requires one parameter to specify its Update Phase, which is very similar
    // to the distinctions between Update, FixedUpdate, and LateUpdate. 
    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        base.ProcessInteractable(updatePhase);
        if (laserRenderer.gameObject.activeSelf) 
            ScanForObjects();
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        animator.SetBool("Opened", true); 
        targetName.SetText("Ready to scan");
    }
    
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        animator.SetBool("Opened", false);
    }

    protected override void OnActivated(ActivateEventArgs args)
    {
        base.OnActivated(args);
        ScannerActivated(true);
        idleText.gameObject.SetActive(false);
        audioSource.Play();
        // haptic
    }

    protected override void OnDeactivated(DeactivateEventArgs args)
    {
        base.OnDeactivated(args);
        ScannerActivated(false);
        idleText.gameObject.SetActive(true);
    }
    
    private void ScannerActivated (bool isActivated)
    {
        laserRenderer.gameObject.SetActive(isActivated);
        targetName.gameObject.SetActive(isActivated);
        targetPosition.gameObject.SetActive(isActivated);
        targetDistance.gameObject.SetActive(isActivated);
    }
    
    private void ScanForObjects() 
    {
        RaycastHit hit;
        // Max length for scanner laser
        Vector3 worldHit = laserRenderer.transform.position + laserRenderer.transform.forward * 1000.0f;
        
        //if hit
        if (Physics.Raycast(laserRenderer.transform.position, laserRenderer.transform.forward, out hit))
        {
            worldHit = hit.point;

            if (currentTarget != hit.collider.gameObject)
            {
                if(currentTarget != null)//If previous target is set, reset its material
                {
                    scannedRenderer.material = oldMaterial;
                }
                currentTarget = hit.collider.gameObject;

                if (currentTarget.TryGetComponent(out MeshRenderer meshRenderer))
                {
                    scannedRenderer = meshRenderer;
                    oldMaterial = scannedRenderer.material;//Store targets current material
                    scannedRenderer.material = newMaterial;//Set target to new material
                    //SetScannedMaterial();
                }
            }
            //If we're not pointing at anything
            
                //scannedRenderer = hit.collider.transform.gameObject.GetComponent<MeshRenderer>();
            targetName.SetText(hit.collider.name);
            targetPosition.SetText(hit.collider.transform.position.ToString());
            targetDistance.SetText(GetDistance(laserRenderer.transform.position, hit.point).ToString());
        }
        else
        {
            if (currentTarget != null)
            {
                scannedRenderer.material = oldMaterial;
                currentTarget = null;
            }

            /*  else
        {
            if (scannedRenderer)
            {
                scannedRenderer.material = oldMaterial;
                scannedRenderer = null;
            }
        }*/
        }

        // sets the position of the second vertex on the line to the worldHit variable
        laserRenderer.SetPosition(1, laserRenderer.transform.InverseTransformPoint(worldHit));
    }

    private float GetDistance(Vector3 pos1, Vector3 pos2)
    {
        return Vector3.Distance(pos1, pos2);
    }

    /*private void SetScannedMaterial()
    {
        if(scannedRenderer.material != newMaterial)
            targetOriginalMaterial = scannedRenderer.material;
            
        scannedRenderer.material = newMaterial;
    }*/
}
