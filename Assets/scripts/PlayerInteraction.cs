using UnityEngine;
using System.Collections.Generic;

public class PlayerInteraction : MonoBehaviour
{
    private List<Interactable> proches = new List<Interactable>();

    void Update()
    {
        proches.RemoveAll(x => x == null || !x.gameObject.activeInHierarchy);

        Interactable meilleur = null;
        float distMin = float.MaxValue;
        foreach (Interactable i in proches)
        {
            float d = Vector2.Distance(transform.position, i.transform.position);
            if (d < distMin) { distMin = d; meilleur = i; }
        }

        if (GameManager.Instance != null)
            GameManager.Instance.objetProche = (meilleur != null) ? meilleur.nom : "";

        if (GameManager.Instance != null && GameManager.Instance.Bloque) return;

        if (meilleur != null && Input.GetKeyDown(KeyCode.E))
            meilleur.Interagir();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Interactable inter = other.GetComponent<Interactable>();
        if (inter != null && !proches.Contains(inter)) proches.Add(inter);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Interactable inter = other.GetComponent<Interactable>();
        if (inter != null) proches.Remove(inter);
    }
}
