using UnityEngine;
using System.Collections.Generic;

public class Inventaire : MonoBehaviour
{
    public static Inventaire Instance;
    public List<string> objets = new List<string>();

    void Awake()
    {
        Instance = this;
    }

    public void Ajouter(string nomObjet)
    {
        objets.Add(nomObjet);
        Debug.Log("Objet ramassé : " + nomObjet);
    }

    public bool ADejaObjet(string nomObjet)
    {
        return objets.Contains(nomObjet);
    }

    public void Retirer(string nomObjet)
    {
        objets.Remove(nomObjet);
    }

    public string Texte()
    {
        if (objets.Count == 0) return "(vide)";
        return string.Join(", ", objets.ToArray());
    }
}
