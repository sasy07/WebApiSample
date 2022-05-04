using Microsoft.AspNetCore.Mvc;
using WebApiSample.Data.Repositories;

namespace WebApiSample.MyApi.Controllers;


[Route("api/[controller]")]
public class ValuesController : Controller
{
   #region constructor

   private readonly UserRepository _userRepository;

   public ValuesController(UserRepository userRepository)
   {
      _userRepository = userRepository;
   }

   #endregion
   
   
}