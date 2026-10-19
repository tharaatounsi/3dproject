using UnityEngine;

public class Coffrefort : Interactable
{
    public Sprite spriteOuvert;
    private bool ouvert = false;

    public override void Interagir()
    {
        if (ouvert)
        {
            Msg("Le coffre-fort est vide.");
            return;
        }
        GameManager.Instance.OuvrirCode(GameManager.Instance.codeSecret, Ouvrir);
    }

    private void Ouvrir()
    {
        ouvert = true;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (spriteOuvert != null) sr.sprite = spriteOuvert;

        Inventaire.Instance.Ajouter("Lanterne");
        Son("ramasser");
        GameManager.Instance.AjouterScore(150, "Coffre-fort ouvert");
        Msg("Le coffre-fort s'ouvre ! Tu prends une lanterne. La cave sera sombre...", 5f);
    }
}
