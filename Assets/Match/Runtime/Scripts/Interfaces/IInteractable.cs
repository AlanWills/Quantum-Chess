using Celeste.Components;

namespace QC.Match
{
    public interface IInteractable
    {
        bool IsInteractable(Instance instance);
        void SetInteractable(Instance instance, bool interactable);
    }
}