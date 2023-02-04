using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class KeypassTaskAction : TaskAction
{
    [SerializeField, Required] private KeypassBehaviour keypassBehaviour;
    [SerializeField, Required] private IntSequenceSO validSequenceSO;
    [ShowInInspector, ReadOnly] private bool isCompleted;

    private void Awake()
    {
        keypassBehaviour.OnSubmit += OnSubmit;
        if (validSequenceSO.Value.Count != keypassBehaviour.CombinationNumbersCount)
        {
            Debug.LogError($"{this.gameObject.GetHierarchyPath()} {nameof(KeypassTaskAction)} and {nameof(KeypassBehaviour)} has different valid sequence lenghts. " +
                $"{validSequenceSO.Value.Count} and {keypassBehaviour.CombinationNumbersCount}");
        }
        

    }
    private void OnDestroy()
    {
        keypassBehaviour.OnSubmit -= OnSubmit;
    }


    [Button]
    public void ResetIsCompleted()
    {
        isCompleted = false;
    }
    public override bool IsCompleted()
    {
        return isCompleted;
    }



    private void OnSubmit()
    {
        if (keypassBehaviour.CombinationInput.ItemsSequenceEqual(validSequenceSO.Value))
        {
            isCompleted = true;
        }

    }
}
