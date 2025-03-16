using Confluent.Kafka;

public static class KafkaAdmin {
	public static async Task DeleteAllMessagesFromTopic(string bootstrapServers, string topic) {
		using var adminClient = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = bootstrapServers }).Build();

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
			.ToList()
		;

		await adminClient.DeleteRecordsAsync(partitionOffsets);
		Console.WriteLine($"All messages successfully deleted from topic: {topic} with {partitionOffsets.Count} partitions.");
	}
}
