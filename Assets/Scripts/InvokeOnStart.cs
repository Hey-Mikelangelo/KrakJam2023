public class InvokeOnStart : UnityEngine.MonoBehaviour
{
    public UnityEngine.Events.UnityEvent OnStart = new();
    private void Start()
    {
        OnStart.Invoke();
    }
}