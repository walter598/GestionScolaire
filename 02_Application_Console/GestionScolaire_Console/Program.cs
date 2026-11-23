using System.Data;
using GestionScolaire.Core.Data;
using GestionScolaire.Core.Repositories;
using MySql.Data.MySqlClient;

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.Title = "Gestion scolaire - Console ADO.NET";

while (true)
{
    Console.Clear();
    Console.WriteLine("╔══════════════════════════════════════════════╗");
    Console.WriteLine("║       GESTION SCOLAIRE - ADO.NET              ║");
    Console.WriteLine("╚══════════════════════════════════════════════╝");
    Console.WriteLine("1. Tester MySqlConnection");
    Console.WriteLine("2. Tester MySqlCommand / ExecuteScalar");
    Console.WriteLine("3. Tester ExecuteNonQuery (mise à jour démo)");
    Console.WriteLine("4. Tester ExecuteReader / MySqlDataReader");
    Console.WriteLine("5. Tester MySqlDataAdapter / DataTable");
    Console.WriteLine("6. Tester DataSet et DataRelation");
    Console.WriteLine("7. Tester DataView (tri/filtre)");
    Console.WriteLine("8. Tester MySqlException");
    Console.WriteLine("9. Afficher les étudiants");
    Console.WriteLine("0. Quitter");
    Console.Write("\nVotre choix : ");
    var choix = Console.ReadLine();
    try
    {
        switch (choix)
        {
            case "1":
                using (var cn = Db.CreateConnection()) { cn.Open(); Console.WriteLine($"Connecté à MySQL {cn.ServerVersion}; état={cn.State}; base={cn.Database}"); }
                break;
            case "2":
                var total = Convert.ToInt64(DbOperations.Scalar("SELECT COUNT(*) FROM etudiant"));
                Console.WriteLine($"ExecuteScalar : {total} étudiant(s).");
                break;
            case "3":
                var changed = DbOperations.Execute("UPDATE matiere SET coefficient=coefficient WHERE code=@code", DbOperations.P("@code", "ALG"));
                Console.WriteLine($"ExecuteNonQuery : {changed} ligne(s) concernée(s), sans altérer les données.");
                break;
            case "4":
                using (var cn = Db.CreateConnection())
                {
                    cn.Open();
                    using var cmd = new MySqlCommand("SELECT matricule,nom,prenom FROM etudiant ORDER BY id", cn);
                    using MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                        Console.WriteLine($"{reader.GetString(reader.GetOrdinal("matricule"))} | {reader.GetString(reader.GetOrdinal("nom"))} {reader.GetString(reader.GetOrdinal("prenom"))}");
                }
                break;
            case "5":
                var dt = DbOperations.Query("SELECT id,code,libelle FROM filiere");
                Console.WriteLine($"DataAdapter.Fill -> DataTable : {dt.Rows.Count} lignes, {dt.Columns.Count} colonnes.");
                foreach (DataRow row in dt.Rows) Console.WriteLine($"{row["code"]} - {row["libelle"]}");
                break;
            case "6":
                using (var cn = Db.CreateConnection())
                using (var ds = new DataSet("GestionScolaire"))
                using (var daF = new MySqlDataAdapter("SELECT id,code,libelle FROM filiere", cn))
                using (var daE = new MySqlDataAdapter("SELECT id,matricule,nom,prenom,filiere_id FROM etudiant", cn))
                {
                    daF.Fill(ds, "filiere"); daE.Fill(ds, "etudiant");
                    ds.Relations.Add("Filiere_Etudiants", ds.Tables["filiere"]!.Columns["id"]!, ds.Tables["etudiant"]!.Columns["filiere_id"]!);
                    Console.WriteLine($"DataSet : {ds.Tables.Count} tables, relation créée.");
                }
                break;
            case "7":
                var viewTable = DbOperations.Query("SELECT id,code,libelle FROM filiere");
                var view = new DataView(viewTable) { Sort = "libelle ASC", RowFilter = "code <> 'SIG'" };
                foreach (DataRowView row in view) Console.WriteLine($"{row["code"]} - {row["libelle"]}");
                Console.WriteLine("DataView : tri alphabétique, filtre code <> SIG.");
                break;
            case "8":
                try { DbOperations.Query("SELECT * FROM table_inexistante_demo"); }
                catch (MySqlException ex) { Console.WriteLine($"MySqlException capturée : n°{ex.Number} - {ex.Message}"); }
                break;
            case "9":
                var students = SchoolRepository.Get(SchoolRepository.Entities[0]);
                foreach (DataRow row in students.Rows) Console.WriteLine($"{row["matricule"]} | {row["prenom"]} {row["nom"]} | {row["filiere"]}");
                break;
            case "0": return;
            default: Console.WriteLine("Choix invalide."); break;
        }
    }
    catch (MySqlException ex) { Console.WriteLine($"Erreur MySQL n°{ex.Number}: {ex.Message}"); }
    catch (Exception ex) { Console.WriteLine($"Erreur : {ex.Message}"); }
    Console.WriteLine("\nAppuyez sur une touche pour continuer...");
    Console.ReadKey();
}
