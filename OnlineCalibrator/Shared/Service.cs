using Accord.Math;
using Stochastique.Distributions;
using Stochastique.Enums;
using Stochastique.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineCalibrator.Shared
{
    public class GrosCalculService: IGrosCalculService
    {
        public async Task<string> CalculDistributionTronques(string values, bool isDiscrete, double min, double max)
        {
            double[] valeurs = values.Split(";").SkipLast(1).Select(x => Convert.ToDouble(x)).ToArray();
            DonneesAAnalyser daa = new DonneesAAnalyser { Values = valeurs, ValeurMinTrukated = min, ValeurMaxTrukated = max, IsDiscreteDistribution = isDiscrete, IncludeTruncatedDistributions = true };
            return string.Concat(daa.GetAllDistributions().Select(a => a.Distribution.ToString() + Environment.NewLine));
        }
        public async Task<string> CalculELL(string values, string distributions)
        {
            double[] valeurs = values.Split(";").SkipLast(1).Select(x => Convert.ToDouble(x)).ToArray();
            var distrib = GetDistrib(distributions)[0];
            return string.Concat(new EELQuantileTest(valeurs,distrib,0.05).PValues.Select(x => x.ToString()+";"));
        }

        public static List<Distribution> GetDistrib(string value)
        {
            
            var distribs = value.Split(Environment.NewLine);
            var rst= new List<Distribution>();
            for (int i = 0; i < distribs.Length; i++) 
            {
                if (!string.IsNullOrEmpty(distribs[i]))
                {
                    var premiersplit = distribs[i].Split('(');
                    TypeDistribution dt = Enum.Parse<TypeDistribution>(premiersplit[0]);
                    Distribution distrib;
                    if ((int)dt > 1000)
                    {
                        distrib = new TruncatedDistribution { BaseDistribution = Distribution.CreateDistribution(dt - 1000) };
                    }
                    else
                    {
                        distrib = Distribution.CreateDistribution(dt);
                    }
                    var splitparam = premiersplit[1].Replace(")", "").Split(';').SkipLast(1);
                    foreach (var split in splitparam)
                    {
                        var splitValeur = split.Split(':');
                        if (distrib is TruncatedDistribution trunk && splitValeur[0] != ParametreName.valeurMin.ToString() && splitValeur[0] != ParametreName.ValeurMax.ToString())
                        {
                            trunk.BaseDistribution.AddParameter(new Parameter
                            {
                                Name = Enum.Parse<ParametreName>(splitValeur[0]),
                                Value = Convert.ToDouble(splitValeur[1])
                            });
                        }
                        else
                        {
                            distrib.AddParameter(new Parameter
                            {
                                Name = Enum.Parse<ParametreName>(splitValeur[0]),
                                Value = Convert.ToDouble(splitValeur[1])
                            });
                        }
                       
                    }
                    rst.Add(distrib);
                }
            }
            return rst;
            
        }

    }

    public interface IGrosCalculService
    {
        Task<string> CalculELL(string values, string distributions);
        Task<string> CalculDistributionTronques(string values, bool isDiscrete, double min, double max);
    }
}
