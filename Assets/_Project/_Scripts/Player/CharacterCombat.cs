using UnityEngine;

public class CharacterCombat : MonoBehaviour
{
    [SerializeField] private InputReader inputReader;

    bool isBlocking;
    bool canAttack = true;

    private void OnEnable()
    {
        inputReader.OnAttackEvent += HandleAttack;
        inputReader.OnBlockStarted += HandleBlockStart;
        inputReader.OnBlockCanceled += HandleBlockCancel;
    }

    private void OnDisable()
    {
        inputReader.OnAttackEvent += HandleAttack;
        inputReader.OnBlockStarted += HandleBlockStart;
        inputReader.OnBlockCanceled += HandleBlockCancel;
    }

    void HandleBlockStart() // si presiono click derecho
    {
        // el jugador quiere que el personaje bloquee daño.
        isBlocking = true;
        // si estoy bloqueando daño, no puedo atacar.
        canAttack = false;
    }

    void HandleBlockCancel() // si suelto click derecho.
    {
        // el jugador ya no quiere que el personaje bloquee daño
        isBlocking = false;
        // si no estoy bloqueando, puedo atacar
        canAttack = true;
    }

    void HandleAttack() // si presiono click izquierdo.
    {
        // si no puedo atacar..
        if (!canAttack) return; // return significa no leer las siguientes lineas hasta el }, es decir termina ahi el método.
        // el if esta reducido asi que esto serïa "si puede atacar".
    }
}
