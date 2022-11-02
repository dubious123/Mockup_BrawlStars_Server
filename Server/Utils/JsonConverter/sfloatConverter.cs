using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Utils.JsonConverter
{
	internal class sfloatConverter : JsonConverter<sfloat>
	{
		public override sfloat Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			if (reader.TokenType != JsonTokenType.StartObject)
			{
				goto Exception;
			}

			reader.Read();
			if (reader.TokenType != JsonTokenType.PropertyName)
			{
				goto Exception;
			}

			var propertyName = reader.GetString();
			if (propertyName != "RawValue")
			{
				goto Exception;
			}

			reader.Read();
			if (reader.TokenType != JsonTokenType.Number)
			{
				goto Exception;
			}

			var res = sfloat.FromRaw(reader.GetUInt32());
			reader.Read();
			if (reader.TokenType != JsonTokenType.EndObject)
			{
				goto Exception;
			}

			return res;
		Exception:
			throw new JsonException();
		}

		public override void Write(Utf8JsonWriter writer, sfloat value, JsonSerializerOptions options)
		{
			writer.WriteStartObject();
			writer.WriteNumber("RawValue", value.RawValue);
			writer.WriteEndObject();
		}
	}
}
