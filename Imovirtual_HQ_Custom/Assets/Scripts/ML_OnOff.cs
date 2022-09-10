using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ML_OnOff : MonoBehaviour
{
    public GameObject Player;



    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("m"))
        {
            print("Menu is On");
            Cursor.lockState = CursorLockMode.None;
            Player.GetComponent<MouseLook>().enabled = false;
        }

        if (Input.GetKeyDown("g"))
        {
            print("Game On");
            Cursor.lockState = CursorLockMode.Locked;
            Player.GetComponent<MouseLook>().enabled = true;
        }




    }
}


