using UnityEngine;

public class Interactable : MonoBehaviour
{
    public string nom = "Objet";

    public virtual void Interagir()
    {
        Debug.Log("Tu interagis avec : " + nom);
        Msg("Tu regardes : " + nom);
    }

    protected void Msg(string texte, float duree = 4f)
    {
        if (GameManager.Instance != null)
            GameManager.Instance.AfficherMessage(texte, duree);
    }

    protected void Son(string nomSon)
    {
        if (GameManager.Instance != null)
            GameManager.Instance.Jouer(nomSon);
    }
}
