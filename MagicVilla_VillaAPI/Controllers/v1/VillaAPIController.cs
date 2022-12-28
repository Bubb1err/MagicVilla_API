using AutoMapper;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTO;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace MagicVilla_VillaAPI.Controllers.v1
{
    //[Route("api/[controller]")]
    [Route("api/v{version:apiVersion}/VillaAPI")]
    [ApiController]
    [ApiVersion("1.0")]
    public class VillaAPIController : ControllerBase
    {
        protected APIResponse response;
        private readonly IVillaRepository villaRepository;
        private readonly IMapper mapper;
        public VillaAPIController(IVillaRepository villaRepository, IMapper mapper)
        {
            this.villaRepository = villaRepository;
            this.mapper = mapper;
            response = new();
        }
        [Authorize]
        [HttpGet]
        [ResponseCache(CacheProfileName = "Default30")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetVillas([FromQuery(Name = "filterOccupancy")] int? occupancy,
            [FromQuery] string? search, int pageSize = 2, int pageNumber = 1)
        {
            try
            {
                IEnumerable<Villa> villaList;
                if(occupancy > 0)
                {
                    villaList = await villaRepository.GetAllAsync(u => u.Occupancy == occupancy,
                        pageSize:pageSize, pageNumber:pageNumber);
                }
                else
                {
                    villaList = await villaRepository.GetAllAsync(pageSize: pageSize, pageNumber: pageNumber);
                }
                if(!string.IsNullOrEmpty(search))
                {
                    villaList = villaList.Where(u => u.Name.ToLower().Contains(search));
                }
                Pagination pagination = new Pagination() { PageNumber = pageNumber, PageSize = pageSize};

                //Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(pagination, typeof(Pagination)));
                response.Result = mapper.Map<List<VillaDTO>>(villaList);
                response.StatusCode = System.Net.HttpStatusCode.OK;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Errors = new List<string>() { ex.ToString() };
            }
            return response;

        }


        [HttpGet("{id:int}", Name = "GetVilla")]
        [Authorize(Roles = "admin")]
        [ResponseCache(CacheProfileName = "Default30")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        //[ProducesResponseType(200, Type = typeof(VillaDTO)]
        //[ProducesResponseType(400)]
        //[ProducesResponseType(404)]
        public async Task<ActionResult<APIResponse>> GetVilla(int id)
        {
            try
            {
                if (id == 0)
                {
                    response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    response.IsSuccess = false;
                    return BadRequest(response);
                }
                var villa = await villaRepository.GetAsync(u => u.Id == id);
                if (villa == null)
                {
                    response.StatusCode = System.Net.HttpStatusCode.NotFound;
                    response.IsSuccess = false;
                    return NotFound(response);
                }
                response.Result = mapper.Map<VillaDTO>(villa);
                response.StatusCode = System.Net.HttpStatusCode.OK;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Errors = new List<string>() { ex.ToString() };
            }
            return response;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> Create([FromBody] VillaCreateDTO createVilla)
        {
            try
            {
                if (await villaRepository.GetAsync(u => u.Name.ToLower() == createVilla.Name.ToLower()) != null)
                {
                    ModelState.AddModelError("CustomError", "Villa`s name must be unique!");
                    response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    response.IsSuccess = false;
                    return BadRequest(response);
                }
                if (createVilla == null)
                {
                    response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    response.IsSuccess = false;
                    return BadRequest(response);
                }
                Villa model = mapper.Map<Villa>(createVilla);
                //Villa model = new()
                //{
                //    Name = createVilla.Name,
                //    Amenity = createVilla.Amenity,
                //    Details = createVilla.Details,
                //    ImageUrl = createVilla.ImageUrl,
                //    Occupancy = createVilla.Occupancy,
                //    Rate = createVilla.Rate,
                //    Sqft = createVilla.Sqft
                //};
                await villaRepository.CreateAsync(model);
                response.Result = mapper.Map<VillaDTO>(model);
                response.StatusCode = System.Net.HttpStatusCode.Created;

                return CreatedAtRoute("GetVilla", new { id = model.Id }, response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Errors = new List<string>() { ex.ToString() };
            }
            return response;

        }

        [Authorize(Roles = "CUSTOM")]
        [HttpDelete("{id:int}", Name = "DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<APIResponse>> DeleteVilla(int id)
        {
            try
            {
                if (id == 0)
                {
                    response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    response.IsSuccess = false;
                    return BadRequest(response);
                }
                var villa = await villaRepository.GetAsync(u => u.Id == id);
                if (villa == null)
                {
                    response.StatusCode = System.Net.HttpStatusCode.NotFound;
                    response.IsSuccess = false;
                    return NotFound(response);
                }
                await villaRepository.RemoveAsync(villa);
                response.StatusCode = System.Net.HttpStatusCode.NoContent;
                response.IsSuccess = true;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Errors = new List<string>() { ex.ToString() };
            }
            return response;
        }

        [HttpPut("{id:int}", Name = "UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<APIResponse>> UpdateVilla(int id, [FromBody] VillaUpdateDTO updateVilla)
        {
            try
            {
                if (id == 0 || id != updateVilla.Id)
                {
                    response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    response.IsSuccess = false;
                    return BadRequest(response);
                }
                Villa model = mapper.Map<Villa>(updateVilla);

                await villaRepository.UpdateAsync(model);
                response.StatusCode = System.Net.HttpStatusCode.NoContent;
                response.IsSuccess = true;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Errors = new List<string>() { ex.ToString() };
            }
            return response;
        }

        [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDTO> patch)
        {
            if (id == 0 || patch == null)
            {
                return BadRequest();
            }
            var villa = await villaRepository.GetAsync(u => u.Id == id, tracked: false);

            VillaUpdateDTO villaDTO = mapper.Map<VillaUpdateDTO>(villa);

            if (villa == null)
            {
                return BadRequest();
            }
            patch.ApplyTo(villaDTO, ModelState);
            Villa model = mapper.Map<Villa>(villaDTO);

            await villaRepository.UpdateAsync(model);

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            return NoContent();

        }
    }
}
