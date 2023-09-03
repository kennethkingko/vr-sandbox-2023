// Script name: InventoryVR
// Script purpose: attaching a gameobject to a certain anchor and having the ability to enable and disable it.
// This script is a property of Realary, Inc

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using TMPro;

public class InventoryVR : MonoBehaviour
{

    private enum ControllerSide
    {
        Left_Controller,
        Right_Controller,
    }

    [Header("========= General setup")]
    [SerializeField] private ControllerSide mainController;
    private InputDeviceCharacteristics mainControllerCharacteristics;
    [SerializeField] public GameObject Anchor;
    [SerializeField] public GameObject Anchor_hand;
    [SerializeField] public GameObject buttonWorksTest;
    [SerializeField] public GameObject gripWorksTest;
    [SerializeField] public GameObject inventoryColliderCheck;

    [SerializeField] public GameObject HUDelement;

    [SerializeField] public GameObject debugLog;
    [SerializeField] public GameObject inventoryTypeLog;

    [SerializeField] public GameObject loadingBar;
    [SerializeField] public GameObject raycastStateText;
    [SerializeField] public GameObject inventoryStateText;
    public float progressBarProgress;

    [Header("=========")]
    // public GameObject itemColliding;
    // public GameObject slotColliding;
    public bool debugHandIsFalse;

    [Header("========= Device debug")]
    public List<InputDevice> mainDevice = new List<InputDevice>();
    public List<InputDevice> subDevice = new List<InputDevice>();
    bool UIActive;

    [SerializeField]
    public float buttonDelay = 0.3f;

    // public InputDevice mainDevice;

    [Header("======= Inventory systems")]


    // possible inventory systems so far!!!: 'slotsdefault', 'shelf', 'wristbased'

    public string currentSelectedInventorySystem;

    public string wantedInventorySystem;

    public bool changeInventoryStateCalled;
    [SerializeField] public GameObject HandInventory;
    [SerializeField] public GameObject ShelfInventory;
    [SerializeField] public GameObject WristInventory;
    [SerializeField] public GameObject MagneticSurface;

    [SerializeField] public GameObject MagicBox;

    [SerializeField] public GameObject HandbasedInventoryHierarchy;

    public List<GameObject> addedInventorySystems;

    public float lastPressTime = 0.5f;

    private bool m_debugMode = true;
    public SlotScriptV3[] loltest;

    [Header("======= Raycast Items")]

    public bool raycastState;

    [SerializeField] public GameObject ray_Lhand;
    [SerializeField] public GameObject ray_Rhand;
    [SerializeField] public GameObject ray_Lcont;
    [SerializeField] public GameObject ray_Rcont;

    [Header("========= Logic debug")]
    public GameObject currentInv;

    public List<GameObject> tempStorageInvMove;

    [Header("========= Disabled item carryover settings")]

    public bool disableCrossInventoryItemCarryover;

    public List<GameObject> defaultObjectList;

    public List<GameObject> defaultObjectListDuplicate;

    public List<Vector3> defaultObjectListPosition;


    public void toggleRaycast()
    {   
        raycastState = !raycastState;

        if (raycastState)
        {
            debugLog.GetComponent<TextMeshProUGUI>().text = "Raycasting enabled";
        }
        else
        {
            debugLog.GetComponent<TextMeshProUGUI>().text = "Raycasting disabled";
        }
    }

    void Start()
    {
        changeInventoryStateCalled = false;
        currentSelectedInventorySystem = "slotsdefault";
        HandbasedInventoryHierarchy.SetActive(false);
        UIActive = false;
        raycastState = true;

        // if (DebugLogger.current == null) {m_debugMode = false;}
        if (mainController == ControllerSide.Left_Controller)
        {
            mainControllerCharacteristics = InputDeviceCharacteristics.Left;
        }
        else
        {
            mainControllerCharacteristics = InputDeviceCharacteristics.Right;
        }

        Debug.Log("main device init");

        addedInventorySystems.Add(HandInventory);
        addedInventorySystems.Add(ShelfInventory);
        addedInventorySystems.Add(WristInventory);
        addedInventorySystems.Add(MagneticSurface);
        addedInventorySystems.Add(MagicBox);

        foreach (GameObject x in defaultObjectList)
        {
            GameObject newnew = Instantiate(x);
            newnew.SetActive(false);
            defaultObjectListDuplicate.Add(newnew);

            // defaultObjectListPosition.Add(x.transform.position);
        }



    }

