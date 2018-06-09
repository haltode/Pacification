using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanguageManager : MonoBehaviour {

    public Text error;          //
    public Text about;          //
    public Text[] back;         //
    public Text[] controls;     //
    public Text[] settings;     //
    public Text[] exit;         //
    public Text[] waiting;      //FUCK
    public Text[] join;         //FUCK
    public Text[] host;         //FUCK
    public Text[] solo;         //FUCK
    public Text[] play;         //
    public Text[] main;         //

    public GameObject[] selected;

    public void English()
    {
        selected[0].SetActive(true);
        selected[1].SetActive(false);
        selected[2].SetActive(false);
        selected[3].SetActive(false);

        error.text = "You've been disconnected";
        about.text = "Developed by Brainless Devs";

        foreach(Text t in back)
            t.text = "BACK";

        exit[0].text = "Are you sure ?";
        exit[1].text = "Yes";
        exit[2].text = "No";

        controls[0].text = "Move Camera";
        controls[1].text = "Unit Actions";
        controls[2].text = "Open Chat";
        controls[3].text = "Navigate between each cities";
        controls[4].text = "Navigate between each units";
        controls[5].text = "End Turn";

        settings[0].text = "GAME VOLUME";
        settings[1].text = "BRIGHTNESS";
        settings[2].text = "Controls";

        main[0].text = "PLAY";
        main[1].text = "EDITOR";
        main[2].text = "SETTINGS";
        main[3].text = "EXIT";

        play[0].text = "SOLO";
        play[1].text = "HOST";
        play[2].text = "JOIN";
    }

    public void French()
    {
        selected[0].SetActive(false);
        selected[1].SetActive(true);
        selected[2].SetActive(false);
        selected[3].SetActive(false);

        error.text = "Vous avez ete deconnecte";
        about.text = "Developpe par Brainless Devs";

        foreach(Text t in back)
            t.text = "RETOUR";

        exit[0].text = "Vous nous quittez ?";
        exit[1].text = "Oui";
        exit[2].text = "Non";

        controls[0].text = "Deplacer la Camera";
        controls[1].text = "Actions des unites";
        controls[2].text = "Ouvrir le tchat";
        controls[3].text = "Naviguer entre les villes";
        controls[4].text = "Naviguer entre les unites";
        controls[5].text = "Finir le tour";

        settings[0].text = "VOLUME DU JEU";
        settings[1].text = "ECLAIRAGE";
        settings[2].text = "Controles";

        main[0].text = "JOUER";
        main[1].text = "EDITEUR";
        main[2].text = "PARAMETRES";
        main[3].text = "QUITTER";

        play[0].text = "SOLO";
        play[1].text = "HEBERGER";
        play[2].text = "REJOINDRE";
    }

    public void German()
    {
        selected[0].SetActive(false);
        selected[1].SetActive(false);
        selected[2].SetActive(false);
        selected[3].SetActive(true);

        error.text = "Sie haben die Verbindung verloren";
        about.text = "Entwickelt von Brainless Devs";

        foreach(Text t in back)
            t.text = "ZURUCK";

        exit[0].text = "Gehst du ?";
        exit[1].text = "Ja";
        exit[2].text = "Nein";

        controls[0].text = "Bewege die Kamera";
        controls[1].text = "Einheiten Aktionen";
        controls[2].text = "Chat offnen";
        controls[3].text = "Navigiere zwischen den Stadten";
        controls[4].text = "Navigiere zwischen den Einheiten";
        controls[5].text = "Endrunde";

        settings[0].text = "SPIELVOLUMEN";
        settings[1].text = "HELLIGKEIT";
        settings[2].text = "Kontrollen";

        main[0].text = "SPIELEN";
        main[1].text = "EDITOR";
        main[2].text = "EINSTELLUNGEN";
        main[3].text = "VERLASSEN";

        play[0].text = "SOLO";
        play[1].text = "GASTGEBER";
        play[2].text = "BEITRETEN";
    }

    public void Despacito()
    {
        selected[0].SetActive(false);
        selected[1].SetActive(false);
        selected[2].SetActive(true);
        selected[3].SetActive(false);

        error.text = "Has sido desconectado";
        about.text = "Desarrollado por Brainless Devs";

        foreach(Text t in back)
            t.text = "ATRAS";

        exit[0].text = "Estas seguro ?";
        exit[1].text = "Si";
        exit[2].text = "No";

        controls[0].text = "Mover la camara";
        controls[1].text = "Acciones de las unidades";
        controls[2].text = "Abrir el chat";
        controls[3].text = "Navegar entre las ciudades";
        controls[4].text = "Navegar entre las unidades";
        controls[5].text = "Acabar el turno";

        settings[0].text = "VOLUMEN DEL JUEGO";
        settings[1].text = "BRILLO";
        settings[2].text = "Controles";

        main[0].text = "JUGAR";
        main[1].text = "EDITOR";
        main[2].text = "AJUSTES";
        main[3].text = "SALIR";

        play[0].text = "SOLO";
        play[1].text = "ALOJAR";
        play[2].text = "UNIRSE";
    }

}
