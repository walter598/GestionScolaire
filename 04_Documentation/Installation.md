# Installation et exécution

## Environnement
- Windows 11
- Visual Studio 2022 Community 17.14 avec la charge de travail « Développement .NET Desktop »
- .NET 8 SDK
- WampServer 3.4 avec MySQL 8.4 actif
- phpMyAdmin
- NuGet `MySql.Data` 8.4.0 (référencé dans le projet Core)

## 1. Base de données
1. Démarrer WampServer et attendre l'icône verte.
2. Ouvrir `http://localhost/phpmyadmin`.
3. Sélectionner l'onglet SQL et exécuter `01_BaseDeDonnees/gestion_scolaire.sql`.
4. Avant exécution, remplacer `CHANGE_ME_Strong#2026` par un mot de passe fort connu de vous.
5. Vérifier que `gestion_scolaire` contient filiere, etudiant, matiere et note.

Le script est destiné à une première installation. Il ne supprime pas la base existante. Il utilise `CREATE TABLE IF NOT EXISTS` et `INSERT IGNORE`.

## 2. Configurer les identifiants
Par défaut, le projet lit les variables d'environnement :
- `GS_DB_SERVER` (localhost)
- `GS_DB_PORT` (3306)
- `GS_DB_NAME` (gestion_scolaire)
- `GS_DB_USER` (app_scolaire)
- `GS_DB_PASSWORD` (CHANGE_ME_Strong#2026)

Dans PowerShell, avant de lancer Visual Studio depuis le même terminal :
```powershell
$env:GS_DB_SERVER="localhost"
$env:GS_DB_PORT="3306"
$env:GS_DB_NAME="gestion_scolaire"
$env:GS_DB_USER="app_scolaire"
$env:GS_DB_PASSWORD="VOTRE_MOT_DE_PASSE"
```
Ne pas utiliser `root` dans les applications.

## 3. Ouvrir et compiler
1. Ouvrir `GestionScolaire.sln` dans Visual Studio 2022.
2. Restaurer les packages NuGet.
3. Définir `GestionScolaire_WinForms` ou `GestionScolaire_Console` comme projet de démarrage.
4. Compiler la solution (`Ctrl+Shift+B`).
5. Lancer avec `F5`.

## 4. Limites de test
Le projet a été généré dans un environnement qui ne dispose pas de Windows, de Visual Studio, du runtime .NET ni d'un serveur MySQL. La compilation Windows Forms et les opérations réelles sur MySQL doivent donc être vérifiées sur le poste Windows cible après configuration.
