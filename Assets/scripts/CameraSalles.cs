using UnityEngine;

// La caméra se place sur la salle où se trouve le joueur.
public class CameraSalles : MonoBehaviour
{
    public Transform cible;

    void LateUpdate()
    {
        if (cible == null) return;
        int s = Mathf.RoundToInt(cible.position.x / GameManager.Ecart);
        transform.position = new Vector3(s * GameManager.Ecart, 0f, -10f);
    }
}
