using AutoMapper;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTO;
using MagicVilla_VillaAPI.Repository;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaAPI.Controllers.v2
{
    [Route("api/v{version:apiVersion}/VillaNumberApi")]
    [ApiController]
    [ApiVersion("2.0")]
    public class VillaNumberAPIController : Controller
    {
        protected APIResponse response;
        private readonly IVillaNumberRepository villaNumbers;
        private readonly IVillaRepository villas;
        private readonly IMapper mapper;
        public VillaNumberAPIController(IVillaNumberRepository villaNumbers, IMapper mapper, IVillaRepository villas)
        {
            response = new();
            this.villaNumbers = villaNumbers;
            this.mapper = mapper;
            this.villas = villas;
        }

        [HttpGet]
        //[MapToApiVersion("2.0")]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

    }
}
