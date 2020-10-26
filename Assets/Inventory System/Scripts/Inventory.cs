/*//////////////////
Dhimant Vyas : Midterm Game Engine 3 : Crafting System 
File
Inventory : 
This file will be used to make the Crafting Table , Main Table as well as Output Table.

/////////////////*/
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System.Linq; // For finding all gameObjects with name

public class Inventory : MonoBehaviour, ISaveHandler
{
    [Tooltip("Reference to the master item table")]
    [SerializeField]
    public ItemTable masterItemTable;         //We need our MasterTable to get the list of all our items that we have in our Store.

    [Tooltip("The object which will hold Item Slots as its direct children")]
    [SerializeField]
    private GameObject inventoryPanel;    //For the Output We will choose outputInventory Panel, For Crafting we will choose Craafting Inventory Panel and for Main tabel we will choose Inventory Panel.
                                          // VERY IMPORTANT TO CHOOSE THE CORRECT PANEL. As we will be DUplicating This Same script thrice to Make All the Tables.
    [Tooltip("List size determines how many slots there will be. Contents will replaced by copies of the first element")]
    [SerializeField]
    private List<ItemSlot> itemSlots;    //Just Like the Panel Choose the right slots.

    [Tooltip("Items to add on Start for testing purposes")]
    [SerializeField]
    private List<Item> startingItems;     // Starting Items will be filled for out main Inventory Panel. We will choose the items that we made in Item Folder also has been assigned Appropriate ID in MAster Table.

    [SerializeField]
    private bool outputSlotbool;    //If this Panel is output panel then make this bool to true. 
    [SerializeField]
    private bool craftingSystem;   //If this Panel is Crafting panel then make this bool to true. 



    [Tooltip("Output Slot")]
    [SerializeField]
    private ItemSlot OutputSlot;   //Selct appropriate Outut slot.


    [SerializeField] 
    public CraftingSystemList RecipeList;  //This is where we select our Recipe list so that panel know our Recipes and when correct Items are in Order then craft Recipe.

    public bool[] craftingSystemarray =
    {                                      // All our Recipe has nine Slot(3*3 Array). At the beginning we will set Everything to False.
        false, false, false,               // In the game we will do 2 for loops to check this bool.
        false, false, false,               // If all this bools turn to true we will make aapropriate Recipe.
        false, false, false
    };


    /// <summary>
    /// Private key used for saving with playerprefs
    /// </summary>
    private string saveKey = "";

    // Start is called before the first frame update
    void Start()
    {
        InitItemSlots();
        InitSaveInfo();

        // init starting items for testing
        for (int i = 0; i < startingItems.Count && i < itemSlots.Count; i++)
        {
            itemSlots[i].SetContents(startingItems[i], 16);
        }
    }

    private void InitItemSlots()     //Intialise the slots.
    {
        Assert.IsTrue(itemSlots.Count > 0, "itemSlots was empty");
        Assert.IsNotNull(itemSlots[0], "Inventory is missing a prefab for itemSlots. Add it as the first element of its itemSlot list");

        // init item slots
        for (int i = 1; i < itemSlots.Count; i++)
        {
            GameObject newObject = Instantiate(itemSlots[0].gameObject, inventoryPanel.transform);  // From the selected Inventory Panel we will Instantiate our SLots.
            ItemSlot newSlot = newObject.GetComponent<ItemSlot>();
            itemSlots[i] = newSlot;
            if (craftingSystem)
            {
                itemSlots[i].craftingslot = true;
            }
            if (outputSlotbool)
            {
                itemSlots[i].outputslot = true;
            }
        }

        foreach (ItemSlot item in itemSlots)
        {
            item.onItemUse.AddListener(OnItemUsed);   //adding Listner for all the slots in our Scene.
        }
    }
    private void InitSaveInfo()
    {
        // init save info
        //assert only one object with the same name, or else we can have key collisions on PlayerPrefs
        Assert.AreEqual(
            Resources.FindObjectsOfTypeAll(typeof(GameObject)).Where(gameObArg => gameObArg.name == gameObject.name).Count(),
            1,
            "More than one gameObject have the same name, therefore there may be save key collisions in PlayerPrefs"
            );

        // set a key to use for saving/loading
        saveKey = gameObject.name + this.GetType().Name;

        //Subscribe to save events on start so we are listening
        GameSaver.OnLoad.AddListener(OnLoad);
        GameSaver.OnSave.AddListener(OnSave);
    }

