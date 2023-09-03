using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class KeyCard : XRGrabInteractable
{
   [SerializeField] private ChangeMaterial materialChanger;

   public void ActivateCard()
   {
      materialChanger.SetOtherMaterial();
   }
}
