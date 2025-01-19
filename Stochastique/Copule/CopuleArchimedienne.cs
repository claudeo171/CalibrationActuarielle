using Stochastique.Distributions;
using Stochastique.Distributions.Continous;
using Expr = MathNet.Symbolics.SymbolicExpression;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Symbolics;
using System.ComponentModel.DataAnnotations;
using static alglib;
using static Accord.Math.FourierTransform;

namespace Stochastique.Copule
{
    [MessagePack.MessagePackObject]
    [MessagePack.Union(0,typeof(CopuleAMH))]
    [MessagePack.Union(1, typeof(CopuleClayton))]
    [MessagePack.Union(2, typeof(CopuleFrank))]
    [MessagePack.Union(3, typeof(CopuleGumbel))]
    [MessagePack.Union(4, typeof(CopuleJoe))]
    public abstract class CopuleArchimedienne:Copule
    {
        //C(u,v) = inverseGenerateur(generateur(u)+generateur(v))
        protected abstract double Generateur(double t);
        
        protected abstract double InverseGenerateur(double t);
        //Loi dont la tranformée de Laplace est égale à la fonction "inverseGenerateur"
        [MessagePack.Key(4)]
        protected Distribution Distribution { get; set; }

        protected abstract Expr InverseGenerator(SymbolicExpression param,List<SymbolicExpression> copuleParameter);

        protected abstract Expr Generator(SymbolicExpression param, List<SymbolicExpression> copuleParameter);
        [MessagePack.Key(5)]
        public int Dimension { get; set; }
        [MessagePack.IgnoreMember]
        public SymbolicExpression densite;
        [MessagePack.IgnoreMember]
        public SymbolicExpression Densite
        {
            get
            {
                if(densite==null)
                {
                    CalculerDensite();
                }
                return densite;
            }
        }
        private void CalculerDensite()
        {
            var listeVariable = new List<SymbolicExpression>();
            var listeVariableCopule = new List<SymbolicExpression>();
            foreach (var v in AllParameters())
            {
                listeVariableCopule.Add(Expr.Variable(v.Name.ToString()));
            }
            var sommeGenerateur = Expr.Zero;
            for (int i = 1; i <= Dimension; i++)
            {
                var variable = Expr.Variable("u" + i);
                listeVariable.Add(variable);
                sommeGenerateur = sommeGenerateur + Generator(variable, listeVariableCopule);
            }
            var rst = InverseGenerator(sommeGenerateur, listeVariableCopule);
            for (int i = 0; i < Dimension; i++)
            {
                rst = rst.Differentiate(listeVariable[i]);
            }
            densite = rst;
        }
        public CopuleArchimedienne(int dimention)
        {
            Dimension = dimention;
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
            try
            {
                return Densite.Evaluate(AllParameters().Select(a => new KeyValuePair<string, FloatingPoint>(a.Name.ToString(), (FloatingPoint)a.Value)).Concat(u.Select((a, i) => new KeyValuePair<string, FloatingPoint>("u" + (i + 1), (FloatingPoint)a))).ToDictionary()).RealValue;
            }
            catch
            { return 0; }
        }
    }
}
