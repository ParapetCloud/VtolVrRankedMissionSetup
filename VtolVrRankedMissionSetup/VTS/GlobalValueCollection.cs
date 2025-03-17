using System.Collections.Generic;
using VtolVrRankedMissionSetup.VT;

namespace VtolVrRankedMissionSetup.VTS
{
    public class GlobalValueCollection
    {
        [VTInlineArray]
        public GlobalValue[] Values => ValueList.ToArray();

        [VTIgnore]
        private List<GlobalValue> ValueList { get; }

        public GlobalValueCollection()
        {
            ValueList = [];
        }

        public GlobalValue CreateGlobalValue(string name, int initialValue = 0)
        {
            GlobalValue value = new()
            {
                Id = ValueList.Count,
                Name = name,
                InitialValue = initialValue,
            };
            ValueList.Add(value);

            return value;
        }
    }
}
