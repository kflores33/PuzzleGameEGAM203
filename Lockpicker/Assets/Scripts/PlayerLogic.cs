using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.UI;

// gather player input to be tested against lock logic script

public class PlayerLogic : MonoBehaviour
{
    // reference to lock logic script
    // reference to player input (mouse, keyboard, etc.)
    public Slider Slider;

    LockLogic _lockLogic; // reference to lock logic script

    private float _tension;
    public float tension;

    public float tensionMultiplier;
    public float multiplierHigh;
    public float multiplierLow;

    #region player input logic
    // gather player input (mouse, keyboard, etc.)
        // specifically, arrow keys to control the pick ( selected pin with left and right, up to tap the pin)
        // for the tension wrench, use a and d to find initial rotation, space to apply tension (during qte?)

    private void Start()
    {
       _lockLogic = FindFirstObjectByType<LockLogic>();
    }
    private void Update()
    {
        if(_lockLogic.canTryToSetPin)
        {
            GetInputPick(); // get player input for the pick
        }
    }

    void GetInputPick()
    {
        // reset tension variable to 0
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {        
            tension = 0;
            // move pick to the right
            _lockLogic.SelectPin(1); // move to the right
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            tension = 0;
            // move pick to the left
            _lockLogic.SelectPin(-1); // move to the left
        }

        if (_lockLogic.CheckForSetPin()) // check if the selected pin is set
        {
            tension = 0; // reset tension variable to 0
        }
        else if (Input.GetButtonDown("Vertical"))
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                // apply tension to the selected pin
                if (_lockLogic.CheckForBindingPin()) // check if the selected pin is a binding pin
                {
                    Debug.Log("binding pin found!");
                    tension += (tensionMultiplier * multiplierLow); // increase tension variable
                    _lockLogic.ComparePinTension(tension); // test player input against lock logic script
                }
                else // if the selected pin is not a binding pin
                {
                    Debug.Log("just a normal pin");
                    tension += (tensionMultiplier * multiplierHigh); // increase tension variable
                }
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (_lockLogic.CheckForBindingPin()) // check if the selected pin is a binding pin
                {
                    tension -= (tensionMultiplier * multiplierHigh); // increase tension variable
                    _lockLogic.ComparePinTension(tension); // test player input against lock logic script
                }
                // release tension on the selected pin
                else tension -= (tensionMultiplier * multiplierHigh); // decrease tension variable
            }
        } 

        // if the player is not holding the up key, decrease tension variable
        if (tension > 0)
        {
            tension -= Time.deltaTime * (tensionMultiplier); // decrease tension variable
        }

        if (tension > 1)
        {
            tension = 1; // clamp tension variable to 1
        }
        else if (tension < 0)
        {
            tension = 0; // clamp tension variable to 0
        }

        Slider.value = tension; // update the slider value to reflect the tension variable
    }

    void GetInputWrench()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {

        }
        if (Input.GetKeyDown(KeyCode.D))
        {

        }
    }
    #endregion

    #region feedback logic
    // provide feedback to the player (visual, audio, etc.)

    // if player sets the binding pin correctly, provide feedback
    // else, do nothing

    // if player applies too much force to pin, provide feedback to communicate the lock is now stuck and needs to be reset

    // if player sets the tension wrench correctly, provide feedback
    // else, reset the lock and all pins to their original state
    #endregion
}
