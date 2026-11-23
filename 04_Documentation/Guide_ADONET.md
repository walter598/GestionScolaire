# Guide des classes ADO.NET utilisées

- **MySqlConnection** : ouvre la session vers MySQL. `Db.CreateConnection()` fabrique une nouvelle connexion pour chaque opération ; `using` assure sa libération.
- **MySqlConnectionStringBuilder** : construit la chaîne de connexion à partir de paramètres.
- **MySqlCommand** : représente une instruction SQL et ses paramètres.
- **MySqlParameter** : transmet les valeurs séparément du SQL et protège contre l'injection SQL.
- **ExecuteScalar()** : récupère une valeur unique, par exemple `COUNT(*)`.
- **ExecuteNonQuery()** : exécute INSERT, UPDATE ou DELETE et retourne le nombre de lignes touchées.
- **ExecuteReader() / MySqlDataReader** : lecture connectée, séquentielle, en lecture seule.
- **MySqlDataAdapter** : pont entre MySQL et les objets en mémoire ; `Fill` remplit une table.
- **DataTable** : table de données en mémoire.
- **DataSet** : ensemble de tables en mémoire ; permet notamment d'établir des `DataRelation`.
- **DataView** : vue triée ou filtrée d'un `DataTable`.
- **BindingSource** : intermédiaire entre les données et les contrôles WinForms ; ici il alimente le DataGridView.
- **MySqlException** : exception spécifique du fournisseur ; `Number` identifie le code MySQL.
- **MySqlCommandBuilder** : le support l'étudie pour générer les commandes d'un adaptateur simple à partir d'un SELECT avec clé primaire. Il n'est pas employé dans les opérations CRUD métier, qui utilisent des requêtes paramétrées explicites.
