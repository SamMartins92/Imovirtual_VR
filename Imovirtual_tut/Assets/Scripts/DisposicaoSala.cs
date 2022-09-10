using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisposicaoSala : MonoBehaviour
{

    public GameObject SalaOne;

    public GameObject SalaTwo;

    public void EnableOne()
    {
        SalaOne.SetActive(true);
        SalaTwo.SetActive(false);
    }

    public void EnableTwo()
    {
        SalaOne.SetActive(false);
        SalaTwo.SetActive(true);
    }


}