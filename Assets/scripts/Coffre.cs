using UnityEngine;

public class Coffre : Interactable
{
    public Sprite spriteOuvert;
    private bool ouvert = false;

    public override void Interagir()
    {
        if (ouvert)
        {
            Msg("Le coffre est déjà ouvert. Relis ton papier dans l'inventaire.");
            return;
        }

        if (!Inventaire.Instance.ADejaObjet("Clé"))
        {
            Son("erreur");
            Msg("Le coffre est verrouillé. Il faut trouver une clé.");
            return;
        }

        ouvert = true;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (spriteOuvert != null) sr.sprite = spriteOuvert;

        Inventaire.Instance.Retirer("Clé");
        Inventaire.Instance.Ajouter("Clé de la bibliothèque");
        Inventaire.Instance.Ajouter("Papier");
        Son("ouvrir");
        GameManager.Instance.AjouterScore(100, "Coffre ouvert");
        Msg("Le coffre s'ouvre ! Tu trouves la clé de la bibliothèque et un papier : \"" + GameManager.Instance.indiceCode + "\"", 7f);
    }
}
