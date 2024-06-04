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
    public class ProfilEnfantsController : Controller
    {
        private readonly kidzkonnectContext _context;

        public ProfilEnfantsController(kidzkonnectContext context)
        {
            _context = context;
        }

        // GET: ProfilEnfants
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
            var profilEnfantsList = await _context.ProfilEnfants
                                                  .Where(p => p.IdUtilisateur == userId)
                                                  .Include(p => p.AvatarFkNavigation)
                                                  .ToListAsync();

            // Ensure that you are passing an instance of 'UtilisateurEtProfils' to the view
            var utilisateurEtProfils = new UtilisateurEtProfils
            {
                User = user,
                ProfilEnfants = profilEnfantsList
            };

            // Pass the 'UtilisateurEtProfils' instance to the view
            return View(utilisateurEtProfils);
        }


        // GET: ProfilEnfants/Details/5
        public async Task<IActionResult> Details()
        {
            // Retrieve the userId from session
            var userIdSession = HttpContext.Session.GetInt32("UserId");
            if (!userIdSession.HasValue)
            {
                // If there is no userId in the session, redirect to the login page or handle accordingly
                return RedirectToAction("SignInUp", "Home");
            }

            int userId = userIdSession.Value;

            // Retrieve the user with the specified userId
            var user = await _context.Utilisateurs.FirstOrDefaultAsync(u => u.IdUtilisateur == userId);
            if (user == null)
            {
                return NotFound();
            }

            // Retrieve all children profiles associated with the user
            var childrenProfiles = await _context.ProfilEnfants
                .Include(p => p.AvatarFkNavigation) // Include related avatar information
                .Where(p => p.IdUtilisateur == userId) // Filter by user id
                .ToListAsync();

            // Create the view model containing user and children profiles
            var model = new UtilisateurEtProfils
            {
                User = user,
                ProfilEnfants = childrenProfiles
            };

            return View(model);
        }


        public async Task<IActionResult> FriendsDetails(int? id)
        {
            // Retrieve the userId from session
            var userIdSession = HttpContext.Session.GetInt32("UserId");
            if (!userIdSession.HasValue)
            {
                // If there is no userId in the session, redirect to the login page or handle accordingly
                return RedirectToAction("SignInUp", "Home");
            }

            if (id == null)
            {
                // If userId is null, redirect to the appropriate action or handle accordingly
                return RedirectToAction("Error", "Home");
            }

            // Retrieve the user with the specified id
            var user = await _context.Utilisateurs.FirstOrDefaultAsync(u => u.IdUtilisateur == id);
            if (user == null)
            {
                return NotFound();
            }

            // Retrieve all children profiles associated with the user
            var childrenProfiles = await _context.ProfilEnfants
                .Include(p => p.AvatarFkNavigation) // Include related avatar information
                .Where(p => p.IdUtilisateur == id) // Filter by user id
                .ToListAsync();

            // Create the view model containing user and children profiles
            var model = new UtilisateurEtProfils
            {
                User = user,
                ProfilEnfants = childrenProfiles
            };

            return View(model);
        }


        public async Task<IActionResult> UserDetailsFromChildProfile(int? idProfil)
        {
            if (idProfil == null)
            {
                return RedirectToAction("Error", "Home");
            }

            // Retrieve the parent user's IdUtilisateur based on the child profile's IdProfil
            var parentUserId = await _context.ProfilEnfants
                .Where(p => p.IdProfil == idProfil)
                .Select(p => p.IdUtilisateur)
                .FirstOrDefaultAsync();

            if (parentUserId == null)
            {
                return NotFound();
            }

            // Redirect to the FriendsDetails action with the retrieved parent user's IdUtilisateur
            return RedirectToAction("FriendsDetails", "ProfilEnfants", new { id = parentUserId });
        }


        // GET: ProfilEnfants/Create
        [HttpGet]
        public IActionResult Create()
        {
            var userIdSession = HttpContext.Session.GetInt32("UserId");
            if (!userIdSession.HasValue)
            {
                // If there is no userId in the session, redirect to the login page or handle accordingly
                return RedirectToAction("SignInUp", "Home");
            }

            int userId = userIdSession.Value;

            // Pass the userId to the view data for later use
            ViewData["UserId"] = userId;

            // Retrieve all avatars from the database
            var avatars = _context.Avatars.ToList();

            // Initialize Avatars in ViewBag only if avatars exist
            if (avatars.Any())
            {
                ViewBag.Avatars = new SelectList(avatars, "IdAvatar", "NomAvatar");
            }
            else
            {
                // Handle case where no avatars are found
                ViewBag.Avatars = new SelectList(new List<Avatar>(), "IdAvatar", "NomAvatar");
                ViewData["ErrorMessage"] = "No avatars found.";
            }

            return View();
        }

        // POST: ProfilEnfants/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdProfil,IdUtilisateur,AvatarFk,Nom,Prenom,DateNaissance,Genre,Interets,Defis,OptionsPriv")] ProfilEnfant profilEnfant)
        {
            if (ModelState.IsValid)
            {
                // Retrieve the userId from the ViewData
                if (ViewData.TryGetValue("UserId", out var userId))
                {
                    // Set the IdUtilisateur property of profilEnfant
                    profilEnfant.IdUtilisateur = Convert.ToInt32(userId);
                }

                // Add the selected avatar to profilEnfant
                if (profilEnfant.AvatarFk == 0)
                {
                    ModelState.AddModelError("AvatarFk", "Please select an avatar");
                    ViewData["Avatars"] = new SelectList(_context.Avatars, "IdAvatar", "Nom", profilEnfant.AvatarFk);
                    return View(profilEnfant);
                }

                _context.Add(profilEnfant);
                await _context.SaveChangesAsync();

                // Redirect to the ProfileEnfant index of the associated user
                return RedirectToAction("Index", "ProfilEnfants", new { userId = profilEnfant.IdUtilisateur });
            }

            // If model state is not valid, repopulate the dropdown list
            ViewData["Avatars"] = new SelectList(_context.Avatars, "IdAvatar", "Nom", profilEnfant.AvatarFk);
            return View(profilEnfant);
        }

        // GET: ProfilEnfants/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ProfilEnfants == null)
            {
                return NotFound();
            }

            var profilEnfant = await _context.ProfilEnfants.FindAsync(id);
            if (profilEnfant == null)
            {
                return NotFound();
            }

            // Retrieve all avatars from the database
            var avatars = _context.Avatars.ToList();

            // Initialize Avatars in ViewBag only if avatars exist
            if (avatars != null && avatars.Any())
            {
                ViewBag.Avatars = new SelectList(avatars, "IdAvatar", "NomAvatar", profilEnfant.AvatarFk);
            }
            else
            {
                // Handle case where no avatars are found
                ViewBag.Avatars = new SelectList(new List<Avatar>(), "IdAvatar", "NomAvatar");
                ViewData["ErrorMessage"] = "No avatars found.";
            }

            ViewData["IdUtilisateur"] = new SelectList(_context.Utilisateurs, "IdUtilisateur", "IdUtilisateur", profilEnfant.IdUtilisateur);
            return View(profilEnfant);
        }

        // POST: ProfilEnfants/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdProfil,IdUtilisateur,AvatarFk,Nom,Prenom,DateNaissance,Genre,Interets,Defis,OptionsPriv")] ProfilEnfant profilEnfant)
        {
            if (id != profilEnfant.IdProfil)
            {
                return NotFound();
            }


            if (ModelState.IsValid)
            {
                try
                {
                    // Retrieve the existing profilEnfant record from the database
                    var existingProfilEnfant = await _context.ProfilEnfants.FindAsync(id);

                    if (existingProfilEnfant == null)
                    {
                        return NotFound();
                    }

                    // Update only the properties that are allowed to be modified
                    existingProfilEnfant.Nom = profilEnfant.Nom;
                    existingProfilEnfant.Prenom = profilEnfant.Prenom;
                    existingProfilEnfant.DateNaissance = profilEnfant.DateNaissance;
                    existingProfilEnfant.Genre = profilEnfant.Genre;
                    existingProfilEnfant.Interets = profilEnfant.Interets;
                    existingProfilEnfant.Defis = profilEnfant.Defis;
                    existingProfilEnfant.OptionsPriv = profilEnfant.OptionsPriv;
                    existingProfilEnfant.AvatarFk = profilEnfant.AvatarFk; // Ensure AvatarFk is updated

                    // Save changes to the database
                    _context.Update(existingProfilEnfant);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Index", new { userId = existingProfilEnfant.IdUtilisateur });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProfilEnfantExists(profilEnfant.IdProfil))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            ViewData["AvatarFk"] = new SelectList(_context.Avatars, "IdAvatar", "IdAvatar", profilEnfant.AvatarFk);
            ViewData["IdUtilisateur"] = new SelectList(_context.Utilisateurs, "IdUtilisateur", "IdUtilisateur", profilEnfant.IdUtilisateur);
            return View(profilEnfant);
        }

        // GET: ProfilEnfants/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ProfilEnfants == null)
            {
                return NotFound();
            }

            var profilEnfant = await _context.ProfilEnfants
                .Include(p => p.AvatarFkNavigation)
                .Include(p => p.IdUtilisateurNavigation)
                .FirstOrDefaultAsync(m => m.IdProfil == id);
            if (profilEnfant == null)
            {
                return NotFound();
            }

            return View(profilEnfant);
        }

        // POST: ProfilEnfants/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ProfilEnfants == null)
            {
                return Problem("Entity set 'kidzkonnectContext.ProfilEnfants'  is null.");
            }
            var profilEnfant = await _context.ProfilEnfants.FindAsync(id);
            if (profilEnfant != null)
            {
                _context.ProfilEnfants.Remove(profilEnfant);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProfilEnfantExists(int id)
        {
            return (_context.ProfilEnfants?.Any(e => e.IdProfil == id)).GetValueOrDefault();
        }

        //---------------------------SUGGESTIONS D'AMIS----------------------//

        public async Task<IActionResult> FriendsSuggestions()
        {
            var userIdSession = HttpContext.Session.GetInt32("UserId");
            if (!userIdSession.HasValue)
            {
                return RedirectToAction("SignInUp", "Home");
            }

            int userId = userIdSession.Value;

            // Retrieve all profiles associated with the logged-in user
            var userProfiles = await _context.ProfilEnfants
                                             .Where(p => p.IdUtilisateur == userId)
                                             .ToListAsync();

            var userInterests = userProfiles.SelectMany(p => p.Interets?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>()).Distinct();
            var userChallenges = userProfiles.SelectMany(p => p.Defis?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>()).Distinct();

            // Retrieve all profiles except the ones associated with the logged-in user
            var allProfiles = await _context.ProfilEnfants
                                            .Where(p => p.IdUtilisateur != userId)
                                            .ToListAsync();

            // Perform the matching logic based on common interests and challenges
            var suggestedProfiles = allProfiles
                .Where(p =>
                    (p.Interets != null && p.Interets.Split(',', StringSplitOptions.RemoveEmptyEntries).Intersect(userInterests).Any()) ||
                    (p.Defis != null && p.Defis.Split(',', StringSplitOptions.RemoveEmptyEntries).Intersect(userChallenges).Any()))
                .ToList();

            // Get the IDs of profiles to which the current user has already sent requests
            var pendingRequests = await _context.Connections
                                                .Where(c => c.IdProfilEnvoyeur == userId && c.EtatConnexion == "requested")
                                                .Select(c => c.IdProfilReceveur)
                                                .ToListAsync();

            // Get the connections where the current user is the sender and the request is pending
            var sentRequests = await _context.Connections
                .Where(c => c.IdProfilEnvoyeur == userId && c.EtatConnexion == "requested")
                .Include(c => c.IdProfilEnvoyeurNavigation) // Assuming there is a navigation property to ProfilEnfant
                .ToListAsync();

            // Determine if the friend request is pending for each suggested profile
            var viewModelList = suggestedProfiles.Select(profile => new FriendsSuggestionsViewModel
            {
                IdProfil = profile.IdProfil,
                FirstName = profile.Prenom,
                LastName = profile.Nom,
                CommonInterests = profile.Interets?.Split(',').Intersect(userInterests).ToList() ?? new List<string>(),
                CommonChallenges = profile.Defis?.Split(',').Intersect(userChallenges).ToList() ?? new List<string>(),
                IsRequestPending = pendingRequests.Contains(profile.IdProfil)

            }).ToList();

            // Create a SelectList for the user's profiles to be used in the dropdown
            ViewBag.ProfilEnfantList = new SelectList(userProfiles, "IdProfil", "Prenom");

            return View(viewModelList);
        }

        //---------------------------KONNECTIONS----------------------//

        // POST: ProfilEnfants/SendFriendRequest
        [HttpPost]
        public async Task<IActionResult> SendFriendRequest(int senderId, int receiverId)
        {
            var userIdSession = HttpContext.Session.GetInt32("UserId");
            if (!userIdSession.HasValue)
            {
                // If there is no userId in the session, redirect to the login page or handle accordingly
                return RedirectToAction("SignInUp", "Home");
            }

            int userId = userIdSession.Value;

            // Now find the user associated with the receiver's profile (idProfilReceveur)
            var receiverUser = await _context.ProfilEnfants
                                             .Where(pe => pe.IdProfil == receiverId)
                                             .Select(pe => pe.IdUtilisateur)
                                             .FirstOrDefaultAsync();

            if (!receiverUser.HasValue)
            {
                // Handle the case where the receiver's user ID does not exist
                return NotFound();
            }

            // Now find the name of the sender
            var senderProfile = await _context.ProfilEnfants
                                              .FirstOrDefaultAsync(pe => pe.IdProfil == senderId);

            if (senderProfile == null)
            {
                // Handle the case where the sender's profile is not found
                return NotFound();
            }

            var senderFirstName = senderProfile.Prenom; // Assuming Prenom is the property representing the first name
            var senderLastName = senderProfile.Nom; // Assuming Nom is the property representing the last name
            var senderGender = senderProfile.Genre;

            var senderNameColor = senderGender == "male" ? "#00B0F0" : senderGender == "female" ? "#FD58BD" : "#8535F3";

            if (senderProfile == null)
            {
                // Handle the case where the sender's profile is not found
                return NotFound();
            }

            // If the receiver's user ID is found, proceed to create the connection
            var connection = new Connection
            {
                IdProfilEnvoyeur = senderId,
                IdProfilReceveur = receiverId,
                EtatConnexion = "requested"
            };

            _context.Connections.Add(connection);
            await _context.SaveChangesAsync(); // Ensure the connection is saved to generate IdDemande

            // After saving, connection.IdDemande is now available
            // Create the notification for the receiver's user ID, not the profile ID
            // Include the connection.IdDemande in the notification message
            var notification = new Notification
            {
                IdUtilisateur = receiverUser.Value,
                Message = $"<span style='color: {senderNameColor}; font-weight: bold;'>You have received a friend request from {senderFirstName} {senderLastName}</span>.",
                DateEnvoi = DateTime.Now,
                EtatLecture = false,
                IdDemande = connection.IdDemande
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync(); // Save the notification

            return RedirectToAction("Index", "Home");// Or return to a relevant view
        }



        /******************************* ACCEPT OR REJECT *****************************************/

        /*[HttpPost]
        public async Task<IActionResult> AcceptFriendRequest(int notificationId)
        {
            var userIdSession = HttpContext.Session.GetInt32("UserId");
            if (!userIdSession.HasValue)
            {
                // If there is no userId in the session, redirect to the login page or handle accordingly
                return RedirectToAction("SignInUp", "Home");
            }

            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification == null)
            {
                return NotFound();
            }

            var connection = await _context.Connections.FirstOrDefaultAsync(c => c.IdDemande == notification.IdDemande);
            if (connection == null)
            {
                // The connection with the provided ID was not found
                return NotFound();
            }

            // Check if the current user is the intended receiver of the connection request
            var receiverProfile = await _context.ProfilEnfants.FirstOrDefaultAsync(p => p.IdProfil == connection.IdProfilReceveur);
            if (receiverProfile == null || receiverProfile.IdUtilisateur != userIdSession.Value)
            {
                // The logged-in user does not match the receiver of the connection request
                return Forbid();
            }

            // Update the connection status to "accepted"
            connection.EtatConnexion = "accepted";

            // Optionally, mark the notification as read
            notification.EtatLecture = true;

            await _context.SaveChangesAsync();

            // Redirect to a confirmation page or back to the notifications list
            return RedirectToAction(nameof(Notifications));
        }*/

        public async Task<IActionResult> Notifications()
        {
            var userIdSession = HttpContext.Session.GetInt32("UserId");
            if (!userIdSession.HasValue)
            {
                return RedirectToAction("SignInUp", "Home");
            }

            var notifications = await _context.Notifications
                .Where(n => n.IdUtilisateur == userIdSession.Value)
                .ToListAsync();

            List<NotificationViewModel> viewModelList = new List<NotificationViewModel>();

            foreach (var notification in notifications)
            {
                var connectionStatus = _context.Connections
                    .FirstOrDefault(c => c.IdDemande == notification.IdDemande)?
                    .EtatConnexion ?? "pending";

                viewModelList.Add(new NotificationViewModel
                {
                    Notification = notification,
                    Status = connectionStatus
                });
            }

            return View(viewModelList);
        }

        [HttpPost]
        public async Task<IActionResult> AcceptFriendRequest(int notificationId)
        {
            var userIdSession = HttpContext.Session.GetInt32("UserId");
            if (!userIdSession.HasValue)
            {
                // If there is no userId in the session, redirect to the login page or handle accordingly
                return RedirectToAction("SignInUp", "Home");
            }

            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification == null)
            {
                return NotFound();
            }

            var connection = await _context.Connections.FirstOrDefaultAsync(c => c.IdDemande == notification.IdDemande);
            if (connection == null)
            {
                // The connection with the provided ID was not found
                return NotFound();
            }

            // Check if the current user is the intended receiver of the connection request
            var receiverProfile = await _context.ProfilEnfants.FirstOrDefaultAsync(p => p.IdProfil == connection.IdProfilReceveur);
            if (receiverProfile == null || receiverProfile.IdUtilisateur != userIdSession.Value)
            {
                // The logged-in user does not match the receiver of the connection request
                return Forbid();
            }

            // Update the connection status to "accepted"
            connection.EtatConnexion = "accepted";

            // Optionally, mark the notification as read
            notification.EtatLecture = true;

            await _context.SaveChangesAsync();

            // Redirect to a confirmation page or back to the notifications list
            return RedirectToAction(nameof(Notifications));
        }


        [HttpPost]
        public async Task<IActionResult> RejectFriendRequest(int notificationId)
        {
            var userIdSession = HttpContext.Session.GetInt32("UserId");
            if (!userIdSession.HasValue)
            {
                // Handle the case where user is not logged in
                return RedirectToAction("SignInUp", "Home");
            }

            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification == null)
            {
                return NotFound();
            }

            var connection = await _context.Connections.FirstOrDefaultAsync(c => c.IdDemande == notification.IdDemande);
            if (connection == null)
            {
                // The connection with the provided ID was not found
                return NotFound();
            }

            // Check if the current user is the intended receiver of the connection request
            var receiverProfile = await _context.ProfilEnfants.FirstOrDefaultAsync(p => p.IdProfil == connection.IdProfilReceveur);
            if (receiverProfile == null || receiverProfile.IdUtilisateur != userIdSession.Value)
            {
                // The logged-in user does not match the receiver of the connection request
                return Forbid();
            }

            // Update the connection status to "accepted"
            connection.EtatConnexion = "rejected";

            // Optionally, mark the notification as read
            notification.EtatLecture = true;

            await _context.SaveChangesAsync();

            // Redirect to a confirmation page or back to the notifications list
            return RedirectToAction(nameof(Notifications));
        }

        /******************************* UNKONNECT *****************************************/
        [HttpPost]
        public async Task<IActionResult> Unkonnect(int idProfil)
        {
            var userIdSession = HttpContext.Session.GetInt32("UserId");
            if (!userIdSession.HasValue)
            {
                // Handle the case where the user is not logged in
                return RedirectToAction("SignInUp", "Home");
            }

            // Retrieve the connection based on the provided IdProfil
            var connection = await _context.Connections.FirstOrDefaultAsync(c => c.IdProfilEnvoyeur == idProfil ||
                                                                                   c.IdProfilReceveur == idProfil &&
                                                                                   c.EtatConnexion == "accepted");
            if (connection == null)
            {
                // The connection with the provided ID was not found
                return NotFound();
            }

            // Update the connection status to "rejected"
            connection.EtatConnexion = "rejected";

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Redirect to a confirmation page or back to the notifications list
            return RedirectToAction("FriendsList", "ProfilEnfants");
        }


        private async Task<IActionResult> RespondToFriendRequest(int requestId, string etatConnexion)
        {
            var userIdSession = HttpContext.Session.GetInt32("UserId");
            if (!userIdSession.HasValue)
            {
                // Handle the case where user is not logged in
                return RedirectToAction("SignInUp", "Home");
            }

            int userId = userIdSession.Value;

            // Retrieve the connection request
            var connection = await _context.Connections.FindAsync(requestId);

            if (connection != null && (connection.IdProfilEnvoyeur == userId || connection.IdProfilReceveur == userId))
            {
                // Update the connection state based on the response
                connection.EtatConnexion = etatConnexion;
                await _context.SaveChangesAsync();
            }
            else
            {
                // Handle the case where the connection does not exist or the user does not have the right to respond
                return NotFound();
            }

            // No need to redirect, just return a success response
            return Ok(); // Or any other appropriate response
        }


        /******************************* NOTIFICATION *****************************************//*

        // GET: ProfilEnfants/Notifications
        public async Task<IActionResult> Notifications()
        {
            var userIdSession = HttpContext.Session.GetInt32("UserId");
            if (!userIdSession.HasValue)
            {
                return RedirectToAction("SignInUp", "Home");
            }

            int userId = userIdSession.Value;

            // Retrieve notifications for the current user
            var notifications = await _context.Notifications
                .Where(n => n.IdUtilisateur == userId)
                .ToListAsync();

            return View(notifications);
        }*/


        /******************************* FRIENDS LIST *****************************************/

        /*        // GET: ProfilEnfants/FriendsList
                public async Task<IActionResult> FriendsList()
                {
                    var userIdSession = HttpContext.Session.GetInt32("UserId");
                    if (!userIdSession.HasValue)
                    {
                        // Handle the case where the user is not logged in
                        return RedirectToAction("SignInUp", "Home");
                    }

                    var acceptedConnections = await _context.Connections
                        .Include(c => c.IdProfilEnvoyeurNavigation)
                        .Include(c => c.IdProfilReceveurNavigation)
                        .Where(c => c.EtatConnexion == "accepted" &&
                                    (c.IdProfilEnvoyeurNavigation.IdUtilisateur == userIdSession ||
                                     c.IdProfilReceveurNavigation.IdUtilisateur == userIdSession))
                        .ToListAsync();

                    var distinctProfiles = acceptedConnections
                        .Where(c => c.EtatConnexion == "accepted")
                        .SelectMany(c => new[] { c.IdProfilEnvoyeurNavigation, c.IdProfilReceveurNavigation })
                        .Distinct()
                        .Where(p => p.IdUtilisateur != userIdSession)
                        .ToList();

                    return View(distinctProfiles);
                }*/

        /*        public async Task<IActionResult> FriendsList()
                {
                    var userIdSession = HttpContext.Session.GetInt32("UserId");
                    if (!userIdSession.HasValue)
                    {
                        // Handle the case where the user is not logged in
                        return RedirectToAction("SignInUp", "Home");
                    }

                    var acceptedConnections = await _context.Connections
                        .Include(c => c.IdProfilEnvoyeurNavigation)
                        .Include(c => c.IdProfilReceveurNavigation)
                        .Where(c => c.EtatConnexion == "accepted" &&
                                    (c.IdProfilEnvoyeurNavigation.IdUtilisateur == userIdSession ||
                                     c.IdProfilReceveurNavigation.IdUtilisateur == userIdSession))
                        .ToListAsync();

                    var distinctProfiles = acceptedConnections
                        .Where(c => c.EtatConnexion == "accepted")
                        .SelectMany(c => new[] { c.IdProfilEnvoyeurNavigation, c.IdProfilReceveurNavigation })
                        .Distinct()
                        .Where(p => p.IdUtilisateur != userIdSession)
                        .ToList();

                    // Pass the distinctProfiles and their corresponding IdProfilEnvoyeurNavigation and IdProfilReceveurNavigation to the ViewBag
                    ViewBag.ProfilesWithConnections = distinctProfiles.Select(p => new
                    {
                        Profile = p,
                        IdProfilEnvoyeurNavigation = acceptedConnections.FirstOrDefault(c => c.IdProfilEnvoyeurNavigation.IdProfil == p.IdProfil)?.IdProfilEnvoyeurNavigation,
                        IdProfilReceveurNavigation = acceptedConnections.FirstOrDefault(c => c.IdProfilReceveurNavigation.IdProfil == p.IdProfil)?.IdProfilReceveurNavigation
                    });

                    return View();
                }*/

        public async Task<IActionResult> FriendsList()
        {
            var userIdSession = HttpContext.Session.GetInt32("UserId");
            if (!userIdSession.HasValue)
            {
                // Handle the case where the user is not logged in
                return RedirectToAction("SignInUp", "Home");
            }

            var acceptedConnections = await _context.Connections
                .Include(c => c.IdProfilEnvoyeurNavigation)
                .Include(c => c.IdProfilReceveurNavigation)
                .Where(c => c.EtatConnexion == "accepted" &&
                            (c.IdProfilEnvoyeurNavigation.IdUtilisateur == userIdSession ||
                             c.IdProfilReceveurNavigation.IdUtilisateur == userIdSession))
                .ToListAsync();

            var profilesWithConnections = acceptedConnections
                .Where(c => c.EtatConnexion == "accepted")
                .SelectMany(c => new[] { c.IdProfilEnvoyeurNavigation, c.IdProfilReceveurNavigation })
                .Distinct()
                .Where(p => p.IdUtilisateur != userIdSession)
                .Select(p => new ProfileWithKonnection
                {
                    Profile = p,
                    IdDemande = acceptedConnections.FirstOrDefault(c =>
                        c.IdProfilEnvoyeurNavigation.IdProfil == p.IdProfil ||
                        c.IdProfilReceveurNavigation.IdProfil == p.IdProfil)?.IdDemande
                })
                .ToList();

            ViewBag.ProfilesWithConnections = profilesWithConnections;

            return View();
        }
    }
}

