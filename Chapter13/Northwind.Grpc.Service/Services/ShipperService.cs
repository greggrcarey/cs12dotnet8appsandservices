using Grpc.Core; // To use ServerCallContext.
using Northwind.EntityModels; // To use NorthwindContext.
using ShipperEntity = Northwind.EntityModels.Shipper;
namespace Northwind.Grpc.Service.Services;
public class ShipperService : Shipper.ShipperBase
{
    private readonly ILogger<ShipperService> _logger;
    private readonly NorthwindContext _db;
    public ShipperService(ILogger<ShipperService> logger, NorthwindContext context)
    {
        _logger = logger;
        _db = context;
    }
    public override async Task<ShipperReply?> GetShipper(ShipperRequest request, ServerCallContext context)
    {
        ShipperEntity? shipper = await _db.Shippers
          .FindAsync(request.ShipperId);
        return shipper is null ? null : ToShipperReply(shipper);
    }
    // A mapping method to convert from a Shipper in the
    // entity model to a gRPC ShipperReply.
    private ShipperReply ToShipperReply(ShipperEntity shipper)
    {
        return new ShipperReply
        {
            ShipperId = shipper.ShipperId,
            CompanyName = shipper.CompanyName,
            Phone = shipper.Phone
        };
    }
}
