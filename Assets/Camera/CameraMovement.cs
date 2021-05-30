﻿using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    // Declare variables
    public float maxRange = 750f;
    protected float moveSpeed = 150f;
    protected new Rigidbody2D camera;
    protected Vector2 movement;
    protected Vector2 mousePos;
    public Interface UI;

    // Booleans variables
    bool LegalMovement = true;

    private void Start()
    {
        camera = this.GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // Check if research open
        if (UI.ResearchOpen) return;

        // Get directional movement
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.LeftShift))
            moveSpeed = 600f;
        else if (Input.GetKeyUp(KeyCode.LeftShift))
            moveSpeed = 150f;
        else if (Input.GetKeyDown(KeyCode.LeftControl))
            moveSpeed = 20f;
        else if (Input.GetKeyUp(KeyCode.LeftControl))
            moveSpeed = 150f;

        LegalMovement = true;

        if (movement.x > 0 && camera.position.x + movement.x > maxRange) { LegalMovement = false; movement.x = 0; }
        if (movement.x < 0 && camera.position.x + movement.x < -maxRange) { LegalMovement = false; movement.x = 0; }
        if (movement.y > 0 && camera.position.y + movement.y > maxRange) { LegalMovement = false; movement.y = 0; }
        if (movement.y < 0 && camera.position.y + movement.y < -maxRange) { LegalMovement = false; movement.y = 0; }

        if (LegalMovement) camera.MovePosition(camera.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}
