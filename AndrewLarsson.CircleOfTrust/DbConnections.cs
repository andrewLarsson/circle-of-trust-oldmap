using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace AndrewLarsson.CircleOfTrust;
// TODO Move this to some kind of common library AndrewLarsson.Common?...
public abstract class NamedDbConnection(IDbConnection connection) : IDbConnection {
	[AllowNull]
	public string ConnectionString {
		get => connection.ConnectionString ?? string.Empty;
		set => connection.ConnectionString = value;
	}
	public int ConnectionTimeout => connection.ConnectionTimeout;
	public ConnectionState State => connection.State;
	public void Open() => connection.Open();
	public void Close() => connection.Close();
	public string Database => connection.Database;
	public void ChangeDatabase(string databaseName) => connection.ChangeDatabase(databaseName);
	public IDbTransaction BeginTransaction() => connection.BeginTransaction();
	public IDbTransaction BeginTransaction(IsolationLevel il) => connection.BeginTransaction(il);
	public IDbCommand CreateCommand() => connection.CreateCommand();
	public void Dispose() {
		Dispose(true);
		GC.SuppressFinalize(this);
	}
	protected virtual void Dispose(bool disposing) {
		if (disposing) {
			connection.Dispose();
		}
	}
}

public class ViewDbConnection(IDbConnection postgreSqlConnection) : NamedDbConnection(postgreSqlConnection);
