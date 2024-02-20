using AozoraSharp.Logging;

namespace AozoraSharp;

public abstract class AozoraObject
{
    public AozoraObject()
    {
        logger = LogManager.Instance.GetLogger(GetType().Name);
    }

    protected readonly ILogger logger;
}
