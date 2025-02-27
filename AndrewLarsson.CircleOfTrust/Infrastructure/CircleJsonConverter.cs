using AndrewLarsson.CircleOfTrust.Model;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AndrewLarsson.CircleOfTrust.Infrastructure;
public class CircleJsonConverter : JsonConverter<Circle> {
	public override Circle Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
		var doc = JsonDocument.ParseValue(ref reader);
		var root = doc.RootElement;

		string title = root.GetProperty("Title").GetString()!;
		string owner = root.GetProperty("Owner").GetString()!;
		string secretKey = root.GetProperty("Lock").GetProperty("secretKey").GetString()!;
		var members = root
			.GetProperty("Members")
			.EnumerateArray()
			.Select(m => m.GetString()!)
			.ToList()
		;
		bool betrayed = root.GetProperty("Betrayed").GetBoolean();

		var constructor = typeof(Circle).GetConstructor(
			BindingFlags.NonPublic | BindingFlags.Instance,
			null,
			[typeof(string), typeof(string), typeof(string), typeof(IEnumerable<string>), typeof(bool)],
			null
		);
		return (Circle) constructor!.Invoke([title, owner, secretKey, members, betrayed]);
	}

	public override void Write(Utf8JsonWriter writer, Circle value, JsonSerializerOptions options) {
		writer.WriteStartObject();

		writer.WriteString("Title", value.Title);
		writer.WriteString("Owner", value.Owner);

		string secretKey = GetPrimaryConstructorField<Lock, string>(value.Lock, "secretKey")!;
		writer.WriteStartObject("Lock");
		writer.WriteString("secretKey", secretKey);
		writer.WriteEndObject();

		var members = value.Members
			.GetType()
			.GetField("members", BindingFlags.NonPublic | BindingFlags.Instance)!
			.GetValue(value.Members)
			as HashSet<string>
			?? []
		;
		writer.WriteStartArray("Members");
		foreach (var member in members) {
			writer.WriteStringValue(member);
		}
		writer.WriteEndArray();

		writer.WriteBoolean("Betrayed", value.Betrayed);

		writer.WriteEndObject();
	}

	private static T? GetPrimaryConstructorField<TClass, T>(TClass instance, string paramName) {
		var field = typeof(TClass).GetField($"<{paramName}>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);
		return (T?) field?.GetValue(instance);
	}
}
