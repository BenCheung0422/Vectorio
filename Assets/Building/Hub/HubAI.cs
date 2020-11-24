﻿using UnityEngine;

public class HubAI : TileClass
{
    protected Camera main;
    protected bool gameOver;
    private GameObject SRVSC;
    [SerializeField] protected Canvas EndScreen;

    // On start, assign weapon variables
    void Start()
    {
        SRVSC = GameObject.Find("Survival");
        InvokeRepeating("SendGold", 0f, 1f);

        main = Camera.main;
        health = 30;
        maxhp = 30;
    }

    // Kill defense
    public override void DestroyTile()
    {
        GameObject.Find("Main Camera").GetComponent<CameraMovement>().enabled = false;
        GameObject.Find("Camera").GetComponent<CameraScroll>().enabled = false;
        GameObject.Find("Building").GetComponent<Building>().enabled = false;
        gameOver = true;
    }

    private void Update()
    {
        if (gameOver)
        {
            main.orthographicSize = Mathf.Lerp(main.orthographicSize, 50, Time.deltaTime * 1f);
            main.gameObject.transform.position = new Vector3(Mathf.Lerp(main.gameObject.transform.position.x, 0, Time.fixedDeltaTime * 1), Mathf.Lerp(main.gameObject.transform.position.y, 0, Time.fixedDeltaTime * 1), main.gameObject.transform.position.z);
        }
        if (gameOver && main.orthographicSize >= 49)
        {
            Instantiate(Effect, transform.position, Quaternion.identity);
            Instantiate(EndScreen, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    // Send gold
    private void SendGold()
    {
        SRVSC.GetComponent<Survival>().AddGold(1);
        SRVSC.GetComponent<Survival>().UpdateGui();
    }
}
