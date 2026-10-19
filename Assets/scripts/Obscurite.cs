using UnityEngine;

// Noir autour du joueur dans la cave (seule la lanterne éclaire).
public class Obscurite : MonoBehaviour
{
    public Transform cible;
    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void LateUpdate()
    {
        if (cible != null)
            transform.position = new Vector3(cible.position.x, cible.position.y, 0f);
        if (GameManager.Instance != null)
            sr.enabled = (GameManager.Instance.salle == 2);
    }
}
