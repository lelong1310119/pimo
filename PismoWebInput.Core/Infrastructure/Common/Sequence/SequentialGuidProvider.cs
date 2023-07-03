using SequentialGuid;
using PismoWebInput.Core.Infrastructure.Common.Timing;

namespace PismoWebInput.Core.Infrastructure.Common.Sequence;

public class SequentialGuidProvider : ISequentialGuidProvider
{
    public Guid NewId()
    {
        return SequentialGuidGenerator.Instance.NewGuid(Clock.Now);
    }
}