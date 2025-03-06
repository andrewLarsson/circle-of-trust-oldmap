using Dapper;
using System.Data;

namespace AndrewLarsson.CircleOfTrust.View;
public static class DapperIdempotentTransactionExtensions {
	static readonly string SelectIdempotentTransactionAlreadyCommitted = @"
		SELECT EXISTS (
			SELECT 1 FROM IdempotentTransactions 
			WHERE IdempotencyKey = CONCAT(@TransactionContext, '|', @TransactionId)
		);
	";
	static readonly string InsertIdempotentTransaction = @"
		INSERT INTO IdempotentTransactions (IdempotencyKey)
		VALUES (CONCAT(@TransactionContext, '|', @TransactionId));
	";

	public static async Task ExecuteIdempotentTransaction(
		this IDbConnection dbConnection,
		string transactionContext,
		string transactionId,
		Func<IDbTransaction, Task> perform
	) {
		var idempotentTransaction = new {
			TransactionContext = transactionContext,
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
		string transactionContext,
		string transactionId,
		string sql,
		object? param = null
	) {
		return dbConnection.ExecuteIdempotentTransaction(transactionContext, transactionId, (transaction) =>
			dbConnection.ExecuteAsync(sql, param, transaction)
		);
	}
}
