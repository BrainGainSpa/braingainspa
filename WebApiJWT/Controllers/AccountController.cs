using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace WebApiJWT.Controllers
{
    public class AccountController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage ValidLogin(string userName, string userPassword)
        {
            if (userName == "admin" && userPassword == "vsbgadmin")
            {
                return Request.CreateResponse(HttpStatusCode.OK, value: TokenManager.GenerateToken(userName));
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadGateway, message: "User name and password is invalid");
            }
        }

        [HttpGet]
        [CustomAthenticationFilter]
        public HttpResponseMessage AuthenticateUser()
        {
            return Request.CreateResponse(HttpStatusCode.OK, value: "Success");
        }
    }
}
