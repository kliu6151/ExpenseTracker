using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Expense_Tracker.Models;
using System.Drawing.Printing;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Expense_Tracker.Filters;
using Microsoft.AspNetCore.Identity;
using Expense_Tracker.Models.Identity;
using System.Security.Claims;
using Expense_Tracker.Services;

namespace Expense_Tracker.Controllers
{
    [RemoveUserFilter]
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserService _userService;

        public CategoryController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IUserService userService)
        {
            _context = context;
            _userManager = userManager;
            _userService = userService;

        }



        // GET: Category
        public async Task<IActionResult> Index()
        {
            var currUser = await _userManager.GetUserAsync(User);
            if (currUser != null && _context != null)
            {
                await _context.Entry(currUser).Collection(u => u.Categories!).LoadAsync();
            }
            var userCategories = currUser!.Categories!.ToList();
            return View(userCategories);

        }


        // GET: Category/AddOrEdit
        public IActionResult AddOrEdit(int id = 0)
        {
            if (id == 0)
                return View(new Category());
            else
                return View(_context.Categories.Find(id));

        }

        // POST: Category/AddOrEdit
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit([Bind("CategoryId,Title,Icon,Type")] Category category)
        {

            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                // Get the current user's ID
/*                var userId = _userService.GetCurrentUserId();
*/                category.UserId = currentUser.Id;

                currentUser.Categories ??= new List<Category>();
                currentUser.Categories.Add(category);

                if (category.CategoryId == 0)
                {
                    // New category, add to the context
                    _context.Add(category);
                }
                else
                {
                    // Existing category, update in the context
                    _context.Update(category);
                }

                // Save changes to the database
                await _context.SaveChangesAsync();

                // Redirect to the Index action
                return RedirectToAction(nameof(Index));
            }

            foreach (var modelStateKey in ModelState.Keys)
            {
                var modelStateVal = ModelState[modelStateKey];
                foreach (var error in modelStateVal.Errors)
                {
                    Debug.WriteLine($"{modelStateKey}: {error.ErrorMessage}");
                }
            }

            // If ModelState is not valid, return to the view with the category
            return View(category);
        }


        // POST: Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Categories == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Categories'  is null.");
            }
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}