using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using TMPro;

public class SlotScriptV3 : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject itemInSlot;
    public Image slotImage;
    public Color originalColor;


    public int countCollisions;
    public InputDevice device;

    public InventoryVR inventoryScript;

    bool lastFrameGrabbed;

    // Magnetic surface  mode variables
    public bool magneticSurfaceMode;
    public List<GameObject> itemsInSlot;

    // Wrist-based stack mode variables

    public bool wristBasedMode;
    public bool wristBased_entrySlotDesignation;
    public bool wristBased_bottomStackDesignation;
    public GameObject wristBased_slotStack;

    public GameObject wristBased_slotStack_bottom;

    [SerializeField] GameObject currentStackTop;
    [SerializeField] GameObject currentStackBottom;
    public List<GameObject> itemsInSlot_wristBased;

    // current object to certain slot distance 
    float omegalul;

    void Start()
    {
        slotImage = GetComponentInChildren<Image>();
        originalColor = slotImage.color;
        inventoryScript = GameObject.Find("InventoryRuntime").GetComponent<InventoryVR>();
        // device = inventoryScript.subDevice[0];
        lastFrameGrabbed = false;
        currentStackTop = null;
        currentStackBottom = null;
        if (wristBased_entrySlotDesignation)
        {
            Debug.Log("============ OOK THIS WORKS!!!!");
            wristBased_slotStack.GetComponent<SlotScriptV3>().wristBased_slotStack_bottom = wristBased_slotStack_bottom;
        }

    }

    void Update()
    {
        // bool xValue;
        // if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.gripButton, out xValue) && xValue)
        // {
        //     Debug.Log("SLOTSCRIPTV3: RH Grip button is pressed.");
        // }
        if (!magneticSurfaceMode)
        {
            if (!this.itemInSlot)
            {
                resetColor();
            }
        }

    }


    private void OnTriggerStay(Collider other)
    {
        if (!magneticSurfaceMode && !wristBasedMode)
        {
            GameObject obj = other.gameObject;
            if (itemInSlot != null)
            {
                return;
            }

            if (!isItem(obj))
            {
                return;
            }

            if (!obj.GetComponent<ItemScriptV3>().beingGrabbed && !obj.GetComponent<ItemScriptV3>().grabStateSemaphore)
            {
                // Debug.Log("Object is NOT being grabbed.");
                return;
            }
            else if (!obj.GetComponent<ItemScriptV3>().beingGrabbed && obj.GetComponent<ItemScriptV3>().grabStateSemaphore)
            {
                Debug.Log("GRAB STATE SEMAPHORE == TRUE, being grabbed = FALSE");
            }
            else
            {
                Debug.Log("GRAB STATE SEMAPHORE == TRUE, being grabbed = TRUE");
            }



            Debug.Log("IMPORTANT: " + obj.GetComponent<ItemScriptV3>().inSlot + " | " + obj.GetComponent<ItemScriptV3>().beingGrabbed);

            if (obj.GetComponent<ColliderList>().getColliderList.Count > 1)
            {
                GameObject otherTemp = other.gameObject;
                List<float> otherObjDistanceCollision = new List<float>();

                foreach (GameObject x in obj.GetComponent<ColliderList>().getColliderList)
                {

                    if (x == this.gameObject)
                    {
                        omegalul = Vector3.Distance(obj.transform.position, x.transform.position);
                    }
                    else
                    {
                        otherObjDistanceCollision.Add(Vector3.Distance(obj.transform.position, x.transform.position));
                    }

                    // Debug.Log("Gameobj " + other.name + " detected collision with multiple slots, one of which is " + x);
                    // Debug.Log("OBJ to Slot " + x + " Distance:" + (otherObjDistanceCollision));

                }

                foreach (float y in otherObjDistanceCollision)
                {
                    if (omegalul > y)
                    {
                        return;
                    }
                }

            }

            Debug.Log("FINAL CHECK STATE!!");
            if (!obj.GetComponent<ItemScriptV3>().beingGrabbed && obj.GetComponent<ItemScriptV3>().grabStateSemaphore)
            {
                Debug.Log("SLOTSCRIPTV3: Insert Item Condition Fulfilled");
                obj.GetComponent<ItemScriptV3>().grabStateSemaphore = false;
                inventoryScript.debugLog.GetComponent<TextMeshProUGUI>().text = "Added " + obj.name;
                insertItem(obj);

            }
            else
            {
                Debug.Log(obj.GetComponent<ItemScriptV3>().beingGrabbed.ToString() + obj.GetComponent<ItemScriptV3>().grabStateSemaphore.ToString());
            }
        }
        else if (wristBasedMode)
        {
            GameObject obj = other.gameObject;
            if (!isItem(obj))
            {
                return;
            }
            if (!wristBased_entrySlotDesignation)
            {
                return;
            }
            if (!obj.GetComponent<ItemScriptV3>().beingGrabbed && !obj.GetComponent<ItemScriptV3>().grabStateSemaphore)
            {
                // Debug.Log("Object is NOT being grabbed.");
                return;
            }
            else if (!obj.GetComponent<ItemScriptV3>().beingGrabbed && obj.GetComponent<ItemScriptV3>().grabStateSemaphore)
            {
                Debug.Log("GRAB STATE SEMAPHORE == TRUE, being grabbed = FALSE");
            }
            else
            {
                Debug.Log("GRAB STATE SEMAPHORE == TRUE, being grabbed = TRUE");
            }
            if (!obj.GetComponent<XRItemInteractionScriptV2>().isGrabbing && obj.GetComponent<ItemScriptV3>().grabStateSemaphore)
            {
                if (!itemsInSlot_wristBased.Contains(obj))
                {
                    Debug.Log("WRIST-ENTRY OK");
                    obj.GetComponent<ItemScriptV3>().grabStateSemaphore = false;
                    inventoryScript.debugLog.GetComponent<TextMeshProUGUI>().text = "Added " + obj.name;
                    insertItem_wristStack(obj);
                }

            }
        }
        else
        {
            GameObject obj = other.gameObject;
            if (!isItem(obj))
            {
                return;
            }
            if (!obj.GetComponent<ItemScriptV3>().beingGrabbed && !obj.GetComponent<ItemScriptV3>().grabStateSemaphore)
            {
                // Debug.Log("Object is NOT being grabbed.");
                return;
            }
            else if (!obj.GetComponent<ItemScriptV3>().beingGrabbed && obj.GetComponent<ItemScriptV3>().grabStateSemaphore)
            {
                Debug.Log("GRAB STATE SEMAPHORE == TRUE, being grabbed = FALSE");
            }
            else
            {
                Debug.Log("GRAB STATE SEMAPHORE == TRUE, being grabbed = TRUE");
            }
            if (!obj.GetComponent<XRItemInteractionScriptV2>().isGrabbing && obj.GetComponent<ItemScriptV3>().grabStateSemaphore)
            {
                if (!itemsInSlot.Contains(obj))
                {

                    obj.GetComponent<ItemScriptV3>().grabStateSemaphore = false;
                    inventoryScript.debugLog.GetComponent<TextMeshProUGUI>().text = "Added " + obj.name;
                    insertItem_Magnetic(obj);
                }

            }

        }

    }

    private void OnTriggerExit(Collider other)
    {
        // Debug.Log("SLOTSCRIPTV3: ONTRIGGEREXIT");

        GameObject obj2 = other.gameObject;
        if (!isItem(obj2))
        {
            return;
        }
        if (!magneticSurfaceMode && !wristBasedMode)
        {

            if (obj2.GetComponent<ColliderList>().getColliderList.Count == 0)
            {
                Debug.Log("NEW: 0BJECT LET GO OF, RESETTING SEMAPHORE STATE");
                obj2.GetComponent<ItemScriptV3>().grabStateSemaphore = false;
                return;
            }

            // if (obj2.GetComponent<ItemScriptV3>().inSlot)
            //     {
            //         obj2.GetComponent<ItemScriptV3>().currentSlot.itemInSlot = null;
            //         obj2.transform.SetParent(null);

            //         obj2.gameObject.GetComponent<ItemScriptV3>().inSlot = false;
            //         obj2.gameObject.GetComponent<ItemScriptV3>().currentSlot.resetColor();
            //         obj2.gameObject.GetComponent<ItemScriptV3>().currentSlot = null;
            //     }
        }
        else if (wristBasedMode)
        {
            if (itemsInSlot != null)
            {
                // Debug.Log("SLOTSCRIPTV3: wristB1");
                foreach (GameObject x in itemsInSlot_wristBased.ToArray())
                {
                    // Debug.Log("SLOTSCRIPTV3: wristB2" + x.gameObject.name + obj2.name);
                    float bottomSlot_distance = Vector3.Distance(obj2.gameObject.transform.position, wristBased_slotStack_bottom.gameObject.transform.position);
                    float topSlot_distance = Vector3.Distance(obj2.gameObject.transform.position, this.gameObject.transform.position);
                    Debug.Log("=================== !!!!! "+ bottomSlot_distance +" | "+ topSlot_distance + " | BOTTOM SLOT?: " + wristBased_bottomStackDesignation);
                    if (bottomSlot_distance < 0.005 || topSlot_distance < 0.005){
                        return;
                    }
                    if (x == obj2)
                    {
                        Debug.Log("SLOTSCRIPTV3: wristB3");
                        Debug.Log("WRIST BASED STACK: " + obj2.name);
                        itemsInSlot_wristBased.Remove(x);
                        updateStackSlotContents();


                    }
                }
            }
        }
        else
        {
            Debug.Log("SLOTSCRIPTV3: magnetic surface");
            if (itemsInSlot != null)
            {
                Debug.Log("SLOTSCRIPTV3: mag1");
                foreach (GameObject x in itemsInSlot)
                {
                    Debug.Log("SLOTSCRIPTV3: mag2" + x.gameObject.name + obj2.name);
                    if (x == obj2)
                    {
                        Debug.Log("SLOTSCRIPTV3: mag3");
                        Debug.Log("MAGNETIC SURFACE ITEM REMOVED: " + obj2.name);
                        itemsInSlot.Remove(x);
                        obj2.GetComponent<ItemScriptV3>().currentSlot.itemInSlot = null;
                        obj2.transform.SetParent(null);
                        obj2.gameObject.GetComponent<ItemScriptV3>().inSlot = false;

                    }
                }
            }
        }

    }

    bool isItem(GameObject obj)
    {
        return obj.GetComponent<ItemScriptV3>();
    }

    // Update is called once per frame

    public void insertItem(GameObject obj)
    {
        Debug.Log("INSERT ITEM METHOD: Successfully inserted " + obj.gameObject.name);
        // obj.GetComponent<Rigidbody>().isKinematic = true;
        countCollisions += 1;
        obj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        obj.transform.localScale = obj.GetComponent<ItemScriptV3>().defaultScale * 0.5f;
        obj.transform.SetParent(this.gameObject.transform, true);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localEulerAngles = obj.GetComponent<ItemScriptV3>().slotRotation;
        obj.GetComponent<ItemScriptV3>().inSlot = true;
        obj.GetComponent<ItemScriptV3>().currentSlot = this;
        itemInSlot = obj;
        slotImage.color = Color.grey;
    }

    public void insertItem_Magnetic(GameObject obj)
    {
        Debug.Log("MAGNETIC INSERT: Successfully inserted " + obj.gameObject.name);
        // obj.GetComponent<Rigidbody>().isKinematic = true;
        obj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        obj.transform.SetParent(this.gameObject.transform, true);
        obj.GetComponent<ItemScriptV3>().inSlot = true;
        obj.GetComponent<ItemScriptV3>().currentSlot = this;
        itemsInSlot.Add(obj);
        slotImage.color = Color.grey;
    }

    public void insertItem_wristStack(GameObject obj)
    {
        Debug.Log("Wrist-based Stack: Successfully inserted " + obj.gameObject.name);
        // obj.GetComponent<Rigidbody>().isKinematic = true;
        obj.transform.localScale = obj.GetComponent<ItemScriptV3>().defaultScale * 0.5f;

        wristBased_slotStack.GetComponent<SlotScriptV3>().itemsInSlot_wristBased.Add(obj);
        wristBased_slotStack.GetComponent<SlotScriptV3>().updateStackSlotContents();
        obj.GetComponent<ItemScriptV3>().inSlot = true;
        obj.GetComponent<ItemScriptV3>().currentSlot = wristBased_slotStack.GetComponent<SlotScriptV3>();
        slotImage.color = Color.grey;
    }

    public void updateStackSlotContents()
    {
        // Debug.Log("UPDATING STACK CONTENTS FROM SLOT " + this.gameObject.name);
        if (itemsInSlot_wristBased.Count == 0)
        {
            currentStackTop = null;
            currentStackBottom = null;
            return;
        }
        else
        {
            // Debug.Log("CURRENT STACK TOP == " + itemsInSlot_wristBased[itemsInSlot_wristBased.Count-1]);
            currentStackTop = itemsInSlot_wristBased[itemsInSlot_wristBased.Count - 1];
            //we set y to 1 so we can display the bottom of the stack!!
            currentStackBottom = itemsInSlot_wristBased[0];
            for (int y = 1; y < itemsInSlot_wristBased.Count - 1; y++)
            {
                itemsInSlot_wristBased[y].SetActive(false);
            }
            itemsInSlot_wristBased[itemsInSlot_wristBased.Count - 1].SetActive(true);
            itemsInSlot_wristBased[0].SetActive(true);
        }

        if (currentStackBottom != currentStackTop)
        {
            currentStackTop.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            currentStackTop.transform.SetParent(this.gameObject.transform, true);
            currentStackTop.transform.localPosition = Vector3.zero;
            currentStackTop.transform.localEulerAngles = currentStackTop.GetComponent<ItemScriptV3>().slotRotation;
            currentStackBottom.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            currentStackBottom.transform.SetParent(wristBased_slotStack_bottom.gameObject.transform, true);
            currentStackBottom.transform.localPosition = Vector3.zero;
            currentStackBottom.transform.localEulerAngles = currentStackBottom.GetComponent<ItemScriptV3>().slotRotation;
        } else {
            currentStackTop.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            currentStackTop.transform.SetParent(this.gameObject.transform, true);
            currentStackTop.transform.localPosition = Vector3.zero;
            currentStackTop.transform.localEulerAngles = currentStackTop.GetComponent<ItemScriptV3>().slotRotation;
        }



        // Debug.Log("Finished");
    }

    public void resetColor()
    {
        slotImage.color = originalColor;
    }


    public void ManualTriggerReleaseItem(GameObject other)
    {
        if (itemInSlot != null)
        {
            if (other.gameObject.GetComponent<ItemScriptV3>().inSlot)
            {
                other.gameObject.GetComponent<ItemScriptV3>().currentSlot.itemInSlot = null;
                other.transform.SetParent(null);
                other.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

                other.gameObject.GetComponent<ItemScriptV3>().inSlot = false;
                other.gameObject.GetComponent<ItemScriptV3>().currentSlot.resetColor();
                other.gameObject.GetComponent<ItemScriptV3>().currentSlot = null;


            }

        }

    }

}
