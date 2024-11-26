using Stochastique.Distributions;
using Stochastique.Distributions.Continous;
using Expr = MathNet.Symbolics.SymbolicExpression;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Symbolics;

namespace Stochastique.Copule
{
    [MessagePack.MessagePackObject]
    public abstract class CopuleArchimedienne:Copule
    {
        //C(u,v) = inverseGenerateur(generateur(u)+generateur(v))
        protected abstract double Generateur(double t);
        
        protected abstract double InverseGenerateur(double t);
        //Loi dont la tranformée de Laplace est égale à la fonction "inverseGenerateur"
        [MessagePack.Key(4)]
        protected Distribution Distribution { get; set; }

        protected abstract Expr InverseGenerator();

        protected double InverseGenerateurDerivate(double t, int ordre)
        {
            var fct = InverseGenerator();
            var variable = SymbolicExpression.Variable("t");
            var values = new Dictionary<string, FloatingPoint>();
            values.Add("t", t);
            foreach (var v in AllParameters())
            {
                values.Add(v.Name.ToString(), v.Value);
            }
            for (int i = 0; i < ordre; i++)
            {
                fct = fct.Differentiate(variable);
            }
            return fct.Evaluate(values).RealValue;
        }

        public override List<List<double>> SimulerCopule(Random r, int nbSim)
        {
            double[] N = Distribution.Simulate(r,nbSim);
            List<List<double>> uniformes = new List<List<double>>();
            ExponentialDistribution loiExp1 = new ExponentialDistribution(1);

            for (int i = 0; i < Dimension; i++)
            {
                uniformes.Add(loiExp1.Simulate(r, nbSim).Select((a, i) => InverseGenerateur( a / N[i])).ToList());
            }

            return uniformes;
        }

        public override double CDFCopula(List<double> u)
        {
            return InverseGenerateur(u.Sum(a => Generateur(a)));
        }
        public override double DensityCopula(IEnumerable<double> u)
        {
            var sumGenerator = u.Sum(a => Generateur(a));
            var sumInverseGenerator = InverseGenerateurDerivate(sumGenerator, Dimension);
            var dividende = u.Prod(a => InverseGenerateurDerivate(Generateur(a), 1));
            var rst = sumInverseGenerator.Divide(dividende, 0);
            if(rst<0)
            {
                rst=0;
            }
            return rst;
        }
    }
}
