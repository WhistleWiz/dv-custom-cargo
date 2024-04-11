using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using UnityEngine;

namespace CC.Common.Conditions
{
    public abstract class CargoCondition : ScriptableObject
    {
        public abstract string ConditionType { get; }

        public enum ConditionMode
        {
            All,
            Any
        }
    }

    public class ConditionConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(CargoCondition);
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            JObject o = JObject.Load(reader);

            switch (o["ConditionType"]!.Value<string>())
            {
                case "Debt":
                    return o.ToObject<ConditionDebt>();
                case "IRLTime":
                    return o.ToObject<ConditionIRLTime>();
                default:
                    return null;
            }
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
