using System.Data;
using GestionScolaire.Core.Data;
using GestionScolaire.Core.Models;
using MySql.Data.MySqlClient;

namespace GestionScolaire.Core.Repositories;

public static class SchoolRepository
{
    public static readonly EntityDefinition[] Entities =
    [
        new() {
            Key="etudiants", Title="Gestion des étudiants", Table="etudiant",
            SelectSql="SELECT e.id,e.matricule,e.nom,e.prenom,e.date_naissance,e.email,e.filiere_id,f.libelle AS filiere,e.date_inscription FROM etudiant e JOIN filiere f ON f.id=e.filiere_id ORDER BY e.id DESC",
            InsertSql="INSERT INTO etudiant(matricule,nom,prenom,date_naissance,email,filiere_id) VALUES(@matricule,@nom,@prenom,@date_naissance,@email,@filiere_id)",
            UpdateSql="UPDATE etudiant SET matricule=@matricule,nom=@nom,prenom=@prenom,date_naissance=@date_naissance,email=@email,filiere_id=@filiere_id WHERE id=@id",
            DeleteSql="DELETE FROM etudiant WHERE id=@id",
            Fields=[new("matricule","Matricule","text",true),new("nom","Nom","text",true),new("prenom","Prénom","text",true),new("date_naissance","Naissance","date"),new("email","Email"),new("filiere_id","ID filière","number",true)],
            InsertParameters=["matricule","nom","prenom","date_naissance","email","filiere_id"],
            UpdateParameters=["matricule","nom","prenom","date_naissance","email","filiere_id","id"]
        },
        new() {
            Key="filieres", Title="Gestion des filières", Table="filiere",
            SelectSql="SELECT id,code,libelle FROM filiere ORDER BY id DESC",
            InsertSql="INSERT INTO filiere(code,libelle) VALUES(@code,@libelle)",
            UpdateSql="UPDATE filiere SET code=@code,libelle=@libelle WHERE id=@id",
            DeleteSql="DELETE FROM filiere WHERE id=@id",
            Fields=[new("code","Code","text",true),new("libelle","Libellé","text",true)],
            InsertParameters=["code","libelle"], UpdateParameters=["code","libelle","id"]
        },
        new() {
            Key="matieres", Title="Gestion des matières", Table="matiere",
            SelectSql="SELECT id,code,intitule,coefficient FROM matiere ORDER BY id DESC",
            InsertSql="INSERT INTO matiere(code,intitule,coefficient) VALUES(@code,@intitule,@coefficient)",
            UpdateSql="UPDATE matiere SET code=@code,intitule=@intitule,coefficient=@coefficient WHERE id=@id",
            DeleteSql="DELETE FROM matiere WHERE id=@id",
            Fields=[new("code","Code","text",true),new("intitule","Intitulé","text",true),new("coefficient","Coefficient","decimal",true)],
            InsertParameters=["code","intitule","coefficient"], UpdateParameters=["code","intitule","coefficient","id"]
        },
        new() {
            Key="notes", Title="Gestion des notes", Table="note",
            SelectSql="SELECT n.id,n.etudiant_id,CONCAT(e.prenom,' ',e.nom) AS etudiant,n.matiere_id,m.intitule AS matiere,n.valeur FROM note n JOIN etudiant e ON e.id=n.etudiant_id JOIN matiere m ON m.id=n.matiere_id ORDER BY n.id DESC",
            InsertSql="INSERT INTO note(etudiant_id,matiere_id,valeur) VALUES(@etudiant_id,@matiere_id,@valeur)",
            UpdateSql="UPDATE note SET etudiant_id=@etudiant_id,matiere_id=@matiere_id,valeur=@valeur WHERE id=@id",
            DeleteSql="DELETE FROM note WHERE id=@id",
            Fields=[new("etudiant_id","ID étudiant","number",true),new("matiere_id","ID matière","number",true),new("valeur","Note / 20","decimal",true)],
            InsertParameters=["etudiant_id","matiere_id","valeur"], UpdateParameters=["etudiant_id","matiere_id","valeur","id"]
        }
    ];

    public static DataTable Get(EntityDefinition entity, string? search = null)
    {
        var sql = entity.SelectSql;
        if (!string.IsNullOrWhiteSpace(search))
        {
            sql = $"SELECT * FROM ({entity.SelectSql.TrimEnd(';')}) AS q WHERE " +
                  string.Join(" OR ", entity.Fields.Select(f => $"CAST(q.`{f.Name}` AS CHAR) LIKE @q"));
            return DbOperations.Query(sql, DbOperations.P("@q", $"%{search.Trim()}%"));
        }
        return DbOperations.Query(sql);
    }

    public static int Save(EntityDefinition entity, IReadOnlyDictionary<string, object?> values, int? id)
    {
        var names = id.HasValue ? entity.UpdateParameters : entity.InsertParameters;
        var parameters = names.Select(n => DbOperations.P("@" + n,
            values.TryGetValue(n, out var v) ? v : null)).ToArray();
        var sql = id.HasValue ? entity.UpdateSql : entity.InsertSql;
        if (entity.Key == "notes" && values.TryGetValue("valeur", out var note) &&
            decimal.TryParse(Convert.ToString(note), out var val) && (val < 0 || val > 20))
            throw new InvalidOperationException("La note doit être comprise entre 0 et 20.");
        return DbOperations.Execute(sql, parameters);
    }

    public static int Delete(EntityDefinition entity, int id) =>
        DbOperations.Execute(entity.DeleteSql, DbOperations.P("@id", id));

    public static DataTable GetFilieres() => DbOperations.Query("SELECT id,code,libelle FROM filiere ORDER BY libelle");
}
