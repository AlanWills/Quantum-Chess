using System.Runtime.CompilerServices;
using Celeste.Components;

namespace QC.Match
{
    public interface IInteractable
    {
        bool IsInteractable(Instance instance);
        void SetInteractable(Instance instance, bool interactable);
    }

    public static class InteractableExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInteractable(this InterfaceHandle<IInteractable> interactable)
        {
            return interactable.iFace.IsInteractable(interactable.instance);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetInteractable(this InterfaceHandle<IInteractable> interactable, bool isInteractable)
        {
            interactable.iFace.SetInteractable(interactable.instance, isInteractable);
        }
    }
}