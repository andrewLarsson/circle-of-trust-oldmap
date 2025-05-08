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
	("Host=localhost;Port=5432;Username=postgres;Password=password;Database=CircleOfTrustView", "public.synchronizationcontexts"),
	("Host=localhost;Port=5432;Username=postgres;Password=password;Database=CircleOfTrustView", "public.idempotenttransactions"),
];
/*List<(string, string)> postgreSqlTables = [
	("Host=db-postgresql-sfo3--do-user--0.h.db.ondigitalocean.com;Port=25060;Username=doadmin;Password=AVNS_;Database=defaultdb;SSL Mode=Require;Trust Server Certificate=true", "public.mt_doc_aggregaterootdocumentcircle"),
	("Host=db-postgresql-sfo3--do-user--0.h.db.ondigitalocean.com;Port=25060;Username=doadmin;Password=AVNS_;Database=defaultdb;SSL Mode=Require;Trust Server Certificate=true", "public.mt_doc_transactiondocument")
];*/

Console.WriteLine("Type a command:");
string? command = Console.ReadLine()?.Trim();
switch (command) {
	case "delete everything":
		await DeleteEverything();
		break;
	case "delete all messages":
		await DeleteAllMessages();
		break;
	case "delete all records":
		await DeleteAllRecords();
		break;
	default:
		Console.WriteLine($"Unknown command: {command}");
		break;
}

async Task DeleteEverything() {
	await DeleteAllMessages();
	await DeleteAllRecords();
}

async Task DeleteAllMessages() {
	foreach ((string bootstrapServers, string topic) in kafkaTopics) {
		await KafkaAdmin.DeleteAllMessagesFromTopic(bootstrapServers, topic);
	}
}

async Task DeleteAllRecords() {
	foreach ((string connectionString, string table) in postgreSqlTables) {
		await PostgreSqlAdmin.DeleteAllRecordsFromTable(connectionString, table);
	}
}

// TODO Add "initialize everything" command (creates the database and the Kafka topics)
