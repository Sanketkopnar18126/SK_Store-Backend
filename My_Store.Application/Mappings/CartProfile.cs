using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using My_Store.Application.DTOs.Cart;
using My_Store.Domain.Entities;

namespace My_Store.Application.Mappings
{
    public class CartProfile:Profile
    {
        public CartProfile()
        {
            CreateMap<CartItem, CartItemDto>()
              .ForMember(dest => dest.ProductImage, opt => opt.MapFrom(src => src.Product.ImageUrls != null && src.Product.ImageUrls.Length > 0
                ? src.Product.ImageUrls[0]
                : string.Empty))
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
            .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice));

            CreateMap<Cart, CartDto>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items))
                
                .ForMember(dest => dest.Total, opt => opt.Ignore()); 
        }
    }
}
