using AutoMapper;
using Discount.Grpc.Entities;
using Discount.Grpc.Protos;
using Discount.Grpc.Repositories;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discount.Grpc.Services
{
    public class DiscountService : DiscountProtoService.DiscountProtoServiceBase
    {

        private readonly IDiscountRepository _discountRepository;
        private readonly ILogger<DiscountService> _logger;
        private readonly IMapper _mapper;

        public DiscountService(IDiscountRepository discountRepository, ILogger<DiscountService> logger, IMapper mapper)
        {
            _discountRepository = discountRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public override async Task<CouponModels> GetDiscount(GetDiscountRequest request, ServerCallContext context)
        {
            // return base.GetDiscount(request, context);
            var coupon = await _discountRepository.GetDiscount(request.ProductName);
            if(coupon == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Discount with ProductName={request.ProductName} is Not Find"));
            }
            _logger.LogInformation("data allready return now");
            var couponModel = _mapper.Map<CouponModels>(coupon);
            return couponModel;
        }

        public override async Task<CouponModels> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
        {
            var coupon = _mapper.Map<Coupon>(request.Coupon);
            await _discountRepository.CreateDiscount(coupon);

            _logger.LogInformation("this createed", coupon.ProductName);

            var couponModel = _mapper.Map<CouponModels>(coupon);
            return couponModel;
        }

        public override async Task<CouponModels> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
        {
            var coupon = _mapper.Map<Coupon>(request.Coupon);
            await _discountRepository.UpdateDiscount(coupon);

            _logger.LogInformation("this Updated", coupon.ProductName);

            var couponModel = _mapper.Map<CouponModels>(coupon);
            return couponModel;
        }
        public override async Task<DeleteDiscountRespons> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
        {
            var result = await _discountRepository.DeleteDiscount(request.ProductName);

            var serverCallContext = new DeleteDiscountRespons
            {
                Success = result
            };
            return serverCallContext;
        }
    }
}
