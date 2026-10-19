using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;

public static class ConstruireJeu
{
    const string DossierPixel = "Assets/sprites/pixel";

    [MenuItem("Jeu/Construire Le Manoir des Brumes")]
    public static void Construire()
    {
        PreparerSprites();
        Sprite carre = CreerSpriteCarre();
        Sprite perso = ChercherPerso();

        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // Caméra
        GameObject camGO = new GameObject("Main Camera");
        camGO.tag = "MainCamera";
        Camera cam = camGO.AddComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 5f;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.04f, 0.04f, 0.07f);
        camGO.transform.position = new Vector3(0, 0, -10);
        camGO.AddComponent<AudioListener>();
        CameraSalles camScript = camGO.AddComponent<CameraSalles>();

        // ===== Salle 0 : le salon =====
        float ox = 0f;
        Enveloppe("Salon", ox, S("sol", carre), Color.white, Color.white, carre);
        Objet("Tapis Salon", S("tapis", carre), new Vector2(ox, 0), -5, false);
        Decor("Etagere", S("etagere", carre), new Vector2(ox - 3.3f, 3.0f));
        Decor("Etagere", S("etagere", carre), new Vector2(ox + 3.3f, 3.0f));
        Decor("Tonneau", S("tonneau", carre), new Vector2(ox - 7.9f, -2.9f));
        Decor("Tonneau", S("tonneau", carre), new Vector2(ox + 7.9f, -2.9f));
        CreerTorche(new Vector2(ox - 1.5f, 2.95f), carre);
        CreerTorche(new Vector2(ox + 1.5f, 2.95f), carre);

        Tiroir tiroir = CreerInter<Tiroir>("Commode", S("tiroir_ferme", carre), new Vector2(ox - 6, 3.0f), true, "Commode");
        tiroir.spriteOuvert = S("tiroir_ouvert", carre);
        Coffre coffre = CreerInter<Coffre>("Coffre", S("coffre_ferme", carre), new Vector2(ox + 6, 2.9f), true, "Coffre");
        coffre.spriteOuvert = S("coffre_ouvert", carre);
        Panneau plaque = CreerInter<Panneau>("Plaque", S("panneau", carre), new Vector2(ox, 2.95f), false, "Plaque de pierre");
        plaque.texte = "Manoir des Brumes. Nul n'en sort sans avoir résolu ses secrets.";

        PorteSalle p01 = CreerInter<PorteSalle>("Porte Salon vers Bibliotheque", S("porte", carre), new Vector2(ox + 8f, 0f), true, "Porte de la bibliothèque");
        p01.salleCible = 1;
        p01.arrivee = new Vector2(30f - 6f, 0f);
        p01.cleRequise = "Clé de la bibliothèque";
        p01.consommerCle = true;
        p01.messageVerrouille = "La porte est fermée à clé. Il faut trouver la clé de la bibliothèque.";
        p01.messageEntree = "Tu entres dans la bibliothèque.";

        // ===== Salle 1 : la bibliothèque =====
        ox = 30f;
        Enveloppe("Bibliotheque", ox, S("sol", carre), new Color(0.8f, 0.85f, 1f), new Color(0.85f, 0.88f, 1f), carre);
        Objet("Tapis Bibliotheque", S("tapis_bleu", carre), new Vector2(ox, 0), -5, false);
        Decor("Etagere", S("etagere", carre), new Vector2(ox - 3.3f, 3.0f));
        Decor("Etagere", S("etagere", carre), new Vector2(ox + 3.3f, 3.0f));
        Decor("Etagere", S("etagere", carre), new Vector2(ox - 5.5f, -3.0f));
        Decor("Etagere", S("etagere", carre), new Vector2(ox + 5.5f, -3.0f));
        CreerTorche(new Vector2(ox - 1.5f, 2.95f), carre);
        CreerTorche(new Vector2(ox + 1.5f, 2.95f), carre);

        Coffrefort cf = CreerInter<Coffrefort>("Coffre-fort", S("coffrefort_ferme", carre), new Vector2(ox - 6, 3.0f), true, "Coffre-fort");
        cf.spriteOuvert = S("coffrefort_ouvert", carre);
        Panneau grimoire = CreerInter<Panneau>("Grimoire", S("grimoire", carre), new Vector2(ox + 6, 3.0f), true, "Vieux grimoire");
        grimoire.texte = "Le feu s'éveille d'abord, puis l'eau coule, enfin l'herbe pousse.";
        grimoire.memoriser = true;

