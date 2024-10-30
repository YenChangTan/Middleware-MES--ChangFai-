using Middleware.model;
using Middleware.TemporaryDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Opc.Ua;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Reflection;

namespace Middleware.controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class MESAPIController : ControllerBase
    {


        [HttpGet]
        public async Task<ActionResult<string>> GetAllNode()
        {
            await Task.Delay(1000);
            return (Ok("success"));
        }

        [HttpPost]
        public async Task<IActionResult> GetPCBData()
        {

            // Read the request body JSON using StreamReader
            using (StreamReader reader = new StreamReader(Request.Body))
            {
                string requestBody = await reader.ReadToEndAsync();

                // Deserialize JSON to Variable object
                Console.WriteLine(requestBody);
                Pcb pcb = JsonConvert.DeserializeObject<Pcb>(requestBody);
                PCBData.pcb.Add(pcb);
            }
            return Ok(new { Receive = true});
        }

        

    }
}
