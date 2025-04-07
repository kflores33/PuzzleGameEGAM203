using UnityEngine;
using UnityEngine.UI;

public class TensionIndicator : MonoBehaviour
{
    public Image sliderHandle;
    public Image sliderFill;
    public Image sliderBack;

    public Color handleColorCorrect;
    public Color fillColorCorrect;

    public Color handleColorClose;
    public Color fillColorClose;

    public Color handleColorDefault;
    public Color fillColorDefault;
    public Color backColorDefault;

    public Color handleColorDanger;
    public Color fillColorDanger;

    public Color handleColorIncorrect;
    public Color fillColorIncorrect;

    public Color handleColorDisabled;
    public Color fillColorDisabled;
    public Color backColorDisabled;

    float _tensionThreshold;
    float _tensionThresholdMax;

    PlayerLogic _playerLogic;
    LockLogic _lockLogic;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _playerLogic = FindFirstObjectByType<PlayerLogic>();
        _lockLogic = FindFirstObjectByType<LockLogic>();

        if(_lockLogic.lockData.difficulty == LockData.Difficulty.Easy)
        {
            _tensionThreshold = 0.15f;
        }
        else if (_lockLogic.lockData.difficulty == LockData.Difficulty.Medium)
        {
            _tensionThreshold = 0.1f;
        }
        else if (_lockLogic.lockData.difficulty == LockData.Difficulty.Hard)
        {
            _tensionThreshold = 0.05f;
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (_lockLogic.CheckForSetPin())
        {
            sliderHandle.color = handleColorDisabled;
            sliderFill.color = fillColorDisabled;
            sliderBack.color = backColorDisabled;
        }
        else if (!_lockLogic.CheckForBindingPin())
        {
            sliderHandle.color = handleColorDefault;
            sliderFill.color = fillColorDefault;
            sliderBack.color = backColorDefault;
        }
        
        if (_lockLogic.CheckForBindingPin() && !_lockLogic.CheckForSetPin())
        {
            if (sliderBack.color != backColorDefault)
            {
                sliderHandle.color = handleColorDefault; // reset the color of the slider handle to default
                sliderFill.color = fillColorDefault; // reset the color of the slider fill to default
                sliderBack.color = backColorDefault; // reset the color of the slider back to default
            }

            float minTension = _lockLogic.CurrentPin.MinTension;
            float maxTension = _lockLogic.CurrentPin.MaxTension;
            float tension = _playerLogic.tension;

            _tensionThresholdMax = 1 - maxTension; // the maximum amount of tension that can be applied to the lock

            if (tension > minTension - _tensionThreshold && tension < minTension)
            {
                // the lerp factor is between 0 (tension threshold) and 1 (minTension)
                float lerpFactor = (tension - (minTension - _tensionThreshold)) /* This part calculates how close tension is to minTension */ / _tensionThreshold /* This part normalizes the values from 0 to 1 */; 
                sliderHandle.color = Color.Lerp(handleColorDefault, handleColorClose, lerpFactor);
                sliderFill.color = Color.Lerp(fillColorDefault, fillColorClose, lerpFactor);
            }
            else if (tension > minTension && tension < maxTension)
            {
                sliderHandle.color = handleColorCorrect;
                sliderFill.color = fillColorCorrect;
            }
            else if (tension > maxTension && tension < 1)
            {
                float lerpFactor = (tension - maxTension) / _tensionThresholdMax;
                sliderHandle.color = Color.Lerp(handleColorDanger, handleColorIncorrect, lerpFactor);
                sliderFill.color = Color.Lerp(fillColorDanger, fillColorIncorrect, lerpFactor);
            }
            else
            {
                sliderHandle.color = handleColorDefault;
                sliderFill.color = fillColorDefault;
            }
        }
    }

    // check if the selected pin is a binding pin
    // determine how close tension is to the correct value
    // as tension increases, interpolate the color of the slider handle and fill
    // probably have a threshold that's like...a little less & a little more than the correct value
}
