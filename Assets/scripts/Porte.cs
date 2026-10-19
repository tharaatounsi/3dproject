using UnityEngine;

// La grande porte de sortie, dans la cave.
public class Porte : Interactable
{
    public override void Interagir()
    {
        if (!GameManager.Instance.leviersResolus)
        {
            Son("erreur");
            Msg("La grande porte est bloquée par un mécanisme. Il doit y avoir des leviers dans la cave...", 5f);
            return;
        }
        GameManager.Instance.Gagner();
    }
}
