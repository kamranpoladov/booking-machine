using BookingMachine.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingMachine.Controllers
{
	[Authorize(Roles = "Admin")]
	public class AdminController : Controller
	{
		private readonly AppDbContext _db;

		public AdminController(AppDbContext db)
		{
			_db = db;
		}
		
		public IActionResult Computers()
		{
			var computers = _db.Computers;
			return View(computers);
		}

		public IActionResult Users()
		{
			var users = _db.Users;
			return View(users);
		}
	}
}