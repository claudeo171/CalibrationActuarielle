using Newtonsoft.Json.Serialization;
using Stochastique.Distributions;

namespace OnlineCalibrator.Shared
{
    internal class CustomSerializationBinder : ISerializationBinder
    {
        public void BindToName(Type serializedType, out string? assemblyName, out string? typeName)
        {
            if(serializedType == typeof(NormalDistribution))
            {
                assemblyName = "Stochastique";
                typeName= "NormalDistribution";
            }
            else
            {
                assemblyName = "";
                typeName = "";
            }
        }

        public Type BindToType(string? assemblyName, string typeName)
        {
            if(assemblyName=="Stochastique" && typeName == "NormalDistribution")
            {
                return typeof(NormalDistribution);
            }
            return null;
        }
    }
}