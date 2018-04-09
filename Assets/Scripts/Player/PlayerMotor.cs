﻿using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : NetworkBehaviour {

	[SerializeField]
	private Camera cam;

    [SerializeField]
    private float maxCamRotation = 85f;

    [SerializeField]
    private float jumpSpeed = 5.0f;


    public Vector3 velocity = Vector3.zero;
	public Vector3 rotation = Vector3.zero;

	private float camRotation = 0;
    private float currCamRotation = 0;

    public bool inJump;
    public bool inAir;
	private Rigidbody rb;

    Player player;

	void Start()
	{
        inJump = false;
        inAir = false;
		rb = GetComponent<Rigidbody> ();
    }

    public void Move(Vector3 _velocity)
    {
        velocity = _velocity;
    }

    public bool Jump()
    {
        if (!inAir)
        {
            rb.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
            inJump = true;
            onGroundTimer = 0;
            return true;
        }
        return false;
    }

    public void Rotate(Vector3 _rotation)
	{
		rotation = _rotation;
	}

	public void CamRotate(float _camRotation)
	{
		camRotation = _camRotation;
	}

	void FixedUpdate()
	{
		PerformMovement ();
		PerformRotation ();
	}

    Vector3 nextPos;
    Vector3 prevPos;
    Vector3 move;
	private void PerformMovement()
	{
		if (velocity != Vector3.zero) 
		{
            move = velocity * Time.fixedDeltaTime;
            nextPos = rb.position + move;
            //fixBounce();
            rb.MovePosition(nextPos);
            //rb.MovePosition (rb.position + velocity * Time.fixedDeltaTime);
        }
	}

    void fixBounce()
    {
        if (inJump) return;

        Vector3 bottom = nextPos - new Vector3(0, .9f, 0);

        RaycastHit hit;
        if (Physics.Raycast(bottom, transform.forward, out hit, 0.5f))
        {
            return;
        }
        if (Physics.Raycast(bottom, Vector3.down, out hit, 0.2f))
        {
            nextPos.y -= hit.distance;
        }
    }

    private void PerformRotation()
	{
		rb.MoveRotation (rb.rotation * Quaternion.Euler (rotation));
		if (cam != null) 
		{
            currCamRotation -= camRotation;
            currCamRotation = Mathf.Clamp(currCamRotation, -maxCamRotation, maxCamRotation);
            cam.transform.localEulerAngles = new Vector3(currCamRotation, 0f, 0f);
		}
	}

    float onGroundTimer;
    void OnCollisionStay(Collision collisionInfo)
    {
        onGroundTimer += Time.deltaTime;
        inAir = false;
        if(onGroundTimer > .1f)
        {
            inJump = false;
        }
        
    }

    void OnCollisionExit(Collision collisionInfo)
    {
        inAir = true;
        
    }
}
