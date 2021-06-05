// this script help You to save slider value only when - slider value change is ended
using UnityEngine;
using UnityEngine.EventSystems;
public class SliderDrag : MonoBehaviour, IPointerUpHandler
{
    public AllOptAudioManager audioManager;
    public string thisSliderName;
    public void OnPointerUp(PointerEventData eventData)
    {
        audioManager.SaveSlidersVolume(thisSliderName);
    }
}
