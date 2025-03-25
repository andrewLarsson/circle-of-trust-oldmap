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
		DO UPDATE SET SynchronizationPosition = EXCLUDED.SynchronizationPosition
		WHERE EXCLUDED.SynchronizationPosition > SynchronizationContexts.SynchronizationPosition;
	";

	public static async Task ExecuteIdempotentTransaction(
		this IDbConnection dbConnection,
		string transactionId,
		string application,
		Synchronization synchronization,
		Func<IDbTransaction, Task> perform
	) {
		ApplicationSynchronizationContext context = synchronization.ForApplication(application);
		var synchronizationContext = new {
			SynchronizationContext = context.ToString(),
			SynchronizationPosition = synchronization.Position
		};
		var idempotentTransaction = new {
			Application = application,
			TransactionId = transactionId
		};
		var alreadyCommitted = await dbConnection.ExecuteScalarAsync<bool>(SelectIdempotentTransactionAlreadyCommitted, idempotentTransaction);
		using var transaction = dbConnection.BeginTransaction();
		await dbConnection.ExecuteAsync(UpsertSynchronizationContextPosition, synchronizationContext, transaction);
		if (alreadyCommitted) {
			transaction.Commit();
			return;
		}
		await dbConnection.ExecuteAsync(InsertIdempotentTransaction, idempotentTransaction, transaction);
		await perform(transaction);
		transaction.Commit();
	}

	public static Task ExecuteIdempotentTransaction(
		this IDbConnection dbConnection,
		string transactionId,
		string application,
		Synchronization synchronization,
		string sql,
		object? param = null
	) {
		return dbConnection.ExecuteIdempotentTransaction(
			transactionId,
			application,
			synchronization,
			(transaction) => dbConnection.ExecuteAsync(sql, param, transaction)
		);
	}
}
