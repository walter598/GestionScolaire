# Guide utilisateur

## Windows Forms
- **Tableau de bord** : affiche les compteurs et l'état de connexion.
- **Étudiants, Filières, Matières, Notes** : sélectionnez une ligne, saisissez ou modifiez les valeurs, puis utilisez Ajouter, Modifier ou Supprimer.
- Pour supprimer, une confirmation est demandée.
- Le champ de recherche filtre les champs définis pour le module.
- **Tests ADO.NET** : chaque bouton lance un test réel et ajoute le résultat au journal.
- **Paramètres** : indique le serveur et la base. Les identifiants sont fournis par variables d'environnement.

## Console
Lancer `GestionScolaire_Console`, puis choisir une opération dans le menu numéroté. Le menu est piloté par `switch`.

## Important pour le module Notes
Les champs `ID étudiant` et `ID matière` attendent les identifiants numériques présents dans les tables. Vérifier les identifiants dans phpMyAdmin ou dans les listes avant d'enregistrer.
