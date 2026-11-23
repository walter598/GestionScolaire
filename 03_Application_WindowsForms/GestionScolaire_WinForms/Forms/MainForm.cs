using System.Data;
using GestionScolaire.Core.Data;
using GestionScolaire.Core.Repositories;
using MySql.Data.MySqlClient;

namespace GestionScolaire.WinForms.Forms;

public sealed class MainForm : Form
{
    private readonly Panel _nav = new() { Dock = DockStyle.Left, Width = 225, BackColor = Color.FromArgb(19, 35, 61) };
    private readonly Panel _top = new() { Dock = DockStyle.Top, Height = 72, BackColor = Color.White };
    private readonly Panel _content = new() { Dock = DockStyle.Fill, BackColor = Color.FromArgb(242, 245, 250), Padding = new Padding(24) };
    private readonly Label _pageTitle = new() { AutoSize = true, Font = new Font("Segoe UI", 17, FontStyle.Bold), ForeColor = Color.FromArgb(25, 42, 70), Location = new Point(24, 20) };

    public MainForm()
    {
        Text = "Gestion scolaire | ADO.NET";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(1100, 700);
        Size = new Size(1380, 850);
        BuildNavigation();
        _top.Controls.Add(_pageTitle);
        Controls.Add(_content); Controls.Add(_top); Controls.Add(_nav);
        ShowDashboard();
    }

    private void BuildNavigation()
    {
        var brand = new Label { Text = "GESTION\\nSCOLAIRE", ForeColor = Color.White, Font = new Font("Segoe UI", 17, FontStyle.Bold), Height = 90, Dock = DockStyle.Top, TextAlign = ContentAlignment.MiddleCenter };
        _nav.Controls.Add(brand);
        var menu = new (string Text, Action Action)[] {
            ("▦  Tableau de bord", ShowDashboard),
            ("♙  Étudiants", () => ShowCrud("etudiants")),
            ("▤  Filières", () => ShowCrud("filieres")),
            ("▣  Matières", () => ShowCrud("matieres")),
            ("✎  Notes", () => ShowCrud("notes")),
            ("⌘  Tests ADO.NET", ShowTests),
            ("⚙  Paramètres", ShowSettings),
            ("ⓘ  À propos", ShowAbout),
            ("⏻  Quitter", Close)
        };
        var y = 105;
        foreach (var item in menu)
        {
            var b = new Button { Text = item.Text, Location = new Point(12, y), Size = new Size(200, 43), FlatStyle = FlatStyle.Flat, ForeColor = Color.White, BackColor = Color.FromArgb(19, 35, 61), TextAlign = ContentAlignment.MiddleLeft, Font = new Font("Segoe UI", 10) };
            b.FlatAppearance.BorderSize = 0; b.FlatAppearance.MouseOverBackColor = Color.FromArgb(39, 75, 120);
            b.Click += (_, _) => item.Action();
            _nav.Controls.Add(b); y += 48;
        }
    }

    private void SetPage(string title, Control page)
    {
        _pageTitle.Text = title;
        _content.Controls.Clear();
        page.Dock = DockStyle.Fill;
        _content.Controls.Add(page);
    }

