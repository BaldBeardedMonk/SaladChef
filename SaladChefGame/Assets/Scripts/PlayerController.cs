using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Vector2 input_movement;

    [SerializeField]
    float moveSpeed = 10f;

    private void Update()
    {
        Move();
    }

    void Move()
    {
        Vector2 movement = new Vector2(input_movement.x, input_movement.y)*moveSpeed*Time.deltaTime;
        transform.Translate(movement);
    }

    private void OnMove(InputValue inputValue)  //function to capture input from the new input system
    {
        input_movement = inputValue.Get<Vector2>();
    }
}
