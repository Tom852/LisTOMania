using LisTOMania.Common.Interfaces.DataLayer;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LisTOMania.Business
{
    public class RepeatableItemResetter : IJob
    {
        private readonly IItemDataAccess dataAccess;

        public RepeatableItemResetter(IItemDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await dataAccess.ResetRepeatableItems();
        }
    }
}