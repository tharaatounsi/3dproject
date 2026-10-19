using UnityEngine;
using System.Collections.Generic;

public class Levier : Interactable
{
    public int index = 0;
    public Sprite spriteOff;
    public Sprite spriteOn;

    private bool actif = false;
    private SpriteRenderer sr;
    private static List<Levier> tous = new List<Levier>();

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (spriteOff != null) sr.sprite = spriteOff;
        tous.Add(this);
    }

    void OnDestroy()
    {
        tous.Remove(this);
    }

    private void Baisser()
    {
        actif = true;
        if (spriteOn != null) sr.sprite = spriteOn;
    }

    private void Remonter()
    {
        actif = false;
        if (spriteOff != null) sr.sprite = spriteOff;
    }

    public override void Interagir()
    {
        if (GameManager.Instance.leviersResolus)
        {
            Msg("Les leviers sont déjà dans la bonne position.");
            return;
        }
        if (actif)
        {
            Msg("Ce levier est déjà baissé.");
            return;
        }

        Baisser();
        Son("clic");

        int resultat = GameManager.Instance.AppuyerLevier(index);
        if (resultat < 0)
        {
            foreach (Levier l in tous) l.Remonter();
            GameManager.Instance.PerdreVie("Un grincement... les leviers remontent. Mauvais ordre !");
        }
        else if (resultat == 1)
        {
            Son("ouvrir");
            Msg("Clac ! Un mécanisme se débloque dans la porte.", 5f);
        }
        else
        {
            Msg("Clic.", 2f);
        }
    }
}
