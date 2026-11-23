# Architecture en couches

Le code d'accès aux données est placé dans le projet réutilisable `04_Core` afin que Console et Windows Forms utilisent exactement les mêmes règles SQL et la même configuration, sans dépendre l'une de l'autre à l'exécution.

- `04_Core/Models` : définitions d'entités et champs.
- `04_Core/Data` : fabrique de connexions et opérations ADO.NET.
- `04_Core/Repositories` : requêtes paramétrées et CRUD.
- `03_Application_WindowsForms/.../Forms` : présentation et événements d'interface.
- `02_Application_Console/...` : menu console avec `switch`.
- `Services`, `Controls`, `Resources`, `Helpers` : répertoires préparés pour les extensions UI et métier.

Le SQL est absent des événements des formulaires : ceux-ci appellent `SchoolRepository` et `DbOperations`.
