using kidzkonnect2._0.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace kidzkonnect2._0.Controllers
{
    public class RechercheController : Controller
    {
        private readonly kidzkonnectContext _context;

        public RechercheController(kidzkonnectContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string query)
        {
            var viewModel = new RechercheViewModel();
            var userIdSession = HttpContext.Session.GetInt32("UserId");
            if (!userIdSession.HasValue)
            {
                return RedirectToAction("SignInUp", "Home");
            }

            int userId = userIdSession.Value;

            var userProfiles = await _context.ProfilEnfants
                                   .Where(p => p.IdUtilisateur == userId)
                                   .ToListAsync();

            if (!string.IsNullOrWhiteSpace(query))
            {
                query = query.ToLower();

                var profilEnfantResults = await _context.ProfilEnfants
                    .Where(pe => pe.Prenom.ToLower().Contains(query) || pe.Nom.ToLower().Contains(query))
                    .Select(pe => new SearchResult
                    {
                        IdProfil = pe.IdProfil,
                        FirstName = pe.Prenom,
                        LastName = pe.Nom
                    })
                    .ToListAsync();
                viewModel.Results = profilEnfantResults.ToList();
            }
            ViewBag.RechercheProfilEnfantList = new SelectList(userProfiles, "IdProfil", "Prenom");
            return View(viewModel);
        }

        public async Task<IActionResult> ProfilEnfantDetails(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Error", "Home");
            }

            var profilEnfant = await _context.ProfilEnfants
                                             .Include(p => p.AvatarFkNavigation)
                                             .FirstOrDefaultAsync(m => m.IdProfil == id);
            var user = await _context.Utilisateurs.FirstOrDefaultAsync(u => u.IdUtilisateur == profilEnfant.IdUtilisateur);

            if (profilEnfant == null || user == null)
            {
                return NotFound();
            }

            var model = new UtilisateurEtProfils
            {
                User = user,
                ProfilEnfants = new List<ProfilEnfant> { profilEnfant }
            };

            return View("~/Views/ProfilEnfants/FriendsDetails.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> SendFriendRequest(int senderId, int receiverId)
        {
            var userIdSession = HttpContext.Session.GetInt32("UserId");
            if (!userIdSession.HasValue)
            {
                return RedirectToAction("SignInUp", "Home");
            }

            int userId = userIdSession.Value;

            var receiverUser = await _context.ProfilEnfants
                                             .Where(pe => pe.IdProfil == receiverId)
                                             .Select(pe => pe.IdUtilisateur)
                                             .FirstOrDefaultAsync();

            if (!receiverUser.HasValue)
            {
                return NotFound();
            }

            var senderProfile = await _context.ProfilEnfants
                                              .FirstOrDefaultAsync(pe => pe.IdProfil == senderId);

            if (senderProfile == null)
            {
                return NotFound();
            }

            var senderFirstName = senderProfile.Prenom;
            var senderLastName = senderProfile.Nom;
            var senderGender = senderProfile.Genre;

            var senderNameColor = senderGender == "male" ? "#00B0F0" : senderGender == "female" ? "#FD58BD" : "#8535F3";

            if (senderProfile == null)
            {
                return NotFound();
            }

            var connection = new Connection
            {
                IdProfilEnvoyeur = senderId,
                IdProfilReceveur = receiverId,
                EtatConnexion = "requested"
            };

            _context.Connections.Add(connection);
            await _context.SaveChangesAsync();


            var notification = new Notification
            {
                IdUtilisateur = receiverUser.Value,
                Message = $"<span style='color: {senderNameColor}; font-weight: bold;'>You have received a friend request from {senderFirstName} {senderLastName}</span>.",
                DateEnvoi = DateTime.Now,
                EtatLecture = false,
                IdDemande = connection.IdDemande
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

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

    }
}