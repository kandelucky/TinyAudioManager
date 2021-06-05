using UnityEngine;

public class TimerAnimEvent : MonoBehaviour
{
    public MinOptSceneManager mainSript;

    public void PlayTick()
    {
        MinOptAudioManager.InstanceMin.PlaySound2D("Tick");
    }
    public void StartTimer()
    {
        mainSript.CallTimer();
    }
}
