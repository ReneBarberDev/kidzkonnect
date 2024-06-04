using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using kidzkonnect2._0.Models;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace kidzkonnect2._0.Controllers
{
    public class UtilisateursController : Controller
    {
        private readonly ILogger<UtilisateursController> _logger;
        private readonly kidzkonnectContext _context;

        public UtilisateursController(kidzkonnectContext context, ILogger<UtilisateursController> logger)
        {
            _context = context;
            _logger = logger;
        }

        private async Task<UtilisateurEtProfils> GetUtilisateurEtProfils(int userId)
        {
            // Fetch the user from the database
            var utilisateur = await _context.Utilisateurs
                                             .FirstOrDefaultAsync(u => u.IdUtilisateur == userId);

            // Fetch the profiles related to the user
            var profils = await _context.ProfilEnfants
                                         .Where(p => p.IdUtilisateur == userId)
                                         .ToListAsync();

            // Return a new instance of UtilisateurEtProfils with the data retrieved
            return new UtilisateurEtProfils
            {
                User = utilisateur,
                ProfilEnfants = profils
            };
        }

        // GET: Utilisateurs/Details/5
        public async Task<IActionResult> Details()
        {
            var userIdSession = HttpContext.Session.GetInt32("UserId");
            if (!userIdSession.HasValue)
            {
                // If there is no userId in the session, redirect to the login page or handle accordingly
                return RedirectToAction("SignInUp", "Home");
            }

            int userId = userIdSession.Value;

            var utilisateur = await _context.Utilisateurs
                .FirstOrDefaultAsync(m => m.IdUtilisateur == userId);
            if (utilisateur == null)
            {
                return NotFound();
            }

            return View(utilisateur);
        }

        // GET: Utilisateurs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Utilisateurs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdUtilisateur,Nom,Prenom,DateNaissance,Adresse,Courriel,Password,EtatConnexion,userPicture")] Utilisateur utilisateur, IFormFile userPicture)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (userPicture != null && userPicture.Length > 0)
                    {
                        // Generate a unique file name
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(userPicture.FileName);

                        // Combine the uploads folder with the file name
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", fileName);

                        // Copy the file to the specified path
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await userPicture.CopyToAsync(stream);
                        }

                        // Set the userPicture property of the utilisateur object
                        utilisateur.UserPicture = "/uploads/" + fileName;
                    }

                    _context.Add(utilisateur);
                    await _context.SaveChangesAsync();

                    // Set the user's IdUtilisateur in the session
                    HttpContext.Session.SetInt32("UserId", utilisateur.IdUtilisateur);

                    // Redirect to the index of profilenfants
                    return RedirectToAction("Index", "ProfilEnfants");
                }
                catch (Exception ex)
                {
                    // Log the exception
                    Console.WriteLine(ex.ToString());
                    return View("Error");
                }
            }

            // If ModelState is not valid, return the entered values to the view
            ViewBag.Email = utilisateur.Courriel; // Make sure Courriel is the correct property holding the email value
            ViewBag.Password = utilisateur.Password;

            return View(utilisateur);
        } 

        // GET: Utilisateurs/Edit/5
        public async Task<IActionResult> Edit()
        {
            var userIdSession = HttpContext.Session.GetInt32("UserId");
            if (!userIdSession.HasValue)
            {
                // If there is no userId in the session, redirect to the login page or handle accordingly
                return RedirectToAction("SignInUp", "Home");
            }

            int userId = userIdSession.Value;

            var utilisateur = await _context.Utilisateurs.FindAsync(userId);
            if (utilisateur == null)
            {
                return NotFound();
            }
            return View(utilisateur);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("IdUtilisateur,Nom,Prenom,DateNaissance,Adresse,Courriel,Password,UserPicture")] Utilisateur utilisateur, IFormFile userPicture)
        {
            var userIdSession = HttpContext.Session.GetInt32("UserId");
            if (!userIdSession.HasValue)
            {
                // If there is no userId in the session, redirect to the login page or handle accordingly
                return RedirectToAction("SignInUp", "Home");
            }

            int userId = userIdSession.Value;

            if (ModelState.IsValid)
            {
                try
                {
                    var existingUtilisateur = await _context.Utilisateurs.FindAsync(userId);
                    if (existingUtilisateur == null)
                    {
                        Console.WriteLine("User not found: The user with the provided ID does not exist.");
                        return NotFound();
                    }

                    // Update properties of existing utilisateur with values from edited utilisateur
                    existingUtilisateur.Nom = utilisateur.Nom;
                    existingUtilisateur.Prenom = utilisateur.Prenom;
                    existingUtilisateur.DateNaissance = utilisateur.DateNaissance;
                    existingUtilisateur.Adresse = utilisateur.Adresse;
                    existingUtilisateur.Courriel = utilisateur.Courriel;
                    existingUtilisateur.Password = utilisateur.Password;

                    // Handle user picture
                    if (userPicture != null && userPicture.Length > 0)
                    {
                        // Generate a unique file name
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(userPicture.FileName);

                        // Combine the uploads folder with the file name
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", fileName);

                        // Copy the file to the specified path
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await userPicture.CopyToAsync(stream);
                        }

                        // Set the UserPicture property of the existingUtilisateur object
                        existingUtilisateur.UserPicture = "/uploads/" + fileName;
                    }

                    // Update existing utilisateur in the database
                    _context.Update(existingUtilisateur);
                    await _context.SaveChangesAsync(); // Save changes to the database

                    Console.WriteLine("User updated successfully.");
                    return RedirectToAction("Index", "ProfilEnfants");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UtilisateurExists(utilisateur.IdUtilisateur))
                    {
                        Console.WriteLine("User not found during update: The user with the provided ID does not exist.");
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception)
                {
                    // Log or handle the exception appropriately
                    Console.WriteLine("An error occurred while updating the user.");
                    ModelState.AddModelError("", "An error occurred while updating the user.");
                    return View(utilisateur);
                }
            }

            Console.WriteLine("Model state is not valid: The provided data is not valid.");
            return View(utilisateur);
        }




        // GET: Utilisateurs/Delete/5
        public async Task<IActionResult> Delete()
        {
            var userIdSession = HttpContext.Session.GetInt32("UserId");
            if (!userIdSession.HasValue)
            {
                // If there is no userId in the session, redirect to the login page or handle accordingly
                return RedirectToAction("SignInUp", "Home");
            }

            int userId = userIdSession.Value;

            var utilisateur = await _context.Utilisateurs
                .FirstOrDefaultAsync(m => m.IdUtilisateur == userId);
            if (utilisateur == null)
            {
                return NotFound();
            }

            return View(utilisateur);
        }

        // POST: Utilisateurs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed()
        {
            var userIdSession = HttpContext.Session.GetInt32("UserId");
            if (!userIdSession.HasValue)
            {
                // If there is no userId in the session, redirect to the login page or handle accordingly
                return RedirectToAction("SignInUp", "Home");
            }

            int userId = userIdSession.Value;

            if (_context.Utilisateurs == null)
            {
                return Problem("Entity set 'kidzkonnectContext.Utilisateurs'  is null.");
            }
            var utilisateur = await _context.Utilisateurs.FindAsync(userId);
            if (utilisateur != null)
            {
                _context.Utilisateurs.Remove(utilisateur);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Create));
        }

        private bool UtilisateurExists(int id)
        {
            return (_context.Utilisateurs?.Any(e => e.IdUtilisateur == id)).GetValueOrDefault();
        }

        //--------------------------------------------------------COOKIES SESSION------------------------------//
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (ModelState.IsValid)
            {
                var utilisateur = await _context.Utilisateurs
                    .FirstOrDefaultAsync(u => u.Courriel == loginViewModel.Courriel && u.Password == loginViewModel.Password);

                if (utilisateur != null)
                {
                    HttpContext.Session.SetInt32("UserId", utilisateur.IdUtilisateur);
                    _logger.LogInformation($"UserId set in session: {utilisateur.IdUtilisateur}"); // Logging the UserId
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    _logger.LogWarning("Invalid login attempt for user: {Email}", loginViewModel.Courriel); // Logging invalid attempt
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View("~/Views/Home/SignInUp.cshtml", loginViewModel);
                }
            }
            else
            {
                _logger.LogWarning("Login attempt with invalid model state for user: {Email}", loginViewModel.Courriel); // Logging model state issues
                return View("~/Views/Home/SignInUp.cshtml", loginViewModel);
            }
        }

        // Logout 
        public IActionResult Logout()
        {
            // Invalidate the session cookie
            HttpContext.Session.Clear();
            HttpContext.Response.Cookies.Delete(".AspNetCore.Session");

            // Redirect to the sign-in/up page
            return RedirectToAction("SignInUp", "Home");
        }
    }
}
