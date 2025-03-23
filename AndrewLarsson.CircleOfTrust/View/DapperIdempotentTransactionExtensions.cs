using Dapper;
using developersBliss.OLDMAP.Application;
using System.Data;

namespace AndrewLarsson.CircleOfTrust.View;
public static class DapperIdempotentTransactionExtensions {
	static readonly string SelectIdempotentTransactionAlreadyCommitted = @"
		SELECT EXISTS (
			SELECT 1 FROM IdempotentTransactions 
			WHERE IdempotencyKey = CONCAT(@Application, '|', @TransactionId)
		);
	";
	static readonly string InsertIdempotentTransaction = @"
		INSERT INTO IdempotentTransactions (IdempotencyKey)
		VALUES (CONCAT(@Application, '|', @TransactionId));
	";
	static readonly string UpsertSynchronizationContextPosition = @"
		INSERT INTO SynchronizationContexts (SynchronizationContext, SynchronizationPosition)
		VALUES (@SynchronizationContext, @SynchronizationPosition)
		ON CONFLICT (SynchronizationContext)
		DO UPDATE SET SynchronizationPosition = EXCLUDED.SynchronizationPosition;
	";

	public static async Task ExecuteIdempotentTransaction(
		this IDbConnection dbConnection,
		string application,
		string transactionId,
		Func<IDbTransaction, Task> perform
	) {
		var idempotentTransaction = new {
			Application = application,
			TransactionId = transactionId
		};
		var alreadyCommitted = await dbConnection.ExecuteScalarAsync<bool>(SelectIdempotentTransactionAlreadyCommitted, idempotentTransaction);
		if (alreadyCommitted) {
			return;
		}
		using var transaction = dbConnection.BeginTransaction();
		await dbConnection.ExecuteAsync(InsertIdempotentTransaction, idempotentTransaction, transaction);
		await perform(transaction);
		transaction.Commit();
	}

	public static Task ExecuteIdempotentTransaction(
		this IDbConnection dbConnection,
		string application,
		string transactionId,
		string sql,
		object? param = null
	) {
		return dbConnection.ExecuteIdempotentTransaction(application, transactionId, (transaction) =>
			dbConnection.ExecuteAsync(sql, param, transaction)
		);
	}

	public static Task ExecuteIdempotentTransactionWithSynchronization(
		this IDbConnection dbConnection,
		string application,
		string transactionId,
		Synchronization synchronization,
		Func<IDbTransaction, Task> perform
	) {
		ApplicationSynchronizationContext context = synchronization.ForApplication(application);
		return dbConnection.ExecuteIdempotentTransaction(application, transactionId, async (transaction) => {
			await dbConnection.ExecuteAsync(UpsertSynchronizationContextPosition, new {
				SynchronizationContext = context.ToString(),
				SynchronizationPosition = synchronization.Position
			}, transaction);
			await perform(transaction);
		});
	}

	public static Task ExecuteIdempotentTransactionWithSynchronization(
		this IDbConnection dbConnection,
		string application,
		string transactionId,
		Synchronization synchronization,
		string sql,
		object? param = null
	) {
		return dbConnection.ExecuteIdempotentTransactionWithSynchronization(
			application,
			transactionId,
			synchronization,
			(transaction) => dbConnection.ExecuteAsync(sql, param, transaction)
		);
	}
}