    private void OnDestroy()
    {
        // Remove listeners on destroy
        GameSaver.OnLoad.RemoveListener(OnLoad);
        GameSaver.OnSave.RemoveListener(OnSave);

        foreach (ItemSlot item in itemSlots)
        {
            item.onItemUse.RemoveListener(OnItemUsed);
        }
    }

    //////// Event callbacks ////////

    void OnItemUsed(Item itemUsed)                        /// when Item slot is clicked this Triggeres.
    {                           
        Cursor.SetCursor(itemUsed.Icon.texture, Vector2.zero, CursorMode.Auto);      //We are simple seting our cursor to the texture of the thing that we are crafting.     
        Debug.Log("Inventory: item used of category " + itemUsed.Category + "and ID: " + itemUsed.ItemID);  //This gives the user the filling of grabbing that Object
        this.SendMessage("Grab", itemUsed.ItemID);      // Turning the grab bool to true and getting what did we actually frap with thr ID.     //https://docs.unity3d.com/ScriptReference/Cursor.SetCursor.html(Documentation for the method to learn.)
        if (this.outputSlotbool)
        {
            this.SendMessage("grabOutput");  //GrabOutput is the method that turn graboutput bool to true.
        }
    }

    public void OnSave()
    {
        //Make empty string
        //For each item slot
        //Get its current item
        //If there is an item, write its id, and its count to the end of the string
        //If there is not an item, write -1 and 0 

        //File format:
        //ID,Count,ID,Count,ID,Count

        string saveStr = "";

        foreach(ItemSlot itemSlot in itemSlots)
        {
            int id = -1;
            int count = 0;

            if(itemSlot.HasItem())
            {
                id = itemSlot.ItemInSlot.ItemID;
                count = itemSlot.ItemCount;
            }

            saveStr += id.ToString() + ',' + count.ToString() + ',';
        }

        PlayerPrefs.SetString(saveKey, saveStr);
    }

    public void OnLoad()
    {
        //Get save string
        //Split save string
        //For each itemSlot, grab a pair of entried (ID, count) and parse them to int
        //If ID is -1, replace itemSlot's item with null
        //Otherwise, replace itemSlot with the corresponding item from the itemTable, and set its count to the parsed count

        string loadedData = PlayerPrefs.GetString(saveKey, "");

        Debug.Log(loadedData);

        char[] delimiters = new char[] { ',' };
        string[] splitData = loadedData.Split(delimiters);

        for(int i = 0; i < itemSlots.Count; i++)
        {
            int dataIdx = i * 2;

            int id = int.Parse(splitData[dataIdx]);
            int count = int.Parse(splitData[dataIdx + 1]);

            if(id < 0)
            {
                itemSlots[i].ClearSlot();
            } else
            {
                itemSlots[i].SetContents(masterItemTable.GetItem(id), count);
            }
        }
    }

    public void craftingItemInspector()         //Checking Our recipe if We have Evrtything in right Order. If we Do than we make that Recipe.
    {
        if (craftingSystem)
        {
            for (int i = 0; i< RecipeList.RecipeList.Length; i++)    // Getting the Recipe List .    { 00 , 01 , 02       } SO the Way we made our Recipe if Our cradting table looks like tha
            {                                                                                 //     { 10 , 11 , 12       } Then we make our Recipe.
                for (int j = 0; j < itemSlots.Count; j++)                                     //     { 20 , 21 , 22       }
                {
                    if (RecipeList.RecipeList[i].baseitems[j] != itemSlots[j].ItemInSlot)
                    {
                        craftingSystemarray[j] = false;     
                    }
                    if(RecipeList.RecipeList[i].baseitems[j] == itemSlots[j].ItemInSlot)
                    {
                        craftingSystemarray[j] = true;       //Turning All the Bools.

                    }
                }
                if (setoutput() == true)
                {
                    OutputSlot.SetContents(RecipeList.RecipeList[i].output, 1);     // If out Bool is to true then Set Contents. 
                }
            }
        }
    }

    private bool setoutput()
    {
        for(int i = 0; i < craftingSystemarray.Length; i++ )        // If the Whole Array is True Then and then Only We make our Item.
        {
            if(craftingSystemarray[i] == false)
            {
                return false;
            }
        }
        return true;
    }
}
