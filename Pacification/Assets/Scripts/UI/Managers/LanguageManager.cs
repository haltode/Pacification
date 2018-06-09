using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanguageManager : MonoBehaviour {

    public Text error;
    public Text about;
    public Text[] back;
    public Text[] controls;
    public Text[] settings;
    public Text[] exit;
    public Text[] waiting;
    public Text[] join;
    public Text[] host;
    public Text[] solo;
    public Text[] play;
    public Text[] main;

    public Slider[] sliders;
    public Text[] slidersName;
    public Text[] slidersValues;

    public GameObject[] selected;

    public Text[] loadRandomButton;

    public string loadButton;
    public string randomButton;

    private void Update()
    {
        for(int i = 0; i < sliders.Length; ++i)
            slidersValues[i].text = "" + sliders[i].value;
    }

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

        slidersName[0].text = "Map Size";
        slidersName[1].text = "Jitter Probability";
        slidersName[2].text = "Land Percentage";
        slidersName[3].text = "Erosion Percentage";
        slidersName[4].text = "Maximum Elevation";
        slidersName[5].text = "Chunk Minimum Size";
        slidersName[6].text = "Chunk Maximum Size";
        slidersName[7].text = "Region Count";
        slidersName[8].text = "Region Border";
        slidersName[9].text = "Resources Percentage";
        slidersName[10].text = "Map Size";
        slidersName[11].text = "Jitter Probability";
        slidersName[12].text = "Land Percentage";
        slidersName[13].text = "Erosion Percentage";
        slidersName[14].text = "Maximum Elevation";
        slidersName[15].text = "Chunk Minimum Size";
        slidersName[16].text = "Chunk Maximum Size";
        slidersName[17].text = "Region Count";
        slidersName[18].text = "Region Border";
        slidersName[19].text = "Resources Percentage";

        solo[0].text = "START";
        solo[1].text = "CANCEL";
        solo[2].text = "Seed ( random  if  empty )";
        solo[3].text = "WARNING: Expect loading time";

        host[0].text = "Enter your username";
        host[1].text = "OPEN SERVER";

        join[0].text = "Host Adress";
        join[1].text = "Enter your username";
        join[2].text = "CONNECT";

        waiting[0].text = "CANCEL";
        waiting[1].text = "CANCEL";
        waiting[2].text = "Waiting for other players...";
        waiting[3].text = "Waiting for other players...";
        waiting[4].text = "START";
        waiting[5].text = "START";
        waiting[6].text = "Seed ( random if empty )";

        loadButton = "LOAD";
        randomButton = "RANDOM";
        loadRandomButton[0].text = loadButton;
        loadRandomButton[1].text = loadButton;
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

        slidersName[0].text = "Taille de la Carte";
        slidersName[1].text = "Probabilite de gigue";
        slidersName[2].text = "Pourcentage de terre";
        slidersName[3].text = "Pourcentage d'erosion";
        slidersName[4].text = "Elevation Maximum";
        slidersName[5].text = "Taille minimale des Chunck";
        slidersName[6].text = "Taille maximum des Chunck";
        slidersName[7].text = "Nombre de regions";
        slidersName[8].text = "Bordure des regions";
        slidersName[9].text = "Pourcentage de ressources";
        slidersName[10].text = "Taille de la Carte";
        slidersName[11].text = "Probabilité de gigue";
        slidersName[12].text = "Pourcentage de terre";
        slidersName[13].text = "Pourcentage d'erosion";
        slidersName[14].text = "Elevation Maximum";
        slidersName[15].text = "Taille minimale des Chunck";
        slidersName[16].text = "Taille maximum des Chunck";
        slidersName[17].text = "Nombre de regions";
        slidersName[18].text = "Bordure des regions";
        slidersName[19].text = "Pourcentage de ressources";

        solo[0].text = "DEMARER";
        solo[1].text = "ANNULER";
        solo[2].text = "Seed ( aleatoire si vide )";
        solo[3].text = "ATTENTION: Attendez vous a un temps de chargement";

        host[0].text = "Entrer le pseudo";
        host[1].text = "DEMARER LE SERVEUR";

        join[0].text = "Adresse de l'hote";
        join[1].text = "Entrer le pseudo";
        join[2].text = "SE CONNECTER";

        waiting[0].text = "ANNULER";
        waiting[1].text = "ANNULER";
        waiting[2].text = "En attente d'autres joueurs...";
        waiting[3].text = "En attente d'autres joueurs...";
        waiting[4].text = "DEMARER";
        waiting[5].text = "DEMARER";
        waiting[6].text = "Seed ( aleatoire si vide )";

        loadButton = "CHARGER";
        randomButton = "ALEATOIRE";
        loadRandomButton[0].text = loadButton;
        loadRandomButton[1].text = loadButton;
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

        slidersName[0].text = "KartengroBe";
        slidersName[1].text = "Jitter-Wahrscheinlichkeit";
        slidersName[2].text = "Flachenanteil";
        slidersName[3].text = "Erosionsprozentsatz";
        slidersName[4].text = "Höhenmaximum";
        slidersName[5].text = "Chunk minimale GroBe";
        slidersName[6].text = "Chunk maximale GroBe";
        slidersName[7].text = "Regionsnummer";
        slidersName[8].text = "Regionsgrenze";
        slidersName[9].text = "Ressourcen Prozentsatze";
        slidersName[10].text = "KartengroBe";
        slidersName[11].text = "Jitter-Wahrscheinlichkeit";
        slidersName[12].text = "Flachenanteil";
        slidersName[13].text = "Erosionsprozentsatz";
        slidersName[14].text = "Höhenmaximum";
        slidersName[15].text = "Chunk minimale GroBe";
        slidersName[16].text = "Chunk maximale GroBe";
        slidersName[17].text = "Regionsnummer";
        slidersName[18].text = "Regionsgrenze";
        slidersName[19].text = "Ressourcen Prozentsatze";

        solo[0].text = "ANFANG";
        solo[1].text = "STORNIEREN";
        solo[2].text = "Seed ( zufällig, wenn leer )";
        solo[3].text = "WARNUNG: Ladezeit erwarten";

        host[0].text = "Nutzername";
        host[1].text = "ÖFFNEN SERVER";

        join[0].text = "Adresse des Gastgebers";
        join[1].text = "Nutzername";
        join[2].text = "VERBINDEN";

        waiting[0].text = "STORNIEREN";
        waiting[1].text = "STORNIEREN";
        waiting[2].text = "Warten auf andere Spieler...";
        waiting[3].text = "Warten auf andere Spieler...";
        waiting[4].text = "ANFANG";
        waiting[5].text = "ANFANG";
        waiting[6].text = "Seed ( zufällig, wenn leer )";

        loadButton = "LADEN";
        randomButton = "ZUFALLIG";
        loadRandomButton[0].text = loadButton;
        loadRandomButton[1].text = loadButton;
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

        slidersName[0].text = "Tamano de la tarjeta";
        slidersName[1].text = "Probabilidad de fluctuacion";
        slidersName[2].text = "Porcentaje de tierra";
        slidersName[3].text = "Porcentaje de erosion";
        slidersName[4].text = "Maxima elevacion";
        slidersName[5].text = "Tamano minimo del Chunk";
        slidersName[6].text = "Tamano maximo del Chunk";
        slidersName[7].text = "Numero de region";
        slidersName[8].text = "Frontera de la region";
        slidersName[9].text = "Porcentajes de recursos";
        slidersName[10].text = "Tamano de la tarjeta";
        slidersName[11].text = "Probabilidad de fluctuacion";
        slidersName[12].text = "Porcentaje de tierra";
        slidersName[13].text = "Porcentaje de erosion";
        slidersName[14].text = "Maxima elevacion";
        slidersName[15].text = "Tamano minimo del Chunk";
        slidersName[16].text = "Tamano maximo del Chunk";
        slidersName[17].text = "Numero de region";
        slidersName[18].text = "Frontera de la region";
        slidersName[19].text = "Porcentajes de recursos";

        solo[0].text = "COMIENZO";
        solo[1].text = "ANULAR";
        solo[2].text = "Seed ( al azar si está vacío )";
        solo[3].text = "ADVERTENCIA: Espere tiempo de carga";

        host[0].text = "Usuario";
        host[1].text = "SERVIDOR ABIERTO";

        join[0].text = "Dirección del servidor";
        join[1].text = "Usuario";
        join[2].text = "CONECTAR";

        waiting[0].text = "ANULAR";
        waiting[1].text = "ANULAR";
        waiting[2].text = "Esperando a otros jugadores...";
        waiting[3].text = "Esperando a otros jugadores...";
        waiting[4].text = "COMIENZO";
        waiting[5].text = "COMIENZO";
        waiting[6].text = "Seed ( al azar si está vacío )";

        loadButton = "CARGAR";
        randomButton = "EL AZAR";
        loadRandomButton[0].text = loadButton;
        loadRandomButton[1].text = loadButton;
    }

}
