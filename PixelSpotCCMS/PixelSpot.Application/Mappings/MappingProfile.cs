using AutoMapper;
using PixelSpot.Application.DTOs;
using PixelSpot.Domain.Entities;
using PixelSpot.Domain.ValueObjects;

namespace PixelSpot.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Value Objects
        CreateMap<BankDetails, BankDetailsDto>().ReverseMap();
        CreateMap<GeoCoordinate, GeoCoordinateDto>().ReverseMap();
        CreateMap<ScreenSize, ScreenSizeDto>().ReverseMap();

        // User entities
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.BankDetails, opt => opt.MapFrom(src => src.BankDetails));

        CreateMap<SubUser, SubUserDto>()
            .ForMember(dest => dest.Permissions, opt => opt.MapFrom(src => src.Permissions));

        CreateMap<Permission, PermissionDto>();

        // Screen entities
        CreateMap<Screen, ScreenDto>()
            .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => $"{src.Owner.FirstName} {src.Owner.LastName}"))
            .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location))
            .ForMember(dest => dest.Size, opt => opt.MapFrom(src => src.Size))
            .ForMember(dest => dest.Pricing, opt => opt.MapFrom(src => src.Pricing))
            .ForMember(dest => dest.Availabilities, opt => opt.MapFrom(src => src.Availabilities));

        CreateMap<ScreenPricing, ScreenPricingDto>();
        CreateMap<ScreenAvailability, ScreenAvailabilityDto>();
        CreateMap<ScreenMetrics, ScreenMetricsDto>();

        // Campaign entities
        CreateMap<Campaign, CampaignDto>()
            .ForMember(dest => dest.AdvertiserName, opt => opt.MapFrom(src => $"{src.Advertiser.FirstName} {src.Advertiser.LastName}"))
            .ForMember(dest => dest.Creatives, opt => opt.MapFrom(src => src.Creatives))
            .ForMember(dest => dest.Bookings, opt => opt.MapFrom(src => src.Bookings))
            .ForMember(dest => dest.Spent, opt => opt.Ignore())
            .ForMember(dest => dest.Remaining, opt => opt.Ignore());

        CreateMap<Creative, CreativeDto>();

        CreateMap<ScreenBooking, ScreenBookingDto>()
            .ForMember(dest => dest.ScreenName, opt => opt.MapFrom(src => src.Screen.Name))
            .ForMember(dest => dest.CreativeName, opt => opt.MapFrom(src => src.Creative.Name));

        CreateMap<CampaignRequest, CampaignRequestDto>()
            .ForMember(dest => dest.AdvertiserName, opt => opt.MapFrom(src => $"{src.Advertiser.FirstName} {src.Advertiser.LastName}"));

        CreateMap<WaitlistEntry, WaitlistEntryDto>()
            .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location));
    }
}
