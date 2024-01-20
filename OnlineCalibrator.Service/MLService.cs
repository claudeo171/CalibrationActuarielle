using OnlineCalibrator.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineCalibrator.Service
{
    public interface IMLService
    {
        Task Train(DonneesAAnalyser donnees,Action a);
    }
    public class MLService:IMLService
    {
        public async Task Train(DonneesAAnalyser donnees, Action a)
        {
            donnees.CalibrerMLI();
            a();
        }
    }
}
