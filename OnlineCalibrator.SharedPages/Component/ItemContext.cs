using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tavenem.DataStorage;

namespace OnlineCalibrator.SharedPages.Component
{
    [JsonSerializable(typeof(IIdItem))]
    [JsonSerializable(typeof(SavedData))]
    [JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
    public partial class ItemContext : JsonSerializerContext;
}
