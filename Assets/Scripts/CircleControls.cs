using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CircleControls : MonoBehaviour
{
    public Rigidbody2D legL;
    public Rigidbody2D legR;
    public Rigidbody2D legShaftL;
    public Rigidbody2D legShaftR;
    public Camera cam;
    public Text text;

    /* Centripetal force
     * F = mvv/r
     */

    float rotationTorqueModifier = 5f;
    float maxRotaionVelocity = 1000f;
    Vector2 inputLegL = Vector2.zero;
    Vector2 inputLegR = Vector2.zero;
    bool stiff = false;

    float inputDistanceRatioL = 0f;
    float inputDistanceRatioR = 0f;
    float actualDistanceRatioL;
    float actualDistanceRatioR;
    float forceModifier = 5000f;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(legL.transform.localPosition.magnitude / legL.gameObject.GetComponent<SliderJoint2D>().connectedAnchor.magnitude);
        cam.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, cam.transform.position.z);
        //Debug.Log("Inputted left extension: " + posL);
        //Debug.Log("Inputted right extension: " + posR);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        actualDistanceRatioL = ApplyForces(legL, legShaftL, inputDistanceRatioL);
        actualDistanceRatioR = ApplyForces(legR, legShaftR, inputDistanceRatioR);
        RotateLegShaftTowardPosition(legL, legShaftL, inputLegL, actualDistanceRatioL);
        RotateLegShaftTowardPosition(legR, legShaftR, inputLegR, actualDistanceRatioR);
        loadText();
    }

    float ApplyForces(Rigidbody2D leg, Rigidbody2D legShaft, float desiredPos)
    {
        Vector2 localPosition2D = new Vector2(leg.transform.localPosition.x, leg.transform.localPosition.y);
        float v = legShaft.transform.InverseTransformDirection(leg.velocity - legShaft.velocity).x;


        float ratio = localPosition2D.magnitude / leg.gameObject.GetComponent<SliderJoint2D>().connectedAnchor.magnitude;
        float difference = desiredPos - ratio;

        if (difference > 0.0f && v < 0)
        {
            leg.AddRelativeForce(Vector2.right * -v);
            legShaft.AddRelativeForce(Vector2.left * v);
        }
        leg.AddRelativeForce(Vector2.right * difference * forceModifier * Time.fixedDeltaTime);
        legShaft.AddRelativeForce(Vector2.left * difference * forceModifier * Time.fixedDeltaTime);
        return ratio;
    }
    
    void RotateLegShaftTowardPosition(Rigidbody2D leg, Rigidbody2D shaft, Vector2 input, float radius)
    {
        stiff = false;
        Vector2 currentDir = new Vector2(Mathf.Cos(shaft.transform.eulerAngles.z * Mathf.Deg2Rad), Mathf.Sin(shaft.transform.eulerAngles.z * Mathf.Deg2Rad));
        float angle = Vector2.SignedAngle(currentDir, input);
        float force = 0f;
        if (input.magnitude < 0.1f) stiff = true;
        else if (Mathf.Abs(angle) > 45f) force = 180f * Mathf.Sign(angle);
        else if (Mathf.Abs(angle) > 5f) force = angle * 4;
        else if (Mathf.Abs(angle) > 1f) force = 20f * Mathf.Sign(angle);
        else stiff = true;
        if (stiff)
        {
            shaft.angularVelocity = 0;
            leg.angularVelocity = 0;
        }
        else
        {
            shaft.angularVelocity = force * rotationTorqueModifier * getCenterOfMassRadius(leg, shaft);
        }
    }

    float getCenterOfMassRadius(Rigidbody2D leg, Rigidbody2D shaft)
    {
        float num = ((leg.centerOfMass + (leg.position - shaft.position)).magnitude * leg.mass) + (shaft.centerOfMass.magnitude * shaft.mass);
        return  num * 1.5f / (leg.mass + shaft.mass);
    }

    void loadText()
    {
        float l = getCenterOfMassRadius(legL, legShaftL);
        float r = getCenterOfMassRadius(legR, legShaftR);
        text.text = "Lrad: " + l + "\nRrad: " + r;
        //text.text = legShaftL.centerOfMass.ToString();
    }

    public void ExtendLegL(InputAction.CallbackContext context)
    {
        inputDistanceRatioL = context.ReadValue<float>();
    }

    public void ExtendLegR(InputAction.CallbackContext context)
    {
        inputDistanceRatioR = context.ReadValue<float>();
    }

    public void SetLegL(InputAction.CallbackContext context)
    {
        inputLegL = context.ReadValue<Vector2>().normalized;
    }

    public void SetLegR(InputAction.CallbackContext context)
    {
        inputLegR = context.ReadValue<Vector2>().normalized;
    }
}
