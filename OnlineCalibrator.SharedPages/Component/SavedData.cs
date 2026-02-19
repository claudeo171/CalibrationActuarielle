using System;
using System.Collections.Generic;
using System.Text;
using Tavenem.DataStorage;

namespace OnlineCalibrator.SharedPages.Component
{
    public class SavedData: IIdItem
    {
        public string Id { get; set; }
        public string Value {  get; set; }

        public bool Equals(IIdItem? other)
        {
            return Id==other?.Id;
        }
    }
}
