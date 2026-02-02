using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VtolVrRankedMissionSetup.VT;
using VtolVrRankedMissionSetup.VT.Methods;

namespace VtolVrRankedMissionSetup.VTS
{
    [VTName("gv")]
    public class GlobalValue
    {
        public string Data => $"{Id};{Name};{Description};{InitialValue};";

        [VTIgnore]
        [Id]
        public int Id { get; set; }

        [VTIgnore]
        public string Name { get; set; } = string.Empty;

        [VTIgnore]
        public string Description { get; set; } = string.Empty;

        [VTIgnore]
        public double InitialValue { get; set; }

        public override string ToString() => $"%gv-{Id}%";

        public static bool operator <(GlobalValue gv, double n) => throw new InvalidOperationException("You can't actually call this method");
        public static bool operator >(GlobalValue gv, double n) => throw new InvalidOperationException("You can't actually call this method");

        public static bool operator ==(GlobalValue gv, double n) => throw new InvalidOperationException("You can't actually call this method");
        public static bool operator !=(GlobalValue gv, double n) => throw new InvalidOperationException("You can't actually call this method");

        public override bool Equals(object? obj)
        {
            throw new InvalidOperationException("You can't actually call this method");
        }

        public override int GetHashCode() => base.GetHashCode();
    }

    public static class GlobalValueExtension
    {
        [EventTarget("Reset Value", "System", TargetId = 2)]
        public static void ResetValue(
            [ParamInfo(CustomParameterName = "Global Value")][IdLink("")] this GlobalValue GlobalValue)
            => throw new NotSupportedException("You can't actually call this");

        [EventTarget("Increment Value", "System", TargetId = 2)]
        public static void IncrementValue(
            [ParamInfo(CustomParameterName = "Global Value")][IdLink("")] this GlobalValue GlobalValue,
            [ParamAttrInfo("UnitSpawnAttributeRange+RangeTypes", "Int")]
            [ParamAttrInfo("MinMax", "(0,99999)")] float Add = 1)
            => throw new NotSupportedException("You can't actually call this");

        [EventTarget("Set Value", "System", TargetId = 2)]
        public static void SetValue(
            [ParamInfo(CustomParameterName = "Global Value")][IdLink("")] this GlobalValue GlobalValue,
            [ParamAttrInfo("UnitSpawnAttributeRange+RangeTypes", "Int")]
            [ParamAttrInfo("MinMax", "(-99999,99999)")] float Set_to)
            => throw new NotSupportedException("You can't actually call this");
    }
}
