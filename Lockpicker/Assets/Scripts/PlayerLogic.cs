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
