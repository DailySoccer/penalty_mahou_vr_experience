using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public enum Movement{Libre, Objetivo, Direccion};

	// Distancia a la posicion deseada a partir de la cual va a colocarse en ella, en vez de moverse con una velocidad hacia ella
	public float thresholdDistanciaDebugg;

	// Mirar libremente o en una direccion fija
	public Movement Movimiento;

	// Objetivo hacia el que mirar
	public Transform targetToLook;
	private Vector3 positionToLook;

	// Objetivo hacia el que moverse
	public Transform targetToMove;

	// Distancia en el plano horizontal al objetivo al que moverse
	public float distance = 10.0f;

	// Altura de la camara respecto al objetivo al que moverse
	public float height = 5.0f;

	// Damping
	public float heightDamping = 2.0f;
	public float rotationDamping = 3.0f;
	public float distanceDamping = 2.0f;

	// Inclinacion de la camara
	public float inclinacion = 30.0f;

	// Distancia en la direccion camara-objetivo en el que se situara la zona de control (area verde)
	public float offsetControl;

	// Velocidad, incremento/decremento por segundo, velocidad maxima, velocidad minima, velocidad con la que se empieza a mover desde el reposo y velocidad minima para pararse en un autokick
	public float speed;
	public float speedIncrease;
	public float maxSpeed;
	public float minSpeed;
	public float startMovingSpeed;
	public float stopMovingSpeed;

	[HideInInspector] public float direccionPatada;
	[HideInInspector] public bool getBalon = true;
	[HideInInspector] public bool increaseSpeed = true;

	// Variables para la camara "intermedia"
	private float currentRot;
	private float targetLookRot;
	private float targetMoveRot;
	private float wantedRot;

	[HideInInspector] public bool entered = false;
	private float startStop;
	
	
	void Start () {
		// Seteamos la posicion inicial de la camara
		if(Movimiento == Movement.Libre)
			positionToLook = targetToMove.transform.position;
		else if(Movimiento == Movement.Direccion)
			positionToLook = targetToLook.position;
		else
			positionToLook = targetToLook.position;

		// Hacemos que empieze a moverse
		increaseSpeed = true;

		// Hacemos que el rigidbody no rote
		GetComponent<Rigidbody>().freezeRotation = true;

		// Direcciones de movimiento y vista para setear la direccion de patada inicial
		Vector2 movePos = Vector2.zero;
		movePos.x = targetToMove.position.x;
		movePos.y = targetToMove.position.z;

		Vector2 lookPos = Vector2.zero;
		lookPos.x = positionToLook.x;
		lookPos.y = positionToLook.z;

		if((lookPos - movePos).normalized.y < 0){
			direccionPatada = 180 + Mathf.Rad2Deg * Mathf.Atan((lookPos - movePos).normalized.x / (lookPos - movePos).normalized.y);
		}
		else{
			direccionPatada = Mathf.Rad2Deg * Mathf.Atan((lookPos - movePos).normalized.x / (lookPos - movePos).normalized.y);
		}
		transform.LookAt (positionToLook);

	}
	
	
	void FixedUpdate () {

		// Esta quieto
		if (!getBalon)
			return;

		// Se ha alcanzado el objetivo/final de nivel
		if (Vector3.Distance(transform.position, targetToMove.position) < distance) {//GameObject.FindWithTag ("Player").GetComponent<PlayerController> ().objectiveReached){
			GetComponent<Rigidbody>().velocity = Vector3.zero;
			getBalon = false;
			return;
		}

		// Dependiendo del estado miramos al objetivo o hacia la direccion que toque
		if(Mathf.Abs(targetToMove.GetComponent<Rigidbody>().velocity.x) > 0.01f && Mathf.Abs(targetToMove.GetComponent<Rigidbody>().velocity.z) > 0.01f){
			if(Movimiento == Movement.Libre)
				positionToLook = GameObject.FindWithTag ("Player").transform.position + new Vector3 ((GameObject.FindWithTag ("Player").GetComponent<Rigidbody>().velocity).normalized.x, 0.0f, (GameObject.FindWithTag ("Player").GetComponent<Rigidbody>().velocity).normalized.z) * 40.0f;
			else if(Movimiento == Movement.Direccion)
				positionToLook = GameObject.FindWithTag ("Player").transform.position + 10.0f * transform.forward;
			else
				positionToLook = targetToLook.transform.position;
		}

		// Posicion del objetivo hacia el que nos movemos en el plano x-z
		Vector2 targetPos;
		targetPos = new Vector2 (targetToMove.position.x, targetToMove.position.z);
		
		// Posicion del objetivo en el plano x-z
		Vector2 lookPos = new Vector2 (positionToLook.x, positionToLook.z);
		
		// Posicion actual en el plano x-z
		Vector2 currentPos = new Vector2 (transform.position.x, transform.position.z);
		
		// Posicion deseada  en el plano x-z
		float wantedX = (targetPos.x + (targetPos - lookPos).normalized.x * distance);
		float wantedY = (targetPos.y + (targetPos - lookPos).normalized.y * distance);
		Vector2 wantedPos = new Vector2 (wantedX, wantedY);
		
		// Direccion hacia la que queremos ir
		Vector2 directionToGo = (wantedPos - currentPos).normalized;

		// Control incremento/decremento de la velocidad
		speedControl (increaseSpeed);

		// Si estas muy cerca de la posicion deseada, te colocas en ella en vez de aplicar velocidad (asi evitas el pasarte y empezar a "temblar")
		if((wantedPos - currentPos).magnitude < thresholdDistanciaDebugg){
			transform.position = new Vector3(wantedPos.x, transform.position.y, wantedPos.y);
			GetComponent<Rigidbody>().velocity = Vector3.zero;
			
			if(!entered){ entered = true; startStop = Time.time + 0.5f; }
			if(Time.time > startStop && entered){ increaseSpeed = false; }
		}
		// Aplicamos la direccion y velocidad calculadas
		else{
			entered = false;
			GetComponent<Rigidbody>().velocity = new Vector3(directionToGo.x * speed * Time.deltaTime, 0.0f, directionToGo.y * speed * Time.deltaTime);
		}

		// Si la vista es en modo direccion mira en ese sentido
		if(Movimiento == Movement.Direccion){
			Vector3 aux = transform.rotation.eulerAngles;
			aux.x = inclinacion;
			transform.eulerAngles = aux;
			return;
		}
		// Si el balon no esta en la zona de control
		else if( ( ( new Vector2(transform.position.x, transform.position.z) +
		            offsetControl * ( new Vector2(transform.position.x, transform.position.z) - new Vector2(targetToMove.position.x, targetToMove.position.z ) ).normalized )
		          - new Vector2(targetToMove.position.x, targetToMove.position.z) ).magnitude > 1){

			if(Mathf.Abs(targetToMove.GetComponent<Rigidbody>().velocity.x) > 0.01f && Mathf.Abs(targetToMove.GetComponent<Rigidbody>().velocity.z) > 0.01f){
				Quaternion rotation = Quaternion.LookRotation( (targetToLook.position * 0.5f +
				                                                (targetToMove.position + targetToMove.GetComponent<Rigidbody>().velocity.normalized * (positionToLook-transform.position).magnitude) * 0.5f)
				                                              - transform.position );
				Vector3 eulerRot = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * rotationDamping).eulerAngles;
				transform.eulerAngles = new Vector3 (inclinacion, eulerRot.y, 0.0f);
			}
			else{
				transform.LookAt(targetToMove.position);
				transform.eulerAngles = new Vector3(inclinacion, transform.eulerAngles.y, 0.0f);
			}
		}
		// Si el balon esta en la zona de control
		else{

			if(Mathf.Abs(targetToMove.GetComponent<Rigidbody>().velocity.x) > 0.01f && Mathf.Abs(targetToMove.GetComponent<Rigidbody>().velocity.z) > 0.01f){
				Quaternion rotation = Quaternion.LookRotation( (positionToLook * 0.9f +
				                                                (targetToMove.position + targetToMove.GetComponent<Rigidbody>().velocity.normalized * (positionToLook-transform.position).magnitude) * 0.1f)
				                                              - transform.position );
				Vector3 eulerRot = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * rotationDamping).eulerAngles;
				transform.eulerAngles = new Vector3 (inclinacion, eulerRot.y, 0.0f);
			}
			else{
				Quaternion rotation = Quaternion.LookRotation( targetToLook.position - transform.position);
				Vector3 eulerRot = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * rotationDamping).eulerAngles;
				transform.eulerAngles = new Vector3 (inclinacion, eulerRot.y, 0.0f);
			}
		}
	}

	public void speedControl(bool increase){
		// Control de incremento/decremento de la velocidad
		if (increase) {
			if(speed < startMovingSpeed){
				speed = startMovingSpeed;
				return;
			}
			if(speed < maxSpeed){
				speed += speedIncrease * Time.deltaTime;
			}
			if(speed > maxSpeed){
				speed = maxSpeed;
			}
		}
		else{
			if(speed > minSpeed){
				speed -= speedIncrease * Time.deltaTime;
			}
			if(speed < minSpeed){
				speed = minSpeed;
			}
		}
	}
	// Obtener el angulo
	float GetAngle(Vector3 input){
		float angle;
		
		if (input.z >= 0.0f && input.x >= 0.0f) {
			angle = Mathf.Rad2Deg * Mathf.Atan (input.z / input.x);
		}
		else if (input.z >= 0.0f && input.x < 0.0f) {
			angle = 180 + Mathf.Rad2Deg * Mathf.Atan (input.z/ input.x);
		}
		else if (input.z < 0.0f && input.x < 0.0f) {
			angle = 180 + Mathf.Rad2Deg * Mathf.Atan (input.z/ input.x);
		}
		else {
			angle = 360 + Mathf.Rad2Deg * Mathf.Atan (input.z / input.x);
		}
		
		return angle;
	}
}
