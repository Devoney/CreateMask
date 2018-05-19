using System;
using CreateMask.Contracts.Interfaces;

namespace CreateMask.Workers
{
    public class DateTimeWorker : IDateTimeWorker
    {
        public DateTime Now => DateTime.Now;
    }
}
