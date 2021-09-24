using Discount.Grpc.Protos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Basket.Api.GrpcService
{
    public class DiscountGrpcService
    {
        private readonly DiscountProtoService.DiscountProtoServiceClient _protoService;

        public DiscountGrpcService(DiscountProtoService.DiscountProtoServiceClient protoService)
        {
            _protoService = protoService;
        }

        public async Task<CouponModels> GetDiscount(string productName)
        {
            var reauest = new GetDiscountRequest { ProductName = productName };
            var coupon = await _protoService.GetDiscountAsync(reauest);
            return coupon;
        }

    }
}
