using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

public class XRItemInteractionScriptV2 : XRGrabInteractable
{
    public bool isGrabbing;

    void Start (){
        isGrabbing = false;
    }

    protected override void OnSelectEntering(SelectEnterEventArgs args)
    {

        Debug.Log("XRCustomScript V3: Item is SELECTED | " + gameObject.name);
        

        isGrabbing = true;

        if (gameObject.GetComponent<ItemScriptV3>() == null)
        {
            return;
        }
        if (!gameObject.GetComponent<ItemScriptV3>().grabStateSemaphore){
            return;
        }
        if (gameObject.GetComponent<ItemScriptV3>().inSlot)
        {   
            gameObject.GetComponent<ItemScriptV3>().currentSlot.inventoryScript.debugLog.GetComponent<TextMeshProUGUI>().text = "Removed "+gameObject.name;
            gameObject.GetComponent<ItemScriptV3>().currentSlot.itemInSlot = null;
            gameObject.transform.parent = null;
            gameObject.transform.localScale = gameObject.GetComponent<ItemScriptV3>().defaultScale;
            gameObject.GetComponent<ItemScriptV3>().inSlot = false;
            gameObject.GetComponent<ItemScriptV3>().currentSlot.resetColor();
            gameObject.GetComponent<ItemScriptV3>().currentSlot = null;
            gameObject.GetComponent<ItemScriptV3>().grabStateSemaphore = false;
        }

        base.OnSelectEntering(args);

        

    }

    protected override void OnSelectExiting(SelectExitEventArgs args)
    {

        Debug.Log("XRCustomScript V3: Item is DROPPED | " + gameObject.name);

        if (!gameObject.GetComponent<ItemScriptV3>().inSlot){
            Debug.Log("Object set to false kinematic");
            gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        }
        if (gameObject.GetComponent<ColliderList>().getColliderList.Count == 0){
            gameObject.GetComponent<ItemScriptV3>().grabStateSemaphore = false;
        }
        
        isGrabbing = false;

        base.OnSelectExiting(args);

        
    }







}