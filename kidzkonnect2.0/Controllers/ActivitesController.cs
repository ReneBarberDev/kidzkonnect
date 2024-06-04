using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using kidzkonnect2._0.Models;

namespace kidzkonnect2._0.Controllers
{
    public class ActivitesController : Controller
    {
        private readonly kidzkonnectContext _context;

        public ActivitesController(kidzkonnectContext context)
        {
            _context = context;
        }

        // GET: Activites
        public async Task<IActionResult> Index()
        {
            var userIdSession = HttpContext.Session.GetInt32("UserId");
            if (!userIdSession.HasValue)
            {
                // If there is no userId in the session, redirect to the login page or handle accordingly
                return RedirectToAction("SignInUp", "Home");
            }
            int userId = userIdSession.Value;

            var user = await _context.Utilisateurs.FirstOrDefaultAsync(u => u.IdUtilisateur == userId);
            if (user == null)
            {
                // Handle the case where the user is not found in the database
                // Log out the user or handle the error accordingly
                return RedirectToAction("SignInUp", "Home");
            }

            // Retrieve activities related to the specified userId
            var activitiesList = await _context.Activites
                .Where(a => a.IdUtilisateurCreateur == userId)
                .Include(a => a.IdUtilisateurCreateurNavigation)
                .ToListAsync();

            // Create an instance of 'UtilisateurEtActivites' with the retrieved data
            var utilisateurEtActivites = new UtilisateurEtActivites
            {
                User = user,
                Activites = activitiesList
            };

            // Pass the 'UtilisateurEtActivites' instance to the view
            return View(utilisateurEtActivites);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activite = await _context.Activites
                .Include(a => a.IdUtilisateurCreateurNavigation)
                .FirstOrDefaultAsync(m => m.IdActivite == id);
            if (activite == null)
            {
                return NotFound();
            }

            // Retrieve the list of ProfilEnfants associated with the session's user
            var userIdSession = HttpContext.Session.GetInt32("UserId");
            if (!userIdSession.HasValue)
            {
                return RedirectToAction("SignInUp", "Home");
            }
            int userId = userIdSession.Value;
            var profilEnfants = await _context.ProfilEnfants
                .Where(pe => pe.IdUtilisateur == userId)
                .ToListAsync();

            // Pass the activity and profilEnfants to the view
            ViewData["ProfilEnfants"] = new SelectList(profilEnfants, "IdProfil", "Prenom");

            // Retrieve the list of participant IDs for the current activity
            var participationIds = await _context.Participations
                .Where(p => p.IdActivite == id)
                .Select(p => p.IdProfil)
                .ToListAsync();

            // Retrieve the participant names using the participant IDs
            var participants = await _context.ProfilEnfants
                .Where(pe => participationIds.Contains(pe.IdProfil))
                .ToListAsync();

            // Pass the activity and the list of participants to the view
            ViewData["Participants"] = participants;

            return View(activite);
        }


        // GET: Activites/Create
        public IActionResult Create()
        {
            ViewData["IdUtilisateurCreateur"] = new SelectList(_context.Utilisateurs, "IdUtilisateur", "IdUtilisateur");
            return View();
        }

        // POST: Activites/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdActivite,TitreActivite,DescriptionActivite,DateActivite,Emplacement,DetailsAdditionnel")] Activite activite)
        {
            var userIdSession = HttpContext.Session.GetInt32("UserId");
            if (!userIdSession.HasValue)
            {
                // If there is no userId in the session, redirect to the login page or handle accordingly
                return RedirectToAction("SignInUp", "Home");
            }

            if (ModelState.IsValid)
            {
                // Set the IdUtilisateurCreateur from the session
                activite.IdUtilisateurCreateur = userIdSession.Value;

                _context.Add(activite);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Activites", new { id = activite.IdActivite });
            }
            // If we reach this point, something went wrong, redisplay form
            return View(activite);
        }


        // GET: Activites/Edit
        public async Task<IActionResult> Edit(int? id)
        {
            var userIdSession = HttpContext.Session.GetInt32("UserId");
            if (!userIdSession.HasValue)
            {
                // If there is no userId in the session, redirect to the login page or handle accordingly
                return RedirectToAction("SignInUp", "Home");
            }

            if (id == null || _context.Activites == null)
            {
                return NotFound();
            }

            var activite = await _context.Activites.FindAsync(id);
            if (activite == null)
            {
                return NotFound();
            }
            return View(activite);
        }

        // POST: Activites/Edit/2
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdActivite,TitreActivite,DescriptionActivite,DateActivite,Emplacement,DetailsAdditionnel")] Activite activite)
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
                    // Retrieve the existing activity from the database
                    var existingActivite = await _context.Activites.FindAsync(id);
                    if (existingActivite == null)
                    {
                        // If the activity with the specified id is not found, return NotFound
                        return NotFound();
                    }

                    // Update properties of the existing activity with values from the edited activity
                    existingActivite.TitreActivite = activite.TitreActivite;
                    existingActivite.DescriptionActivite = activite.DescriptionActivite;
                    existingActivite.DateActivite = activite.DateActivite;
                    existingActivite.Emplacement = activite.Emplacement;
                    existingActivite.DetailsAdditionnel = activite.DetailsAdditionnel;

                    // Assuming there's logic for handling related picture upload

                    // Update the activity in the database
                    _context.Update(existingActivite);
                    await _context.SaveChangesAsync();

