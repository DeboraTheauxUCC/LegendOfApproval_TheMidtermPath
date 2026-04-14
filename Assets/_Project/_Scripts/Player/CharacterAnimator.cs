using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CharacterAnimator : MonoBehaviour
{
    //referencia a componente de animación.
    public Animator animatorComponent;

    private void HandleBackwardsAnimation(bool state)
    {
        animatorComponent.SetBool("isBackwards", state); // activo o cancelo animacion reversa
    }

    private void HandleAttackAnimation()
    {
        animatorComponent.SetTrigger("attack"); // activo la animación de ataque.
    }

    private void HandleBlockingAnimation(bool state) //defensa
    {
        // activo la animación de bloquear. Configurando el parámetro en true.
        animatorComponent.SetBool("isBlocking", state);
    }

    private void HandleMovementAnimation(Vector3 movementVelocity)
    {
        // actualizar el parámetro de velocidad en la animación para diferenciar entre caminar y correr
        animatorComponent.SetFloat("movementSpeed", movementVelocity.magnitude);
    }

}
