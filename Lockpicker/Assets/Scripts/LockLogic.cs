using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Pin
{
    public int pinNumber; // Pin number (determined by order in list)

    public bool isNextBindingPin; // is the pin the next binding pin in the sequence
    public bool isSet; // is the pin set

    public bool isSelected; // is the pin selected by the player

    public float MinTension; // minimum amount of tension required to set the pin
    public float MaxTension;
}

public class LockLogic : MonoBehaviour
{
    public LockData lockData; // reference to scriptable object
    // reference to player script

    public Transform WrenchParentedPos;
    public Transform PickParentedPos;

    TensionWrench wrench;
    Pick pick;

    bool _wrenchInLock; // is the tension wrench in the lock
    [HideInInspector] public bool WrenchHasBeenParented; // has the tension wrench been parented to the lock

    bool _pickInLock; // is the pick in the lock
    [HideInInspector] public bool PickHasBeenParented; // has the pick been parented to the lock

    private void Start()
    {
        if (FindFirstObjectByType<TensionWrench>() != null)
        {
            wrench = FindFirstObjectByType<TensionWrench>();
        }
        if (FindAnyObjectByType<Pick>() != null)
        {
            pick = FindAnyObjectByType<Pick>();
            pick.gameObject.SetActive(false);
        }
        else Debug.LogError("Pick not found in scene");

        GeneratePins(); // generate the pins based on the count in the scriptable object
    }

    private void Update()
    {
        if (!WrenchHasBeenParented) { pick.gameObject.SetActive(true); CheckForWrench(); }
        else
        {
            if (!PickHasBeenParented) { CheckForPick(); }
            else
            {
                // if the pick is in the lock and the wrench is in the lock
                // check for player input
                // if player input is correct, unlock the lock
                // else, reset the lock and all pins to their original state
            }
        }
    }
    #region pin logic
        #region making lists

    public List<Pin> pins = new List<Pin>(); // pins in the order the player will interact with them
    int _pinIndex;
    public List<Pin> bindingPins = new List<Pin>(); // the order of binding pins in the lock
    private void GeneratePins()
    {
        // generate a list of pins based on the count in the scriptable object
        for (int i = 0; i < lockData.pinCount; i++)
        {
            Pin pin = new Pin();

            pin.pinNumber = i; // set the pin number based on the order in the list
            pin.MinTension = GeneratePinTensionRange().Item1; // set the min tension based on difficulty
            pin.MaxTension = GeneratePinTensionRange().Item2; // set the max tension based on difficulty
            pins.Add(pin);
        }

        SetPinOrder(); // set the pin order based on the shuffled list
    }

    private Tuple<float, float> GeneratePinTensionRange()
    {
        float minTension = 0f; // minimum amount of tension required to set the pin
        float maxTension = 0f; // maximum amount of tension required to set the pin

        if (lockData.difficulty == LockData.Difficulty.Easy)
        {
            minTension = UnityEngine.Random.Range(0.15f, 0.55f);
            maxTension = minTension + 0.4f;
        }
        else if (lockData.difficulty == LockData.Difficulty.Medium)
        {
            minTension = UnityEngine.Random.Range(0.25f, 0.65f);
            maxTension = minTension + 0.25f;
        }
        else if (lockData.difficulty == LockData.Difficulty.Hard)
        {
            minTension = UnityEngine.Random.Range(0.35f, 0.75f);
            maxTension = minTension + 0.15f;
        }

        return Tuple.Create(minTension, maxTension);
    }

    public List<int> pinOrder = new List<int>(); // list of pin numbers in random order
    private void SetPinOrder()
    {
        bindingPins = new List<Pin>(pins); // create a copy of the pins list to shuffle

        // shuffle order of the pins
        bindingPins.Shuffle();

        for (int i = 0; i < bindingPins.Count; i++)
        {
            Pin pin = pins[i]; // get the pin at index i

            //pin.pinNumber = i; // set the pin number based on the order in the list
            pinOrder.Add(i); // add the pin number to the pin order list

            if(i == 0) // if the pin is the first pin in the list
            {
                pin.isNextBindingPin = true; // set the pin as the first binding pin
            }
            else
            {
                pin.isNextBindingPin = false; 
            }

            bindingPins[i] = pin;
        }
    }
    #endregion

