/*//////////////////
Dhimant Vyas : Midterm Game Engine 3 : Crafting System 
File
Item Slot : 
For Using the Items  / Removing The Items / Checking If the Slot has any Item.

/////////////////*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

public class ItemSlot : MonoBehaviour
{

    public GameObject PlayerCharacter;
    private Sprite sprite;

    // Event callbacks
    public UnityEvent<Item> onItemUse;   //Using Events For Item Use Purposes.

    // flag to tell ItemSlot it needs to update itself after being changed
    private bool b_needsUpdate = true;

    // Declared with auto-property
    public Item ItemInSlot { get; private set; }
    public int ItemCount { get; private set; }

    public bool craftingslot;  ///Is It Crafting Slot?
    public bool outputslot;   // Is it Output Slot?


    // scene references
    [SerializeField]
    private TMPro.TextMeshProUGUI itemCountText;   

    [SerializeField]
    private Image itemIcon;   //For the Grabbing Icon for our Crafting System.


    private void Start()
    {
        sprite = this.GetComponent<Image>().sprite;
        PlayerCharacter = GameObject.Find("PlayerCharacter");

    }


    private void Update()
    {
        if (b_needsUpdate)  ///If it needs Updating We do so.
        {
            UpdateSlot();
        }
    }

    /// <summary>
    /// Returns true if there is an item in the slot
    /// </summary>
    /// <returns></returns>
    public bool HasItem()
    {
        return ItemInSlot != null;
    }

    /// <summary>
    /// Removes everything in the item slot
    /// </summary>
    /// <returns></returns>
    public void ClearSlot()
    {
        ItemInSlot = null;
        b_needsUpdate = true;
    }

    /// <summary>
    /// Attempts to remove a number of items. Returns number removed
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    public int TryRemoveItems(int count)
    {
        if (count > ItemCount)
        {
            int numRemoved = ItemCount;
            ItemCount -= numRemoved;
            b_needsUpdate = true;
            return numRemoved;
        }
        else
        {
            ItemCount -= count;
            b_needsUpdate = true;
            return count;
        }
    }

    /// <summary>
    /// Sets what is contained in this slot
    /// </summary>
    /// <param name="item"></param>
    /// <param name="count"></param>
    public void SetContents(Item item, int count)
    {
        ItemInSlot = item;
        ItemCount = count;
        b_needsUpdate = true;
    }

    /// <summary>
    /// Activate the item currently held in the slot
    /// </summary>
    public void UseItem()     ///Where Magic Happens.
    { 

        if (ItemInSlot == null)
        {
            //Debug.Log("Here");

            if (!outputslot)  // If its Not an Output Slot Then do this.
                if (PlayerCharacter.GetComponent<GrabnGo>().grabbed == true) // Are we Grabbing any thing?
                {
                    SetContents(PlayerCharacter.GetComponent<Inventory>().masterItemTable.GetItem(PlayerCharacter.GetComponent<GrabnGo>().item), 1); //Then Set THe contents.
                    PlayerCharacter.GetComponent<GrabnGo>().grabbed = false;  //Now that we have set Everything We turn that to False.
                    Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);   // Setting our Cursor to Nothing.
                    ItemCount = 1;
                    b_needsUpdate = true;
                    if (PlayerCharacter.GetComponent<GrabnGo>().grabbedOutput)  // Are We grabbing output?
                    {
                        GameObject[] gameObject = GameObject.FindGameObjectsWithTag("ItemSlotCrafting");  // Lets go the Crafting Table.
                        foreach (var item in gameObject)
                        {
                            item.SendMessage("ClearSlot");  
                        }
                        PlayerCharacter.GetComponent<GrabnGo>().grabbedOutput = false;
                    }
                    craftingTraversal();  //Run this Method.
                    return;
                }  
        }
        if (ItemInSlot != null)
        {
            if (ItemCount >= 1)
            {
                Debug.Log("CraftHere");
                ItemInSlot.Use();  // First er use the Item.
                onItemUse.Invoke(ItemInSlot);  //Invoke On ITem Use.
                ItemCount--;
                b_needsUpdate = true;
            }
        }
    }

    private void craftingTraversal()
    {
        GameObject gameObject = GameObject.FindGameObjectWithTag("CraftedOutput");  // Find Our OutputSlot
        gameObject.SendMessage("startTraversing"); //Run Traversal Method.
    }

    public void startTraversing()
    {
        PlayerCharacter.SendMessage("craftingItemInspector"); //Traversing just Runs this method from our Player. This method is in INVENTORY and Checks if we have our Recipe.
    }

    /// <summary>
    /// Update visuals of slot to match items contained
    /// </summary>
    private void UpdateSlot()
    {
        if (ItemCount == 0)
        {
            ItemInSlot = null;  //Setting the item SLot to Null Cause Item Count is Zero So it wont have Anything.
            itemCountText.text = ""; //So that Nothing will show as Text.

        }

        if (ItemInSlot != null)
        {
            itemCountText.text = ItemCount.ToString();
            itemIcon.sprite = ItemInSlot.Icon;
            itemIcon.gameObject.SetActive(true);
            if (ItemCount == 0) itemCountText.text = ""; // If we dont do this then we  will have some Text left even after we remove the Item from that place .
        }
        else  // If Item count is not zero but Item in slot is Null
        {
            itemIcon.gameObject.GetComponent<Image>().sprite = sprite;
            ItemCount = 0;
           itemCountText.text = "";

        }

        b_needsUpdate = false;
    }
}


