using System;
using Newtonsoft.Json;
using PDMF.Algorithms.GMDH.Models;
using PDMF.Data.Enums;
using PDMF.Functions.Forecasting.Models;

namespace PDMF.Functions.Forecasting.Services
{
    public static class ModelParser
    {
        public static ParseModel DeserializeModel(ModelType type, string modelJson)
        {
            switch (type)
            {
                case ModelType.GMDH:
                {
                    return new ParseModel
                    {
                        Model = JsonConvert.DeserializeObject<WienerBinaryModel>(modelJson),
                        ModelType = typeof(WienerBinaryModel)
                    };
                }
                default:
                {
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);   
                }
            }
        }
    }
}