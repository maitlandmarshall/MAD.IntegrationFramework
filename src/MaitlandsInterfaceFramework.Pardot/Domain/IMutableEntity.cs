using System;
using System.Collections.Generic;
using System.Text;

namespace MaitlandsInterfaceFramework.Pardot.Domain
{
    public interface IImmutableEntity : IEntity
    {
        DateTime CreatedAt { get; set; }
    }

    public interface IMutableEntity : IImmutableEntity
    {
        DateTime UpdatedAt { get; set; }
    }
}
