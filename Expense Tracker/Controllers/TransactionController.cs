using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Expense_Tracker.Models;
using Microsoft.AspNetCore.Authorization;
using Expense_Tracker.Filters;
using Expense_Tracker.Services;
using Microsoft.AspNetCore.Identity;
using Expense_Tracker.Models.Identity;
using System.Diagnostics;
using Expense_Tracker.DTOs;

namespace Expense_Tracker.Controllers
{
    [Authorize]
    [RemoveUserFilter]
    public class TransactionController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;

        public TransactionController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IUserService userService)
        {
            _context = context;
            _userManager = userManager;
            _userService = userService;
        }

        // GET: Transaction
        public async Task<IActionResult> Index()
        {
            var currUser = await _userManager.GetUserAsync(User);

            var userTransactionsDto = currUser!.Transactions!
               .Select(t => new RecentTransactionDto
               {
                   CategoryId = t.CategoryId,
                   CategoryTitleWithIcon = t.CategoryTitleWithIcon,
                   Date = t.Date,
                   FormattedAmount = t.FormattedAmount
               })
               .ToList();

            return View(userTransactionsDto);
            /* LoadAsync future use possible */
            /*if (currUser != null && _context != null)
            {
                await _context.Entry(currUser).Collection(u => u.Transactions!).Query().Include(t => t.Category).LoadAsync();
            }
            var userTransactions = currUser!.Transactions!.ToList();
            return View(userTransactions);
        }*/
        }

        // GET: Transaction/AddOrEdit
        public IActionResult AddOrEdit(int id = 0)
        {

            PopulateCategories();
            if (id == 0)
                return View(new Transaction());
            else
                return View(_context.Transactions.Find(id));
        }

        // POST: Transaction/AddOrEdit
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit([Bind("TransactionId,CategoryId,Amount,Note,Date")] Transaction transaction)
        {
            var userId = _userService.GetCurrentUserId();
            Debug.WriteLine("BEFORE valid transaction model");
            if (ModelState.IsValid)
            {
                Debug.WriteLine("Valid transaction model");
                var currentUser = await _userManager.GetUserAsync(User);
                // Get the current user's ID
                /*                var userId = _userService.GetCurrentUserId();
                */
                transaction.UserId = currentUser.Id;
                Debug.WriteLine("UserId: " + currentUser.Id);
                Debug.WriteLine("CategoryId: " + transaction.CategoryId);
                transaction.Category = await _context.Categories.FindAsync(transaction.CategoryId);
                currentUser.Transactions ??= new List<Transaction>();

                Debug.WriteLine(transaction.Amount);
                Debug.WriteLine(transaction.Category);
                Debug.WriteLine(transaction.Category.Title);
                Debug.WriteLine(transaction.Category.Icon);
                    
                if (transaction.TransactionId == 0)
                {
                    currentUser.Transactions.Add(transaction);
                    _context.Add(transaction);
                }
                else
                {
                    _context.Update(transaction);

                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            PopulateCategories();
            return View(transaction);
        }

        // POST: Transaction/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Transactions == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Transactions'  is null.");
            }
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction != null)
            {
                _context.Transactions.Remove(transaction);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        [NonAction]
        public void PopulateCategories()
        {
            var userId = _userService.GetCurrentUserId();
            var CategoryCollection = _context.Categories
                .Where(c => c.UserId == userId)  // Filter by the current user's ID
                .ToList();
            Category DefaultCategory = new() { CategoryId = 0, Title = "Choose a Category" };
            CategoryCollection.Insert(0, DefaultCategory);
            ViewBag.Categories = CategoryCollection;
        }
    }
}