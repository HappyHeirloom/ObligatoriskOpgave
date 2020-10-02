using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Threading.Tasks;
using Fan;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FanREST.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FanOutPutController : ControllerBase
    {
        FanOutput fan = new FanOutput();
        // Create list of fanoutputs
        private static List<FanOutput> FanList = new List<FanOutput>()
        {
            new FanOutput(0,"FanRoom2001", 15, 50),
            new FanOutput(0,"FanRoom189", 20, 40),
            new FanOutput(0,"FanRoom20", 25, 32),
            new FanOutput(0,"FanRoom301", 23, 67),
            new FanOutput(0,"FanRoom809", 21, 46),
            new FanOutput(0,"FanRoom13201", 18, 55),

        };

        // GET: api/<FanOutPutController>
        [HttpGet]
        public List<FanOutput> Get()
        {
            return FanList;
        }

        // GET api/<FanOutPutController>/5
        [HttpGet("{id}")]
        public FanOutput Get(int id)
        {
            return FanList[id];
        }

        // POST api/<FanOutPutController>
        [HttpPost]
        public void Post([FromBody] FanOutput value)
        {
            FanList.Add(value);
        }

        // PUT api/<FanOutPutController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] FanOutput value)
        {
            FanOutput fan = Get(id);
            if (fan != null)
            {
                fan.Id = value.Id;
                fan.Name = value.Name;
                fan.Temp = value.Temp;
                fan.Fugt = value.Fugt;
            }
        }

        // DELETE api/<FanOutPutController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            FanOutput fan = Get(id);
            FanList.Remove(fan);
        }
    }
}