                    // Redirect to the Index action upon successful update
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    // Log the concurrency exception for debugging purposes
                    Console.WriteLine($"Concurrency exception occurred: {ex.Message}");
                    // Handle concurrency conflicts if necessary
                    ModelState.AddModelError(string.Empty, "Concurrency error occurred.");
                    return View(activite);
                }
                catch (Exception ex)
                {
                    // Log any other exceptions that may occur during the update process
                    Console.WriteLine($"An error occurred while updating the activity: {ex.Message}");
                    ModelState.AddModelError(string.Empty, "An error occurred while updating the activity.");
                    return View(activite);
                }
            }

            // If ModelState is not valid, return the Edit view with the provided activity
            return View(activite);
        }


        private bool ActiviteExists(int id)
        {
            return _context.Activites.Any(e => e.IdActivite == id);
        }

        // GET: Activites/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Activites == null)
            {
                return NotFound();
            }

            var activite = await _context.Activites
                .Include(a => a.IdUtilisateurCreateurNavigation)
                .FirstOrDefaultAsync(m => m.IdActivite == id);
            if (activite == null)
            {
                return NotFound();
            }

            return View(activite);
        }

        // POST: Activites/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Activites == null)
            {
                return Problem("Entity set 'kidzkonnectContext.Activites'  is null.");
            }
            var activite = await _context.Activites.FindAsync(id);
            if (activite != null)
            {
                _context.Activites.Remove(activite);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> FriendsActivities()
        {
            var userIdSession = HttpContext.Session.GetInt32("UserId");
            if (!userIdSession.HasValue)
            {
                return RedirectToAction("SignInUp", "Home");
            }

            int currentUserId = userIdSession.Value; // Rename the outer scope userId to avoid conflict

            // Get the child profiles IDs associated with the current user
            var childProfileIds = await _context.ProfilEnfants
                                                .Where(p => p.IdUtilisateur == currentUserId)
                                                .Select(p => p.IdProfil)
                                                .ToListAsync();

            // Get the friend connections where the connection is accepted
            var friendConnections = await _context.Connections
                .Where(c => childProfileIds.Contains(c.IdProfilEnvoyeur ?? 0) && c.EtatConnexion == "accepted" ||
                            childProfileIds.Contains(c.IdProfilReceveur ?? 0) && c.EtatConnexion == "accepted")
                .ToListAsync();

            // Get distinct friend profile IDs from the connections
            var friendProfileIds = friendConnections
                .SelectMany(c => new[] { c.IdProfilEnvoyeur, c.IdProfilReceveur })
                .Where(id => id != null)
                .Distinct()
                .ToList();

            // Get the user IDs associated with the friend profiles
            var friendUserIds = await _context.ProfilEnfants
                                              .Where(pe => friendProfileIds.Contains(pe.IdProfil))
                                              .Select(pe => pe.IdUtilisateur)
                                              .Distinct()
                                              .ToListAsync();

            // Get the activities from these user IDs
            var activities = await _context.Activites
                                   .Include(a => a.IdUtilisateurCreateurNavigation) // Include the user who created the activity
                                   .Where(a => friendUserIds.Contains(a.IdUtilisateurCreateur ?? 0))
                                   .ToListAsync();

            // Filter out activities created by the session's user
            activities = activities.Where(a => a.IdUtilisateurCreateur != currentUserId).ToList();

            // Organize activities by user
            var utilisateurEtActivitesList = new List<UtilisateurEtActivites>();
            foreach (var friendUserId in friendUserIds) // Rename the loop variable to avoid conflict
            {
                // Find the user associated with the current user ID
                var user = await _context.Utilisateurs.FirstOrDefaultAsync(u => u.IdUtilisateur == friendUserId);
                if (user != null)
                {
                    // Retrieve activities related to the specified userId
                    var userActivities = activities.Where(a => a.IdUtilisateurCreateur == friendUserId).ToList();

                    // Create an instance of 'UtilisateurEtActivites' with the retrieved data
                    var utilisateurEtActivites = new UtilisateurEtActivites
                    {
                        User = user,
                        Activites = userActivities
                    };

                    utilisateurEtActivitesList.Add(utilisateurEtActivites);
                }
            }

            return View(utilisateurEtActivitesList);
        }
        public async Task<IActionResult> JoinActivity(int idActivite, int profilEnfantId)
        {
            var userIdSession = HttpContext.Session.GetInt32("UserId");
            if (!userIdSession.HasValue)
            {
                return RedirectToAction("SignInUp", "Home");
            }
            int userId = userIdSession.Value;

            var isChildOfUser = _context.ProfilEnfants.Any(pe => pe.IdProfil == profilEnfantId && pe.IdUtilisateur == userId);
            if (!isChildOfUser)
            {
                return RedirectToAction("Error");
            }

            var activity = await _context.Activites.FindAsync(idActivite);
            if (activity == null)
            {
                return RedirectToAction("Error");
            }

            var participationExists = await _context.Participations.AnyAsync(p => p.IdActivite == idActivite && p.IdProfil == profilEnfantId);
            if (participationExists)
            {
                return RedirectToAction("AlreadyJoined");
            }

            var newParticipation = new Participation
            {
                IdActivite = idActivite,
                IdProfil = profilEnfantId
            };

            _context.Add(newParticipation);
            await _context.SaveChangesAsync();

            var childProfile = await _context.ProfilEnfants.FirstOrDefaultAsync(pe => pe.IdProfil == profilEnfantId);
            var notificationMessage = $"[Participation] {childProfile.Prenom} {childProfile.Nom} is attending the {activity.TitreActivite} activity.";
            var notification = new Notification
            {
                IdUtilisateur = userId,
                Message = notificationMessage,
                DateEnvoi = DateTime.Now,
                EtatLecture = false
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Activites", new { id = idActivite });
        }
    }
}

