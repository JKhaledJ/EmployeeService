using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity;
using EmpsData;
using System.Web.Http.Cors;
using System.Threading;

namespace EmployeeService.Controllers
{
    [EnableCorsAttribute("*", "*", "*")]
    public class EmployeesController : ApiController
    {
        EmployeeDBEntities entities = new EmployeeDBEntities();
       

        [HttpGet]
        [BasicAuthentication]
        public HttpResponseMessage Get(string gender="All")
        {
            string username = Thread.CurrentPrincipal.Identity.Name;

            using (entities)
            {
                switch (username.ToLower())
                {
                    case "male": return Request.CreateResponse(HttpStatusCode.OK, entities.Emps.Where(x=>x.Gender.ToLower()==gender).ToList()); break;
                    case "female": return Request.CreateResponse(HttpStatusCode.OK, entities.Emps.Where(x => x.Gender.ToLower() == gender).ToList()); break;
                    default: return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
                
            }
        }
        [HttpGet]
        public HttpResponseMessage GetByID(int id)
        {
            using (entities)
            {
                var emp= entities.Emps.FirstOrDefault(e=>e.ID==id);
                if (emp!=null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, emp);
                }
                else
                {
                    var message = Request.CreateResponse(HttpStatusCode.NotFound,"Employee with id "+ id+" is not found.");
                    return message;
                }
                
            }
        }
        [HttpPost]
        public HttpResponseMessage Post(Emp employee)
        {
            try
            {
                 using (entities)
                {
                    entities.Emps.Add(employee);
                    entities.SaveChanges();

                    var message = Request.CreateResponse(HttpStatusCode.Created,employee);
                    message.Headers.Location = new Uri(Request.RequestUri+employee.ID.ToString());
                    return message;
                }
            }
            catch(Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex);
            }
          
        }
        /// <summary>
        /// Put
        /// </summary>
        /// <param name="employee"></param>
        [HttpPut]
        public HttpResponseMessage Put(int id,[FromBody]Emp employee)
        {
            try
            {
                var emp = entities.Emps.FirstOrDefault(x=>x.ID==id);
                if (emp==null)
                {
                    var message = Request.CreateResponse(HttpStatusCode.NotFound, "This employee is not found.");

                    return message;
                }
                else
                {
                    entities.Entry(employee).State = EntityState.Modified;

                    entities.SaveChanges();
                    var message = Request.CreateResponse(HttpStatusCode.OK, "Successfully updated");

                    return message;
                }
            }
            catch(Exception ex)
            {
                var message= Request.CreateResponse(HttpStatusCode.BadRequest,ex);
                return message;
            }
            

        }
        [HttpDelete]
        public HttpResponseMessage Delete(int id)
        {
            using (entities)
            {
                var emp = entities.Emps.Find(id);
                if (emp != null)
                {
                    entities.Emps.Remove(emp);
                    entities.SaveChanges();
                    var message = Request.CreateResponse(HttpStatusCode.OK,"The item has been successfully deleted.");
                    return message;
                }
                else
                {
                    var message = Request.CreateResponse(HttpStatusCode.BadRequest, "The employee with id = "+id+" does not exist.");
                    return message;
                }
                

                //entities.Entry(emp).State = System.Data.Entity.EntityState.Deleted;
                
            }



        }
        
    }
}
