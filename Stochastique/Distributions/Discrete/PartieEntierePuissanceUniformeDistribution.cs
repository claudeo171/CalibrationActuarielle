using MessagePack;
using Stochastique.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastique.Distributions.Discrete
{
    [MessagePackObject]
    public partial class PartieEntierePuissanceUniformeDistribution : DiscreteDistribution
    {
        [IgnoreMember]
        public override TypeDistribution Type => TypeDistribution.PartieEntierePuissanceUniforme;
        [IgnoreMember]
        public double Theta=> GetParameter(ParametreName.theta).Value;
        public PartieEntierePuissanceUniformeDistribution()
        {

        }
        public PartieEntierePuissanceUniformeDistribution(double  theta)
        {
            AddParameter(new Parameter(ParametreName.theta, theta));
        }

        public override IEnumerable<Parameter> CalibrateWithMoment(IEnumerable<double> values)
        {
            throw new NotImplementedException();
        }

        public override double Simulate(Random r)
        {
            return Math.Floor(Math.Pow(r.NextDouble(), -Theta));
        }

        public override double ExpextedValue()
        {
            throw new NotImplementedException();
        }

        public override double Kurtosis()
        {
            throw new NotImplementedException();
        }

        public override double Skewness()
        {
            throw new NotImplementedException();
        }

        public override double Variance()
        {
            throw new NotImplementedException();
        }

        protected override double PDFInt(int k)
        {
            return Math.Pow(k, -Theta) - Math.Pow(k + 1, -Theta);
        }
    }
}
