using System.Runtime.Serialization;
using Newtonsoft.Json;
using PDMF.Data.Algorithms.Matrices;

namespace PDMF.Algorithms.GMDH.Models
{
    [DataContract]
    public abstract class BaseGMDHModel
    {
        [DataMember]
        public int ModelIndex { get; set; }
        [DataMember]
        public int RoundIndex { get; set; }
        [DataMember]
        public int FirstVariableIndex { get; set; }
        [DataMember]
        public int SecondVariableIndex { get; set;}
        [DataMember]
        public double ExternalCriterion { get; set; }
        [DataMember]
        protected double RegularCriterion { get; set; }
        [DataMember]
        protected double UnbiasednessCriterion { get; set; }
        [DataMember]
        public Matrix BModelValues { get; set; }
        [DataMember]
        public Matrix BTestModelValues { get; set; }
        
        [JsonIgnore]
        protected readonly Matrix XValues;
        [JsonIgnore]
        protected readonly Matrix YValues;
        
        protected BaseGMDHModel()
        {
        }
        
        protected BaseGMDHModel(int modelIndex, int index1, int index2, Matrix xValues, Matrix yValues)
        {
            ModelIndex = modelIndex;
            FirstVariableIndex = index1;
            SecondVariableIndex = index2;
            XValues = xValues;
            YValues = yValues;
        }

        protected BaseGMDHModel(int modelIndex, int roundIndex, Matrix xValues, Matrix yValues)
        {
            ModelIndex = modelIndex;
            RoundIndex = roundIndex;
            XValues = xValues;
            YValues = yValues;
        }
    }
}