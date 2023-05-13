using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Xml.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using STGeneticsTest.DAL;
using STGeneticsTest.Models;

namespace STGeneticsTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnimalController : Controller
    {
        private readonly IConfiguration _config;

        public AnimalController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet("get_all"), Authorize(Roles = "Admin")]
        public async Task<ActionResult<DefaultResponse<IEnumerable<AnimalDTO>>>> GetAllAnimals()
        {
            DefaultResponse<IEnumerable<AnimalDTO>> response = new DefaultResponse<IEnumerable<AnimalDTO>>();

            try
            {
                DAL.AnimalTE animalTE = new DAL.AnimalTE(_config);
                response.Data = await animalTE.GetAllAnimals();
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Msg = ex.Message;
            }

            return Ok(response);

        }

        [HttpGet("get/{animalId}")]
        public async Task<ActionResult<DefaultResponse<AnimalDTO>>> GetAnimal(int animalId)
        {
            DefaultResponse<AnimalDTO> response = new DefaultResponse<AnimalDTO>();

            try
            {
                DAL.AnimalTE animalTE = new DAL.AnimalTE(_config);

                var data = await animalTE.GetAnimal(animalId);
                if (data.AnimalId == 0)
                    return NotFound($"Animal with ID {animalId} not found.");
                else
                    response.Data = data;
            }
            catch(Exception ex)
            {
                response.Error = true;
                response.Msg = ex.Message;
            }

            return Ok(response);
        }

        [HttpPost("create"), Authorize(Roles = "Admin")]
        public async Task<ActionResult<DefaultResponse<string>>> CreateAnimal(Animal animal)
        {
            DefaultResponse<string> response = new DefaultResponse<string>();

            try
            {
                DAL.AnimalTE animalTE = new DAL.AnimalTE(_config);

                response.Data = await animalTE.CreateAnimal(animal);
            }
            catch(Exception ex)
            {
                response.Error = true;
                response.Msg = ex.Message;
            }

            return Ok(response);
        }

        [HttpPut("update"), Authorize(Roles = "Admin")]
        public async Task<ActionResult<DefaultResponse<AnimalDTO>>> UpdateAnimal(Animal animal)
        {
            DefaultResponse<AnimalDTO> response = new DefaultResponse<AnimalDTO>();

            try
            {
                DAL.AnimalTE animalTE = new DAL.AnimalTE(_config);

                AnimalDTO animalDTO = await animalTE.UpdateAnimal(animal);
                if (animalDTO.AnimalId == 0)
                {
                    return NotFound($"Animal with ID {animal.AnimalId} not found.");
                }
                else
                {
                    response.Data = animalDTO;
                }
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Msg = ex.Message;
            }

            return Ok(response);

          
        }

        [HttpDelete("delete/{animalId}"), Authorize(Roles = "Admin")]
        public async Task<ActionResult<DefaultResponse<string>>> DeleteAnimal(int animalId)
        {
            DefaultResponse<string> response = new DefaultResponse<string>();

            try
            {
                DAL.AnimalTE animalTE = new DAL.AnimalTE(_config);

                string res = await animalTE.DeleteAnimal(animalId);

                if (res == string.Empty)
                {
                    return NotFound($"Animal with ID {animalId} not found.");
                }
                else
                {
                    response.Data = res;
                }
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Msg = ex.Message;
            }

            return Ok(response);


        }

        [HttpPost("get_filters"), Authorize(Roles = "Admin")]
        public async Task<ActionResult<DefaultResponse<IEnumerable<AnimalDTO>>>> GetAnimalsFiltered(AnimalFilteredRequest request)
        {
            DefaultResponse<IEnumerable<AnimalDTO>> response = new DefaultResponse<IEnumerable<AnimalDTO>>();
            try
            {
                DAL.AnimalTE animalTE = new DAL.AnimalTE(_config);
                string nameTrimmed = request.Name.Trim();

                IEnumerable<AnimalDTO> animals = await animalTE.GetAnimalsFiltered(request);

                response.Data = animals;
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Msg = ex.Message;
            }

            return Ok(response);
        }


        [HttpPost("Purchase"), Authorize(Roles = "Admin")]
        public async Task<ActionResult<DefaultResponse<Purchase>>> Purchase(PurchaseRequest request)
        {
            DefaultResponse<Purchase> response = new DefaultResponse<Purchase>();

            try
            {
                DAL.AnimalTE animalTE = new DAL.AnimalTE(_config);

                Purchase Purchase = await animalTE.Purchase(request);
                if(Purchase.PurchaseId == 0)
                    return NotFound($"No animals active to purchase.");
                else
                    response.Data = Purchase;
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Msg = ex.Message;
            }

            return Ok(response);
        }
    }
}
