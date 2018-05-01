using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace CreateMask.Workers.Exceptions
{
    [Serializable]
    public class LowHighMeasurementsSwappedException : Exception
    {
        public LowHighMeasurementsSwappedException()
        {
        }

        public LowHighMeasurementsSwappedException(string message)
            : base(message)
        {
        }

        public LowHighMeasurementsSwappedException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected LowHighMeasurementsSwappedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));
            base.GetObjectData(info, context);
        }
    }
}
