using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineCalibrator.Shared
{
    [MessagePackObject]
    public class DonneeContainer
    {
        [Key(0)]
        public DonneesImportes Value { get; set; }
        public event Action OnStateChange;
        public void SetValue(DonneesImportes value)
        {
            this.Value = value;
            NotifyStateChanged();
        }
        private void NotifyStateChanged() => OnStateChange?.Invoke();
    }
}
