using UnityEngine;

[CreateAssetMenu(fileName = "LockData", menuName = "Scriptable Objects/LockData")]
public class LockData : ScriptableObject
{
    [Tooltip("Consider difficulty when setting this number")]public int pinCount; // number of pins in the lock

    public float minWrenchTension; // minimum amount of tension required to rotate the lock
    public float maxWrenchTension; // maximum (safe) amount of tension required to rotate the lock

    public enum Difficulty
    {
        Easy,
        Medium,
        Hard
    }
    public Difficulty difficulty;
}
