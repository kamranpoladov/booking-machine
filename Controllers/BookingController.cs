using System.Security.Claims;
using System.Threading.Tasks;
using BookingMachine.Auth;
using BookingMachine.Interfaces;
using BookingMachine.Models;
using BookingMachine.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookingMachine.Controllers
{
    public class BookingController : Controller
    {
        private readonly IComputerRepository _computerRepository;
        private readonly IQueueRepository _queueRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly string _currentUserId;

        public BookingController(IComputerRepository computerRepository, IQueueRepository queueRepository,
            IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager)
        {
            _computerRepository = computerRepository;
            _queueRepository = queueRepository;
            _currentUserId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            _userManager = userManager;
        }
        
        public IActionResult Book() => View();

        [HttpPost]
        public async Task<IActionResult> Book(BookingViewModel bookingViewModel)
        {
            var user = await _userManager.FindByIdAsync(_currentUserId);
            var amountBooked = await _computerRepository.Book(bookingViewModel.AmountOfPlaces, bookingViewModel.Room,
                _currentUserId);
            if (amountBooked < bookingViewModel.AmountOfPlaces)
            {
                _queueRepository.PutInQueue(user, bookingViewModel.Room,
                    bookingViewModel.AmountOfPlaces - amountBooked);
            }
            return RedirectToAction("Index","Home");
        }

        public IActionResult Add()
        {
            var bookingViewModel = new BookingViewModel
            {
                AmountOfPlaces = 0,
                Room = Room.Any
            };
            return View(bookingViewModel);
        }

        public IActionResult Withdraw()
        {
            _computerRepository.RemoveBooking(_currentUserId, _computerRepository.GetAmountOfPlacesForUser(_currentUserId));
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Delete(int compId)
        {
            _computerRepository.DeleteBooking(compId, _currentUserId);
            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult DeleteForUser(string userId, int compId)
        {
            _computerRepository.DeleteBooking(compId, userId);
            return RedirectToAction("Computers", "Admin");
        }

        public IActionResult LeaveQueue()
        {
            _queueRepository.LeaveQueue(_currentUserId);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult RemovePlaceFromQueue(int queueId)
        {
            _queueRepository.RemovePlaceFromQueue(queueId);
            return RedirectToAction("Index", "Home");
        }
    }
}