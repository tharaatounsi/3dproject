using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum Etat { Menu, Jeu, Code, Carte, Victoire, Defaite }

    public const float Ecart = 30f; // distance entre le centre de deux salles

    public static GameManager Instance;
    private static bool sauterMenu = false;

    public Etat etat = Etat.Menu;
    public bool Bloque { get { return etat != Etat.Jeu; } }

    [Header("Énigmes")]
    public string codeSecret = "4271";
    public string indiceCode = "Quatre murs, deux yeux, sept jours, un soleil. Voilà le code.";
    public int[] ordreLeviers = { 0, 1, 2 };
    public bool leviersResolus = false;

    [Header("Score et santé")]
    public int viesMax = 3;
    public int vies = 3;
    public int score = 0;
    private int bonusVies = 0;
    private string dernierGain = "";
    private float finGain = 0f;
    private bool[] visitee = { true, false, false };

    [Header("Progression")]
    public bool[] salleOuverte = { true, false, false };
    public int salle = 0;
    [HideInInspector] public string indiceLeviers = "";
    [HideInInspector] public string objetProche = "";

    private string[] nomsSalles = { "Le salon", "La bibliothèque", "La cave" };
    private Vector3 posJoueur = Vector3.zero;

    private int progression = 0;
    private string message = "";
    private float finMessage = 0f;
    private string saisie = "";
    private string erreurCode = "";
    private string codeAttendu = "";
    private System.Action surCodeReussi;

    private Texture2D fond, bord, vignette, caseOuverte, caseFermee, marqueur, coeurPlein, coeurVide;
    private GUIStyle styleTitre, styleTexte, stylePetit, styleBouton, styleScore;
    private AudioSource sourceSons, sourceMusique;

    Texture2D Couleur(Color c)
    {
        Texture2D t = new Texture2D(1, 1);
        t.SetPixel(0, 0, c);
        t.Apply();
        return t;
    }

    // Petit cœur en pixel art (9 x 8)
    Texture2D CreerCoeur(Color c, Color reflet)
    {
        string[] m = {
            ".XX...XX.",
            "XXXX.XXXX",
            "XXXXXXXXX",
            "XXXXXXXXX",
            ".XXXXXXX.",
            "..XXXXX..",
            "...XXX...",
            "....X...."
        };
        Texture2D t = new Texture2D(9, 8);
        t.filterMode = FilterMode.Point;
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 9; x++)
            {
                Color col = (m[y][x] == 'X') ? c : new Color(0f, 0f, 0f, 0f);
                t.SetPixel(x, 7 - y, col);
            }
        }
        t.SetPixel(1, 6, reflet);
        t.Apply();
        return t;
    }

    void Awake()
    {
        Instance = this;
        fond = Couleur(new Color(0.05f, 0.04f, 0.08f, 0.88f));
        bord = Couleur(new Color(0.85f, 0.68f, 0.25f, 1f));
        caseOuverte = Couleur(new Color(0.32f, 0.24f, 0.14f, 1f));
        caseFermee = Couleur(new Color(0.12f, 0.12f, 0.15f, 1f));
        marqueur = Couleur(new Color(1f, 0.9f, 0.3f, 1f));
        coeurPlein = CreerCoeur(new Color(0.9f, 0.15f, 0.2f, 1f), new Color(1f, 0.7f, 0.7f, 1f));
        coeurVide = CreerCoeur(new Color(0.25f, 0.25f, 0.3f, 1f), new Color(0.25f, 0.25f, 0.3f, 1f));

        // Vignette : bords sombres pour l'ambiance
        int n = 128;
        vignette = new Texture2D(n, n, TextureFormat.RGBA32, false);
        vignette.wrapMode = TextureWrapMode.Clamp;
        for (int y = 0; y < n; y++)
        {
            for (int x = 0; x < n; x++)
            {
                float dx = (x - n / 2f) / (n / 2f);
                float dy = (y - n / 2f) / (n / 2f);
                float d = Mathf.Sqrt(dx * dx + dy * dy);
                float a = Mathf.SmoothStep(0.95f, 1.6f, d) * 0.28f;
                vignette.SetPixel(x, y, new Color(0f, 0f, 0f, a));
            }
        }
        vignette.Apply();

        sourceSons = gameObject.AddComponent<AudioSource>();
        sourceMusique = gameObject.AddComponent<AudioSource>();
        AudioClip m = Resources.Load<AudioClip>("Sons/musique");
        if (m != null)
        {
            sourceMusique.clip = m;
            sourceMusique.loop = true;
            sourceMusique.volume = 0.35f;
            sourceMusique.Play();
        }
    }

    void Start()
    {
        vies = viesMax;
        score = 0;
        etat = sauterMenu ? Etat.Jeu : Etat.Menu;
        if (etat == Etat.Jeu)
            AfficherMessage("Tu es enfermé dans le manoir. Trouve un moyen de sortir !", 5f);
    }

    public void Jouer(string nomSon)
    {
        AudioClip c = Resources.Load<AudioClip>("Sons/" + nomSon);
        if (c != null && sourceSons != null) sourceSons.PlayOneShot(c, 0.8f);
    }

    public void AfficherMessage(string texte, float duree = 4f)
    {
        message = texte;
        finMessage = Time.time + duree;
    }

    // ---------- Score et santé

    public void AjouterScore(int points, string raison)
    {
        score += points;
        dernierGain = "+" + points + "  " + raison;
        finGain = Time.time + 3f;
    }

    public void PerdreVie(string raison)
    {
        if (etat == Etat.Victoire || etat == Etat.Defaite) return;
        vies--;
        Jouer("erreur");
        if (vies <= 0)
        {
            vies = 0;
            etat = Etat.Defaite;
            return;
        }
        AfficherMessage(raison + "  (-1 vie, il t'en reste " + vies + ")", 5f);
    }

    // Points la première fois qu'on entre dans une salle
    public void PremiereVisite(int numeroSalle)
    {
        if (numeroSalle < 0 || numeroSalle >= visitee.Length) return;
        if (visitee[numeroSalle]) return;
        visitee[numeroSalle] = true;
        AjouterScore(numeroSalle == 1 ? 100 : 150, "Nouvelle salle : " + nomsSalles[numeroSalle]);
    }

    public int AppuyerLevier(int index)
    {
        if (index == ordreLeviers[progression])
        {
            progression++;
            if (progression >= ordreLeviers.Length)
            {
                leviersResolus = true;
                AjouterScore(200, "Leviers résolus");
                return 1;
            }
            return 0;
        }
        progression = 0;
        return -1;
    }

    public void OuvrirCode(string code, System.Action quandReussi)
    {
        codeAttendu = code;
        surCodeReussi = quandReussi;
        saisie = "";
        erreurCode = "";
        etat = Etat.Code;
    }

    public void Gagner()
    {
        bonusVies = vies * 100;
        score += bonusVies;
        etat = Etat.Victoire;
        Jouer("victoire");
    }

    void ValiderCode()
    {
        if (saisie == codeAttendu)
        {
            etat = Etat.Jeu;
            Jouer("ouvrir");
            if (surCodeReussi != null) surCodeReussi();
        }
        else
        {
            erreurCode = "Code incorrect ! (-1 vie)";
            saisie = "";
            PerdreVie("Code incorrect !");
        }
    }

    string Objectif()
    {
        if (!salleOuverte[1]) return "Trouve comment ouvrir la porte du salon.";
        if (!salleOuverte[2]) return "Ouvre le coffre-fort de la bibliothèque.";
        if (!leviersResolus) return "Active les leviers de la cave dans le bon ordre.";
        return "Sors par la grande porte de la cave !";
    }

    void Update()
    {
        if (Inventaire.Instance != null)
        {
            posJoueur = Inventaire.Instance.transform.position;
            salle = Mathf.Clamp(Mathf.RoundToInt(posJoueur.x / Ecart), 0, 2);
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            if (etat == Etat.Jeu) etat = Etat.Carte;
            else if (etat == Etat.Carte) etat = Etat.Jeu;
        }
        if (etat == Etat.Carte && Input.GetKeyDown(KeyCode.Escape)) etat = Etat.Jeu;

        if (etat != Etat.Code) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            etat = Etat.Jeu;
            return;
        }
        if (Input.GetKeyDown(KeyCode.Backspace) && saisie.Length > 0)
            saisie = saisie.Substring(0, saisie.Length - 1);

        foreach (char c in Input.inputString)
        {
            if (c >= '0' && c <= '9' && saisie.Length < 4)
                saisie += c;
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            ValiderCode();
    }

    void Cadre(Rect r)
    {
        GUI.DrawTexture(new Rect(r.x - 2, r.y - 2, r.width + 4, r.height + 4), bord);
        GUI.DrawTexture(r, fond);
    }

    void InitStyles()
    {
        if (styleTitre != null) return;

        styleTitre = new GUIStyle(GUI.skin.label);
        styleTitre.fontSize = 40;
        styleTitre.fontStyle = FontStyle.Bold;
        styleTitre.alignment = TextAnchor.MiddleCenter;
        styleTitre.normal.textColor = new Color(0.95f, 0.8f, 0.35f);

        styleTexte = new GUIStyle(GUI.skin.label);
        styleTexte.fontSize = 22;
        styleTexte.wordWrap = true;
        styleTexte.alignment = TextAnchor.MiddleCenter;
        styleTexte.normal.textColor = Color.white;

        stylePetit = new GUIStyle(GUI.skin.label);
        stylePetit.fontSize = 18;
        stylePetit.wordWrap = true;
        stylePetit.alignment = TextAnchor.UpperLeft;
        stylePetit.normal.textColor = Color.white;

        styleScore = new GUIStyle(GUI.skin.label);
        styleScore.fontSize = 22;
        styleScore.fontStyle = FontStyle.Bold;
        styleScore.alignment = TextAnchor.UpperLeft;
        styleScore.normal.textColor = new Color(0.95f, 0.8f, 0.35f);

        styleBouton = new GUIStyle(GUI.skin.button);
        styleBouton.fontSize = 24;
    }

    void OnGUI()
    {
        if (fond == null) return;
        float s = Screen.height / 600f;
        GUI.matrix = Matrix4x4.Scale(new Vector3(s, s, 1f));
        float W = Screen.width / s;
        float H = 600f;
        InitStyles();

        if (etat == Etat.Menu) { DessinerMenu(W, H); return; }
        if (etat == Etat.Victoire) { DessinerVictoire(W, H); return; }
        if (etat == Etat.Defaite) { DessinerDefaite(W, H); return; }

        GUI.DrawTexture(new Rect(0, 0, W, H), vignette);
        DessinerHUD(W, H);
        if (etat == Etat.Code) DessinerCode(W, H);
        if (etat == Etat.Carte) DessinerCarte(W, H);
    }

    void DessinerMenu(float W, float H)
    {
        GUI.DrawTexture(new Rect(0, 0, W, H), fond);
        Cadre(new Rect(W / 2 - 360, 60, 720, 470));
        GUI.Label(new Rect(0, 90, W, 60), "Le Manoir des Brumes", styleTitre);
        GUI.Label(new Rect(W / 2 - 320, 165, 640, 240),
            "Tu es entré dans un vieux manoir, et la porte s'est refermée derrière toi.\n" +
            "Traverse le salon, la bibliothèque et la cave. Résous les énigmes pour t'échapper !\n" +
            "Tu as 3 vies : chaque erreur t'en coûte une.\n\n" +
            "Flèches ou ZQSD : se déplacer     E : interagir     M : carte", styleTexte);
        if (GUI.Button(new Rect(W / 2 - 100, 430, 200, 60), "Jouer", styleBouton))
        {
            etat = Etat.Jeu;
            AfficherMessage("Fouille la pièce. Approche-toi des objets et appuie sur E.", 5f);
        }
    }

    string Rang()
    {
        if (score >= 950) return "Maître de l'évasion";
        if (score >= 800) return "Grand explorateur";
        return "Apprenti détective";
    }

    void DessinerVictoire(float W, float H)
    {
        GUI.DrawTexture(new Rect(0, 0, W, H), fond);
        Cadre(new Rect(W / 2 - 330, 90, 660, 430));
        GUI.Label(new Rect(0, 120, W, 70), "Bravo, tu t'es échappé !", styleTitre);
        GUI.Label(new Rect(0, 200, W, 40), "La grande porte s'ouvre. L'air frais de la nuit t'accueille.", styleTexte);
        GUI.Label(new Rect(0, 250, W, 40), "Vies restantes : " + vies + " / " + viesMax + "   (bonus +" + bonusVies + ")", styleTexte);
        GUI.Label(new Rect(0, 290, W, 40), "Score final : " + score + "     " + Rang(), styleTexte);
        if (GUI.Button(new Rect(W / 2 - 100, 350, 200, 60), "Rejouer", styleBouton))
        {
            sauterMenu = true;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        if (GUI.Button(new Rect(W / 2 - 100, 430, 200, 60), "Quitter", styleBouton))
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }

    void DessinerDefaite(float W, float H)
    {
        GUI.DrawTexture(new Rect(0, 0, W, H), fond);
        Cadre(new Rect(W / 2 - 330, 140, 660, 340));
        GUI.Label(new Rect(0, 180, W, 70), "Tu as perdu...", styleTitre);
        GUI.Label(new Rect(0, 260, W, 40), "Tu n'as plus de vies. Les brumes du manoir t'ont piégé.", styleTexte);
        GUI.Label(new Rect(0, 305, W, 40), "Score : " + score, styleTexte);
        if (GUI.Button(new Rect(W / 2 - 100, 370, 200, 60), "Réessayer", styleBouton))
        {
            sauterMenu = true;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    void DessinerHUD(float W, float H)
    {
        Inventaire inv = Inventaire.Instance;
        string texteInv = (inv != null) ? inv.Texte() : "(vide)";
        bool papier = (inv != null) && inv.ADejaObjet("Papier");

        int lignes = 3;
        if (papier) lignes++;
        if (indiceLeviers != "") lignes++;

        float h = 10 + lignes * 25;
        Cadre(new Rect(12, 12, 740, h));
        float y = 16;
        GUI.Label(new Rect(18, y, 725, 28), nomsSalles[salle] + "     (M : carte)", stylePetit); y += 25;
        GUI.Label(new Rect(18, y, 725, 28), "Objectif : " + Objectif(), stylePetit); y += 25;
        GUI.Label(new Rect(18, y, 725, 28), "Inventaire : " + texteInv, stylePetit); y += 25;
        if (papier) { GUI.Label(new Rect(18, y, 725, 28), "Papier : " + indiceCode, stylePetit); y += 25; }
        if (indiceLeviers != "") { GUI.Label(new Rect(18, y, 725, 28), "Grimoire : " + indiceLeviers, stylePetit); y += 25; }

        // Vies et score
        float yb = 12 + h + 10;
        Cadre(new Rect(12, yb, 560, 44));
        for (int i = 0; i < viesMax; i++)
            GUI.DrawTexture(new Rect(24 + i * 34, yb + 10, 27, 24), i < vies ? coeurPlein : coeurVide);
        GUI.Label(new Rect(135, yb + 9, 170, 30), "Score : " + score, styleScore);
        if (Time.time < finGain)
        {
            GUI.contentColor = new Color(0.6f, 1f, 0.6f);
            GUI.Label(new Rect(310, yb + 12, 260, 30), dernierGain, stylePetit);
            GUI.contentColor = Color.white;
        }

        if (etat == Etat.Jeu && objetProche != "")
        {
            Cadre(new Rect(W / 2 - 200, 410, 400, 40));
            GUI.Label(new Rect(W / 2 - 200, 410, 400, 40), "[E]  " + objetProche, styleTexte);
        }

        if (Time.time < finMessage && message != "")
        {
            Cadre(new Rect(W / 2 - 360, 470, 720, 100));
            GUI.Label(new Rect(W / 2 - 350, 470, 700, 100), message, styleTexte);
        }
    }

    void DessinerCode(float W, float H)
    {
        float px = W / 2 - 170;
        Cadre(new Rect(px, 60, 340, 440));
        GUI.Label(new Rect(px, 70, 340, 40), "Serrure du coffre-fort", styleTexte);

        string aff = "";
        for (int i = 0; i < 4; i++)
            aff += (i < saisie.Length ? saisie[i].ToString() : "_") + "  ";
        GUI.Label(new Rect(px, 115, 340, 60), aff, styleTitre);

        string[] touches = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "C", "0", "OK" };
        for (int i = 0; i < touches.Length; i++)
        {
            int col = i % 3;
            int lig = i / 3;
            Rect r = new Rect(px + 20 + col * 105, 190 + lig * 60, 95, 52);
            if (GUI.Button(r, touches[i], styleBouton))
            {
                string t = touches[i];
                Jouer("clic");
                if (t == "C") saisie = "";
                else if (t == "OK") ValiderCode();
                else if (saisie.Length < 4) saisie += t;
            }
        }

        GUI.Label(new Rect(px, 435, 340, 30), erreurCode, styleTexte);
        GUI.Label(new Rect(px, 465, 340, 30), "Échap : fermer", stylePetit);
    }

    void DessinerCarte(float W, float H)
    {
        GUI.DrawTexture(new Rect(0, 0, W, H), fond);
        Cadre(new Rect(W / 2 - 400, 80, 800, 420));
        GUI.Label(new Rect(0, 95, W, 50), "Carte du manoir", styleTitre);

        float bw = 200f, bh = 130f, ecart = 40f;
        float x0 = W / 2 - (3 * bw + 2 * ecart) / 2f;
        float y0 = 200f;

        for (int i = 0; i < 2; i++)
        {
            Texture2D t = salleOuverte[i + 1] ? caseOuverte : caseFermee;
            GUI.DrawTexture(new Rect(x0 + (i + 1) * bw + i * ecart, y0 + bh / 2 - 6, ecart, 12), t);
        }

        for (int i = 0; i < 3; i++)
        {
            float bx = x0 + i * (bw + ecart);
            if (i == salle)
                GUI.DrawTexture(new Rect(bx - 4, y0 - 4, bw + 8, bh + 8), bord);
            GUI.DrawTexture(new Rect(bx, y0, bw, bh), salleOuverte[i] ? caseOuverte : caseFermee);

            string nom = salleOuverte[i] ? nomsSalles[i] : "???";
            GUI.Label(new Rect(bx, y0 + 6, bw, 30), nom, styleTexte);
            if (!salleOuverte[i])
                GUI.Label(new Rect(bx, y0 + 50, bw, 30), "Verrouillée", styleTexte);
        }

        float nx = Mathf.InverseLerp(salle * Ecart - 9f, salle * Ecart + 9f, posJoueur.x);
        float ny = Mathf.InverseLerp(-4f, 4f, posJoueur.y);
        float mx = x0 + salle * (bw + ecart) + nx * bw;
        float my = y0 + (1f - ny) * bh;
        GUI.DrawTexture(new Rect(mx - 6, my - 6, 12, 12), marqueur);

        GUI.Label(new Rect(0, 360, W, 40), "Le point jaune, c'est toi.", styleTexte);
        GUI.Label(new Rect(W / 2 - 350, 405, 700, 60), "Objectif : " + Objectif(), styleTexte);
        GUI.Label(new Rect(0, 465, W, 30), "M ou Échap : fermer", stylePetit);
    }
}
