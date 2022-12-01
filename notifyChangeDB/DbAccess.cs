using System.Data.Common;
using System.Data.SQLite;
using Npgsql;
using Oracle.ManagedDataAccess.Client;
using DT = System.Data;
using QC = Microsoft.Data.SqlClient;

public class DbAccess
{
    protected string ConnectionString { get; set; } = string.Empty;
    protected DbConnection? conn;

    public DbAccess()
    {
    }

    public virtual void Open()
    {
    }

    public virtual void Run()
    {
    }

    public static DbAccess? CreateDB(string type, string connectString)
    {
        DbAccess? db = null;

        switch (type)
        {
            case "SQLite":
                db = new SQLiteAccess(connectString);
                break;
            case "PostgreSQL":
                db = new PostgresAccess(connectString);
                break;
            case "Oracle":
                db = new OracleAccess(connectString);
                break;
            case "SqlServer":
                db = new SqlServerAccess(connectString);
                break;
            default:
                break;
        }

        return db;
    }
}

public class SQLiteAccess : DbAccess
{
    public SQLiteAccess(string conString)
    {
        ConnectionString = conString;
    }

    public override void Open()
    {
        conn = new SQLiteConnection(ConnectionString);
        conn.Open();
    }

    public override void Run()
    {
    }
}

public class PostgresAccess : DbAccess
{
    public PostgresAccess(string conString)
    {
        ConnectionString = conString;
    }

    public override void Open()
    {
        conn = new NpgsqlConnection(ConnectionString);
        conn.Open();
    }

    public override void Run()
    {
        using (DbCommand cmd = new NpgsqlCommand())
        {
            cmd.CommandText = "SELECT * from public.win_cpu limit 100";

            cmd.Connection = conn as NpgsqlConnection;
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                        Console.WriteLine($"{i} {reader.GetName(i)} {reader.GetValue(i)} {reader.GetDataTypeName(i)}");
                }
            }
        }
    }
}


public class OracleAccess : DbAccess
{
    public OracleAccess(string conString)
    {
        ConnectionString = conString;
    }

    public override void Open()
    {
        conn = new OracleConnection();
        conn.ConnectionString = ConnectionString;

        // DB接続
        conn.Open();
    }

    public override void Run()
    {
        DbCommand cmd = conn.CreateCommand();

        cmd.CommandText = "SELECT * FROM dummy WHERE ROWNUM <= 100";
        // cmd.ExecuteNonQuery();

        DbDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            for (int i = 0; i < reader.FieldCount; i++)
                Console.WriteLine($"{i} {reader.GetName(i)} {reader.GetValue(i)} {reader.GetDataTypeName(i)}");
        }
    }
}


public class SqlServerAccess : DbAccess
{
    public SqlServerAccess(string conString)
    {
        ConnectionString = conString;
    }

    public override void Open()
    {
        conn = new QC.SqlConnection(ConnectionString);
        conn.Open();
    }

    public override void Run()
    {
        using (DbCommand command = conn.CreateCommand())
        {
            command.Connection = conn;
            command.CommandType = DT.CommandType.Text;
            command.CommandText = @"SELECT TOP 5  * FROM dummy;";

            DbDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                    Console.WriteLine($"{i} {reader.GetName(i)} {reader.GetValue(i)} {reader.GetDataTypeName(i)}");
            }
        }
    }
}
