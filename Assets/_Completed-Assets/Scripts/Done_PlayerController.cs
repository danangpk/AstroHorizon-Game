using UnityEngine;
using System.Collections;

[System.Serializable]
public class Done_Boundary
{
    public float xMin, xMax, zMin, zMax;
}

public class Done_PlayerController : MonoBehaviour
{
    public float speed;
    public float tilt;
	public float dampingRadius;
	public float velocitylag;
	public float fireRate;

    public Done_Boundary boundary;
    public GameObject shot;
    public Transform shotSpawn;

	private Vector3 target;
    private float nextFire;
	private Quaternion calibrationQuaternion;

	void Start()
	{
	}

    void Update()
    {
		bool triggered = false;
		if (Input.touchCount > 0) 
		{
			triggered = true;
		}
		if (triggered && Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            Instantiate(shot, shotSpawn.position, Quaternion.identity);
            GetComponent<AudioSource>().Play();
        }
    }

	void FixedUpdate ()
	{
		Vector3? touchPos = null;
		if (Input.mousePresent && Input.GetMouseButton(0))
		{
			touchPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f);
		}
		else if(Input.touchCount > 0)
		{
			touchPos = new Vector3(Input.touches[0].position.x, Input.touches[0].position.y, 0.0f);
		}
		if (touchPos != null)
		{ 
			target = Camera.main.ScreenToWorldPoint(touchPos.Value);
			target.y = GetComponent<Rigidbody>().position.y;
		}

		Vector3 offset = target - GetComponent<Rigidbody>().position;

		float magnitude = offset.magnitude;
		if (magnitude > dampingRadius)
			magnitude = dampingRadius;
		float dampening = magnitude / dampingRadius;

		Vector3 desiredVelocity = offset.normalized * speed * dampening;

		GetComponent<Rigidbody>().velocity += (desiredVelocity - GetComponent<Rigidbody>().velocity) * velocitylag;

		// Clamp to bounds
		GetComponent<Rigidbody>().position = new Vector3
			(
				Mathf.Clamp(GetComponent<Rigidbody>().position.x, boundary.xMin, boundary.xMax),
				0.0f,
				Mathf.Clamp(GetComponent<Rigidbody>().position.z, boundary.zMin, boundary.zMax)
			);

		GetComponent<Rigidbody>().rotation = Quaternion.Euler(0.0f, 0.0f, GetComponent<Rigidbody>().velocity.x * -tilt);
	}
}