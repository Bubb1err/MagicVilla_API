using AutoMapper;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTO;
using MagicVilla_VillaAPI.Repository;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaAPI.Controllers.v1
{
    [Route("api/v{version:apiVersion}/VillaNumberApi")]
    [ApiController]
    [ApiVersion("1.0"/*Deprecated = true*/)]
    //[ApiVersion("2.0")]
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        //[MapToApiVersion("1.0")]
        public async Task<ActionResult<APIResponse>> GetVillaNumbers()
        {
            try
            {
                IEnumerable<VillaNumber> numbers = await villaNumbers.GetAllAsync();
                response.Result = mapper.Map<List<VillaNumberDTO>>(numbers);
                response.StatusCode = System.Net.HttpStatusCode.OK;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Errors = new List<string> { ex.ToString() };
            }
            return response;
        }


        //mapping to a new version 

        //[HttpGet]
        //[MapToApiVersion("2.0")]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}


        [HttpGet("{id:int}", Name = "GetVillaNumber")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetVillaNumber(int id)
        {
            try
            {
                if (id == 0)
                {
                    response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    response.IsSuccess = false;
                    return BadRequest(response);
                }
                var villaNumber = await villaNumbers.GetAsync(u => u.VillaNo == id);
                if (villaNumber == null)
                {
                    response.StatusCode = System.Net.HttpStatusCode.NotFound;
                    response.IsSuccess = false;
                    return NotFound(response);
                }
                response.Result = mapper.Map<VillaNumberDTO>(villaNumber);
                response.StatusCode = System.Net.HttpStatusCode.OK;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Errors = new List<string> { ex.ToString() };
            }
            return response;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CreateVillaNumber([FromBody] VillaNumberCreateDTO numberCreateDTO)
        {
            try
            {
                if (await villas.GetAsync(u => u.Id == numberCreateDTO.VillaID) == null)
                {
                    ModelState.AddModelError("Custom error", "Villa does not exist");
                    response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    response.IsSuccess = false;
                    return BadRequest(response);
                }
                if (await villaNumbers.GetAsync(u => u.VillaNo == numberCreateDTO.VillaNo) != null)
                {
                    ModelState.AddModelError("CustomError", "Villa`s number must be unique!");
                    response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    response.IsSuccess = false;
                    return BadRequest(response);
                }
                if (numberCreateDTO == null)
                {
                    response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    response.IsSuccess = false;
                    return BadRequest(response);
                }
                VillaNumber model = mapper.Map<VillaNumber>(numberCreateDTO);
                await villaNumbers.CreateAsync(model);
                response.Result = mapper.Map<VillaNumberDTO>(model);
                response.StatusCode = System.Net.HttpStatusCode.Created;

                return CreatedAtRoute("GetVillaNumber", new { id = model.VillaNo }, response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Errors = new List<string>() { ex.ToString() };
            }
            return response;
        }
        [HttpDelete("{id:int}", Name = "DeleteVillaNumber")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> DeleteVillaNumber(int id)
        {
            try
            {
                if (id == 0)
                {
                    response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    response.IsSuccess = false;
                    return BadRequest(response);
                }
                var villaNumber = await villaNumbers.GetAsync(u => u.VillaNo == id);
                if (villaNumber == null)
                {
                    response.StatusCode = System.Net.HttpStatusCode.NotFound;
                    response.IsSuccess = false;
                    return NotFound(response);
                }
                await villaNumbers.RemoveAsync(villaNumber);
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

        [HttpPut("{id:int}", Name = "UpdateVillaNumber")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> UpdateVillaNumber(int id, [FromBody] VillaNumberUpdateDTO updateVillaNumber)
        {
            try
            {
                if (await villas.GetAsync(u => u.Id == updateVillaNumber.VillaID) == null)
                {
                    ModelState.AddModelError("Custom error", "Villa does not exist");
                    response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    response.IsSuccess = false;
                    return BadRequest(response);
                }
                if (id == 0 || id != updateVillaNumber.VillaNo)
                {
                    response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    response.IsSuccess = false;
                    return BadRequest(response);
                }
                VillaNumber model = mapper.Map<VillaNumber>(updateVillaNumber);

                await villaNumbers.UpdateAsync(model);
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

    }
}
