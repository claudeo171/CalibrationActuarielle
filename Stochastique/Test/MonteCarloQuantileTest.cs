using Accord.Math.Optimization;
using MathNet.Numerics.Random;
using MersenneTwister;
using MessagePack;
using OnlineCalibrator.Shared;
using Stochastique.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique.Test
{
    [MessagePackObject]
    public class MonteCarloQuantileTest : TestStatistique
    {
        [Key(5)]
        public double[] Values { get; set; }
        [Key(6)]
        public Distribution Distribution { get; set; }
        [Key(7)]
        public List<List<double>> ValueList { get; set; }
        [Key(8)]
        public List<double> PValues { get; set; }

        [Key(9)]
        public double alpha;

        [IgnoreMember]
        public double Alpha
        {
            get
            {
                return alpha;
            }
            set
            {
                alpha = value;
                if (PValues != null && PValues.Count > 0)
                {
                    PValue = PValues[(int)(Math.Round(Alpha * (PValues.Count - 1), MidpointRounding.AwayFromZero))];
                }
            }
        }
        public MonteCarloQuantileTest(double[] values,Distribution distribution, double alpha)
        {
            TypeTestStatistique = TypeTestStatistique.MonteCarloQuantile;
            Values=values.Order().ToArray();
            Distribution = distribution;
            Alpha=alpha;
            Simulate();
            ComputePValues();
            switch (Distribution.Type)
            {
                case Enums.TypeDistribution.Normal:
                    StateH0 = TypeDonnees.Normal;
                    StateH1 = TypeDonnees.NotNormal;
                    break;
                default:
                    StateH0 = TypeDonnees.FollowDistribution;
                    StateH1 = TypeDonnees.NotFollowDistribution;
                    break;
                    //TODO A completer
            }
            PValue = PValues[(int)(Math.Round(Alpha*(PValues.Count-1),MidpointRounding.AwayFromZero))];

        }

        private void Simulate()
        {
            var random = MT64Random.Create(1234);
            var element = new List<List<double>>();
            for (int i = 0; i < Math.Min(10000,10000000000/Values.Length); i++)
            {
                element.Add(Distribution.Simulate(random,Values.Length).Order().ToList());
            }
            ValueList = new List<List<double>>();
            for (int i = 0; i < Values.Length; i++)
            {
                var mean = Distribution.InverseCDF((0.5 + i )/ Values.Length);
                ValueList.Add(element.Select(x =>  x[i]-mean).Order().ToList());
            }
            
        }
        private void ComputePValues()
        {
            PValues=new List<double>();
            var val = Values.Select((a, i) => a - Distribution.InverseCDF((0.5 + i) / Values.Length)).ToArray();
            for(int i = 0; i < val.Length; i++)
            {
                if (ValueList[i][0] > val[i])
                {
                    PValues.Add(0);
                }
                else if (ValueList[i][ValueList[i].Count-1]< val[i])
                {
                    PValues.Add(1);
                }
                else
                {
                    int indexMin = 0;
                    int indexMax = ValueList[i].Count-1;
                    while (!(ValueList[i][indexMin] <= val[i] && ValueList[i][indexMin+1] >= val[i]))
                    {
                        int indiceMoy=(indexMin+indexMax)/2;
                        if(ValueList[i][indiceMoy] < val[i])
                        {
                            indexMin = indiceMoy;
                        }
                        else
                        {
                            indexMax = indiceMoy;
                        }
                    }
                    PValues.Add(1-(indexMin+0.5) / ValueList[i].Count);
                }
            }
        }
    }
}
