using UnityEngine;

[CreateAssetMenu(fileName = "CharacterState", menuName = "  Game/CharacterState")]
public class CharacterState : ScriptableObject
{
    public enum MovementState
    {
        Walking,
        Running,
        NotRunning,
        Backward
    }

    public enum CombatState
    {
        Attacking,
        Blocking
    }

    private MovementState _movementState;
    private CombatState _combatState;

    public MovementState GetCurrentMovementState => _movementState; //propiedad con un get
    public CombatState GetCurrentCombatState => _combatState;

    public void SetMovementState(MovementState newState) => _movementState = newState; // un set
    public void SetCombatState(CombatState newState) => _combatState = newState;
}
