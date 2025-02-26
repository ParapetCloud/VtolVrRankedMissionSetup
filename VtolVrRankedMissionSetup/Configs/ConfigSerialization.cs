using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace VtolVrRankedMissionSetup.Configs
{
    public static class ConfigSerialization
    {
        private static JsonSerializerOptions? _serializerOptions;
        public static JsonSerializerOptions SerializerOptions
        {
            get
            {
                if (_serializerOptions == null)
                {
                    _serializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
                    _serializerOptions.Converters.Add(new Vector3JsonConverter());
                }

                return _serializerOptions;
            }
        }
    }
}
