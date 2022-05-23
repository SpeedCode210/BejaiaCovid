# BejaiaCovid
Un bot instagram permettant d'accéder aux dernières statistiques du covid-19 de l'Algérie ainsi que de la wilaya de Béjaïa.

Pour exécuter une commande, il suffit de l'envoyer en message direct à l'instagram relié au bot.

Compte officiel du bot : https://www.instagram.com/algeria_covid/

## Commandes disponibles

| Commande      | Description                                                      |
|---------------|------------------------------------------------------------------|
| !help         | Affiche la liste des commandes                                   |
| !summary      | Affiche le bilan global depuis le début de la pandémie           |
| !day          | Affiche le bilan du jour                                         |
| !byage        | Affiche les données globale classées par tranche d'âge           |
| !bejaia       | Affiche le bilan de la wilaya de Béjaïa depuis le début du covid |
| !wilaya id    | Affiche le bilan global pour la wilaya choisie                   |
| !wilayaday id | Affiche le bilan du jour pour la wilaya choisie                  |

## Dépendances de compilation

- .NET SDK 6.0
- JetBrains Rider 2021.3+ / Visual Studio 2022

## Dépendances d'exécution

- Serveur Linux ou Windows
- .NET Runtime 6.0

## Librairies et APIs utilisées
- [ramtinak/InstagramApiSharp](https://github.com/ramtinak/InstagramApiSharp) pour les intéractions avec l'API Instagram.
- [API corona-dz.live](https://api.corona-dz.live/) pour la récupération des données relatives au covid-19.
