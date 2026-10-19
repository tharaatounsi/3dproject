using UnityEngine;

public class Torche : MonoBehaviour
{
    public Sprite[] frames;
    public SpriteRenderer lueur;

    private SpriteRenderer sr;
    private float minuteur = 0f;
    private int i = 0;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        minuteur += Time.deltaTime;
        if (frames != null && frames.Length > 1 && minuteur > 0.15f)
        {
            minuteur = 0f;
            i = (i + 1) % frames.Length;
            sr.sprite = frames[i];
        }

        if (lueur != null)
        {
            Color c = lueur.color;
            c.a = 0.30f + 0.08f * Mathf.Sin(Time.time * 9f + transform.position.x);
            lueur.color = c;
        }
    }
}
