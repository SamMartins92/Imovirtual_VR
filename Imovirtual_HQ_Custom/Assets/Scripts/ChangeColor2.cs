using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColor2 : MonoBehaviour
{

    [SerializeField] GameObject[] tapeteGameObjects;
    [SerializeField] Material[] tapeteMaterials;
    private int tapeteMaterialIndex = 0;
    [SerializeField] GameObject[] armarioCozinhaGameObjects;
    [SerializeField] Material[] armarioCozinhaMaterials;
    private int armarioCozinhaMaterialIndex = 0;
    [SerializeField] GameObject[] mesaJantarGameObjects;
    [SerializeField] Material[] mesaJantarMaterials;
    private int mesaJantarMaterialIndex = 0;
    [SerializeField] GameObject[] sofaGameObjects;
    [SerializeField] Material[] sofaMaterials;
    private int sofaMaterialIndex = 0;

    [SerializeField] GameObject[] chao;
    [SerializeField] GameObject[] MesaSala;
    [SerializeField] GameObject[] MobiliaSala;
    [SerializeField] GameObject[] Cortinas;
    [SerializeField] GameObject[] CadeirasCozinha;
    [SerializeField] GameObject[] CortinasCozinha;
    [SerializeField] GameObject[] TapeteQuarto;
    [SerializeField] GameObject[] CamaQuarto;
    [SerializeField] GameObject[] CortinasQuarto;

    private int conjuntoMateriaisIndex = 0;

    public Conjunto[] conjutos;

    public void ChangeTapeteMaterial()
    {
        tapeteMaterialIndex = ++tapeteMaterialIndex % tapeteMaterials.Length;
        foreach (GameObject obj in tapeteGameObjects)
        {
            obj.GetComponent<Renderer>().material = tapeteMaterials[tapeteMaterialIndex];

        }
    }

    public void ChangeMovelCozinhaMaterial()
    {
        armarioCozinhaMaterialIndex = ++armarioCozinhaMaterialIndex % armarioCozinhaMaterials.Length;
        foreach (GameObject obj in armarioCozinhaGameObjects)
        {
            obj.GetComponent<Renderer>().sharedMaterial = armarioCozinhaMaterials[armarioCozinhaMaterialIndex];
        }
    }

    public void ChangeMesaJantarMaterial()
    {
        mesaJantarMaterialIndex = ++mesaJantarMaterialIndex % mesaJantarMaterials.Length;
        foreach (GameObject obj in mesaJantarGameObjects)
        {
            obj.GetComponent<Renderer>().sharedMaterial = mesaJantarMaterials[mesaJantarMaterialIndex];
        }
    }

    public void ChangeSofaMaterial()
    {
        sofaMaterialIndex = ++sofaMaterialIndex % sofaMaterials.Length;
        foreach (GameObject obj in sofaGameObjects)
        {
            obj.GetComponent<Renderer>().sharedMaterial = sofaMaterials[sofaMaterialIndex];
        }
    }

    public void ChangeConjunto(int conjunto)
    {
        foreach (GameObject obj in chao)
        {
            Renderer renderer = obj.GetComponent<Renderer>();
            Material[] arrayAux = renderer.sharedMaterials;
                arrayAux[1] = conjutos[conjunto].chao;
            renderer.sharedMaterials = arrayAux;

        }
        foreach (GameObject obj in sofaGameObjects)
        {
            obj.GetComponent<Renderer>().sharedMaterial = conjutos[conjunto].Sofa;
        }
        foreach (GameObject obj in MesaSala)
        {
            Renderer renderer = obj.GetComponent<Renderer>();
            obj.GetComponent<Renderer>().sharedMaterial = conjutos[conjunto].MesaSala;
        }
        foreach (GameObject obj in MobiliaSala)
        {
            obj.GetComponent<Renderer>().sharedMaterial = conjutos[conjunto].MobiliaSala;
        }
        foreach (GameObject obj in tapeteGameObjects)
        {
            obj.GetComponent<Renderer>().sharedMaterial = conjutos[conjunto].Tapete;
        }
        foreach (GameObject obj in Cortinas)
        {
            obj.GetComponent<Renderer>().sharedMaterial = conjutos[conjunto].Cortinas;
        }
        foreach (GameObject obj in mesaJantarGameObjects)
        {
            obj.GetComponent<Renderer>().sharedMaterial = conjutos[conjunto].MesaJantar;
        }
        foreach (GameObject obj in armarioCozinhaGameObjects)
        {
            obj.GetComponent<Renderer>().sharedMaterial = conjutos[conjunto].MobiliaCozinha;
        }
        foreach (GameObject obj in CadeirasCozinha)
        {
            obj.GetComponent<Renderer>().sharedMaterial = conjutos[conjunto].CadeirasCozinha;
        }
        foreach (GameObject obj in CortinasCozinha)
        {
            obj.GetComponent<Renderer>().sharedMaterial = conjutos[conjunto].CortinasCozinha;
        }
        foreach (GameObject obj in TapeteQuarto)
        {
            obj.GetComponent<Renderer>().sharedMaterial = conjutos[conjunto].TapeteQuarto;
        }
        foreach (GameObject obj in CamaQuarto)
        {
            obj.GetComponent<Renderer>().sharedMaterial = conjutos[conjunto].CamaQuarto;
        }
        foreach (GameObject obj in CortinasQuarto)
        {
            obj.GetComponent<Renderer>().sharedMaterial = conjutos[conjunto].CortinasQuarto;
        }

    }

    [Serializable]
    public struct Conjunto
    {
        public Material chao;
        public Material Sofa;
        public Material MesaSala;
        public Material MobiliaSala;
        public Material Tapete;
        public Material Cortinas;
        public Material MesaJantar;
        public Material MobiliaCozinha;
        public Material CadeirasCozinha;
        public Material CortinasCozinha;
        public Material TapeteQuarto;
        public Material CamaQuarto;
        public Material CortinasQuarto;
    }
}