    private void ShowDashboard()
    {
        var p = new Panel { AutoScroll = true };
        var intro = new Label { Text = "Vue d’ensemble de votre établissement", AutoSize = true, Font = new Font("Segoe UI", 12), ForeColor = Color.DimGray, Location = new Point(4, 8) };
        p.Controls.Add(intro);
        var cards = new FlowLayoutPanel { Location = new Point(0, 55), Size = new Size(1050, 145), WrapContents = false, AutoScroll = true };
        p.Controls.Add(cards);
        try
        {
            AddCard(cards, "Étudiants", ScalarCount("SELECT COUNT(*) FROM etudiant"), Color.FromArgb(42, 105, 190));
            AddCard(cards, "Filières", ScalarCount("SELECT COUNT(*) FROM filiere"), Color.FromArgb(22, 145, 115));
            AddCard(cards, "Matières", ScalarCount("SELECT COUNT(*) FROM matiere"), Color.FromArgb(220, 135, 45));
            AddCard(cards, "Notes", ScalarCount("SELECT COUNT(*) FROM note"), Color.FromArgb(126, 91, 175));
            var status = new Label { Text = "● Connexion MySQL : opérationnelle", ForeColor = Color.DarkGreen, Font = new Font("Segoe UI", 11, FontStyle.Bold), AutoSize = true, Location = new Point(8, 235) };
            p.Controls.Add(status);
        }
        catch (Exception ex)
        {
            p.Controls.Add(new Label { Text = "● Connexion MySQL : échec — " + ex.Message, ForeColor = Color.DarkRed, AutoSize = true, Location = new Point(8, 235), MaximumSize = new Size(900, 0) });
        }
        var shortcuts = new Label { Text = "Accès rapide", AutoSize = true, Font = new Font("Segoe UI", 14, FontStyle.Bold), Location = new Point(8, 300) };
        p.Controls.Add(shortcuts);
        var y = 345;
        foreach (var key in new[] { "etudiants", "filieres", "matieres", "notes" })
        {
            var def = SchoolRepository.Entities.First(x => x.Key == key);
            var b = new Button { Text = "Ouvrir " + def.Title, Location = new Point(8, y), Size = new Size(240, 48), BackColor = Color.FromArgb(42, 105, 190), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            b.Click += (_, _) => ShowCrud(key); p.Controls.Add(b); y += 60;
        }
        SetPage("Tableau de bord", p);
    }

    private static long ScalarCount(string sql) => Convert.ToInt64(DbOperations.Scalar(sql));
    private static void AddCard(Control parent, string title, long value, Color color)
    {
        var card = new Panel { Size = new Size(220, 125), BackColor = Color.White, Margin = new Padding(8), Padding = new Padding(14) };
        card.Controls.Add(new Label { Text = title, AutoSize = true, ForeColor = Color.DimGray, Font = new Font("Segoe UI", 10), Location = new Point(12, 12) });
        card.Controls.Add(new Label { Text = value.ToString(), AutoSize = true, ForeColor = color, Font = new Font("Segoe UI", 25, FontStyle.Bold), Location = new Point(12, 43) });
        parent.Controls.Add(card);
    }

    private void ShowCrud(string key)
    {
        var def = SchoolRepository.Entities.First(x => x.Key == key);
        SetPage(def.Title, new CrudPage(def));
    }

    private void ShowTests() => SetPage("Tests ADO.NET", new TestsPage());
    private void ShowSettings()
    {
        var p = new Panel();
        p.Controls.Add(new Label { Text = "Configuration de la connexion", Font = new Font("Segoe UI", 15, FontStyle.Bold), AutoSize = true, Location = new Point(8, 8) });
        p.Controls.Add(new Label { Text = Db.SafeDescription + "\\nUtilisateur et mot de passe configurables via variables d’environnement GS_DB_USER et GS_DB_PASSWORD.", AutoSize = true, Location = new Point(8, 55), MaximumSize = new Size(850, 0) });
        SetPage("Paramètres de connexion", p);
    }
    private void ShowAbout()
    {
        var p = new Panel();
        p.Controls.Add(new Label { Text = "Gestion scolaire — Projet académique ADO.NET\\nC# · .NET 8 · Windows Forms · MySQL 8.4 · MySql.Data 8.4.0\\nArchitecture en couches et requêtes paramétrées.", AutoSize = true, Font = new Font("Segoe UI", 12), Location = new Point(8, 8) });
        SetPage("À propos", p);
    }
}

internal sealed class CrudPage : UserControl
{
    private readonly GestionScolaire.Core.Models.EntityDefinition _def;
    private readonly BindingSource _binding = new();
    private readonly DataGridView _grid = new() { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, SelectionMode = DataGridViewSelectionMode.FullRowSelect, MultiSelect = false, BackgroundColor = Color.White, BorderStyle = BorderStyle.None };
    private readonly Dictionary<string, Control> _inputs = new();
    private readonly TextBox _search = new() { Width = 260, PlaceholderText = "Rechercher..." };
    private int? _selectedId;

