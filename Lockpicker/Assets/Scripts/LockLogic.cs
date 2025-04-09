using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public struct Pin
{
    public int pinNumber; // Pin number (determined by order in list)

    public bool isNextBindingPin; // is the pin the next binding pin in the sequence
    public bool isSet; // is the pin set

    public bool isSelected; // is the pin selected by the player

    public float MinTension; // minimum amount of tension required to set the pin
    public float MaxTension;

    public float LastTension; // last amount of tension applied to the pin
}

public class LockLogic : MonoBehaviour
{
    public LockData lockData; // reference to scriptable object

    public Transform WrenchParentedPos;
    public Transform PickParentedPos;

    TensionWrench wrench;
    Pick pick;

    bool _wrenchInLock; // is the tension wrench in the lock
    [HideInInspector] public bool WrenchHasBeenParented; // has the tension wrench been parented to the lock

    bool _pickInLock; // is the pick in the lock
    [HideInInspector] public bool PickHasBeenParented; // has the pick been parented to the lock

    public bool canTryToSetPin; // can the player try to set the pin

    public ParticleSystem winParticles;
    public GameObject winScreen;
    public GameObject pickTensionGauge;

    public TMP_Text pinCount;
    int _setPins = 0; // number of pins set by the player

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

        pickTensionGauge.SetActive(false);

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
                canTryToSetPin = true; // player can try to set the pin
                pickTensionGauge.SetActive(true);
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
        float tensionRange = 0f; // fixed range of tension

        if (lockData.difficulty == LockData.Difficulty.Easy)
        {
            minTension = UnityEngine.Random.Range(0.2f, 0.35f);
            tensionRange = 0.4f;
        }
        else if (lockData.difficulty == LockData.Difficulty.Medium)
        {
            minTension = UnityEngine.Random.Range(0.25f, 0.5f);
            tensionRange = 0.3f;
        }
        else if (lockData.difficulty == LockData.Difficulty.Hard)
        {
            minTension = UnityEngine.Random.Range(0.35f, 0.60f);
            tensionRange = 0.25f;
        }

        maxTension = minTension + tensionRange;

        return Tuple.Create(minTension, maxTension);
    }

    private void SetPinOrder()
    {
        bindingPins = new List<Pin>(pins); // create a copy of the pins list to shuffle

        // shuffle order of the pins
        bindingPins.Shuffle();

        for (int i = 0; i < bindingPins.Count; i++)
        {
            Pin pin = bindingPins[i]; // get the pin from the shuffled bindingPins list

            if (i == 0) // if the pin is the first pin in the list
            {
                pin.isNextBindingPin = true; // set the pin as the first binding pin
                Debug.Log("First binding pin: " + pin.pinNumber); // log the first binding pin
            }
            else
            {
                pin.isNextBindingPin = false;
            }

            bindingPins[i] = pin; // update the pin in the bindingPins list
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

        float pickPos = (0.5f/pins.Count) * direction;

        if(_pinIndex > -1 && _pinIndex < pins.Count) pick.ChangePos(pickPos);

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
                    if (CheckIfAllPinsSet())
                    {
                        Debug.Log("All pins are set!"); // log that all pins are set
                        Instantiate(winParticles); // play the win particles
                        winScreen.SetActive(true); // show the win screen
                    }
                }
                else // if there are more pins left in the list
                {
                    // set the next binding pin to true
                    Pin nextPin = bindingPins[i + 1];
                    nextPin.isNextBindingPin = true;

                    Debug.Log("Next binding pin: " + nextPin.pinNumber); // log the next binding pin

                    bindingPins[i + 1] = nextPin;
                    break;
                }
            }
        }
    }

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

                if(SetPinCoroutine == null)
                {
                    SetPinCoroutine = StartCoroutine(SetPinIE(tension)); // start the coroutine to set the pin
                }

                return true; // tension is within range
            }
        }

        if (SetPinCoroutine != null)
        {
            StopCoroutine(SetPinCoroutine); // stop the coroutine if the tension is not within range
            SetPinCoroutine = null; // reset the coroutine
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
    public bool CheckForSetPin()
    {
        // check if the current pin is set
        for (int i = 0; i < bindingPins.Count; i++)
        {
            if (CurrentPin.pinNumber == bindingPins[i].pinNumber)
            {
                if (bindingPins[i].isSet)
                {
                    return true; // current pin is set
                }
            }
        }
        return false; // current pin is not set
    }

    public void SetPin(float tension)
    {
        // set the pin as set
        for (int i = 0; i < bindingPins.Count; i++)
        {
            if (CurrentPin.pinNumber == bindingPins[i].pinNumber)
            {
                Pin currentPin = bindingPins[i];
                currentPin.isSet = true; // set the pin as set
                currentPin.LastTension = tension;

                bindingPins[i] = currentPin; // update the pin in the list
                SetNextBindingPin(); // set the next binding pin in the sequence
                break;
            }
        }

        _setPins++; // increment the number of set pins
        pinCount.text = $"{_setPins}/{lockData.pinCount}" ; // update the pin count text
    }

    public void ResetPins()
    {
        // reset all pins to their original state
        for (int i = 0; i < bindingPins.Count; i++)
        {
            Pin currentPin = bindingPins[i];
            currentPin.isSet = false; // set the pin as not set

            if (i == 0) // if the pin is the first pin in the list
            {
                currentPin.isNextBindingPin = true; // set the pin as the first binding pin
                Debug.Log("First binding pin: " + currentPin.pinNumber); // log the first binding pin
            }
            else
            {
                currentPin.isNextBindingPin = false;
            }

            bindingPins[i] = currentPin; // update the pin in the list
        }
    }

    public Coroutine SetPinCoroutine;
    public IEnumerator SetPinIE(float tension)
    {
        yield return new WaitForSeconds(1.5f);

        SetPin(tension); // set the pin
        StopCoroutine(SetPinCoroutine); // stop the coroutine
        SetPinCoroutine = null; // reset the coroutine
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
                wrench.transform.localRotation = Quaternion.identity;

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
                pick.transform.localRotation = Quaternion.identity;

                PickHasBeenParented = true;
            }
        }
    }
    #endregion
}
