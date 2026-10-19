using UnityEngine;

public class Panneau : Interactable
{
    [TextArea]
    public string texte = "Un message gravé dans la pierre.";
    public bool memoriser = false;

    public override void Interagir()
    {
        Msg(texte, 6f);
        if (memoriser && GameManager.Instance != null)
        {
            if (GameManager.Instance.indiceLeviers == "")
                GameManager.Instance.AjouterScore(25, "Grimoire lu");
            GameManager.Instance.indiceLeviers = texte;
        }
    }
}
