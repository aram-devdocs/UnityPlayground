using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class playerController : MonoBehaviour
{

    /*
     * 
     * Todo:
     * Add horizontal raycasts so collisions with walls arent buggy
     * Fix odd movement on slopes
     * Fix bouncing on physics objects
     * 
     */

    [SerializeField] float maxSpeed = 7.5f;
    [SerializeField] float acceleration = 0.4f;
    [SerializeField] float halting = 0.4f;

    [SerializeField] float jumpHeight = 6f;
    [SerializeField] bool keepMomentumInAir = true;

    Vector3 newVelocity;

    float speed;
    float vectorXSpeed;
    int prevDirection;

    bool isGrounded = false;
    int groundRays = 3;
    int sideRays = 5;
    float rayMargin = 0.1f;

    Rect box;

    Rigidbody rb;
    Collider col;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        newVelocity = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {

        float horizontal = Input.GetAxisRaw("Horizontal");
        bool jump = Input.GetButton("Fire3");

        box = new Rect(
            col.bounds.min.x,
            col.bounds.min.y,
            col.bounds.size.x,
            col.bounds.size.y
        );

        groundCheck();

        if (horizontal != 0)
        {
            if (horizontal > 0) prevDirection = 1; else prevDirection = -1;

            if (speed < maxSpeed)
            {
                speed += acceleration;
            }

            if (speed > maxSpeed) speed = maxSpeed;

            vectorXSpeed = horizontal * speed;
        }
        else
        {
            if (keepMomentumInAir)
            {
                if (isGrounded)
                {
                    if (speed > 0) speed -= halting;
                }
            } else
            {
                if (speed > 0) speed -= halting;
            }

            vectorXSpeed = Mathf.Lerp(speed * prevDirection, horizontal, halting);
        }

        if (speed < 0) speed = 0;

        if (jump)
        {
            if (isGrounded)
            {
                isGrounded = false;
                rb.velocity = new Vector3(rb.velocity.x, jumpHeight, 0);
            }
        }

        newVelocity = new Vector3(vectorXSpeed, rb.velocity.y, 0);

        if (vectorXSpeed != 0)
        {
            sideCheck(vectorXSpeed);
        }

    }

    void sideCheck(float newSpeed)
    {
        RaycastHit hit;

        Vector3 start = new Vector3(box.center.x, box.yMin + rayMargin, 0);
        Vector3 end = new Vector3(box.center.x, box.yMax - rayMargin, 0);

        int mask = LayerMask.NameToLayer("FallingTile");

        float distance = box.width / 2 + Mathf.Abs(newSpeed * Time.deltaTime);
        Vector3 direction = newSpeed > 0 ? Vector3.right : Vector3.left;

        bool found = false;
        for (int i = 0; i < sideRays; i++)
        {
            float lerpAmount = (float)i / ((float)sideRays - 1);
            Vector3 origin = Vector3.Lerp(start, end, lerpAmount);
            Ray ray = new Ray(origin, direction);

            found = Physics.Raycast(ray, out hit, distance, mask);

            Debug.DrawLine(origin, hit.point, Color.red);

            if (found) 
            {
                newVelocity = new Vector3(0, rb.velocity.y, 0);
                break;
            }
        }
    }

    void groundCheck()
    {
        RaycastHit hit;
        float distance = box.height / 2 + (isGrounded ? rayMargin : Mathf.Abs(rb.velocity.y * Time.deltaTime));

        Vector3 start = new Vector3(box.xMin - rayMargin, box.center.y, 0);
        Vector3 end = new Vector3(box.xMax + rayMargin, box.center.y, 0);

        int mask = ~(1 << LayerMask.NameToLayer("FallingTile"));

        bool found = false;
        for (int i = 0; i < groundRays; i++)
        {
            float lerpAmount = (float)i / ((float)groundRays - 1);
            Vector3 origin = Vector3.Lerp(start, end, lerpAmount);
            Ray ray = new Ray(origin, Vector3.down);
            
            found = Physics.Raycast(ray, out hit, distance, mask);
            
            //Debug.DrawLine(origin, hit.point, Color.red);

            if (found)
            {
                isGrounded = true;
                break;
            }
            else
            {
                isGrounded = false;
            }
        }
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position += newVelocity * Time.deltaTime);
    }
}
