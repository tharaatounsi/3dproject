using UnityEngine;

public class Tiroir : Interactable
{
    public Sprite spriteOuvert;
    private bool ouvert = false;

    public override void Interagir()
    {
        if (ouvert)
        {
            Msg("Le tiroir est vide.");
            return;
        }
        ouvert = true;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (spriteOuvert != null) sr.sprite = spriteOuvert;
        else sr.color = new Color(0.4f, 0.3f, 0.2f);

        Inventaire.Instance.Ajouter("Clé");
        Son("ramasser");
        GameManager.Instance.AjouterScore(50, "Clé trouvée");
        Msg("Tu ouvres le tiroir et tu trouves une petite clé !");
    }
}
