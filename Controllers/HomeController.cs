using BookingMachine.Interfaces;
using Microsoft.AspNetCore.Mvc;
using BookingMachine.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace BookingMachine.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IComputerRepository _computerRepository;
        private readonly IQueueRepository _queueRepository;

        public HomeController(IComputerRepository computerRepository, IQueueRepository queueRepository)
        {
            _computerRepository = computerRepository;
            _queueRepository = queueRepository;
        }
        public IActionResult Index()
        {
            _computerRepository.RemoveOverdue();
            _queueRepository.BookFromQueue();
            _queueRepository.MoveQueue();
            var homeViewModel = new HomeViewModel
            {
                CommonPlaces = _computerRepository.GetNumberOfFreeCommons(),
                VipPlaces = _computerRepository.GetNumberOfFreeVips()
            };
            return View(homeViewModel);
        }
        
        [AllowAnonymous]
        public IActionResult Error() => View();
    }
}