    void Update()
    {
        // if(Anchor.transform.parent.gameObject.activeSelf){
        //     Anchor_hand.transform.position = Anchor.transform.position;
        // }


        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Left, mainDevice);

        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Right, subDevice);

        if (mainDevice.Count == 1)
        {
            // Device found
            openCloseInventory(mainDevice[0]);
        }
        else
        {
            // Debug.Log("Controller not found:");
        }


        if (subDevice.Count >= 1)
        {
            // Device found
            releaseGrip(subDevice[0]);
            // Debug.Log("R CTRL F");
        }
        else
        {
            // Debug.Log("Controller not found:");
        }

        if (lastPressTime > 0f)
        {
            lastPressTime -= Time.deltaTime;
        }

        ray_Lhand.SetActive(raycastState);
        ray_Rhand.SetActive(raycastState);
        ray_Lcont.SetActive(raycastState);
        ray_Rcont.SetActive(raycastState);
        
        if (raycastState){
            raycastStateText.GetComponent<TextMeshProUGUI>().text = "Raycasting ON";
        } else {
            raycastStateText.GetComponent<TextMeshProUGUI>().text = "Raycasting OFF";
        }


        if (currentSelectedInventorySystem == "slotsdefault" || currentSelectedInventorySystem == "wristbased")
        {
            if (HandbasedInventoryHierarchy.activeSelf)
            {   
                inventoryStateText.GetComponent<TextMeshProUGUI>().text = "Inventory ACTIVE";
                if (Anchor.transform.parent.gameObject.activeSelf)
                {
                    debugHandIsFalse = true;
                    HandbasedInventoryHierarchy.transform.position = Vector3.Lerp(HandbasedInventoryHierarchy.transform.position, Anchor.transform.position, 1);
                    HandbasedInventoryHierarchy.transform.eulerAngles = new Vector3(Anchor.transform.eulerAngles.x + 15, Anchor.transform.eulerAngles.y, 0);
                }

                if (Anchor_hand.transform.parent.gameObject.activeSelf && !Anchor.transform.parent.gameObject.activeSelf)
                {
                    debugHandIsFalse = false;
                    HandbasedInventoryHierarchy.transform.position = Vector3.Lerp(HandbasedInventoryHierarchy.transform.position, Anchor_hand.transform.position, 1);
                    HandbasedInventoryHierarchy.transform.eulerAngles = new Vector3(Anchor_hand.transform.eulerAngles.x + 15, Anchor_hand.transform.eulerAngles.y, 0);
                }

            } else {
                inventoryStateText.GetComponent<TextMeshProUGUI>().text = "Inventory INACTIVE";
            }
        }

        if (changeInventoryStateCalled)
        {
            if (!disableCrossInventoryItemCarryover)
            {
                changeInventorySystemState(currentSelectedInventorySystem, wantedInventorySystem);
                changeInventoryStateCalled = false;
            }
            else
            {
                changeInventorySystemState_DEBUG(currentSelectedInventorySystem, wantedInventorySystem);
                changeInventoryStateCalled = false;
            }

        }

        loadingBar.GetComponent<Slider>().value = progressBarProgress;


        // Debug.Log(itemColliding+" | "+slotColliding);

    }



    private void openCloseInventory(InputDevice x)
    {

        bool primaryButtonPressed = false;
        x.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primaryButton, out primaryButtonPressed);
        if (primaryButtonPressed && lastPressTime <= 0 && (currentSelectedInventorySystem != "shelf" && currentSelectedInventorySystem != "magnetic" && currentSelectedInventorySystem != "magicbox"))
        {
            // Debug.Log("Primary button pressed!");
            buttonWorksTest.SetActive(!buttonWorksTest.activeSelf);
            HandbasedInventoryHierarchy.SetActive(!HandbasedInventoryHierarchy.activeSelf);
            if (HandbasedInventoryHierarchy.activeSelf)
            {
                debugLog.GetComponent<TextMeshProUGUI>().text = "Opened inventory";
            }
            else
            {
                debugLog.GetComponent<TextMeshProUGUI>().text = "Closed inventory";
            }
            lastPressTime = buttonDelay;
        }
    }

    public void enabledDisableHUD()
    {
        HUDelement.SetActive(!HUDelement.activeSelf);
    }

    private void releaseGrip(InputDevice x)
    {
        bool gripButtonPressed;
        x.TryGetFeatureValue(UnityEngine.XR.CommonUsages.gripButton, out gripButtonPressed);
        if (gripButtonPressed)
        {
            // Debug.Log("Trigger test");
            gripWorksTest.SetActive(!gripWorksTest);
        }

    }

    public void checkInventorySystemState(string systemName)
    {

        switch (systemName)
        {
            case "shelf":
                Debug.Log("Inventory System now SHELF");
                break;
            case "slotsdefault":
                Debug.Log("Inventory System now VIRTUAL SLOTS");
                break;
            case "wristbased":
                Debug.Log("Inventory System now WRISTBASED");
                break;
            case "magnetic":
                Debug.Log("Inventory System now MAGNETIC SURFACE");
                break;
            case "magicbox":
                Debug.Log("Inventory System now MAGIC BOX");
                break;
        }

    }




    public void changeInventorySystemState_DEBUG(string currentSystem, string wantedSystem)
    {

        if (currentSystem == wantedSystem)
        {
            return;
        }

        switch (currentSystem)
        {
            case "shelf":
                currentInv = ShelfInventory;
                break;
            case "slotsdefault":
                currentInv = HandInventory;
                break;
            case "wristbased":
                currentInv = WristInventory;
                break;
            case "magnetic":
                currentInv = MagneticSurface;
                break;
            case "magicbox":
                currentInv = MagicBox;
                break;
        }

        if (currentInv == ShelfInventory || currentInv == HandInventory)
        {
            for (int i = 0; i < currentInv.transform.childCount; i++)
            {
                if (currentInv.transform.GetChild(i).GetComponent<SlotScriptV3>() != null)
                {
                    if (currentInv.transform.GetChild(i).GetComponent<SlotScriptV3>().itemInSlot)
                    {
                        currentInv.transform.GetChild(i).GetComponent<SlotScriptV3>().resetColor();
                        currentInv.transform.GetChild(i).GetComponent<SlotScriptV3>().itemInSlot = null;
                    }

                }
            }
        }
        else
        {
            for (int i = 0; i < currentInv.transform.childCount; i++)
            {
                if (currentInv.transform.GetChild(i).GetComponent<SlotScriptV3>() != null)
                {
                    if (currentInv.transform.GetChild(i).GetComponent<SlotScriptV3>().magneticSurfaceMode)
                    {

                        currentInv.transform.GetChild(i).GetComponent<SlotScriptV3>().itemsInSlot.Clear();

                    }
                    else if (currentInv.transform.GetChild(i).GetComponent<SlotScriptV3>().wristBasedMode)
                    {
                        if (currentInv.transform.GetChild(i).GetComponent<SlotScriptV3>().wristBased_entrySlotDesignation)
                        {
                            foreach (GameObject x in currentInv.transform.GetChild(i).GetComponent<SlotScriptV3>().wristBased_slotStack.GetComponent<SlotScriptV3>().itemsInSlot_wristBased)
                            {
                                x.SetActive(true);
                            }
                            currentInv.transform.GetChild(i).GetComponent<SlotScriptV3>().wristBased_slotStack.GetComponent<SlotScriptV3>().itemsInSlot_wristBased.Clear();
                            currentInv.transform.GetChild(i).GetComponent<SlotScriptV3>().wristBased_slotStack.GetComponent<SlotScriptV3>().updateStackSlotContents();
                        }


                    }

                }
            }
        }


        switch (wantedSystem)
        {
            case "shelf":
                currentInv.SetActive(false);
                ShelfInventory.SetActive(true);
                wantedInventorySystem = "";
                currentSelectedInventorySystem = "shelf";
                break;
            case "slotsdefault":
                currentInv.SetActive(false);
                HandInventory.SetActive(true);
                wantedInventorySystem = "";
                currentSelectedInventorySystem = "slotsdefault";
                break;
            case "wristbased":
                currentInv.SetActive(false);
                WristInventory.SetActive(true);
                wantedInventorySystem = "";
                currentSelectedInventorySystem = "wristbased";
                break;
            case "magnetic":
                currentInv.SetActive(false);
                MagneticSurface.SetActive(true);
                wantedInventorySystem = "";
                currentSelectedInventorySystem = "magnetic";
                break;
            case "magicbox":
                currentInv.SetActive(false);
                MagicBox.SetActive(true);
                wantedInventorySystem = "";
                currentSelectedInventorySystem = "magicbox";
                break;
        }

        for (int x = 0; x < defaultObjectList.Count; x++)
        {

            defaultObjectList[x].transform.position = defaultObjectListDuplicate[x].transform.position;
            defaultObjectList[x].transform.rotation = defaultObjectListDuplicate[x].transform.rotation;
            defaultObjectList[x].GetComponent<ItemScriptV3>().forceReset();

        }

    }

    public void changeInventorySystemState(string currentSystem, string wantedSystem)
    {

        if (currentSystem == wantedSystem)
        {
            return;
        }

        switch (currentSystem)
        {
            case "shelf":
                currentInv = ShelfInventory;
                break;
            case "slotsdefault":
                currentInv = HandInventory;
                break;
            case "wristbased":
                currentInv = WristInventory;
                break;
            case "magnetic":
                currentInv = MagneticSurface;
                break;
            case "magicbox":
                currentInv = MagicBox;
                break;
        }

        if (currentInv == ShelfInventory || currentInv == HandInventory)
        {
            for (int i = 0; i < currentInv.transform.childCount; i++)
            {
                if (currentInv.transform.GetChild(i).GetComponent<SlotScriptV3>() != null)
                {
                    if (currentInv.transform.GetChild(i).GetComponent<SlotScriptV3>().itemInSlot)
                    {
                        tempStorageInvMove.Add(currentInv.transform.GetChild(i).GetComponent<SlotScriptV3>().itemInSlot);
                        currentInv.transform.GetChild(i).GetComponent<SlotScriptV3>().ManualTriggerReleaseItem(currentInv.transform.GetChild(i).GetComponent<SlotScriptV3>().itemInSlot);
                    }

                }
            }
        }
        else
        {
            for (int i = 0; i < currentInv.transform.childCount; i++)
            {
                if (currentInv.transform.GetChild(i).GetComponent<SlotScriptV3>() != null)
                {
                    if (currentInv.transform.GetChild(i).GetComponent<SlotScriptV3>().magneticSurfaceMode)
                    {
                        for (int x = 0; x < currentInv.transform.GetChild(i).GetComponent<SlotScriptV3>().itemsInSlot.Count; i++)
                        {
                            tempStorageInvMove.Add(currentInv.transform.GetChild(i).GetComponent<SlotScriptV3>().itemsInSlot[x]);
                        }
                        currentInv.transform.GetChild(i).GetComponent<SlotScriptV3>().itemsInSlot.Clear();

                    }
                    else if (currentInv.transform.GetChild(i).GetComponent<SlotScriptV3>().wristBasedMode)
                    {
                        if (currentInv.transform.GetChild(i).GetComponent<SlotScriptV3>().wristBased_entrySlotDesignation)
                        {
                            for (int x = 0; x < currentInv.transform.GetChild(i).GetComponent<SlotScriptV3>().wristBased_slotStack.GetComponent<SlotScriptV3>().itemsInSlot_wristBased.Count; i++)
                            {
                                tempStorageInvMove.Add(currentInv.transform.GetChild(i).GetComponent<SlotScriptV3>().wristBased_slotStack.GetComponent<SlotScriptV3>().itemsInSlot_wristBased[x]);

                            }
                            currentInv.transform.GetChild(i).GetComponent<SlotScriptV3>().wristBased_slotStack.GetComponent<SlotScriptV3>().itemsInSlot_wristBased.Clear();
                        }


                    }

                }
            }
        }




        Debug.Log("tempStorage size:" + tempStorageInvMove.Count);

        switch (wantedSystem)
        {
            case "shelf":
                transferInventoryMethod(ShelfInventory);

                ShelfInventory.SetActive(true);
                wantedInventorySystem = "";
                currentSelectedInventorySystem = "shelf";
                break;
            case "slotsdefault":
                transferInventoryMethod(HandInventory);
                HandInventory.SetActive(true);
                wantedInventorySystem = "";
                currentSelectedInventorySystem = "slotsdefault";
                break;
            case "wristbased":
                transferInventoryMethod(WristInventory);
                WristInventory.SetActive(true);
                wantedInventorySystem = "";
                currentSelectedInventorySystem = "wristbased";
                break;
            case "magnetic":
                transferInventoryMethod(MagneticSurface);
                MagneticSurface.SetActive(true);
                wantedInventorySystem = "";
                currentSelectedInventorySystem = "magnetic";
                break;
            case "magicbox":
                transferInventoryMethod(MagicBox);
                MagicBox.SetActive(true);
                wantedInventorySystem = "";
                currentSelectedInventorySystem = "magicbox";
                break;
        }

    }


    public void transferInventoryMethod(GameObject invType)
    {


        loltest = invType.GetComponentsInChildren<SlotScriptV3>();

        int yx = 0;

        foreach (SlotScriptV3 xd in loltest)
        {
            yx += 1;
        }

        Debug.Log("TOTAL SLOTS IN INVENTORY SYSTEM " + invType.name + ": " + yx);

        currentInv.SetActive(false);

        if (yx >= tempStorageInvMove.Count)
        {
            Debug.Log(">>>>>:" + yx + " greater than temp storage count");
            if (invType != WristInventory && invType != MagneticSurface)
            {
                for (int i = 0; i < yx + 1; i++)
                {
                    for (int j = 0; j < tempStorageInvMove.Count; j++)
                    {
                        if (invType.transform.GetChild(i).GetComponent<SlotScriptV3>() != null)
                        {
                            Debug.Log(i + " // " + yx);


                            if (invType.transform.GetChild(i).GetComponent<SlotScriptV3>().itemInSlot == null)
                            {
                                Debug.Log("INSERTING ITEM into new INVENTORY:" + tempStorageInvMove[j] + " | COUNT: " + j + " out of " + tempStorageInvMove.Count);
                                invType.transform.GetChild(i).GetComponent<SlotScriptV3>().insertItem(tempStorageInvMove[j]);
                                tempStorageInvMove.RemoveAt(j);
                            }

                        }
                        break;
                    }
                }

            }
            else
            {
                if (invType == WristInventory)
                {
                    for (int i = 0; i < yx + 1; i++)
                    {
                        for (int j = 0; j < tempStorageInvMove.Count; j++)
                        {
                            if (invType.transform.GetChild(i).GetComponent<SlotScriptV3>() != null)
                            {
                                if (invType.transform.GetChild(i).GetComponent<SlotScriptV3>().wristBased_entrySlotDesignation)
                                {
                                    invType.transform.GetChild(i).GetComponent<SlotScriptV3>().wristBased_slotStack.GetComponent<SlotScriptV3>().itemsInSlot_wristBased.Add(tempStorageInvMove[j]);

                                    tempStorageInvMove.RemoveAt(j);
                                }
                            }

                            break;
                        }
                        if (invType.transform.GetChild(i).GetComponent<SlotScriptV3>() != null)
                        {
                            if (invType.transform.GetChild(i).GetComponent<SlotScriptV3>().wristBased_entrySlotDesignation)
                            {
                                invType.transform.GetChild(i).GetComponent<SlotScriptV3>().wristBased_slotStack.GetComponent<SlotScriptV3>().updateStackSlotContents();
                            }
                        }


                    }

                }
                else if (invType == MagneticSurface)
                {
                    for (int i = 0; i < yx + 1; i++)
                    {
                        for (int j = 0; j < tempStorageInvMove.Count; j++)
                        {
                            if (invType.transform.GetChild(i).GetComponent<SlotScriptV3>() != null)
                            {
                                if (!invType.transform.GetChild(i).GetComponent<SlotScriptV3>().magneticSurfaceMode)
                                {
                                    invType.transform.GetChild(i).GetComponent<SlotScriptV3>().itemsInSlot.Add(tempStorageInvMove[j]);
                                    tempStorageInvMove.RemoveAt(j);
                                }
                            }
                            break;
                        }
                    }
                }
            }
            tempStorageInvMove.Clear();
        }
        else
        {
            Debug.Log("<<<<<:" + yx + " less than temp storage count");
            // for (int i = 0; i < yx; i++)
            // {
            //     if (invType.transform.GetChild(i).GetComponent<SlotScriptV3>() != null)
            //     {
            //          if (invType.transform.GetChild(i).GetComponent<SlotScriptV3>().itemInSlot == null)
            //         {
            //             invType.transform.GetChild(i).GetComponent<SlotScriptV3>().insertItem(tempStorageInvMove[0]);
            //             tempStorageInvMove.RemoveAt(0);
            //             break;
            //         } else {
            //             break;
            //         }

            //     }
            // }

            for (int i = 0; i < yx + 1; i++)
            {
                for (int j = 0; j < tempStorageInvMove.Count; j++)
                {
                    if (invType.transform.GetChild(i).GetComponent<SlotScriptV3>() != null)
                    {
                        if (invType.transform.GetChild(i).GetComponent<SlotScriptV3>().itemInSlot == null)
                        {
                            Debug.Log("INSERTING ITEM into new INVENTORY:" + tempStorageInvMove[j]);
                            invType.transform.GetChild(i).GetComponent<SlotScriptV3>().insertItem(tempStorageInvMove[j]);
                            tempStorageInvMove.RemoveAt(j);
                        }

                    }
                    break;
                }
            }

            for (int k = 0; k < tempStorageInvMove.Count; k++)
            {
                invType.transform.GetChild(0).GetComponent<SlotScriptV3>().ManualTriggerReleaseItem(currentInv.transform.GetChild(k).GetComponent<SlotScriptV3>().itemInSlot);
                Debug.Log(tempStorageInvMove[k].name + " expected to drop to ground");
            }

            tempStorageInvMove.Clear();
        }
    }

    public void setInventoryToHands()
    {
        wantedInventorySystem = "slotsdefault";
        changeInventoryStateCalled = true;
        inventoryTypeLog.GetComponent<TextMeshProUGUI>().text = "Slots anchored to hand";
        debugLog.GetComponent<TextMeshProUGUI>().text = "Changed inventory system to Slots anchored to hand";

    }
    public void setInventoryToShelf()
    {
        wantedInventorySystem = "shelf";
        changeInventoryStateCalled = true;
        inventoryTypeLog.GetComponent<TextMeshProUGUI>().text = "Shelf";
        debugLog.GetComponent<TextMeshProUGUI>().text = "Changed inventory system to Shelf";

    }
    public void setInventoryToWrist()
    {
        wantedInventorySystem = "wristbased";
        changeInventoryStateCalled = true;
        inventoryTypeLog.GetComponent<TextMeshProUGUI>().text = "Wrist-based Stack";
        debugLog.GetComponent<TextMeshProUGUI>().text = "Changed inventory system to Wrist-based Stack";

    }

    public void setInventoryToMagnetic()
    {
        wantedInventorySystem = "magnetic";
        changeInventoryStateCalled = true;
        inventoryTypeLog.GetComponent<TextMeshProUGUI>().text = "Magnetic Surface";
        debugLog.GetComponent<TextMeshProUGUI>().text = "Changed inventory system to Magnetic Surface";

    }

    public void setInventoryToMagicBox()
    {
        wantedInventorySystem = "magicbox";
        changeInventoryStateCalled = true;
        inventoryTypeLog.GetComponent<TextMeshProUGUI>().text = "Magic Box";
        debugLog.GetComponent<TextMeshProUGUI>().text = "Changed inventory system to Magic Box";

    }
}
