namespace Server.Utils.JsonConverter
{
	public class Vector2Converter : JsonConverter<Vector2>
	{
		public override Vector2 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			float x = 0f;
			float y = 0f;
			if (reader.TokenType != JsonTokenType.StartObject) goto Exception;
			for (int i = 0; i < 2; i++)
			{
				reader.Read();
				if (reader.TokenType != JsonTokenType.PropertyName) goto Exception;
				var propertyName = reader.GetString()!.ToLower();
				switch (propertyName)
				{
					case "x":
						reader.Read();
						if (reader.TokenType != JsonTokenType.Number) goto Exception;
						x = reader.GetSingle();
						break;
					case "y":
						reader.Read();
						if (reader.TokenType != JsonTokenType.Number) goto Exception;
						y = reader.GetSingle();
						break;
					default:
						goto Exception;
				}
			}

			reader.Read();
			if (reader.TokenType != JsonTokenType.EndObject) goto Exception;
			return new Vector2(x, y);
		Exception:
			throw new JsonException();
		}

		public override void Write(Utf8JsonWriter writer, Vector2 value, JsonSerializerOptions options)
		{
			writer.WriteStartObject();
			writer.WriteNumber("x", value.X);
			writer.WriteNumber("y", value.Y);
			writer.WriteEndObject();
		}
	}
}
