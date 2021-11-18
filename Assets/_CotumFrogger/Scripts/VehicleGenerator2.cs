using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleGenerator2 : MonoBehaviour
{
    [SerializeField]
    private int maxTravelDistance;

    [SerializeField]
    private int velocity;

    public enum Direction { Forwards, Backwards };
    [SerializeField]
    public Direction direction;

    Transform trans;
    Vector3 initialPosition;
    private float traveledDistance;
    private int vehicleDirection;

    void Start()
    {
        trans = GetComponent<Transform>();
        initialPosition = new Vector3(trans.position.x, trans.position.y, trans.position.z);
        traveledDistance = 0;
        vehicleDirection = direction == Direction.Forwards ? 1 : -1;
    }

    void Update()
    {
        trans.Translate(0, 0, velocity * Time.deltaTime * vehicleDirection);
        traveledDistance += velocity * Time.deltaTime;

        //Debug.Log("traveled_distance: " + traveledDistance);

        if (traveledDistance >= maxTravelDistance)
        {
            trans.position = new Vector3(initialPosition.x, initialPosition.y, initialPosition.z);
            traveledDistance = 0;
        }
    }
}