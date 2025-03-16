List<(string, string)> kafkaTopics = [
	("localhost:9092", "AndrewLarsson.CircleOfTrust.Circle.DomainMessages"),
	("localhost:9092", "AndrewLarsson.CircleOfTrust.Circle.DomainEvents")
];
List<(string, string)> postgreSqlTables = [
	("Host=localhost;Port=5432;Username=postgres;Password=password;Database=CircleOfTrust", "public.mt_doc_aggregaterootdocumentcircle"),
	("Host=localhost;Port=5432;Username=postgres;Password=password;Database=CircleOfTrust", "public.mt_doc_transactiondocument"),
	("Host=localhost;Port=5432;Username=postgres;Password=password;Database=CircleOfTrustView", "public.circlestats"),
	("Host=localhost;Port=5432;Username=postgres;Password=password;Database=CircleOfTrustView", "public.userstats"),
	("Host=localhost;Port=5432;Username=postgres;Password=password;Database=CircleOfTrustView", "public.userstatscirclemembers"),
	("Host=localhost;Port=5432;Username=postgres;Password=password;Database=CircleOfTrustView", "public.idempotenttransactions"),
];

Console.WriteLine("Type a command:");
string? command = Console.ReadLine()?.Trim();
switch (command) {
	case "delete everything":
		await DeleteEverything();
		break;

	default:
		Console.WriteLine($"Unknown command: {command}");
		break;
}

async Task DeleteEverything() {
	foreach ((string bootstrapServers, string topic) in kafkaTopics) {
		await KafkaAdmin.DeleteAllMessagesFromTopic(bootstrapServers, topic);
	}

	foreach ((string connectionString, string table) in postgreSqlTables) {
		await PostgreSqlAdmin.DeleteAllRecordsFromTable(connectionString, table);
	}
}
