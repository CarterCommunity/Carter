namespace Carter
{
    using System;

    public abstract class RouteMetaData
    {
        public virtual string Description { get; }

        public virtual string Tag { get; }

        public virtual Type Request { get; }

        public virtual RouteMetaDataResponse[] Responses { get; }
        
        public virtual string OperationId { get; }
    }
}