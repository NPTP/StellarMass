using System;
using UnityEngine.InputSystem;

namespace StellarMass.InputManagement.MapInstances
{
    // [Serializable]
    // MARKER.ClassDefinition.Start
    // public class MapInstanceTemplate : MapInstance
        // MARKER.CommaAndInterfaceName.Start
        // , InputActions.ITemplateActions
        // MARKER.CommaAndInterfaceName.End
    // {
        // MARKER.ActionsStructProperty.Start
        // private InputActions.TemplateActions TemplateActions { get; }
        // MARKER.ActionsStructProperty.End

        // MARKER.EventFields.Start
        // public event Action<ActionState, Vector2> @@OnMove;
        // public event Action<ActionState, float> @OnTurn;
        // public event Action<ActionState> @OnShoot;
        // MARKER.EventFields.End
        
        // MARKER.Constructor.Start
        // public Template(InputActions.TemplateActions templateActions)
        // {
        // TemplateActions = gameplayActions;
        // TemplateActions.AddCallbacks(this);
        // ActionMap = TemplateActions.Get();
        // }
        // MARKER.Constructor.End

        // MARKER.AddCallbacks.Start
        // protected sealed override void AddCallbacks() => InputActions.GameplayActions.AddCallbacks(this);
        // MARKER.AddCallbacks.End
        // MARKER.RemoveCallbacks.Start
        // protected sealed override void RemoveCallbacks() => InputActions.GameplayActions.RemoveCallbacks(this);
        // MARKER.RemoveCallbacks.End
        
        // MARKER.InterfaceMethods.Start
        // void InputActions.ITemplateActions.OnMove(InputAction.CallbackContext context) => OnMove?.Invoke(GetActionState(context), context.ReadValue<Vector2>());
        // void InputActions.ITemplateActions.OnTurn(InputAction.CallbackContext context) => OnTurn?.Invoke(GetActionState(context), context.ReadValue<float>());
        // void InputActions.ITemplateActions.OnShoot(InputAction.CallbackContext context) => OnShoot?.Invoke(GetActionState(context));
        // MARKER.InterfaceMethods.End
    // }
    // MARKER.ClassDefinition.End
}