using Domain.Enum;
using Domain.Services;

namespace BLL.Services;

public class PricingTierService : IPricingTierService
{
    public PricingTier GetPricingTier(string ipAddress)
    {
        return PricingTier.Free;
    }
}