    public Pin CurrentPin;
    public void SelectPin(int direction)
    {

        if (direction == -1) // move "left" (down)
        {
            _pinIndex--; // decrement the pin index
        }
        else if (direction == 1) // move "right" (up)
        {
            _pinIndex++; // increment the pin index
        }

        _pinIndex = Mathf.Clamp(_pinIndex, 0, pins.Count - 1);
        //_pinIndex = _pinIndex < 0 ? pins.Count - 1 : _pinIndex >= pins.Count ? 0 : _pinIndex; // circluar looping
         CurrentPin = pins[_pinIndex];
        Debug.Log("Current pin: " + CurrentPin.pinNumber); // log the current pin number
    }

    // set the next binding pin in the sequence
    private void SetNextBindingPin()
    {
        // set the next binding pin in the sequence
        for (int i = 0; i < bindingPins.Count; i++)
        {
            if (bindingPins[i].isNextBindingPin)
            {
                // set this pin to false 
                Pin currentPin = bindingPins[i];
                currentPin.isSet = true; // set the pin as set
                currentPin.isNextBindingPin = false;
                bindingPins[i] = currentPin; // update the pin in the list

                if (i == bindingPins.Count - 1) // if this is the last pin in the list...
                {
                    CheckIfAllPinsSet(); // check if all pins are set
                }
                else // if there are more pins left in the list
                {
                    // set the next binding pin to true
                    Pin nextPin = bindingPins[i + 1];
                    nextPin.isNextBindingPin = true;
                    bindingPins[i + 1] = nextPin;
                    break;
                }
            }
        }
    }

    //check if all pins are set
    private bool CheckIfAllPinsSet()
    {
        // check if all pins are set
        for (int i = 0; i < bindingPins.Count; i++)
        {
            if (!bindingPins[i].isSet)
            {
                return false; // not all pins are set
            }
        }
        return true; // all pins are set
    }

    public bool ComparePinTension(float tension)
    {
        if(CheckForBindingPin())
        {
            // check if the tension is within the range of the pin
            if (tension >= CurrentPin.MinTension && tension <= CurrentPin.MaxTension)
            {
                Debug.Log("tension is in range!");
                return true; // tension is within range
            }
        }

        return false; // tension is not within range
    }

    public bool CheckForBindingPin()
    {
        // check if the current pin is a binding pin
        for (int i = 0; i < bindingPins.Count; i++)
        {
            if (CurrentPin.pinNumber == bindingPins[i].pinNumber)
            {
                if (bindingPins[i].isNextBindingPin)
                {
                    //Debug.Log("Current pin is a binding pin!");
                    return true; // current pin is a binding pin
                }
            }
        }
        return false; // current pin is not a binding pin
    }

    #endregion

        #region rotation logic (tools)

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
    private void CheckForPick()
    {
        if (!_pickInLock)
        {
            if (pick.CheckAlignment())
            {
                _pickInLock = true;
            }
            else _pickInLock = false;
        }
        else
        {
            // if the pick has not been parented to the lock
            if (!PickHasBeenParented)
            {
                // parent the pick to the lock
                pick.transform.position = PickParentedPos.position;
                pick.transform.SetParent(PickParentedPos);
                // set the position of the pick to the position of the lock
                // set the rotation of the pick to the rotation of the lock
                PickHasBeenParented = true;

                // set range of ideal rotation for the tension wrench (based on SO data)


                // set range of ideal amount of tension applied (based on SO data)

            }
            
        }
    }
    #endregion
}
