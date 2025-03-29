using System.Collections.Generic;
using UnityEngine;

public struct Pin
{
    public int pinNumber; // Pin number (determined by order in list)
    public bool isNextBindingPin; // is the pin the next binding pin in the sequence
}

public class LockLogic : MonoBehaviour
{
    // reference to scriptable object
    // reference to player script

    #region pin logic
    // generate list of pins according to the count on the scriptable object
    // for example, if the scriptable object has a count of 5, generate a list of 5 pins

    List<Pin> pins = new List<Pin>(); // list of pins
                                      // set the pinNumber for each pin in the list based on the index of the pin in the list

    // generate a random order for the pins
    List<int> pinOrder = new List<int>(); // list of pin numbers in random order

    #endregion

    #region rotation logic (tension wrench)
    // set range of ideal rotation for the tension wrench (based on SO data)
    // set range of ideal amount of tension applied (based on SO data)
    #endregion
}
