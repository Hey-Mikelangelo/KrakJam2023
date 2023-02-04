using Sirenix.OdinInspector;
using UnityEngine;

public class TaskActionCompletionListener : MonoBehaviour
{
    [SerializeField, Required] private TaskAction taskAction;
    public UltEvent OnCompleted = new();
    private void Update()
    {
        if(taskAction.IsCompleted())
        {
            OnCompleted.Invoke();
            this.enabled = false;
        }
    }
}