        PorteSalle p10 = CreerInter<PorteSalle>("Porte Bibliotheque vers Salon", S("porte", carre), new Vector2(ox - 8f, 0f), true, "Retour au salon");
        p10.salleCible = 0;
        p10.arrivee = new Vector2(6f, 0f);
        p10.messageEntree = "Tu retournes dans le salon.";

        PorteSalle p12 = CreerInter<PorteSalle>("Porte Bibliotheque vers Cave", S("porte", carre), new Vector2(ox + 8f, 0f), true, "Porte de la cave");
        p12.salleCible = 2;
        p12.arrivee = new Vector2(60f - 6f, 0f);
        p12.cleRequise = "Lanterne";
        p12.consommerCle = false;
        p12.messageVerrouille = "Derrière cette porte, c'est le noir complet. Il te faudrait une lanterne.";
        p12.messageEntree = "Tu descends dans la cave. Il fait très sombre.";

        // ===== Salle 2 : la cave =====
        ox = 60f;
        Enveloppe("Cave", ox, S("sol_cave", carre), Color.white, new Color(0.65f, 0.7f, 0.85f), carre);
        Decor("Rocher", S("rocher", carre), new Vector2(ox - 7.8f, 3.0f));
        Decor("Rocher", S("rocher", carre), new Vector2(ox - 7.8f, -2.9f));
        Decor("Rocher", S("rocher", carre), new Vector2(ox + 7.6f, 3.0f));
        Decor("Rocher", S("rocher", carre), new Vector2(ox + 3.5f, 3.0f));
        Decor("Tonneau", S("tonneau", carre), new Vector2(ox - 3.2f, 3.0f));

        CreerLevier("rouge", 0, new Vector2(ox - 3, -2.8f), carre);
        CreerLevier("bleu", 1, new Vector2(ox, -2.8f), carre);
        CreerLevier("vert", 2, new Vector2(ox + 3, -2.8f), carre);

        PorteSalle p21 = CreerInter<PorteSalle>("Porte Cave vers Bibliotheque", S("porte", carre), new Vector2(ox - 8f, 0f), true, "Retour à la bibliothèque");
        p21.salleCible = 1;
        p21.arrivee = new Vector2(30f + 6f, 0f);
        p21.messageEntree = "Tu remontes dans la bibliothèque.";

        CreerInter<Porte>("Grande Porte", S("porte", carre), new Vector2(ox + 8f, 0f), true, "Grande porte de sortie");

        // ===== Joueur =====
        GameObject joueur = new GameObject("Player");
        joueur.tag = "Player";
        joueur.transform.position = new Vector3(0, 0, 0);
        SpriteRenderer sr = joueur.AddComponent<SpriteRenderer>();
        sr.sortingOrder = 5;
        BoxCollider2D col = joueur.AddComponent<BoxCollider2D>();
        float echelle;
        if (perso != null)
        {
            sr.sprite = perso;
            echelle = 0.9f / perso.bounds.size.y;
            joueur.transform.localScale = new Vector3(echelle, echelle, 1f);
            col.size = new Vector2(perso.bounds.size.x * 0.7f, perso.bounds.size.y * 0.8f);
        }
        else
        {
            sr.sprite = carre;
            sr.color = new Color(0.3f, 0.8f, 0.9f);
            echelle = 0.7f;
            joueur.transform.localScale = new Vector3(echelle, echelle, 1f);
            col.size = Vector2.one;
        }
        Rigidbody2D rb = joueur.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        joueur.AddComponent<PlayerMovement>();
        joueur.AddComponent<PlayerInteraction>();
        joueur.AddComponent<Inventaire>();
        camScript.cible = joueur.transform;

        // Ombre sous le joueur
        GameObject ombre = new GameObject("Ombre");
        ombre.transform.parent = joueur.transform;
        SpriteRenderer so = ombre.AddComponent<SpriteRenderer>();
        so.sprite = S("ombre", carre);
        so.sortingOrder = 4;
        ombre.transform.localPosition = new Vector3(0, -0.4f / echelle, 0);
        ombre.transform.localScale = new Vector3(0.8f / echelle, 0.6f / echelle, 1f);

