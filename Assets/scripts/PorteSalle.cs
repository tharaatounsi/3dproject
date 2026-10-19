using UnityEngine;

public class PorteSalle : Interactable
{
    public int salleCible = 1;
    public Vector2 arrivee = Vector2.zero;
    public string cleRequise = "";
    public bool consommerCle = false;
    public string messageVerrouille = "La porte est verrouillée.";
    public string messageEntree = "";

    private bool deverrouillee = false;

    public override void Interagir()
    {
        string debut = "";

        if (!deverrouillee && cleRequise != "")
        {
            if (!Inventaire.Instance.ADejaObjet(cleRequise))
            {
                Son("erreur");
                Msg(messageVerrouille, 5f);
                return;
            }
            deverrouillee = true;
            if (consommerCle) Inventaire.Instance.Retirer(cleRequise);
            debut = "Tu déverrouilles la porte. ";
        }

        GameManager.Instance.salleOuverte[salleCible] = true;
        GameManager.Instance.PremiereVisite(salleCible);
        Son("ouvrir");

        Transform joueur = Inventaire.Instance.transform;
        Rigidbody2D rb = joueur.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.position = arrivee;
            rb.velocity = Vector2.zero;
        }
        joueur.position = new Vector3(arrivee.x, arrivee.y, 0f);

        if (messageEntree != "" || debut != "")
            Msg(debut + messageEntree, 4f);
    }
}
