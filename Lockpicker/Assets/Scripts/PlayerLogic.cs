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

    public void SelectPin()
    {
        // use arrow keys to cycle through the pins
        

        // when player presses up key, apply tension to the selected pin 
        // holding the up key will increase tension variable, of which there is a randomly determined threshold
            // if held for too long, the pin will become stuck and the lock will reset
            // down key will decrease tension variable
            // holding the button will only increase after brief delay

        // tension slowly decreases over time, starts decreasing after a brief delay

        // tapping the key will apply a small amount of tension
    }

    private void Start()
    {
       _lockLogic = FindFirstObjectByType<LockLogic>();
    }
    private void Update()
    {
        if (Input.GetButtonDown("Horizontal"))
        {
            tension = 0; // reset tension variable to 0
            if (Input.GetAxis("Horizontal") > 0)
            {
                // move pick to the right
                _lockLogic.SelectPin(1); // move to the right
            }
            else if (Input.GetAxis("Horizontal") < 0)
            {
                // move pick to the left
                _lockLogic.SelectPin(-1); // move to the left
            }
        }

        if (Input.GetButtonDown("Vertical"))
        {
            if (Input.GetAxis("Vertical") > 0)
            {
                // apply tension to the selected pin
                if (_lockLogic.CheckForBindingPin()) // check if the selected pin is a binding pin
                {
                    Debug.Log("binding pin found!");
                    tension += (tensionMultiplier * multiplierHigh); // increase tension variable
                    _lockLogic.ComparePinTension(tension); // test player input against lock logic script
                }
                else // if the selected pin is not a binding pin
                {
                    Debug.Log("just a normal pin");
                    tension += (tensionMultiplier * multiplierLow); // increase tension variable
                }
            }
            else if (Input.GetAxis("Vertical") < 0)
            {
                if (_lockLogic.CheckForBindingPin()) // check if the selected pin is a binding pin
                {
                    tension -= (tensionMultiplier * multiplierHigh); // increase tension variable
                    _lockLogic.ComparePinTension(tension); // test player input against lock logic script
                }
                // release tension on the selected pin
                else tension -= (tensionMultiplier * multiplierLow); // decrease tension variable
            }

            if (tension > 1)
            {
                tension = 1; // clamp tension variable to 1
            }
            else if (tension < 0)
            {
                tension = 0; // clamp tension variable to 0
            }
            else
            {
                // if the player is not holding the up key, decrease tension variable
                if (tension > 0)
                {
                    tension -= Time.deltaTime * (tensionMultiplier); // decrease tension variable
                }
            }

            Slider.value = tension; // update the slider value to reflect the tension variable
        }
    }

    // test player input against lock logic script 
    // if player input is correct, unlock the lock
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
