## Gameplay

- map
    - génération procédurale
    - biome: montagne, forêt, plaine, rivière, désert
    - route
    - ressources
        - luxe (eg: argent, or, diamant, etc.)
        - stratégique (eg: chevaux, fer, charbon, etc.)
- 3 types d'unités
    - colon (=> construit les villes)
    - attaquant (=> baston)
    - ouvrier (=> améliore les cases)
- évolution
    - sciences milestones
- autorégulation
    - argent
    - seuil de crise économique qui donne un malus (eg: production, science)
- interface
    - menu ville (résumé ville + choix production)
    - alerte simple assez passive
    - sauvegarde, menu
- musique/bruitage
- modèle 3D villes/animation/particules
- réseau
- devmode

## Implémentation

- classe ville
    - production
    - sciences
    - argent
    - lvl
    - nb habitant (milestones habitants)
    - qualité de vie (pour représenter le bonheur, nourriture)
    - liste bâtiments
- classe bâtiments
    - influence directement les caractéristiques d'une ville
- classe units
    - points de déplacement
    - coût d'entretien
    - héritage
        - classe ouvrier
        - classe colon
        - classe attaquant

## Rôles

- map: thibault, antoine
- ia: thibault
- réseau: valérian/cédric/antoine
- graphisme/animation/particule: cédric
- esclave latex: All of us
- site web: valérian
