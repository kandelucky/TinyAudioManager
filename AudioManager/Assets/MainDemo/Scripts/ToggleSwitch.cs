using UnityEngine;
using UnityEngine.UI;

public class ToggleSwitch : MonoBehaviour
{
    public Toggle targetToggle;
    public Text changeTxt;

    void OnEnable()
    {
        Image targetImage = targetToggle.targetGraphic as Image;
        if (targetToggle.isOn)
        {
            targetImage.enabled = false;
            changeTxt.text = "is ON";
        }
        else
        {
            targetImage.enabled = true;
            changeTxt.text = "is OFF";
        }
        targetToggle.toggleTransition = Toggle.ToggleTransition.None;
        targetToggle.onValueChanged.AddListener(OnTargetToggleValueChanged);
    }

    void OnTargetToggleValueChanged(bool newValue)
    {
        Image targetImage = targetToggle.targetGraphic as Image;
        if (targetImage != null)
        {
            if (newValue)
            {
                targetImage.enabled = false;
                changeTxt.text = "is ON";
            }
            else
            {
                targetImage.enabled = true;
                changeTxt.text = "is OFF";
            }
        }
    }
}
