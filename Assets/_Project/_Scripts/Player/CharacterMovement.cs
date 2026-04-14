using System.ComponentModel;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CharacterMovement : MonoBehaviour
{
    [Header("Dependencies")]
    // clase que detecta los inputs del jugador.
    [SerializeField] private InputReader inputReader;
    [SerializeField] private CharacterState characterState;

    //referencia a componente de física.
    public Rigidbody rigidbodyComponent;
    // vector de movimiento en x,y,z su magnitud define la velocidad.
    Vector3 movementVelocity;
    //multiplicador del vector de movimiento.
    public float movementSpeed;
    // dirección Z positivo local de la cámara.
    Vector3 cameraForward;
    // dirección X positivo local de la cámara.
    Vector3 cameraRight;

    // Quaternion es un tipo de dato con 4 coordenadas, utilizado para rotaciones.
    // guarda el valor de rotación objetivo.
    Quaternion objectiveRotation;
    // valor de interpolación
    [Range(0, 1)] public float rotationSlerpSpeed;

    // variables lógicas que sirven como "bandera" de los inputs del jugador
    bool isSprinting;
    bool isBackwards;
    bool canMove = true;
    bool canRotate = true;

    Vector2 moveInput;

    void Awake()
    {
        // valor inicial de rotación: la actual.
        objectiveRotation = transform.rotation;
    }

    void OnEnable() // al activar el componente.
    {
        // suscripción a eventos de inputs del jugador.
        // lo que va a ocurrir cuando el evento ocurra.
        // evento se emite => método reacciona.
        inputReader.OnMoveEvent += HandleMove;
        inputReader.OnSprintStarted += HandleSprintStart;
        inputReader.OnSprintCanceled += HandleSprintCancel;
    }

    void OnDisable() // al desactivar el componente.
    {
        // cancelar la suscripción a eventos cuando el personaje no está activo
        // para evitar errores.
        inputReader.OnMoveEvent -= HandleMove;
        inputReader.OnSprintStarted -= HandleSprintStart;
        inputReader.OnSprintCanceled -= HandleSprintCancel;
    }

    // esto es un método que asigna un dato, escrito en una sola linea.
    // guarda el vector de dirección en la variable moveInput
    void HandleMove(Vector2 input) => moveInput = input; // si presiono WASD...

    void HandleSprintStart() // si presiono shift.
    {
        characterState.SetMovementState(CharacterState.MovementState.Running);
    }

    // método con una sola linea, reducido...
    // el jugador ya no quiere que el personaje corra.
    void HandleSprintCancel() => characterState.SetMovementState(CharacterState.MovementState.Walking);


    void Update() // durante todo el tiempo del juego, en el que el componente está activo.
    {
        if (canMove) Move(); // si quiero moverme, calcular movimiento.
        if (canRotate) Rotate(); // si quiero rotar, calcular rotaion.
        if (isBlocking) Block(); // si quiero bloquear, bloquear.
    }

    void FixedUpdate() // en cada frame en el que se actualiza el motor de físicas
    {
        // actualizar la velocidad linear según el vector de movimiento calculado.
        rigidbodyComponent.linearVelocity = movementVelocity;
    }

    void Move() // calcular el movimiento, efectivamente mover el personaje hacia una dirección.
    {
        // al presionar A o D moveInput.x puede ser -1, 0, 1 es decir izquierda, nada, derecha. Guardo este valor en el vector de movimiento, en la coordenada "x".
        movementVelocity.x = moveInput.x; 
        // al presionar W o S moveInput.y puede ser -1, 0, 1 es decir abajo, nada, arriba. Prestar atención que, como es una escena 3D guardamos el valor de la coordenada "y" en la coordenada "z" del vector de movimiento, esto se traduce a atras, nada o adelante.
        movementVelocity.z = moveInput.y; 

        // el vector de movementVelocity siempre tiene y = 0, ya que no estoy calculando un salto o subir escalera ni nada que tenga que ver con altura.

        cameraForward = Camera.main.transform.forward; // guardo el eje z de la cámara.
        cameraRight = Camera.main.transform.right; // guardo el eje x de la cámara.
        cameraForward.y = 0; // no me intereza la influencia de la altura de la cámara, mantengo en 0.
        cameraRight.y = 0; // lo mismo.
        cameraForward.Normalize(); // convierto su magnitud en 1, porque solo me interesa la dirección.
        cameraRight.Normalize(); // lo mismo.

        // obtenemos el forward y el right de la cámara para obtener la base del sistema de coordenadas de la cámara.
        // y esto nos va a servir para luego calcular el movimiento del personaje desde el punto de vista de la cámara.

        // calculo el movimiento, usando como "pivot" u "origen" los ejes cartesianos de la cámara.
        // esto es una transformación lineal. 
        movementVelocity = cameraForward * movementVelocity.z + cameraRight * movementVelocity.x;

        if (movementVelocity.sqrMagnitude > 1f) // si la magnitud es mayor a 1 (por ejemplo en diagonales... hipotenusa...)
            movementVelocity.Normalize(); // normalizar para mantener magnitud en 1 y no tener mayor velocidad al ir en diagonal.

        // esto es un if que asigna un valor en sus ternas, pero simplificado en una linea.
        // se lee asi: si quiero correr y no estoy yendo en reversa, entonces duplico la velicidad => speed = movementSpeed * 2.
        // si la condicion anterior no se cumple, entonces speed = movementSpeed, mantengo la velocidad.
        // resultado = (condicion, predicado lógico a evaluar) ? valor si true : valor si false.
        float speed = (isSprinting && !isBackwards) ? movementSpeed * 2 : movementSpeed; 

        // se multiplica la velocidad actual por la velocidad, que puede ser el valor normal o duplicado.
        movementVelocity *= speed; //*= es una multiplicacion acumulada, es lo mismo que movementVelocity =  movementVelocity * speed;

    }


    void Rotate()
    {
        if (movementVelocity == Vector3.zero) return; // si no me estoy moviendo, no calculo rotaciones.

        // producto escalar: multiplico dos vectores y obtengo un valor escalar.
        // para calcular el angulo entre la dirección "z" positiva de la cámara y la dirección de movimiento
        float dotForward = Vector3.Dot(movementVelocity.normalized, cameraForward);

        // la idea es que si estoy corriendo puedo ir para cualquier direccion y el personaje se gira mirando hacia dicha direccion
        // pero si no estoy corriendo y quiero ir en reversa, el personaje no rota.

        if (!isSprinting) // si no quiero correr
        {
            if (!isBackwards && dotForward < -0.7f) // no estoy en reversa y el ángulo entre la cámara y mi mov es menor a -0.7
            {
                isBackwards = true; // eso significa que si quiero ir en reversa
            }
            else if (isBackwards && dotForward > 0f) // sino, si estoy yendo en reversa y el ángulo es mayor a cero
            {
                isBackwards = false; // ya no estoy yendo en reversa
            }
        }

        // Obtengo e quaternion de rotación que me permite girar el personaje hacia la posición o dirección de movimiento.
        objectiveRotation = Quaternion.LookRotation(movementVelocity);

        if (isBackwards) // si estoy yendo hacia atras en reversa
        {
            objectiveRotation = Quaternion.Euler(
                objectiveRotation.eulerAngles.x,
                objectiveRotation.eulerAngles.y + 180f,
                objectiveRotation.eulerAngles.z); // es una rotacion tipo auto haciendo marcha atras, con un flip horizontal (180 grados en y)
        }

        transform.rotation = Quaternion.Slerp(
            transform.rotation, objectiveRotation, rotationSlerpSpeed); //rotacion con una interpolacion esferica a una velocidad de rotationSpeed... que? nada, rota.. pero de manera suave, fluida. 
    }

    void Block()
    {
        if (movementVelocity.sqrMagnitude > 0.01f) //  al bloquear reducir de a poquito la velocidad
        {
            movementVelocity *= 0.9f;
        }
        else if (movementVelocity != Vector3.zero)
        {
            movementVelocity = Vector3.zero;
        }
    }
}