        // Obscurité de la cave (suit le joueur)
        Sprite obs = AssetDatabase.LoadAssetAtPath<Sprite>(DossierPixel + "/obscurite.png");
        if (obs != null)
        {
            GameObject nuit = new GameObject("Obscurite");
            SpriteRenderer sn = nuit.AddComponent<SpriteRenderer>();
            sn.sprite = obs;
            sn.sortingOrder = 100;
            nuit.transform.localScale = new Vector3(6f, 6f, 1f);
            Obscurite o = nuit.AddComponent<Obscurite>();
            o.cible = joueur.transform;
            sn.enabled = false;
        }

        // Gestionnaire du jeu
        GameObject gm = new GameObject("GameManager");
        gm.AddComponent<GameManager>();

        // Sauvegarde de la scène
        if (!Directory.Exists("Assets/Scenes")) Directory.CreateDirectory("Assets/Scenes");
        string chemin = "Assets/Scenes/ManoirDesBrumes.unity";
        EditorSceneManager.SaveScene(scene, chemin);
        EditorBuildSettings.scenes = new EditorBuildSettingsScene[] { new EditorBuildSettingsScene(chemin, true) };
        AssetDatabase.SaveAssets();

        EditorUtility.DisplayDialog("Jeu construit",
            "La scène 'ManoirDesBrumes' est créée dans Assets/Scenes.\nClique sur Play pour jouer !", "OK");
    }

    // Sol, murs de la salle centrée sur x = ox
    static void Enveloppe(string nom, float ox, Sprite sol, Color teinteSol, Color teinteMur, Sprite carre)
    {
        GameObject racine = new GameObject("Salle " + nom);
        GameObject s = Tuile("Sol", sol, new Vector2(ox, 0), new Vector2(17, 7), -10, false);
        s.GetComponent<SpriteRenderer>().color = teinteSol;
        s.transform.parent = racine.transform;

        Sprite mur = S("mur", carre);
        GameObject[] murs = new GameObject[] {
            Tuile("Mur Haut", mur, new Vector2(ox, 4), new Vector2(18, 1), 0, true),
            Tuile("Mur Bas", mur, new Vector2(ox, -4), new Vector2(18, 1), 0, true),
            Tuile("Mur Gauche", mur, new Vector2(ox - 9, 0), new Vector2(1, 9), 0, true),
            Tuile("Mur Droite", mur, new Vector2(ox + 9, 0), new Vector2(1, 9), 0, true)
        };
        foreach (GameObject m in murs)
        {
            m.GetComponent<SpriteRenderer>().color = teinteMur;
            m.transform.parent = racine.transform;
        }
    }

    static GameObject Decor(string nom, Sprite spr, Vector2 pos)
    {
        return Objet(nom, spr, pos, 1, true);
    }

    // ---------- Création d'objets

    static GameObject Tuile(string nom, Sprite spr, Vector2 pos, Vector2 taille, int ordre, bool collider)
    {
        GameObject go = new GameObject(nom);
        go.transform.position = new Vector3(pos.x, pos.y, 0);
        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = spr;
        sr.drawMode = SpriteDrawMode.Tiled;
        sr.size = taille;
        sr.sortingOrder = ordre;
        if (collider)
        {
            BoxCollider2D c = go.AddComponent<BoxCollider2D>();
            c.size = taille;
        }
        return go;
    }

    static GameObject Objet(string nom, Sprite spr, Vector2 pos, int ordre, bool collider)
    {
        GameObject go = new GameObject(nom);
        go.transform.position = new Vector3(pos.x, pos.y, 0);
        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = spr;
        sr.sortingOrder = ordre;
        if (collider)
        {
            BoxCollider2D c = go.AddComponent<BoxCollider2D>();
            c.size = new Vector2(spr.bounds.size.x * 0.9f, spr.bounds.size.y * 0.8f);
        }
        return go;
    }

    static T CreerInter<T>(string nom, Sprite spr, Vector2 pos, bool solide, string nomAffiche) where T : Interactable
    {
        GameObject go = Objet(nom, spr, pos, 1, solide);
        BoxCollider2D zone = go.AddComponent<BoxCollider2D>();
        zone.isTrigger = true;
        zone.size = new Vector2(spr.bounds.size.x + 1.4f, spr.bounds.size.y + 1.4f);
        T script = go.AddComponent<T>();
        script.nom = nomAffiche;
        return script;
    }

    static void CreerLevier(string couleur, int index, Vector2 pos, Sprite carre)
    {
        Sprite off = S("levier_" + couleur + "_off", carre);
        Sprite on = S("levier_" + couleur + "_on", carre);
        Levier l = CreerInter<Levier>("Levier " + couleur, off, pos, false, "Levier " + couleur);
        l.index = index;
        l.spriteOff = off;
        l.spriteOn = on;
    }

    static void CreerTorche(Vector2 pos, Sprite carre)
    {
        GameObject go = Objet("Torche", S("torche_1", carre), pos, 2, false);
        Torche t = go.AddComponent<Torche>();
        t.frames = new Sprite[] { S("torche_1", carre), S("torche_2", carre) };

        GameObject lu = new GameObject("Lueur");
        lu.transform.parent = go.transform;
        lu.transform.localPosition = new Vector3(0, 0.2f, 0);
        lu.transform.localScale = new Vector3(5f, 5f, 1f);
        SpriteRenderer sl = lu.AddComponent<SpriteRenderer>();
        sl.sprite = S("lueur", carre);
        sl.color = new Color(1f, 0.8f, 0.5f, 0.3f);
        sl.sortingOrder = -2;
        t.lueur = sl;
    }

    // ---------- Sprites

    static Sprite S(string nom, Sprite secours)
    {
        Sprite s = AssetDatabase.LoadAssetAtPath<Sprite>(DossierPixel + "/" + nom + ".png");
        return s != null ? s : secours;
    }

    static void PreparerSprites()
    {
        AssetDatabase.Refresh();
        if (!Directory.Exists(DossierPixel)) return;
        foreach (string f in Directory.GetFiles(DossierPixel, "*.png"))
        {
            string chemin = f.Replace("\\", "/");
            TextureImporter ti = AssetImporter.GetAtPath(chemin) as TextureImporter;
            if (ti == null) continue;
            ti.textureType = TextureImporterType.Sprite;
            ti.spritePixelsPerUnit = 32f;
            string nomFichier = Path.GetFileNameWithoutExtension(chemin);
            ti.filterMode = (nomFichier == "obscurite" || nomFichier == "lueur") ? FilterMode.Bilinear : FilterMode.Point;
            ti.mipmapEnabled = false;
            ti.wrapMode = TextureWrapMode.Repeat;
            ti.textureCompression = TextureImporterCompression.Uncompressed;
            TextureImporterSettings st = new TextureImporterSettings();
            ti.ReadTextureSettings(st);
            st.spriteMeshType = SpriteMeshType.FullRect;
            ti.SetTextureSettings(st);
            ti.SaveAndReimport();
        }
    }

    static Sprite CreerSpriteCarre()
    {
        string dossier = "Assets/sprites";
        string chemin = dossier + "/carre.png";
        if (!Directory.Exists(dossier)) Directory.CreateDirectory(dossier);
        if (!File.Exists(chemin))
        {
            Texture2D tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, Color.white);
            tex.Apply();
            File.WriteAllBytes(chemin, tex.EncodeToPNG());
            Object.DestroyImmediate(tex);
            AssetDatabase.ImportAsset(chemin);
        }
        TextureImporter ti = AssetImporter.GetAtPath(chemin) as TextureImporter;
        if (ti != null)
        {
            ti.textureType = TextureImporterType.Sprite;
            ti.spritePixelsPerUnit = 1f;
            ti.filterMode = FilterMode.Point;
            ti.SaveAndReimport();
        }
        return AssetDatabase.LoadAssetAtPath<Sprite>(chemin);
    }

    static Sprite ChercherPerso()
    {
        string[] guids = AssetDatabase.FindAssets("tile_0085");
        foreach (string guid in guids)
        {
            string chemin = AssetDatabase.GUIDToAssetPath(guid);
            foreach (Object o in AssetDatabase.LoadAllAssetsAtPath(chemin))
            {
                Sprite s = o as Sprite;
                if (s != null && s.name == "tile_0085") return s;
            }
        }
        return null;
    }
}
