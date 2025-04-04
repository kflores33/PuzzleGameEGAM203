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

    public Transform WrenchParentedPos;

    TensionWrench wrench;
    Pick pick;

    bool _wrenchInLock; // is the tension wrench in the lock
    [HideInInspector] public bool WrenchHasBeenParented; // has the tension wrench been parented to the lock

    private void Start()
    {
        if (FindFirstObjectByType<TensionWrench>() != null)
        {
            wrench = FindFirstObjectByType<TensionWrench>();
        }
        if(FindAnyObjectByType<Pick>() != null)
        {
            pick = FindAnyObjectByType<Pick>();
            pick.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if(!WrenchHasBeenParented) CheckForWrench();
    }

    #region pin logic
    // generate list of pins according to the count on the scriptable object
    // for example, if the scriptable object has a count of 5, generate a list of 5 pins

    List<Pin> pins = new List<Pin>(); // list of pins
                                      // set the pinNumber for each pin in the list based on the index of the pin in the list

    // generate a random order for the pins
    List<int> pinOrder = new List<int>(); // list of pin numbers in random order

    #endregion

    #region rotation logic (tension wrench)

    private void CheckForWrench()
    {
        // if the tension wrench is in the lock
        if (!_wrenchInLock)
        {
            if (wrench.CheckAlignment())
            {
                _wrenchInLock = true;
            }
            else _wrenchInLock = false;
        }
        else
        {
            // if the tension wrench has not been parented to the lock
            if (!WrenchHasBeenParented)
            {
                // parent the tension wrench to the lock
                wrench.transform.position = WrenchParentedPos.position;
                wrench.transform.SetParent(WrenchParentedPos);

                // set the position of the tension wrench to the position of the lock
                // set the rotation of the tension wrench to the rotation of the lock
                WrenchHasBeenParented = true;
            }
        }
    }

    // set range of ideal rotation for the tension wrench (based on SO data)


    // set range of ideal amount of tension applied (based on SO data)


    #endregion
}
