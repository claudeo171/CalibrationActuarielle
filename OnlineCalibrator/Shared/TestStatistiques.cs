using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineCalibrator.Shared
{
    public class TestStatistiques
    {
        public double PValue { get; set; }
        public TypeTestStatistique TypeTestStatistique { get; set; }

        public TypeDonnees StateH0 { get; set; }
        public TypeDonnees StateH1 { get; set; }
        public TypeDonnees GetTypeDonnee(double alpha)
        {
            if (PValue > alpha)
            {
                return StateH0;
            }
            else
            {
                return TypeDonnees.NotNormal;
            }
        }
    }
}
