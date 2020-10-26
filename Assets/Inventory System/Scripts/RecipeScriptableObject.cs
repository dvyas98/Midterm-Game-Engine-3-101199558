/*//////////////////
Dhimant Vyas : Midterm Game Engine 3 : Crafting System 
File
RecipeScriptableObjec : 
Making our Recipe. This will Take an array of Inopt Items which are our Base Items.
And One output Item which will be our Recipe.
/////////////////*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/RecipeScriptableObject")]
public class RecipeScriptableObject : ScriptableObject
{
    [SerializeField]
    public Item output;


    [SerializeField]
    public Item[] baseitems;

}
