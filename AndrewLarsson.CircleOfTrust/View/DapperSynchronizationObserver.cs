using Dapper;
using developersBliss.OLDMAP.Application;
using System.Data;

namespace AndrewLarsson.CircleOfTrust.View;

public class DapperSynchronizationObserver(IDbConnection dbConnection) : ISynchronizationObserver {
	static readonly string SelectAllPositions = "SELECT SynchronizationContext, SynchronizationPosition FROM SynchronizationContexts;";
	public async Task<IEnumerable<(ApplicationSynchronizationContext, long)>> ObservePositions() {
		List<(ApplicationSynchronizationContext, long)> observedPositions = [];
		var positions = await dbConnection.QueryAsync<(string, long)>(SelectAllPositions);
		foreach ((string synchronizationContext, long position) in positions) {
			observedPositions.Add((new ApplicationSynchronizationContext(synchronizationContext), position));
		}
		return observedPositions;
	}
}