    public CrudPage(GestionScolaire.Core.Models.EntityDefinition def)
    {
        _def = def;
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 3, ColumnCount = 1 };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 52));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 170));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        var toolbar = new FlowLayoutPanel { Dock = DockStyle.Fill };
        var searchButton = MakeButton("Rechercher", Color.FromArgb(42,105,190)); searchButton.Click += (_,_) => LoadRows(_search.Text);
        var refresh = MakeButton("Actualiser", Color.FromArgb(80,95,115)); refresh.Click += (_,_) => LoadRows();
        toolbar.Controls.Add(_search); toolbar.Controls.Add(searchButton); toolbar.Controls.Add(refresh);
        var editor = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoScroll = true, BackColor = Color.White, Padding = new Padding(10) };
        foreach (var field in def.Fields)
        {
            var group = new Panel { Width = 190, Height = 65 };
            group.Controls.Add(new Label { Text = field.Label, AutoSize = true, Location = new Point(2, 0), ForeColor = Color.FromArgb(55,65,80) });
            Control input;
            if (field.Name == "filiere_id" || field.Name == "etudiant_id" || field.Name == "matiere_id")
            {
                var combo = new ComboBox { Width = 175, Location = new Point(2, 25), DropDownStyle = ComboBoxStyle.DropDownList };
                var lookup = field.Name switch
                {
                    "filiere_id" => DbOperations.Query("SELECT id,libelle AS label FROM filiere ORDER BY libelle"),
                    "etudiant_id" => DbOperations.Query("SELECT id,CONCAT(prenom,' ',nom,' (',matricule,')') AS label FROM etudiant ORDER BY nom"),
                    _ => DbOperations.Query("SELECT id,intitule AS label FROM matiere ORDER BY intitule")
                };
                combo.DataSource = lookup;
                combo.DisplayMember = "label";
                combo.ValueMember = "id";
                input = combo;
            }
            else
            {
                input = new TextBox { Width = 175, Location = new Point(2, 25) };
            }
            group.Controls.Add(input); editor.Controls.Add(group); _inputs[field.Name] = input;
        }
        var actions = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 42 };
        var add = MakeButton("Ajouter", Color.FromArgb(22,145,115)); add.Click += (_,_) => Save(false);
        var update = MakeButton("Modifier", Color.FromArgb(42,105,190)); update.Click += (_,_) => Save(true);
        var delete = MakeButton("Supprimer", Color.FromArgb(190,55,55)); delete.Click += (_,_) => Delete();
        var clear = MakeButton("Nouveau", Color.FromArgb(90,100,115)); clear.Click += (_,_) => ClearInputs();
        actions.Controls.AddRange([add, update, delete, clear]);
        var editorHost = new Panel { Dock = DockStyle.Fill }; editorHost.Controls.Add(editor); editorHost.Controls.Add(actions);
        layout.Controls.Add(toolbar,0,0); layout.Controls.Add(editorHost,0,1); layout.Controls.Add(_grid,0,2);
        Controls.Add(layout);
        _grid.DataSource = _binding;
        _grid.SelectionChanged += (_,_) => SelectCurrent();
        LoadRows();
    }

    private static Button MakeButton(string text, Color color)
    {
        var b = new Button { Text = text, Width = 110, Height = 34, BackColor = color, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Margin = new Padding(5,0,5,0) };
        b.FlatAppearance.BorderSize = 0; return b;
    }
    private void LoadRows(string? search = null)
    {
        try { _binding.DataSource = SchoolRepository.Get(_def, search); if (_grid.Columns.Contains("id")) _grid.Columns["id"]!.Visible = false; }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Chargement impossible", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }
    private void SelectCurrent()
    {
        if (_grid.CurrentRow?.DataBoundItem is not DataRowView row) return;
        if (row.Row.Table.Columns.Contains("id")) _selectedId = Convert.ToInt32(row["id"]);
        foreach (var field in _def.Fields)
        {
            if (!_inputs.ContainsKey(field.Name) || !row.Row.Table.Columns.Contains(field.Name)) continue;
            if (_inputs[field.Name] is ComboBox combo)
                combo.SelectedValue = Convert.ToInt32(row[field.Name]);
            else if (_inputs[field.Name] is TextBox textBox)
                textBox.Text = Convert.ToString(row[field.Name]) ?? "";
        }
    }
    private void Save(bool update)
    {
        if (update && !_selectedId.HasValue) { MessageBox.Show("Sélectionnez d'abord une ligne."); return; }
        var values = new Dictionary<string, object?>();
        foreach (var f in _def.Fields)
        {
            if (_inputs[f.Name] is ComboBox combo)
            {
                if (combo.SelectedValue == null) { MessageBox.Show($"Sélectionnez {f.Label}."); return; }
                values[f.Name] = Convert.ToInt32(combo.SelectedValue);
                continue;
            }
            var value = _inputs[f.Name] is TextBox textBox ? textBox.Text.Trim() : "";
            if (f.Required && value.Length == 0) { MessageBox.Show($"Le champ {f.Label} est obligatoire."); return; }
            if (value.Length == 0) { values[f.Name] = null; continue; }
            try
            {
                values[f.Name] = f.Type switch
                {
                    "number" => int.Parse(value),
                    "decimal" => decimal.Parse(value, System.Globalization.CultureInfo.CurrentCulture),
                    "date" => DateTime.Parse(value).Date,
                    _ => value
                };
            }
            catch { MessageBox.Show($"Valeur invalide pour {f.Label}."); return; }
        }
        try
        {
            if (_def.Key == "notes")
            {
                var v = Convert.ToDecimal(values["valeur"]);
                if (v < 0 || v > 20) throw new InvalidOperationException("La note doit être comprise entre 0 et 20.");
            }
            SchoolRepository.Save(_def, values, update ? _selectedId : null);
            MessageBox.Show(update ? "Modification effectuée." : "Enregistrement effectué.", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadRows(); ClearInputs();
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Opération impossible", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
    }
    private void Delete()
    {
        if (!_selectedId.HasValue) { MessageBox.Show("Sélectionnez une ligne."); return; }
        if (MessageBox.Show("Confirmer la suppression de cet enregistrement ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
        try { SchoolRepository.Delete(_def, _selectedId.Value); LoadRows(); ClearInputs(); }
        catch (MySqlException ex) { MessageBox.Show($"Erreur MySQL {ex.Number}: {ex.Message}"); }
        catch (Exception ex) { MessageBox.Show(ex.Message); }
    }
    private void ClearInputs()
    {
        foreach (var control in _inputs.Values)
        {
            if (control is TextBox tb) tb.Clear();
            else if (control is ComboBox cb && cb.Items.Count > 0) cb.SelectedIndex = 0;
        }
        _selectedId = null; _grid.ClearSelection();
    }
}

internal sealed class TestsPage : UserControl
{
    private readonly TextBox _log = new() { Multiline = true, ReadOnly = true, ScrollBars = ScrollBars.Vertical, Dock = DockStyle.Fill, Font = new Font("Consolas", 10), BackColor = Color.FromArgb(20,28,42), ForeColor = Color.FromArgb(180,235,200) };
    public TestsPage()
    {
        var root = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2 };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 160)); root.RowStyles.Add(new RowStyle(SizeType.Percent,100));
        var buttons = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoScroll = true };
        Add("MySqlConnection", TestConnection); Add("MySqlCommand", TestCommand); Add("ExecuteScalar", TestScalar);
        Add("ExecuteNonQuery", TestNonQuery); Add("ExecuteReader / DataReader", TestReader);
        Add("MySqlDataAdapter", TestAdapter); Add("DataTable", TestDataTable); Add("DataSet", TestDataSet); Add("DataView", TestDataView);
        Add("MySqlException", TestException);
        root.Controls.Add(buttons,0,0); root.Controls.Add(_log,0,1); Controls.Add(root);
        void Add(string label, Action action) { var b = new Button { Text=label, Width=190, Height=42, BackColor=Color.FromArgb(42,105,190), ForeColor=Color.White, FlatStyle=FlatStyle.Flat, Margin=new Padding(5) }; b.Click += (_,_) => Run(label,action); buttons.Controls.Add(b); }
    }
    private void Write(string s) => _log.AppendText($"[{DateTime.Now:HH:mm:ss}] {s}{Environment.NewLine}");
    private void Run(string name, Action action) { try { action(); } catch (Exception ex) { Write($"{name} — ERREUR : {ex.Message}"); } }
    private void TestConnection() { using var cn=Db.CreateConnection(); cn.Open(); Write($"Connexion ouverte. MySQL {cn.ServerVersion}; état={cn.State}"); }
    private void TestCommand() { using var cn=Db.CreateConnection(); cn.Open(); using var cmd=new MySqlCommand("SELECT DATABASE()",cn); Write("MySqlCommand.CommandText exécuté : "+cmd.ExecuteScalar()); }
    private void TestScalar() => Write("ExecuteScalar COUNT étudiants = "+DbOperations.Scalar("SELECT COUNT(*) FROM etudiant"));
    private void TestNonQuery() => Write("ExecuteNonQuery sans modification fonctionnelle : "+DbOperations.Execute("UPDATE matiere SET coefficient=coefficient WHERE code=@c",DbOperations.P("@c","ALG"))+" ligne(s).");
    private void TestReader() { using var cn=Db.CreateConnection(); cn.Open(); using var cmd=new MySqlCommand("SELECT matricule,nom,prenom FROM etudiant",cn); using var rd=cmd.ExecuteReader(); int n=0; while(rd.Read()){Write($"{rd["matricule"]} - {rd["prenom"]} {rd["nom"]}");n++;} Write($"MySqlDataReader : {n} ligne(s)."); }
    private void TestAdapter() { using var cn=Db.CreateConnection(); using var da=new MySqlDataAdapter("SELECT id,code,libelle FROM filiere",cn); var dt=new DataTable(); da.Fill(dt); Write($"MySqlDataAdapter.Fill : {dt.Rows.Count} ligne(s) chargée(s)."); }
    private void TestDataTable() { var dt=DbOperations.Query("SELECT id,code,libelle FROM filiere"); Write($"DataTable en mémoire : {dt.TableName}, {dt.Rows.Count} ligne(s), {dt.Columns.Count} colonne(s)."); }
    private void TestDataSet() { using var cn=Db.CreateConnection(); using var ds=new DataSet(); using var a=new MySqlDataAdapter("SELECT id,code,libelle FROM filiere",cn); using var b=new MySqlDataAdapter("SELECT id,matricule,filiere_id FROM etudiant",cn); a.Fill(ds,"filiere");b.Fill(ds,"etudiant");ds.Relations.Add("filiere_etudiants",ds.Tables["filiere"]!.Columns["id"]!,ds.Tables["etudiant"]!.Columns["filiere_id"]!);Write($"DataSet : {ds.Tables.Count} tables et {ds.Relations.Count} relation."); }
    private void TestDataView() { var dt=DbOperations.Query("SELECT id,code,libelle FROM filiere"); var dv=new DataView(dt){Sort="libelle ASC"}; Write($"DataView : {dv.Count} ligne(s), tri libelle ASC."); }
    private void TestException() { try { DbOperations.Query("SELECT * FROM table_absente_demo"); } catch(MySqlException ex){Write($"MySqlException n°{ex.Number} interceptée : {ex.Message}");} }
}
