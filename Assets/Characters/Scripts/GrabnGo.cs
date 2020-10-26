/*//////////////////
Dhimant Vyas : Midterm Game Engine 3 : Crafting System 
File
Grab n Go : 
Simple function if Grabbed SOmething Turn the bool True.
If that something is output turn that true if not turn Grab to true.

/////////////////*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabnGo : MonoBehaviour
{
    public bool grabbed;
    public bool grabbedOutput;
    public int item;
    // Start is called before the first frame update
  
    void Grab(int type)
    {
        grabbed = true;
        item = type;

    }
    void grabOutput()
    {
        grabbedOutput = true;
    }
}
