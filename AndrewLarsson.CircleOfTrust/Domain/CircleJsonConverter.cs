using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
/*
namespace AndrewLarsson.CircleOfTrust.Domain;
public class CircleJsonConverter : JsonConverter<Circle> {
	public override Circle Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
		var json = JsonDocument.ParseValue(ref reader).RootElement;
		string title = json.GetProperty("title").GetString()
			?? throw new NullReferenceException()
		;
		string owner = json.GetProperty("owner").GetString()
			?? throw new NullReferenceException()
		;
		string secretKey = json.GetProperty("secretKey").GetString()
			?? throw new NullReferenceException()
		;
		List<string> members = json
			.GetProperty("members")
			.EnumerateArray()
			.Select(e => e.GetString() ?? throw new NullReferenceException())
			.ToList()
		;
		bool betrayed = json.GetProperty("betrayed").GetBoolean();
		var constructor = typeof(Circle).GetConstructor(
			BindingFlags.NonPublic | BindingFlags.Instance,
			null,
			[
				typeof(string),
				typeof(string),
				typeof(string),
				typeof(IEnumerable<string>),
				typeof(bool)
			],
			null
		) ?? throw new JsonException("Private constructor for Circle not found.");
		var circle = constructor.Invoke([
			title,
			owner,
			secretKey,
			members,
			betrayed
		]);
		return (Circle) circle;
	}

	public override void Write(Utf8JsonWriter writer, Circle value, JsonSerializerOptions options) {
		writer.WriteStartObject();
		writer.WriteString("title", value.Title);
		writer.WriteString("owner", value.Owner);
		var secretKeyField = typeof(Lock).GetField("secretKey", BindingFlags.NonPublic | BindingFlags.Instance);
		string secretKey = (string) (secretKeyField?.GetValue(value.Lock)
			?? throw new NullReferenceException()
		);
		writer.WriteString("secretKey", secretKey);
		var membersField = value.Members.GetType().GetField("members", BindingFlags.NonPublic | BindingFlags.Instance);
		var members = (HashSet<string>) (membersField?.GetValue(value.Members)
			?? throw new NullReferenceException()
		);
		writer.WriteStartArray("members");
		foreach (var member in members) {
			writer.WriteStringValue(member);
		}
		writer.WriteEndArray();
		writer.WriteBoolean("betrayed", value.Betrayed);
		writer.WriteEndObject();
	}
}
*/
