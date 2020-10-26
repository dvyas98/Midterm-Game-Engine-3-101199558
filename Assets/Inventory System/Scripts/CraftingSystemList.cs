/*//////////////////
Dhimant Vyas : Midterm Game Engine 3 : Crafting System 
File
Crafting System List. : 
This is the List for all of our Recipes.

/////////////////*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/CaraftingSystemList")]
public class CraftingSystemList: ScriptableObject
{
    [SerializeField] 
    public RecipeScriptableObject[] RecipeList;


}
