using System;
using System.Text.Json.Serialization;
using System.Text.Json;
using VtolVrRankedMissionSetup.Configs.AirbaseLayout;
using VtolVrRankedMissionSetup.VT;

namespace VtolVrRankedMissionSetup.Configs
{
    public class AltSpawnJsonConverter : JsonConverter<AltSpawnConfig>
    {
        public override AltSpawnConfig Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var jsonDocument = JsonDocument.ParseValue(ref reader);
            if (jsonDocument.RootElement.ValueKind == JsonValueKind.String)
            {
                return new AltSpawnConfig() { Type = JsonSerializer.Deserialize(jsonDocument.RootElement.GetRawText(), ConfigSerialization.Default.AircraftType) };
            }

            if (jsonDocument.RootElement.ValueKind != JsonValueKind.Object)
            {
                throw new JsonException();
            }

            AltSpawnConfig config = new();

            if (!jsonDocument.RootElement.TryGetProperty("type", out JsonElement typeElement))
                throw new JsonException();

            if (jsonDocument.RootElement.TryGetProperty("slots", out JsonElement slotElement))
                config.Slots = slotElement.GetInt32();

            if (jsonDocument.RootElement.TryGetProperty("altPosition", out JsonElement altPositionElement))
                config.AltPosition = JsonSerializer.Deserialize(slotElement.GetRawText(), ConfigSerialization.Default.Vector3);

            config.Type = JsonSerializer.Deserialize(typeElement.GetRawText(), ConfigSerialization.Default.AircraftType);

            return config;
        }

        public override void Write(Utf8JsonWriter writer, AltSpawnConfig value, JsonSerializerOptions options)
        {
            if (value.Slots != null || value.AltPosition != null)
            {
                writer.WriteStartObject();
                writer.WriteString("type", value.Type.ToString());

                if (value.Slots != null)
                    writer.WriteNumber("slots", value.Slots.Value);

                if (value.AltPosition != null)
                {
                    writer.WritePropertyName("altPosition");
                    writer.WriteRawValue(JsonSerializer.Serialize(value.AltPosition.Value, ConfigSerialization.Default.Vector3));
                }

                writer.WriteEndObject();
            }
            else
            {
                writer.WriteStringValue(value.Type.ToString());
            }
        }
    }
}
