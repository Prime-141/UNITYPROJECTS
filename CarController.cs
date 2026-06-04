
using Unity.VisualScripting;
using UnityEngine;

public class CarController : MonoBehaviour
{
[SerializeField] private WheelCollider frontRightWheelCollider;
[SerializeField] private WheelCollider backRightWheelCollider;
[SerializeField] private WheelCollider frontLeftWheelCollider;
[SerializeField] private WheelCollider backLeftWheelCollider;
    
[SerializeField] private Transform frontRightWheelTransform;

[SerializeField] private Transform backRightWheelTransform;

[SerializeField] private Transform frontLeftWheelTransform;
[SerializeField] private Transform backLeftWheelTransform;
    
[SerializeField] private Transform carCentreOfMassTransform;


[SerializeField] private float motorForce = 2500f;
[SerializeField] private float steeringAngle = 24f;


[SerializeField] private float brakeForce = 4000f;

[SerializeField] private float normalDamping = 0.05f;
[SerializeField] private float brakeDamping = 2.5f;
[SerializeField] private float dampingLerpSpeed = 5f;
[SerializeField] UIManger UIManger;

//Speed based steering
[SerializeField] float maxSteerAngle = 24f;
[SerializeField] float minSteerAngle = 8f;
[SerializeField] float maxSpeed = 150f;


[SerializeField] float downForce = 100f;

private Rigidbody rigidBody;
    float verticalInput;
    float horizontalInput;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.centerOfMass = carCentreOfMassTransform.localPosition;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        GetInput();
        MotorForce();
        UpdateWheels();
        Steering();
        ApplyBrakes();
        PowerSteering();
        Debug.Log(CarSpeed());

       // downforce  
        rigidBody.AddForce(-transform.up * downForce * rigidBody.linearVelocity.magnitude);
    }
    
    void GetInput()
    {
        verticalInput = Input.GetAxis("Vertical");
        horizontalInput = Input.GetAxis("Horizontal");
    }

    void ApplyBrakes()
    {
        if(Input.GetKey(KeyCode.Space))
        {
         // Optional: light wheel braking
        frontRightWheelCollider.brakeTorque = brakeForce;
        frontLeftWheelCollider.brakeTorque = brakeForce;
        backLeftWheelCollider.brakeTorque = brakeForce;
        backRightWheelCollider.brakeTorque = brakeForce;
        // Smooth POWER BRAKE using drag
        rigidBody.linearDamping = Mathf.Lerp(
            rigidBody.linearDamping,
            brakeDamping,
            Time.fixedDeltaTime * dampingLerpSpeed
        );
        }
        else
        {
        frontRightWheelCollider.brakeTorque = 0f;
        frontLeftWheelCollider.brakeTorque = 0f;
        backLeftWheelCollider.brakeTorque = 0f;
        backRightWheelCollider.brakeTorque = 0f;
        rigidBody.linearDamping = Mathf.Lerp(
            rigidBody.linearDamping,
            normalDamping,
            Time.fixedDeltaTime * dampingLerpSpeed
        );
        }
        
    }
    void MotorForce()
    {
       // frontRightWheelCollider.motorTorque = motorForce*verticalInput;
      //  frontLeftWheelCollider.motorTorque = motorForce*verticalInput;
      float currentSpeed = CarSpeed(); // km/h

    if (currentSpeed < maxSpeed)
        {
        float speedFactor = 1f - (currentSpeed / maxSpeed);
        float finalTorque = motorForce * speedFactor * verticalInput;

        frontRightWheelCollider.motorTorque = finalTorque;
        frontLeftWheelCollider.motorTorque = finalTorque;
        }
    else
        {
        frontRightWheelCollider.motorTorque = 0f;
        frontLeftWheelCollider.motorTorque = 0f;
        }
    }
    void Steering()
    {
        frontRightWheelCollider.steerAngle = steeringAngle*horizontalInput;
        frontLeftWheelCollider.steerAngle = steeringAngle*horizontalInput;
    }
    void PowerSteering()
    {
        if(horizontalInput==0)
        {
          //  transform.rotation= Quaternion.Euler(0f,0f,0f);
            // for delay
            transform.rotation= Quaternion.Slerp(transform.rotation,Quaternion.Euler(0,0,0), Time.deltaTime);
        }
    }
    void UpdateWheels()
    {
        RotateWheel(frontRightWheelCollider,frontRightWheelTransform);
        RotateWheel(backRightWheelCollider,backRightWheelTransform);
        RotateWheel(frontLeftWheelCollider,frontLeftWheelTransform);
        RotateWheel(backLeftWheelCollider,backLeftWheelTransform);
    }
    void RotateWheel(WheelCollider wheelCollider, Transform transform)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        transform.position = pos;
        transform.rotation = rot;


    }

    void HandleSteering()
    {
    float speed = rigidBody.linearVelocity.magnitude * 3.6f; // km/h
    float steerAngle = Mathf.Lerp(maxSteerAngle, minSteerAngle, speed / maxSpeed);

    float steerInput = Input.GetAxis("Horizontal");

    frontLeftWheelCollider.steerAngle = steerAngle * steerInput;
    frontRightWheelCollider.steerAngle = steerAngle * steerInput;
    }

    void HandleMotor()
    {
    float speed = rigidBody.linearVelocity.magnitude * 3.6f;
    float torqueLimiter = Mathf.Clamp01(1f - speed / maxSpeed);

    float motor = motorForce * torqueLimiter * Input.GetAxis("Vertical");

    backLeftWheelCollider.motorTorque = motor;
    backRightWheelCollider.motorTorque = motor;
    }
    void AdjustGrip()
    {
    float speedFactor = rigidBody.linearVelocity.magnitude / maxSpeed;

    WheelFrictionCurve side = frontLeftWheelCollider.sidewaysFriction;
    side.stiffness = Mathf.Lerp(2.2f, 3.0f, speedFactor);

    frontLeftWheelCollider.sidewaysFriction = side;
    frontRightWheelCollider.sidewaysFriction = side;
    }
    
    public float CarSpeed()
    {
        float speed = rigidBody.linearVelocity.magnitude*3.6f; //km/h
        return speed;
    }
      private void OnCollisionEnter(Collision collision)
{
    Debug.Log("Car hit: " + collision.gameObject.name);

    if (collision.gameObject.CompareTag("TrafficVehicle"))
    {
         UIManger.GameOver();
    }
    if (collision.gameObject.CompareTag("Obstacle"))
        {
            UIManger.GameOver();
        }
}

}
