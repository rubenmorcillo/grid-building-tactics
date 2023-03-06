using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float normalSpeed;
    public float fastSpeed;
    private float movementSpeed;
    public float movementTime;
    public float rotationAmount;

    public Vector3 newPosition;
    public Quaternion newRotation;
    public Vector3 rotateStartPosition;
    public Vector3 rotateCurrentPosition;

    public Vector3 dragStartPosition;
    public Vector3 dragCurrentPosition;


    private Camera cam;
    private int minFieldView = 10;
    private int maxFieldView = 60;

    // Start is called before the first frame update
    void Start()
    {
        newPosition = transform.position;
        newRotation = transform.rotation;
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovementInput();
        HandleMouseInput();
        HandleZoom();
    }
    
    void HandleMouseInput()
	{
        if (Input.GetMouseButtonDown(2))
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            float entry;

            if(plane.Raycast(ray, out entry))
			{
                dragStartPosition = ray.GetPoint(entry);
			}
        }
        if (Input.GetMouseButton(2))
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            float entry;

            if (plane.Raycast(ray, out entry))
            {
                dragCurrentPosition = ray.GetPoint(entry);

                newPosition = transform.position + (dragStartPosition - dragCurrentPosition);
            }
        }


  //      if (Input.GetMouseButtonDown(1))
		//{
  //          rotateStartPosition = Input.mousePosition;
		//}
  //      if (Input.GetMouseButton(1))
		//{
  //          rotateCurrentPosition = Input.mousePosition;

  //          Vector3 difference = rotateStartPosition - rotateCurrentPosition;

  //          rotateStartPosition = rotateCurrentPosition;

  //          newRotation *= Quaternion.Euler(Vector3.up * (-difference.x / 5f));
		//}
    }
    void HandleMovementInput()
	{
        if (Input.GetKey(KeyCode.LeftShift))
		{
            movementSpeed = fastSpeed;
		}
		else
		{
            movementSpeed = normalSpeed;
		}


        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
		{
            newPosition += ((transform.forward + transform.right) * movementSpeed);
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            newPosition += ((transform.forward + transform.right) * -movementSpeed);

        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            newPosition += ((transform.right - transform.forward) * movementSpeed);
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            newPosition += ( (transform.right - transform.forward) * -movementSpeed);
        }

        if (Input.GetKey(KeyCode.E))
		{
            newRotation *= Quaternion.Euler(Vector3.up * rotationAmount);
		}

        if (Input.GetKey(KeyCode.Q))
        {
            newRotation *= Quaternion.Euler(Vector3.up * -rotationAmount);

        }

        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * movementTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * movementTime);

    }

    void HandleZoom()
	{

        if (Input.mouseScrollDelta.y != 0)
        {
            cam.fieldOfView += -Input.mouseScrollDelta.y;
        }
        //if (Input.GetKey(KeyCode.Q))
        //{
        //    cam.orthographicSize++;

        //}
        //if (Input.GetKey(KeyCode.E))
        //{
        //    cam.fieldOfView--;
        //}

        if (cam.fieldOfView > maxFieldView)
        {
            cam.fieldOfView = maxFieldView;
        }
        if (cam.fieldOfView < minFieldView)
        {
            cam.fieldOfView = minFieldView;
        }
    }
}
