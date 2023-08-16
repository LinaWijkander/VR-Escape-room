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

  [SerializeField] private GameObject attachGameObject;
  [SerializeField] private GameObject redLight;
  [SerializeField] private GameObject greenLight;
  private Vector3 swipeStartPos;
  private Vector3 swipeEndPos;
  [SerializeField] private float minSwipeDist = 0.012f;
  private bool swipeAngleValid;
  private PlaySoundsFromList audioPlayer;

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
    
    // resetta nån annan gång?
    redLight.GetComponent<ChangeMaterial>().SetOriginalMaterial();
    greenLight.GetComponent<ChangeMaterial>().SetOriginalMaterial();

    objectsHovering = interactablesHovered;

    swipeStartPos = interactablesHovered[0].transform.position;
    //args.interactableObject om det ite funkar med det ovan
    
  }

  protected override void OnHoverExited(HoverExitEventArgs args)
  {
    base.OnHoverExited(args);
    
    
    swipeEndPos = interactablesHovered[0].transform.position;


    // eller subtract entry pos from exit pos to get distance traveled. Sen om abs värdet är större än min dist - unlock
    // Check if should unlock
    if (ValidSwipeDistance(swipeStartPos.y, swipeEndPos.y) && swipeAngleValid)
    {
      Debug.Log("Valid Swipe dist & angle");
      greenLight.GetComponent<ChangeMaterial>().SetOtherMaterial();
      audioPlayer.PlayAtIndex(1);
      unlockEvent();
    }
    else
    {
      redLight.GetComponent<ChangeMaterial>().SetOtherMaterial();
      audioPlayer.PlayAtIndex(0);
    }
    
    objectsHovering.Clear();
  }

  // om inte funkar ta dot prod mellan keycard up och world up och se till de är paralella, om mindre än
  // certain value - cancel the swipe
  private bool ValidSwipe(Vector3 dir1, Vector3 dir2)
  {
    float dotProduct = Vector3.Dot(dir1, dir2);
    
    return dotProduct <= -1 + allowedSwipeMargin || dotProduct >= -1 -allowedSwipeMargin;
  }

  private bool ValidSwipeDistance(float pos1, float pos2)
  {
    float distance = math.distance(pos1, pos2);

    return distance >= minSwipeDist; 
  }

  // todo!
  private bool ValidSwipeSpeed()
  {
    return false;
  }

  // om inte funkar - ha i update bara
  public override void ProcessInteractor(XRInteractionUpdateOrder.UpdatePhase updatePhase)
  {
    base.ProcessInteractor(updatePhase);
    
    // chck if not empty
    if(IsHovering(objectsHovering[0]))
      if (!ValidSwipe(objectsHovering[0].transform.GetChild(0).transform.forward, attachGameObject.transform.forward))
      {
        Debug.Log("Swipe Invalid");
        swipeAngleValid = false;
      }
    swipeAngleValid = true;
  }

  public override bool CanSelect(IXRSelectInteractable interactable)
  {
    return false;
  }
}
