using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using My_Store.Application.DTOs.Payment;
using My_Store.Domain.Entities;

namespace My_Store.Application.Mappings
{
    public class PaymentProfile:Profile
    {
        public PaymentProfile()
        {
            CreateMap<CreatePaymentOrderRequestDto, Payment>();

            CreateMap<Payment, CreatePaymentOrderResponseDto>()
                .ForMember(dest => dest.Key, opt => opt.Ignore());
        }
    }
}
