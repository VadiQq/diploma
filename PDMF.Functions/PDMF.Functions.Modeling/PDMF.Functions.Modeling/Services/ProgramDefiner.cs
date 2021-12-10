using System;
using PDMF.Data.Azure.Models;
using PDMF.Data.Azure.Models.QueueMessages;
using PDMF.Data.Enums;

namespace PDMF.Functions.Modeling.Services
{
    public class ProgramDefiner
    {
        public static BatchProgramSettings GetProgram(ModelingQueueMessage message)
        {
            switch (message.ModelType)
            {
                case ModelType.GMDH:
                {
                    return new BatchProgramSettings
                    {
                        ProgramName = "PDMF.GMDHBatchProgram",
                        ProgramPath = "GMDH",
                        DatasetId = message.DatasetId,
                        ModelingTaskId = message.ModelingTaskId,
                        DesiredColumn = message.DesiredColumn
                    };
                }
                default:
                {
                    throw new ArgumentOutOfRangeException(nameof(message.ModelType), message.ModelType, null);
                }
            }
        }
    }
}