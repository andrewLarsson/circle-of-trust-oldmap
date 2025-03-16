using Confluent.Kafka;

Console.Write("Enter the Kafka topic to delete all messages from: ");
string? topic = Console.ReadLine()?.Trim();

if (string.IsNullOrEmpty(topic)) {
	Console.WriteLine("Topic name cannot be empty.");
	return;
}

const string bootstrapServers = "localhost:9092";

using var adminClient = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = bootstrapServers }).Build();

// Fetch metadata to get partitions
var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(5));
var topicMetadata = metadata.Topics.FirstOrDefault(t => t.Topic == topic);

if (topicMetadata == null) {
	Console.WriteLine($"Topic '{topic}' not found.");
	return;
}

var partitionOffsets = topicMetadata.Partitions
	.Select(p => new TopicPartitionOffset(
		new TopicPartition(topic, new Partition(p.PartitionId)),
		Offset.End // Deletes all records
	))
	.ToList();

if (!partitionOffsets.Any()) {
	Console.WriteLine("No partitions found for the topic.");
	return;
}

Console.WriteLine($"Deleting messages from {partitionOffsets.Count} partitions...");

await adminClient.DeleteRecordsAsync(partitionOffsets);

Console.WriteLine($"All messages successfully deleted from topic: {topic}");
