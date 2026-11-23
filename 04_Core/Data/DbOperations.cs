using System.Data;
using MySql.Data.MySqlClient;

namespace GestionScolaire.Core.Data;

public static class DbOperations
{
    public static DataTable Query(string sql, params MySqlParameter[] parameters)
    {
        using var cn = Db.CreateConnection();
        using var cmd = new MySqlCommand(sql, cn);
        if (parameters.Length > 0) cmd.Parameters.AddRange(parameters);
        using var da = new MySqlDataAdapter(cmd);
        var table = new DataTable();
        da.Fill(table);
        return table;
    }

    public static int Execute(string sql, params MySqlParameter[] parameters)
    {
        using var cn = Db.CreateConnection();
        cn.Open();
        using var cmd = new MySqlCommand(sql, cn);
        if (parameters.Length > 0) cmd.Parameters.AddRange(parameters);
        return cmd.ExecuteNonQuery();
    }

    public static object? Scalar(string sql, params MySqlParameter[] parameters)
    {
        using var cn = Db.CreateConnection();
        cn.Open();
        using var cmd = new MySqlCommand(sql, cn);
        if (parameters.Length > 0) cmd.Parameters.AddRange(parameters);
        return cmd.ExecuteScalar();
    }

    public static MySqlParameter P(string name, object? value) =>
        new(name, value ?? DBNull.Value);
}
