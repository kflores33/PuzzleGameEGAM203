using System.Diagnostics.CodeAnalysis;
using UnityEngine;

// gather player input to be tested against lock logic script

public class PlayerLogic : MonoBehaviour
{
    // reference to lock logic script
    // reference to player input (mouse, keyboard, etc.)

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
