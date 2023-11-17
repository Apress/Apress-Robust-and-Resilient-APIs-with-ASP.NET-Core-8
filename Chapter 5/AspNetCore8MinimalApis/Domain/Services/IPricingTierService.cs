using Domain.Enum;

namespace Domain.Services;

public interface IPricingTierService
{
    public PricingTier GetPricingTier(string ipAddress);
}
