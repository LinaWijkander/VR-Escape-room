using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.Collections;
using Unity.Mathematics;
using Unity.XRContent.Interaction;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// todo force card through a particular path. So snap to track and as long as hand close enough..
// kan ladda ner projektet och kolla i slutet

public class CardReader : XRSocketInteractor
{
  // prevent that it snaps into place - dont set collider to trigger 
  // hover start börja kolla hur långt swipat åt rätt håll
  // om nått en viss distance - unlock door
  private List<IXRHoverInteractable> objectsHovering = new();
  //private GameObject firstSwipedObject;
  
  [Tooltip("-1 is perfect swipe, 0 is perpendicular")]
  [SerializeField][Range(-0.0f, -0.5f)] private float allowedSwipeMargin = 0.1f;

  [SerializeField] private GameObject swipeTrack;
  [SerializeField] private GameObject redLight;
  [SerializeField] private GameObject greenLight;
  private Vector3 swipeStartPos;
  private Vector3 swipeEndPos;
  [SerializeField] private float minSwipeDist = 0.012f;
  private bool swipeAngleValid;
  private PlaySoundsFromList audioPlayer;
  private GameObject hoveringObject;   // Layermask only accepts KeyCard

  public delegate void UnlockDelegate();

  public event UnlockDelegate unlockEvent;
  
  
  
  protected override void Awake()
  {
    base.Awake();
    audioPlayer = GetComponent<PlaySoundsFromList>();
  }


  protected override void OnHoverEntered(HoverEnterEventArgs args)
  {
    base.OnHoverEntered(args);
    
    // Reset somewhere else?
    redLight.GetComponent<ChangeMaterial>().SetOriginalMaterial();
    greenLight.GetComponent<ChangeMaterial>().SetOriginalMaterial();

    hoveringObject = args.interactableObject.transform.gameObject;
    swipeStartPos = hoveringObject.transform.position;
  }


  protected override void OnHoverExited(HoverExitEventArgs args)
  {
    base.OnHoverExited(args);
    
    swipeEndPos = hoveringObject.transform.position;

    if (swipeAngleValid)
    {
      if (ValidSwipeDistance(swipeStartPos.y, swipeEndPos.y))
      {
        greenLight.GetComponent<ChangeMaterial>().SetOtherMaterial();
        audioPlayer.PlayAtIndex(1);
        unlockEvent(); // funkar inte?
      }
      else
      {
        redLight.GetComponent<ChangeMaterial>().SetOtherMaterial();
        audioPlayer.PlayAtIndex(1);
      }
    }
    hoveringObject = null;
  }
  
  
  //dot product = -1 is an optimal swipe
  private bool ValidSwipe(Vector3 dir1, Vector3 dir2)
  {
    float dotProduct = Vector3.Dot(dir1, dir2);
    Debug.Log("DOT product: " + dotProduct);
    return dotProduct <= -1 + allowedSwipeMargin;
  }

  
  private bool ValidSwipeDistance(float pos1, float pos2)
  {
    float distance = math.distance(pos1, pos2);
    Debug.Log("Swipe distance: " + distance +" Valid? " + (distance >= minSwipeDist));
    return distance >= minSwipeDist; 
  }

  
  // todo!
  private bool ValidSwipeSpeed()
  {
    return false;
  }

 
  public override void ProcessInteractor(XRInteractionUpdateOrder.UpdatePhase updatePhase)
  {
    base.ProcessInteractor(updatePhase);
    
    if(hoveringObject)
      if (!ValidSwipe(hoveringObject.transform.forward, swipeTrack.transform.forward))
      {
        Debug.Log("Swipe Invalid");
        swipeAngleValid = false;
      }
      else
      {
        swipeAngleValid = true;
        Debug.Log("Swipe VALID");
      }
  }

  public override bool CanSelect(IXRSelectInteractable interactable)
  {
    return false;
  }
}
