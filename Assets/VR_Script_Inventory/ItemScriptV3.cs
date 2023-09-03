using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemScriptV3 : MonoBehaviour
{
    // Start is called before the first frame update

    public bool inSlot;
    public Vector3 slotRotation = Vector3.zero;

    public bool beingGrabbed;
    public bool grabStateSemaphore;

    public Vector3 defaultScale;
    public SlotScriptV3 currentSlot;


    void Start()
    {
        defaultScale = transform.localScale;
        inSlot = false;
        grabStateSemaphore = false;
    }

    void FixedUpdate()
    {


        if (this.gameObject.GetComponent<XRItemInteractionScriptV2>().isGrabbing)
        {
            beingGrabbed = true;
            grabStateSemaphore = true;
        }
        else
        {
            beingGrabbed = false;
        }

    }

    public void forceReset()
    {
        this.transform.parent = null;
        this.transform.localScale = defaultScale;
        this.inSlot = false;
        this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        this.GetComponent<ColliderList>().resetColliderList();
        beingGrabbed = false;
        grabStateSemaphore = false;
        currentSlot = null;
    }


}

